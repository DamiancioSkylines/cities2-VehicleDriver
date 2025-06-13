// <copyright file="Mod.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver
{
    // Known Issues:
    // - Null Reference Exception (NRE) time-bomb crash after releasing accident-involved vehicles that happen under user control.
    // - ESC key cannot be used to cancel driving; please use ENTER instead.
    // - Driving is currently restricted to a horizontal plane, due to a conflict with the OutOfControlSystem, which handles proper terrain and net collision queries but causes jitter as its fighting mod movement. Simply not finished implementation.

    // Warning: Removing explicit qualifiers for components may cause memory leaks, possibly due to PrefabRef interactions called at "EnterControl", also some components give ambiguous errors when explicit qualifier is missing.
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Colossal.Logging;
    using Game;
    using Game.Input;
    using Game.Modding;
    using Game.Objects;
    using Game.Prefabs;
    using Game.SceneFlow;
    using Game.Simulation;
    using Game.Tools;
    using JetBrains.Annotations;
    using Unity.Entities;
    using UnityEngine.InputSystem;
    using VehicleDriver.Components;
    using VehicleDriver.Helpers;
    using VehicleDriver.Settings;
    using VehicleDriver.Systems;

    /// <summary>
    /// Represents the main mod class that integrates with the game via the IMod interface.
    /// This class encapsulates the initialisation and disposal processes, as well as manages core mod functionality,
    /// including input mappings, settings, and tool systems for manual vehicle control.
    /// </summary>
    [UsedImplicitly]
    public class Mod : IMod
    {
        /// <summary>
        /// Represents the name of the action that facilitates the toggling of control entities
        /// within the VehicleDriver mod. This constant is used to identify and bind a specific
        /// user interaction or input action for enabling and disabling control-related functionality.
        /// </summary>
        public const string ToggleControlEntityActionName = "ToggleControl";

        /// <summary>Represents the name of the input action for controlling the gas and brake of a vehicle, for acceleration and deceleration.</summary>
        public const string AxisGasBrakeActionName = "GasBrakeAxis";

        /// <summary>Represents the name of the input action for controlling the steering of a vehicle.</summary>
        public const string AxisSteerActionName = "SteerAxis";

        /// <summary> Represents the name of the input action for activating the handbrake of a vehicle.</summary>
        public const string ButtonHandbrakeActionName = "HandbrakeKey";

        /// <summary>Represents the name of the input action for respawning the controlled entity.</summary>
        public const string ButtonRespawnActionName = "Respawn";

        /// <summary>
        /// Provides a logging mechanism for the VehicleDriver namespace, allowing for consistent
        /// and centralised logging of events, errors, and informational messages within the mod.
        /// This logger is configured to suppress error messages from being displayed in the user interface.
        /// </summary>
        public static readonly ILog LOG = LogManager.GetLogger(nameof(VehicleDriver)).SetShowsErrorsInUI(false);

        /// <summary>
        /// Represents the settings configuration for the <see cref="Mod"/> class,
        /// managing UI customisation and input keybindings.
        /// This field is marked as internal to allow direct access by other mod systems (e.g., <see cref="ControlToolSystem"/>)
        /// for retrieving user-defined settings, which is essential for the mod's behavior.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This field needs to be internal for direct access by other mod systems for retrieving user settings, which is essential for proper functionality.")]
        internal Setting Setting;

        /// <summary>
        /// Gets or sets the <see cref="InputHelper"/> instance, which manages input actions and provides their current state.
        /// This field is marked as internal to allow direct access by other mod systems (e.g., <see cref="ControlToolSystem"/>)
        /// for reading input values, which is necessary for the mod's core control logic and performance within the ECS.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This field needs to be internal for direct access by other mod systems for reading input values, which is essential for proper functionality and performance within the ECS.")]
        internal InputHelper InputHelper;

        /// <summary>
        /// This field stores the control data of the currently controlled entity.
        /// It is marked as internal and static to allow direct access from various parts of the mod,
        /// including the <see cref="ExitControl"/> method and other systems that need to reference
        /// the original state of the controlled entity for restoration purposes.
        /// This direct access is necessary for the mod's architecture and performance in the game's ECS environment.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This field needs to be internal and static for direct access by other mod systems and methods for proper functionality and performance within the ECS.")]
        private static EntityControlData controlData;

        private ToolSystem toolSystem;
        private DefaultToolSystem defaultToolSystem;
        private ControlToolSystem controlToolSystem;
        private PrefabSystem prefabSystem;
        private CameraControlSystem cameraControlSystem;
        private Entity savedControlledEntity = Entity.Null;
        private EntityControlData savedControlData;

        /// <summary>
        /// Gets the singleton instance of the <see cref="Mod"/> class.
        /// </summary>
        public static Mod Instance { get; private set; }

        /// <summary>
        /// Gets the default <see cref="EntityManager"/> instance from the game's default world.
        /// This provides access to ECS operations for entities.
        /// </summary>
        private static EntityManager EntityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

        /// <summary>
        /// Called when the mod is disposed (e.g., game shutdown or mod unload).
        /// Releases resources and performs clean-up when the mod is disposed of.
        /// Unsubscribes from game events and input actions to prevent memory leaks or unintended behavior.
        /// Disables the entity input handler and attempts to gracefully release any controlled entities.
        /// </summary>
        public void OnDispose()
        {
            // Unsubscribe from events to prevent memory leaks or issues on mod unloading
            GameManager.instance.onGameLoadingComplete -= this.OnGameLoadingComplete; // Unsubscribe
            var toggleControlAction = this.Setting?.GetAction(ToggleControlEntityActionName);
            if (toggleControlAction != null)
            {
                // Unsubscribe
                toggleControlAction.onInteraction -= this.OnToggleControlInteraction;
            }

            var respawnAction = this.Setting?.GetAction(ButtonRespawnActionName);
            if (respawnAction != null)
            {
                // Unsubscribe
                respawnAction.onInteraction -= this.OnRespawnInteraction;
            }

            this.InputHelper?.Disable();

            // If an entity is still controlled, try to release it gracefully
            if (this.toolSystem.activeTool != this.controlToolSystem || this.savedControlledEntity == Entity.Null || !World.DefaultGameObjectInjectionWorld.EntityManager.Exists(this.savedControlledEntity))
            {
                return;
            }

            this.ExitControl();
        }

        /// <summary>
        /// Called during the initialization phase of the mod. Sets up the necessary systems, input handlers,
        /// settings, and configurations required for the mod to function. Register update dependencies
        /// and ensure proper integration with the game's update systems and event architecture.
        /// </summary>
        /// <param name="updateSystem">The update system responsible for scheduling and managing
        /// updates in the relevant system phases for the mod.</param>
        public void OnLoad(UpdateSystem updateSystem)
        {
            Instance = this;
            this.Setting = new Setting(this);

            // Initialise systems and handlers early
            this.toolSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ToolSystem>();

            // Get the ControlToolSystem from the world as it's now a standalone system
            this.controlToolSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ControlToolSystem>();
            this.defaultToolSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<DefaultToolSystem>();
            this.cameraControlSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraControlSystem>();

            this.InputHelper = new InputHelper();

            this.Setting.RegisterInOptionsUI();
            try
            {
                this.Setting.RegisterKeyBindings();
            }
            catch (Exception ex)
            {
                LOG.Warn(ex);
            }

            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEn(this.Setting));
            GameManager.instance.onGameLoadingComplete += this.OnGameLoadingComplete;

            // Explicitly set the Setting reference on the ControlToolSystem and CameraControlSystem here, after they're initialised.
            this.controlToolSystem.Setting = this.Setting;
            this.cameraControlSystem.SetSetting(this.Setting);

            // Ensure ControlToolSystem updates after ToolUpdate phase
            updateSystem.UpdateAfter<ControlToolSystem>(SystemUpdatePhase.ToolUpdate);

            // Ensure CameraControlSystem updates after LateUpdate
            updateSystem.UpdateAfter<CameraControlSystem>(SystemUpdatePhase.LateUpdate);

            // Register HidingUISystem to be updated before the UI update phase to ensure the UI state is managed correctly.
            updateSystem.UpdateBefore<Systems.HidingUISystem>(SystemUpdatePhase.UIUpdate);
        }

        /// <summary>
        /// Resets the driving tool and mod's internal state.
        /// This method is called after control is released, whether successfully or with issues.
        /// </summary>
        /// <param name="success">True if exiting driving mode was successful, false otherwise.</param>
        internal void ExitControlCleanup(bool success)
        {
            this.toolSystem.activeTool = this.defaultToolSystem;
            this.savedControlledEntity = Entity.Null;

            if (success)
            {
                LOG.Info("[ExitControlCleanup] Exited driving mode successfully.");
            }
            else
            {
                LOG.Warn("[ExitControlCleanup] Exited driving mode with issues.");
            }
        }

        /// <summary>
        /// Releases manual control of an entity, attempting to restore its original Components
        /// based on the flags stored in the <see cref="EntityControlData"/> Component.
        /// </summary>
        private void ExitControl()
        {
            var entity = this.controlToolSystem.GetTarget();
            LOG.Info($"[ExitControl] Started for entity {entity.Index}:{entity.Version}");

            // There is a case when controlled vehicle got into accident during manual control, after a while vehicle gets deleted and we can't escape Hidden UI or mod ControlToolSystem if user did not exit the vehicle. Two Part Problem here and in HidingUISystem todo
            // If no target entity is set or it no longer exists, exit and perform cleanup.
            if (entity == Entity.Null || !EntityManager.Exists(entity))
            {
                LOG.Info("[ExitControl] No target entity or entity destroyed — skipping restore.");
                this.ExitControlCleanup(false); // Clean up mod state without affecting an invalid entity
                return;
            }

            // Switch back to the DefaultToolSystem and disable mod input.
            this.controlToolSystem.SetDefaultState();
            this.toolSystem.activeTool = this.defaultToolSystem;
            this.InputHelper.Disable();

            // Notify CameraControlSystem to restore original camera.
            this.cameraControlSystem.OnExitControl();

            // Retrieve the EntityControlData Component to check original states.
            if (!EntityManager.HasComponent<EntityControlData>(entity))
            {
                LOG.Warn($"[ExitControl] Vehicle {entity.Index}:{entity.Index} missing EntityControlData on release. Cannot fully restore original state.");
            }

            // Restore components based on the original state of the vehicle (whether it was parked or moving).
            ControlDeactivatorHelper.RestoreEntityComponents(EntityManager, entity, Mod.controlData);

            this.ExitControlCleanup(true);
        }

        /// <summary>
        /// Called when the game loading is complete. Sets up the toggle control action and respawn action.
        /// Also applies necessary system patches to prevent conflicts with vanilla game systems.
        /// </summary>
        /// <param name="purpose">The purpose for which the game is loading.</param>
        /// <param name="mode">The game mode (e.g., Gameplay, Editor).</param>
        private void OnGameLoadingComplete(Colossal.Serialization.Entities.Purpose purpose, GameMode mode)
        {
            this.prefabSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PrefabSystem>();

            // Patch VehicleOutOfControlSystem to prevent AI from interfering with controlled vehicles.
            // This ensures that entities with EntityControlData are not processed by the vanilla OutOfControlSystem.
            PatchSystemHelper.PatchSystemQuery<VehicleOutOfControlSystem>(
                allTypes: new[]
                {
                    ComponentType.ReadOnly<Game.Vehicles.OutOfControl>(),
                    ComponentType.ReadOnly<Game.Simulation.UpdateFrame>(),
                    ComponentType.ReadWrite<Game.Objects.Transform>(),
                    ComponentType.ReadWrite<Game.Objects.Moving>(),
                    ComponentType.ReadWrite<Game.Objects.TransformFrame>(),
                },
                noneTypes: new[] { ComponentType.ReadOnly<Game.Common.Deleted>(), ComponentType.ReadOnly<Game.Tools.Temp>(), ComponentType.ReadOnly<TripSource>(), ComponentType.ReadOnly<EntityControlData>() });

            // Patch CarMoveSystem to prevent it from affecting the controlled vehicle.
            // This ensures that entities with EntityControlData are not processed by the vanilla CarMoveSystem,
            // as their movement is handled by the mod.
            PatchSystemHelper.PatchSystemQuery<CarMoveSystem>(
                allTypes: new[]
                {
                    ComponentType.ReadOnly<Game.Vehicles.Car>(),
                    ComponentType.ReadOnly<Game.Simulation.UpdateFrame>(),
                    ComponentType.ReadWrite<Game.Objects.Transform>(),
                    ComponentType.ReadWrite<Game.Objects.Moving>(),
                    ComponentType.ReadWrite<Game.Objects.TransformFrame>(),
                },
                noneTypes: new[] { ComponentType.ReadOnly<Game.Common.Deleted>(), ComponentType.ReadOnly<Game.Tools.Temp>(), ComponentType.ReadOnly<Game.Objects.TripSource>(), ComponentType.ReadOnly<Game.Vehicles.OutOfControl>(), ComponentType.ReadOnly<EntityControlData>() });

            var toggleControlAction = this.Setting.GetAction(ToggleControlEntityActionName);
            if (toggleControlAction != null)
            {
                toggleControlAction.shouldBeEnabled = true;

                // Ensure only one subscription by unsubscribing first
                toggleControlAction.onInteraction -= this.OnToggleControlInteraction;
                toggleControlAction.onInteraction += this.OnToggleControlInteraction;
            }
            else
            {
                LOG.Warn("Toggle control action not available — skipping activation");
            }

            var respawnAction = this.Setting.GetAction(ButtonRespawnActionName);
            if (respawnAction != null)
            {
                respawnAction.shouldBeEnabled = true;

                // Ensure only one subscription by unsubscribing first
                respawnAction.onInteraction -= this.OnRespawnInteraction;
                respawnAction.onInteraction += this.OnRespawnInteraction;
            }
            else
            {
                LOG.Warn("Respawn action not available — skipping activation");
            }
        }

        /// <summary>
        /// Handles the interaction for the toggle control action.
        /// Switches between controlling an entity and releasing control based on the current tool state.
        /// </summary>
        /// <param name="action">The proxy action that triggered the interaction.</param>
        /// <param name="phase">The phase of the input action (e.g., Started, Performed, Canceled).</param>
        private void OnToggleControlInteraction(ProxyAction action, InputActionPhase phase)
        {
            if (phase != InputActionPhase.Started)
            {
                return;
            }

            if (this.toolSystem.activeTool == this.controlToolSystem)
            {
                this.ExitControl();
            }
            else
            {
                this.TakeControl();
            }
        }

        // Respawn is not currently implemented fully todo

        /// <summary>
        /// Handles the interaction event for the respawn action.
        /// Triggers the respawn logic for the currently controlled entity if the control tool is active.
        /// </summary>
        /// <param name="action">The proxy action that triggered the interaction.</param>
        /// <param name="phase">The phase of the input action (e.g., Started, Performed, Canceled).</param>
        private void OnRespawnInteraction(ProxyAction action, InputActionPhase phase)
        {
            if (phase != InputActionPhase.Started)
            {
                return;
            }

            if (this.toolSystem.activeTool == this.controlToolSystem)
            {
                this.controlToolSystem.RespawnEntity();
            }
            else
            {
                LOG.Info("[OnRespawnInteraction] No vehicle is currently controlled to respawn");
            }
        }

        /// <summary>
        /// Attempts to take manual control of the currently selected entity.
        /// Validates the entity, determines its original state (type, size, etc.),
        /// adds custom control components, removes conflicting vanilla components,
        /// and switches the active tool to the custom control system.
        /// </summary>
        private void TakeControl()
        {
            // Capture the currently selected entity before any clearing actions
            var entity = this.toolSystem.selected;

            // Validate entity: Must exist, be a Car, and not be destroyed or already involved in an accident.
            if (entity == Entity.Null || !EntityManager.Exists(entity) || !EntityManager.HasComponent<Game.Vehicles.Car>(entity) || EntityManager.HasComponent<Game.Common.Destroyed>(entity) || EntityManager.HasComponent<Game.Events.InvolvedInAccident>(entity))
            {
                LOG.Info("[TakeControl] Blocked, entity is invalid or destroyed or not a car, or already in an accident.");
                return;
            }

            // Prevent taking control if already controlled by this mod
            if (this.toolSystem.activeTool == this.controlToolSystem && this.savedControlledEntity != Entity.Null && this.savedControlledEntity == entity)
            {
                LOG.Warn($"[TakeControl] Blocked: entity {entity.Index}:{entity.Version} is already controlled by this mod. Cannot take over again.");
                return;
            }

            // Prevent taking control if already OutOfControl by another mod/system (and not by this mod)
            if (EntityManager.HasComponent<Game.Vehicles.OutOfControl>(entity) && !EntityManager.HasComponent<EntityControlData>(entity))
            {
                LOG.Warn($"[TakeControl] Blocked: entity {entity.Index}:{entity.Version} is already OutOfControl, likely by another mod. Cannot take over.");
                return;
            }

            LOG.Info($"[TakeControl] Started for entity {entity.Index}:{entity.Version}");

            this.savedControlledEntity = entity;

            // Determine and store original entity types and states using the new helper.
            var newControlData = EntityDataHelper.DetermineEntityControlData(EntityManager, entity, this.prefabSystem);

            // Assign the newly created struct to savedControlData
            this.savedControlData = newControlData;
            this.controlToolSystem.SetOriginalComponentList(entity);

            // Apply the necessary components to the entity for manual control.
            ControlActivatorHelper.ApplyControlComponents(EntityManager, entity, this.savedControlData);

            // Set the static ControlData in Mod for Mod's own logic (e.g., ExitControl) to access
            Mod.controlData = this.savedControlData;

            // Inform CameraControlSystem to manage camera.
            this.cameraControlSystem.SetControlledEntity(entity);
            this.cameraControlSystem.OnTakeControl();

            // Switch the current tool to ControlToolSystem and enable input.
            this.controlToolSystem.SetTarget(entity);
            this.controlToolSystem.SetDrivingState();
            this.toolSystem.activeTool = this.controlToolSystem;
            this.InputHelper.Enable();
        }
    }
}
