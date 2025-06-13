// <copyright file="ComponentLogHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver.Helpers
{
    using System;
    using System.Collections.Generic;
    using Unity.Entities;

    /// <summary>
    /// Provides utility methods for logging and comparing components on entities.
    /// </summary>
    public static class ComponentLogHelper
    {
        // Removed private static readonly ILog LOG = LogManager.GetLogger(nameof(VehicleDriver)).SetShowsErrorsInUI(false);
        // Now using Mod.LOG directly.

        /// <summary>
        /// Lists all components present on a given entity.
        /// </summary>
        /// <param name="entityManager">The EntityManager instance to use for querying components.</param>
        /// <param name="entity">The entity whose components are to be listed.</param>
        /// <returns>A list of component full names (including namespace and assembly reference).</returns>
        public static List<string> ListComponents(EntityManager entityManager, Entity entity)
        {
            var componentsList = new List<string>();

            // Ensure the entity exists before trying to get its components.
            if (!entityManager.Exists(entity))
            {
                componentsList.Add($"Entity {entity.Index}:{entity.Version} does not exist.");
                return componentsList;
            }

            // Use the provided EntityManager instance to get component types.
            using var types = entityManager.GetComponentTypes(entity);
            foreach (var type in types)
            {
                try
                {
                    var managedType = TypeManager.GetType(type.TypeIndex);
                    if (managedType != null)
                    {
                        // Add the full component name with assembly reference for better logging and IDE highlighting.
                        componentsList.Add(managedType.FullName);
                    }
                    else
                    {
                        // Handle cases where TypeManager.GetType returns null (e.g., unknown type index).
                        componentsList.Add($"Unknown or Null Type (Index: {type.TypeIndex})");
                    }
                }
                catch (Exception ex)
                {
                    // Catching general exceptions for debugging component types.
                    // Log the actual exception message for better debugging.
                    componentsList.Add($"Error getting type for index {type.TypeIndex}: {ex.Message}");
                }
            }

            return componentsList;
        }

        /// <summary>
        /// Compares two lists of component names and logs which components were added and which were removed.
        /// </summary>
        /// <param name="prefix">A prefix string to prepend to the log messages.</param>
        /// <param name="beforeComponentList">The list of components before a change.</param>
        /// <param name="afterComponentList">The list of components after a change.</param>
        public static void CompareListComponents(string prefix, List<string> beforeComponentList, List<string> afterComponentList)
        {
            var added = new List<string>(afterComponentList);
            added.RemoveAll(beforeComponentList.Contains);

            var removed = new List<string>(beforeComponentList);
            removed.RemoveAll(afterComponentList.Contains);

            if (added.Count > 0 || removed.Count > 0)
            {
                Mod.LOG.Info(prefix + " Added:   " + string.Join(" • ", added));
                Mod.LOG.Info(prefix + " Removed: " + string.Join(" • ", removed));
            }
        }
    }
}
