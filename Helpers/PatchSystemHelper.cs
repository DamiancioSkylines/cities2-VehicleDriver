// <copyright file="PatchSystemHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver.Helpers
{
    using System;
    using System.Reflection;
    using Unity.Entities;

    // ReSharper disable once RedundantNameQualifier
    using VehicleDriver.Components;

    /// <summary>
    /// Provides utility methods for patching existing Unity ECS systems, specifically by modifying
    /// their <see cref="EntityQuery"/> instances to include or exclude specific components.
    /// This is useful for modding scenarios where you need to alter the behavior of vanilla systems
    /// without directly modifying their compiled code.
    /// </summary>
    public static class PatchSystemHelper
    {
        // Logger instance for mod-specific logging
        // private static readonly ILog LOG = LogManager.GetLogger(nameof(PatchSystemHelper)).SetShowsErrorsInUI(false);

        /// <summary>
        /// Attempts to patch an existing EntityQuery in a vanilla system to exclude entities with specific components.
        /// </summary>
        /// <typeparam name="TSystem">The type of the vanilla SystemBase to patch.</typeparam>
        /// <param name="allTypes">Components that must be present in the query (used to find the correct query field).</param>
        /// <param name="anyTypes">Components of which at least one must be present in the query.</param>
        /// <param name="noneTypes">Components to add to the query's 'None' array, effectively excluding entities with these components.</param>
        public static void PatchSystemQuery<TSystem>(
            ComponentType[] allTypes,
            ComponentType[] anyTypes = null,
            ComponentType[] noneTypes = null)
            where TSystem : SystemBase
        {
            Mod.LOG.Info($"[PatchSystemQuery] Starting patch attempt for system: {typeof(TSystem).Name}");

            var world = World.DefaultGameObjectInjectionWorld;
            var system = world.GetExistingSystemManaged<TSystem>();
            if (system == null)
            {
                Mod.LOG.Warn($"[PatchSystemQuery] System {typeof(TSystem).Name} not found in the world. Skipping patch.");
                return;
            }

            var queryFields = typeof(TSystem)
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in queryFields)
            {
                if (field.FieldType != typeof(EntityQuery))
                {
                    Mod.LOG.Debug($"[PatchSystemQuery] Skipping field '{field.Name}' in {typeof(TSystem).Name}: Not an EntityQuery type.");
                    continue;
                }

                Mod.LOG.Debug($"[PatchSystemQuery] Found potential EntityQuery field: '{field.Name}' in {typeof(TSystem).Name}.");

                var originalQuery = (EntityQuery)field.GetValue(system);
                if (originalQuery.Equals(default(EntityQuery)))
                {
                    Mod.LOG.Debug($"[PatchSystemHelper] Skipping field '{field.Name}' in {typeof(TSystem).Name}: Query is default/uninitialized.");
                    continue;
                }

                // Removed direct retrieval of originalQueryDesc due to API limitations.
                // We will now only log the *new* query's description to confirm the patch.
                Mod.LOG.Info($"[PatchSystemQuery] Found original query field '{field.Name}' in {typeof(TSystem).Name}. Attempting to replace it.");

                // Manually build a new query with EntityControlData excluded
                var newQueryDesc = new EntityQueryDesc
                {
                    All = allTypes, // Use provided allTypes to match the target query
                    Any = anyTypes ?? Array.Empty<ComponentType>(), // Use provided anyTypes or empty
                    None = AppendManualTag(noneTypes), // Append our exclusion tag
                };

                var newQuery = system.EntityManager.CreateEntityQuery(newQueryDesc);
                field.SetValue(system, newQuery);

                // patchedAnyQuery = true;

                // Log new query description
                Mod.LOG.Info($"[PatchSystemQuery] Patched {typeof(TSystem).Name} query '{field.Name}' (AFTER patch):");
                Mod.LOG.Info($"  All: {string.Join(", ", newQueryDesc.All)}");
                Mod.LOG.Info($"  Any: {string.Join(", ", newQueryDesc.Any)}");
                Mod.LOG.Info($"  None: {string.Join(", ", newQueryDesc.None)}");

                Mod.LOG.Info($"[PatchSystemQuery] Successfully patched {typeof(TSystem).Name} query '{field.Name}' to exclude EntityControlData.");

                // We return here because typically a system has one main query we want to patch.
                // If there are multiples, this logic would need to be adjusted.
                return;
            }

            Mod.LOG.Warn($"[PatchSystemQuery] No patchable query found in {typeof(TSystem).Name} matching criteria. Patch failed for this system.");
        }

        /// <summary>
        /// Appends the EntityControlData component to an array of ComponentTypes for exclusion.
        /// </summary>
        /// <param name="existingNone">Existing ComponentTypes to exclude.</param>
        /// <returns>A new array including existing exclusions and EntityControlData.</returns>
        private static ComponentType[] AppendManualTag(ComponentType[] existingNone)
        {
            if (existingNone == null || existingNone.Length == 0)
            {
                return new[] { ComponentType.ReadOnly<EntityControlData>() };
            }

            var result = new ComponentType[existingNone.Length + 1];
            Array.Copy(existingNone, result, existingNone.Length);
            result[^1] = ComponentType.ReadOnly<EntityControlData>();
            return result;
        }
    }
}
