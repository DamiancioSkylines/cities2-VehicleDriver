// <copyright file="ControlActivatorHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Helpers
{
    using Unity.Entities;
    using VehicleDriver.Components;

    /// <summary>
    /// Activates manual control over an entity by applying the necessary components.
    /// This class centralizes the logic for preparing an entity for manual driving.
    /// </summary>
    public static class ControlActivatorHelper
    {
        /// <summary>
        /// Applies the necessary components to an entity when it is brought under manual control.
        /// This includes removing vanilla AI/parked components and adding mod-specific control components.
        /// </summary>
        /// <param name="entityManager">The <see cref="EntityManager"/> instance to manipulate entity components.</param>
        /// <param name="entity">The <see cref="Entity"/> to which components are to be applied.</param>
        /// <param name="controlData">The <see cref="EntityControlData"/> struct containing the entity's original state,
        /// which is also added to the entity to mark it as mod-controlled.</param>
        public static void ApplyControlComponents(EntityManager entityManager, Entity entity, EntityControlData controlData)
        {
            // Taking control over car currently leaves original spot in the net blocked or something like that, causing massive traffic congestion todo
            // Mod.LOG.Info($"[ControlActivatorHelper] Applying control components for entity {entity.Index}:{entity.Version}.");

            // Add our custom control data Component to mark this vehicle as manually controlled by our mod
            ComponentHelper.SafeAddComponent<EntityControlData>(entityManager, entity);
            entityManager.SetComponentData(entity, controlData);

            // Remove (ParkedCar, ParkedTrain, Stopped) to allow manual control and prevent vanilla AI interference
            ComponentHelper.SafeRemoveComponent<Game.Vehicles.ParkedCar>(entityManager, entity);
            ComponentHelper.SafeRemoveComponent<Game.Vehicles.ParkedTrain>(entityManager, entity);
            ComponentHelper.SafeRemoveComponent<Game.Objects.Stopped>(entityManager, entity);

            // Mod.LOG.Info("[ControlActivatorHelper] Removed Parked/Stopped components.");

            // Add OutOfControl and Moving Components necessary for manual control, and to disable AI
            ComponentHelper.SafeAddComponent<Game.Objects.Moving>(entityManager, entity); // Ensure it's marked as moving
            ComponentHelper.SafeAddComponent<Game.Vehicles.OutOfControl>(entityManager, entity); // This is key to disable AI

            // Mod.LOG.Info("[ControlActivatorHelper] Added Moving and OutOfControl components.");

            // Remove AI pathfinding and navigation Components.
            // These three are possible to restore probably (or reset to default for AI to re-path).
            ComponentHelper.SafeRemoveComponent<Game.Vehicles.CarNavigation>(entityManager, entity);
            ComponentHelper.SafeRemoveComponent<Game.Vehicles.CarCurrentLane>(entityManager, entity);
            ComponentHelper.SafeRemoveComponent<Game.Pathfind.PathOwner>(entityManager, entity);

            // Mod.LOG.Info("[ControlActivatorHelper] Removed AI navigation components.");

            // These are buffers, more complicated to restore but vanilla systems populate them correctly on pathfinding trigger.
            ComponentHelper.SafeRemoveBuffer<Game.Vehicles.CarNavigationLane>(entityManager, entity);
            ComponentHelper.SafeRemoveBuffer<Game.Pathfind.PathElement>(entityManager, entity);

            // Mod.LOG.Info("[ControlActivatorHelper] Removed navigation buffers.");

            // Add a basic Target Component if it didn't have one or if it was removed, set to Entity.Null
            // This is to satisfy systems that expect a Target component, but we don't want it to point anywhere specific for manual driving.
            ComponentHelper.SafeAddComponent<Game.Common.Target>(entityManager, entity);
            entityManager.SetComponentData(entity, new Game.Common.Target(Entity.Null));

            // Mod.LOG.Info("[ControlActivatorHelper] Added/Reset Target component.");

            // Allows smooth animated transition to control mode by ensuring TransformFrame buffer exists.
            if (!entityManager.HasBuffer<Game.Objects.TransformFrame>(entity))
            {
                entityManager.AddBuffer<Game.Objects.TransformFrame>(entity);

               // Mod.LOG.Info("[ControlActivatorHelper] Added TransformFrame buffer.");
            }

            // Ensure InterpolatedTransform is present for smooth rendering.
            ComponentHelper.SafeAddComponent<Game.Rendering.InterpolatedTransform>(entityManager, entity);

            // Mod.LOG.Info("[ControlActivatorHelper] Added InterpolatedTransform component.");

            // Remove Swaying to prevent vehicle body wiggles. Will need a custom solution if desired.
            ComponentHelper.SafeRemoveComponent<Game.Rendering.Swaying>(entityManager, entity);

            // Mod.LOG.Info("[ControlActivatorHelper] Removed Swaying component.");

            // Mark entity as Updated to ensure game systems re-evaluate its state.
            ComponentHelper.SafeAddComponent<Game.Common.Updated>(entityManager, entity);

            // Mod.LOG.Info("[ControlActivatorHelper] Added Updated component.");
        }
    }
}
