// <copyright file="ControlDeactivatorHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Helpers
{
    using Unity.Entities;
    using VehicleDriver.Components;
    using VehicleDriver.Enums;

    /// <summary>
    /// Provides utility methods for restoring the state of entities after manual control is released.
    /// This class centralizes the logic for deactivating manual control and reverting entity components.
    /// </summary>
    public static class ControlDeactivatorHelper
    {
        /* - Null Reference Exception (NRE) time-bomb crash after releasing accident-involved vehicles that happen too under user control. To repeat drive into multiple cars and wait todo
            Suspect that The core problem is the timing gap where vehicles is released, accident involved car exists with InvolvedInAccident but without a properly linked m_Event (or with corrupted CarNavigation/CarCurrentLane data) for many frames before AccidentVehicleSystem can fix it, allowing CarNavigationSystem to repeatedly crash.
            But it might be something else after, as I tried to keep the OutOfControl and vanilla state for AccidentVehicleSystem do its magic yet still CarNavigationSystem is crashing wtf.
            Its a race condition or something

         ObjectCollisionSystem: OnUpdate() is called every frame, however internal job process on sub-frame basis, This means collision detection and initial impact processing are very frequent.
         Function: Detects collisions, creates Impact event entities, adds Damaged and potentially InvolvedInAccident components to vehicles. Crucially, its ResolveCollisionsJob sets Impact.m_Event to null if both entities are new to an accident.

         AccidentVehicleSystem:  Simulation phase. Updates every 64 frames.
         Function: Processes InvolvedInAccident entities. Its AccidentVehicleJob handles stopping vehicles (StopVehicle), initiating fires, adding injuries, and trying to find/add an AccidentSite if involvedInAccident.m_Event is null (if (this.m_TargetElements.HasBuffer(involvedInAccident.m_Event) && this.FindAccidentSite(involvedInAccident.m_Event) == Entity.Null) block). It also adds traffic accident icons.

         DamagedVehicleSystem: Simulation phase. Updates every 512 frames (very infrequent).
         Function: Handles Damaged vehicles, primarily by requesting MaintenanceRequest if needed and performing cleanup if Destroyed.m_Cleared is 1.0.

         MaintenanceVehicleDispatchSystem: Simulation phase. Updates every 16 frames.
         Function: Dispatches maintenance vehicles based on MaintenanceRequests.

         CarNavigationSystem: (As discussed, likely very frequent, e.g., every frame or every few frames, as it doesn't explicitly override GetUpdateInterval and deals with real-time movement : likely Simulation phase
         Function: Calculates optimal lanes, updates CarNavigation and CarCurrentLane.

         By the time AccidentVehicleSystem runs and tries to populate involvedInAccident.m_Event, the CarNavigationSystem has likely already thrown multiple NREs for your car because it's been trying to navigate it for many frames while m_Event was null and its internal navigation data was corrupted.
         Idk how to fix this.
         */

        /// <summary>
        /// Restores the components of an entity based on its original state stored in <see cref="EntityControlData"/>.
        /// This method handles reverting changes made during manual control, such as re-adding
        /// navigation components for moving vehicles or setting parked state.
        /// </summary>
        /// <param name="entityManager">The <see cref="EntityManager"/> instance to manipulate entity components.</param>
        /// <param name="entity">The <see cref="Entity"/> whose components are to be restored.</param>
        /// <param name="controlData">The <see cref="EntityControlData"/> containing the original state of the entity.</param>
        public static void RestoreEntityComponents(EntityManager entityManager, Entity entity, EntityControlData controlData)
        {
            // Restore components based on the original state of the vehicle (whether it was parked or moving).
            if (controlData.VehicleState == VehicleState.Parked)
            {
                // Mod.LOG.Info($"[ControlDeactivatorHelper.RestoreEntityComponents] Restoring originally parked state for entity {entity.Index}:{entity.Version}.");
                ComponentHelper.SafeRemoveComponent<Game.Objects.Moving>(entityManager, entity);
                switch (controlData.VehicleType)
                {
                    case Enums.VehicleType.Train:
                        ComponentHelper.SafeAddComponent<Game.Vehicles.ParkedTrain>(entityManager, entity);
                        ComponentHelper.SafeAddComponent<Game.Objects.Stopped>(entityManager, entity);
                        break;
                    case Enums.VehicleType.Car:
                        ComponentHelper.SafeAddComponent<Game.Vehicles.ParkedCar>(entityManager, entity);
                        ComponentHelper.SafeAddComponent<Game.Objects.Stopped>(entityManager, entity);
                        break;
                }

                ComponentHelper.SafeRemoveComponent<Game.Rendering.InterpolatedTransform>(entityManager, entity);
                ComponentHelper.SafeRemoveBuffer<Game.Objects.TransformFrame>(entityManager, entity);
                ComponentHelper.SafeRemoveComponent<Game.Common.Target>(entityManager, entity);
            }
            else if (controlData.VehicleState == VehicleState.Moving)
            {
                // Mod.LOG.Info($"[ControlDeactivatorHelper.RestoreEntityComponents] Restoring originally moving state for entity {entity.Index}:{entity.Version}.");
                if (controlData.HadCarNavigation)
                {
                    ComponentHelper.SafeAddComponentData<Game.Vehicles.CarNavigation>(entityManager, entity);
                    entityManager.SetComponentData(entity, default(Game.Vehicles.CarNavigation));
                }

                if (controlData.HadCarCurrentLane)
                {
                    ComponentHelper.SafeAddComponentData<Game.Vehicles.CarCurrentLane>(entityManager, entity);
                    entityManager.SetComponentData(entity, default(Game.Vehicles.CarCurrentLane));
                }

                if (controlData.HadPathOwner)
                {
                    ComponentHelper.SafeAddComponentData<Game.Pathfind.PathOwner>(entityManager, entity);
                    var newPathOwner = default(Game.Pathfind.PathOwner);
                    newPathOwner.m_State |= Game.Pathfind.PathFlags.Obsolete;
                    newPathOwner.m_State &= ~(Game.Pathfind.PathFlags.Failed | Game.Pathfind.PathFlags.Stuck | Game.Pathfind.PathFlags.Pending);
                    entityManager.SetComponentData(entity, newPathOwner);
                }

                ComponentHelper.SafeRemoveBuffer<Game.Vehicles.CarNavigationLane>(entityManager, entity);
                ComponentHelper.SafeRemoveBuffer<Game.Pathfind.PathElement>(entityManager, entity);
                entityManager.AddBuffer<Game.Vehicles.CarNavigationLane>(entity);
                var pathElementBuffer = entityManager.AddBuffer<Game.Pathfind.PathElement>(entity);
                if (pathElementBuffer.Length == 0)
                {
                    pathElementBuffer.Add(default);
                }

                ComponentHelper.SafeAddComponentData<Game.Common.Target>(entityManager, entity);
                entityManager.SetComponentData(entity, new Game.Common.Target(controlData.HadTarget ? controlData.OriginalTarget : Entity.Null));

                // Mod.LOG.Info($"[ControlDeactivatorHelper.RestoreEntityComponents] Restored Target for moving entity {entity.Index}:{entity.Version} to {entityManager.GetComponentData<Game.Common.Target>(entity).m_Target.Index}:{entityManager.GetComponentData<Game.Common.Target>(entity).m_Target.Version}.");
                if (controlData.HadSwaying)
                {
                    ComponentHelper.SafeAddComponentData<Game.Rendering.Swaying>(entityManager, entity);
                }
            }
            else
            {
                Mod.LOG.Warn($"[ControlDeactivatorHelper.RestoreEntityComponents] Unable to determine original vehicle state for entity {entity.Index}:{entity.Version} (VehicleState: {controlData.VehicleState}). Defaulting to basic restoration.");
            }

            ComponentHelper.SafeRemoveComponent<Game.Vehicles.OutOfControl>(entityManager, entity);

            // It gets updated in OnStopRunning in ControlToolSystem
            // ComponentHelper.SafeAddComponent<Game.Common.Updated>(entityManager, entity);
        }
    }
}
