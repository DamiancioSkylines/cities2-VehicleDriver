// <copyright file="ControlSystem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Systems
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Game;
    using Game.Objects;
    using Game.Simulation;
    using Game.Tools;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using VehicleDriver.Components;
    using VehicleDriver.Enums;
    using VehicleDriver.Helpers;
    using VehicleDriver.Settings;

    /// <summary>
    /// Represents a custom tool system for manually controlling vehicles within the game.
    /// Inherited from <see cref="ToolBaseSystem"/> to provide a custom tool functionality
    /// tailored for driving simulation and allows easy way to hide highlight outline over entities.
    /// </summary>
    public partial class ControlSystem : GameSystemBase
    {
        /// <summary>
        /// Represents the settings configuration for the control tool within the vehicle driving mod.
        /// Manages UI, keybindings, and input configurations used for vehicle control systems.
        /// This field is marked as internal to allow direct assignment from the <see cref="Mod"/> class
        /// during initialization, which is necessary for the tool to access user-defined settings.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This field needs to be internal for direct assignment from the Mod class and access by other methods within this system for proper functionality.")]
        internal Setting Setting;

        // Internal constants for simplified physics parameters.
        private const float CSteeringResponse = 15f; // Increased for snappier steering
        private const float CRotationalDrag = 5f;    // Increased to reduce low-speed drifting
        private const float CDriftActivationSpeed = 5f; // Speed threshold for drift to activate

        private Entity entity;
        private float angularVelocity;
        private float gasAnalog;
        private float revAnalog;
        private float steerAnalog;

        private List<string> originalComponentList; // Stores components before control was taken.
        [JetBrains.Annotations.UsedImplicitly] // Marked as used implicitly to suppress warnings.
        private List<string> beforeComponentList; // Stores components when OnStartRunning is called.
        private List<string> afterComponentList;  // Stores components when OnStopRunning is called.

        // Fields to store determined EntityType, VehicleState, VehicleType, CarType, CarSize, TrainType, WatercraftType, AircraftType.
        private EntityType entityType;
        private VehicleState vehicleState;
        private VehicleDriver.Enums.VehicleType vehicleType;
        private CarType carType;
        private CarSize carSize;
        private TrainType trainType;
        private WatercraftType watercraftType;
        private AircraftType aircraftType;

        // Field to store the EntityControlData for the currently controlled entity (non-static instance).
        private EntityControlData currentEntityControlData;

        private EntityQuery tempQuery; // Used in GetAllowApply, likely for preventing tool usage on temporary entities.

        private SimulationSystem simulationSystem; // Added for checking pause state

        /// <summary>
        /// Represents the various operational states of the control tool system.
        /// Only Driving and Default are currently implemented.
        /// </summary>
        /// <remarks>
        /// The <c>State</c> enum is used to define the current operational mode of the
        /// control tool system. It allows for switching between different modes such as
        /// driving, walking, and flying, or remaining in a default idle state.
        /// </remarks>
        public enum State
        {
            /// <summary>
            /// Represents the default or idle state of the system.
            /// </summary>
            /// <remarks>
            /// The <c>Default</c> state indicates that no specific operation or mode is active.
            /// This state is typically used as the initial or fallback state within the <c>State</c> enumeration.
            /// </remarks>
            Default,

            /// <summary>
            /// Represents the state of manually driving a vehicle within the control tool system.
            /// </summary>
            /// <remarks>
            /// This state indicates that the user has initiated manual vehicle operation,
            /// enabling direct control over the vehicle's navigation and behaviour.
            /// The <see cref="State.Driving"/> state is typically active when the player
            /// assumes direct control for precise maneuvering or specific gameplay scenarios.
            /// </remarks>
            Driving,

            /// <summary>
            /// Represents the state in which the system enables movement on foot.
            /// </summary>
            /// <remarks>
            /// The <c>Walking</c> state is used when the system transitions to pedestrian movement,
            /// allowing entities to navigate or operate in walking mode.
            /// </remarks>
            Walking,

            /// <summary>
            /// Represents the operational state where the vehicle is in flight mode.
            /// </summary>
            /// <remarks>
            /// This state signifies that the system is actively controlling a flying vehicle.
            /// In this mode, flight-specific mechanics, such as elevation and forward motion in three-dimensional space,
            /// are engaged and managed by the control system.
            /// </remarks>
            Flying,
        }

        /// <summary>
        /// Gets allow public.
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Sets the original list of components associated with a specified <paramref name="targetEntity"/>.
        /// Captures the current state of components for future reference or restoration purposes.
        /// This is typically called before any modifications are made to the <paramref name="targetEntity"/>.
        /// </summary>
        /// <param name="targetEntity">The entity whose components are to be recorded and stored in the original list.</param>
        public void SetOriginalComponentList(Entity targetEntity)
        {
            this.originalComponentList = ComponentLogHelper.ListComponents(this.EntityManager, targetEntity);
        }

        /// <summary>
        /// Retrieves the current target entity for this tool.
        /// The target is the entity that the tool is currently interacting with or controlling.
        /// </summary>
        /// <returns>The entity currently assigned as the target for the tool.</returns>
        public Entity GetTarget() => this.entity;

        /// <summary>
        /// Sets the target entity for the control tool system and initializes related state variables.
        /// This method prepares the control tool system to interact with the specified entity,
        /// resetting any prior state to ensure proper operation.
        /// </summary>
        /// <param name="targetEntity">The entity to be controlled by the system.</param>
        public void SetTarget(Entity targetEntity)
        {
            this.entity = targetEntity;

            // this.hasPreviousFrame = false;
            this.angularVelocity = 0f;
            this.gasAnalog = 0f;
            this.revAnalog = 0f;
            this.steerAnalog = 0f;

            // this.previousTransform = default;
            // this.previousMoving = default;
        }

        /// <summary>
        /// Respawns the associated entity and resets its physical state to a default configuration.
        /// Ensures the entity has the required components (e.g., <see cref="Game.Objects.Transform"/> and <see cref="Moving"/>)
        /// and adjusts its rotation, velocity, and control signals to initial values.
        /// Preserves the entity's forward direction on the XZ plane while maintaining the original Y-axis (yaw) rotation.
        /// Updates the game engine to reflect the changes and logs the result for debugging purposes.
        /// </summary>
        public void RespawnEntity()
        {
            if (!this.EntityManager.Exists(this.entity))
            {
                Mod.LOG.Warn($"[RespawnEntity] Cannot respawn: Entity {this.entity.Index}:{this.entity.Version} does not exist.");
                return;
            }

            // Ensure Moving Component is present
            ComponentHelper.SafeAddComponent<Game.Objects.Moving>(this.EntityManager, this.entity);

            var transform = this.EntityManager.GetComponentData<Game.Objects.Transform>(this.entity);
            var moving = this.EntityManager.GetComponentData<Game.Objects.Moving>(this.entity);

            // Preserve the current Y-axis rotation (yaw), but reset X (pitch) and Z (roll)
            var currentRotation = transform.m_Rotation;
            var currentForward = math.mul(currentRotation, new float3(0, 0, 1));
            currentForward.y = 0; // Flatten the forward vector to the XZ plane
            currentForward = math.normalize(currentForward);

            // Set rotation to look along the flattened forward vector, with Y-axis as up
            transform.m_Rotation = quaternion.LookRotation(currentForward, new float3(0, 1, 0));

            // Reset velocity and angular velocity to zero
            moving.m_Velocity = float3.zero;
            moving.m_AngularVelocity = float3.zero;
            this.angularVelocity = 0f;
            this.gasAnalog = 0f;
            this.revAnalog = 0f;
            this.steerAnalog = 0f;

            this.EntityManager.SetComponentData(this.entity, transform);
            this.EntityManager.SetComponentData(this.entity, moving);

            // Ensure the game engine processes this change
            ComponentHelper.SafeAddComponent<Game.Common.Updated>(this.EntityManager, this.entity);
            Mod.LOG.Info($"[RespawnEntity] Entity {this.entity.Index}:{this.entity.Version} respawned.");
        }

        /// <summary>
        /// Updates the tool system to enter the driving state.
        /// Transitions the current state to indicate that the vehicle is being driven manually.
        /// Enables functionality specific to manual vehicle control.
        /// </summary>
        public void SetDrivingState()
        {
            this.CurrentState = State.Driving;
        }

        /// <summary>
        /// Transitions the control tool system to the walking state.
        /// Sets the tool's state to <see cref="State.Walking"/> to indicate that the controlled entity
        /// is intended to behave as if it is walking within the game.
        /// This modifies the internal state to reflect the walking behaviour mode.
        /// </summary>
        public void SetWalkingState()
        {
            this.CurrentState = State.Walking;
        }

        /// <summary>
        /// Sets the control tool system's state to the "Flying" state.
        /// Updates the internal state of the tool system to represent the flying mode,
        /// which may alter the behaviour or functionality of the vehicle control system.
        /// This method directly modifies the state to ensure the appropriate conditions
        /// for flying mode are established.
        /// </summary>
        public void SetFlyingState()
        {
            this.CurrentState = State.Flying;
        }

        /// <summary>
        /// Resets the tool system's state to the default configuration.
        /// Ensures the system is returned to its initial or neutral state for consistent behaviour.
        /// </summary>
        public void SetDefaultState()
        {
            this.CurrentState = State.Default;
        }

        /// <summary>
        /// Called when the system is created. Initializes internal state and queries.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Mod.LOG.Info("[ControlSystem] OnCreate: ControlSystem created.");

            this.originalComponentList = new List<string>();
            this.beforeComponentList = new List<string>();
            this.afterComponentList = new List<string>();

            // Initialize tempQuery to get temporary entities, often used to prevent interaction with them.
            this.tempQuery = this.GetEntityQuery(ComponentType.ReadOnly<Game.Tools.Temp>());

            // Get SimulationSystem to check for pause state
            this.simulationSystem = this.World.GetOrCreateSystemManaged<SimulationSystem>();
        }

        /// <summary>
        /// Called when the system starts running. Retrieves control data and logs initial component state.
        /// </summary>
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            // Retrieve control data from the entity once the tool starts running.
            // Store it in the instance-specific field.
            this.currentEntityControlData = this.EntityManager.GetComponentData<EntityControlData>(this.entity);

            // Log the determined entity types and states for debugging.
            this.entityType = this.currentEntityControlData.EntityType;
            this.vehicleState = this.currentEntityControlData.VehicleState;
            this.vehicleType = this.currentEntityControlData.VehicleType;
            this.carType = this.currentEntityControlData.CarType;
            this.carSize = this.currentEntityControlData.CarSize;
            this.trainType = this.currentEntityControlData.TrainType;
            this.watercraftType = this.currentEntityControlData.WatercraftType;
            this.aircraftType = this.currentEntityControlData.AircraftType;
            Mod.LOG.Info($"[ControlSystem] OnStartRunning: Target entity {this.entity.Index}:{this.entity.Version} EntityType: {this.entityType} VehicleState: {this.vehicleState} VehicleType: {this.vehicleType} CarType: {this.carType} CarSize: {this.carSize} TrainType: {this.trainType} WatercraftType: {this.watercraftType} AircraftType: {this.aircraftType}");

            // Capture components before any further modifications in this running cycle for comparison.
            this.beforeComponentList = ComponentLogHelper.ListComponents(this.EntityManager, this.entity);
        }

        /// <summary>
        /// Called when the system stops running. Cleans up control-related components and logs component changes.
        /// </summary>
        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            // Always remove mod-specific control components upon exiting control. better at the end of the changes.
            ComponentHelper.SafeRemoveComponent<VehicleDriver.Components.EntityControlData>(this.EntityManager, this.entity);

            // Mark entity as Updated to ensure game systems process its new state after control is released.
            ComponentHelper.SafeAddComponent<Game.Common.Updated>(this.EntityManager, this.entity);

            // Capture components after modifications for comparison and logging.
            this.afterComponentList = ComponentLogHelper.ListComponents(this.EntityManager, this.entity);

            // Compare component lists to track changes.
            ComponentLogHelper.CompareListComponents("[ControlSystem] Original vs OnStopRunning,      ", this.originalComponentList, this.afterComponentList);
        }

        /// <summary>
        /// Called every frame to update the system. Handles different tool states and schedules the driving job.
        /// </summary>
        protected override void OnUpdate() // This was not working
        {
            // 1. Grab the current dependency handle from the system
            var inputDeps = this.Dependency;

            // If the simulation is paused, prevent driving input from being processed.
            if (this.simulationSystem.selectedSpeed == 0.0f)
            {
                // Optionally, reset input analogue values to prevent "sticky" input if unpaused later
                this.gasAnalog = 0f;
                this.revAnalog = 0f;
                this.steerAnalog = 0f;
                return; // Stop further processing in this frame if paused
            }

            // Allow driving planes helicopters and rockets to be controlled later TODO
            switch (this.CurrentState)
            {
                case State.Default:
                    break;
                case State.Driving:
                    // JobHandle jobHandle = this.Drive(inputDeps);
                    this.Driving();
                    return;

                    // case State.Walking: // Not yet implemented
                    // case State.Flying: // Not yet implemented
            }

            this.Dependency = inputDeps;
        }

        /// <summary>
        /// Implements the core driving logic, applying player input to the controlled entity's physics.
        /// This method calculates forces and updates the entity's position, rotation, and velocity based on input and physics settings.
        /// </summary>
        protected void Driving()
        {
            if (this.entity == Entity.Null)
            {
                return;
            }

            if (!this.EntityManager.Exists(this.entity))
            {
                Mod.LOG.Info($"[ControlSystem] OnUpdate: Target entity {this.entity.Index}:{this.entity.Version} no longer exists. Exiting.");
                Mod.Instance.ExitControlCleanup(false);
                return;
            }

            if (this.Setting == null)
            {
                Mod.LOG.Warn("[ControlSystem] OnUpdate: m_Setting is NULL. This should not happen if initialized correctly. Attempting to get from Mod.Instance.");

                this.Setting = Mod.Instance?.Setting;
                if (this.Setting == null)
                {
                    Mod.LOG.Critical("[ControlSystem] OnUpdate: m_Setting is still NULL after fallback attempt. Cannot apply movement. Exiting update.");
                    return; // Prevent NRE
                }
            }

            // Ensure the target vehicle has a GameTransform and Moving component for position and velocity.
            ComponentHelper.SafeAddComponent<Transform>(this.EntityManager, this.entity);
            ComponentHelper.SafeAddComponent<Moving>(this.EntityManager, this.entity);

            // Get current transform and moving components.
            var currentMoving = this.EntityManager.GetComponentData<Moving>(this.entity);
            var currentTransform = this.EntityManager.GetComponentData<Transform>(this.entity);

            // Delta time for frame-rate independent calculations.
            var deltaTime = SystemAPI.Time.DeltaTime;

            // Get raw input values from the InputHelper.
            // Read the current RAW digital state of the axis inputs (0, 1, or -1)
            var inputHandler = Mod.Instance.InputHelper;
            var rawGasBrake = inputHandler.GasBrakeAction.ReadValue<float>();
            var rawSteer = inputHandler.SteerAction.ReadValue<float>();
            var handbrakeActive = inputHandler.HandbrakeAction.IsPressed();

            // Calculate forward speed.
            var fwdSpeed = math.dot(currentMoving.m_Velocity, math.mul(currentTransform.m_Rotation, new float3(0, 0, 1)));

            // Reset analogue ramp values if the car is stopped and no input is given.
            if (math.abs(fwdSpeed) < 0.1f && math.abs(rawGasBrake) < 0.01f)
            {
                this.gasAnalog = 0f;
                this.revAnalog = 0f;
            }

            // Separate raw gas and raw brake/reverse inputs
            // Only positive part for gas
            // Only negative part for brake/reverse
            var rawGas = math.max(0f, rawGasBrake);
            var rawBrakeRev = math.min(0f, rawGasBrake);

            // Smooth the gas input
            this.gasAnalog = math.lerp(this.gasAnalog, rawGas, deltaTime * (rawGas > 0.01f ? this.Setting.AnalogRampUpSpeed : this.Setting.AnalogRampDownSpeed));

            // Smooth the reverse input
            this.revAnalog = math.lerp(this.revAnalog, rawBrakeRev, deltaTime * (rawBrakeRev < -0.01f ? this.Setting.AnalogRampUpSpeed : this.Setting.AnalogRampDownSpeed));

            // Smooth the steering input using new dedicated settings
            this.steerAnalog = math.lerp(this.steerAnalog, rawSteer, deltaTime * (math.abs(rawSteer) > 0.01f ? this.Setting.AnalogSteerRampUpSpeed : this.Setting.AnalogSteerRampDownSpeed));

            // Determine the effective gas/brake/reverse value to use
            float effGasBrake;

            if (rawGasBrake > 0)
            {
                // User is pressing gas (always analogue)
                effGasBrake = this.gasAnalog;
            }
            else if (rawGasBrake < 0)
            {
                // User is pressing brake/reverse
                // If the vehicle is moving forward (forward speed > 0.1f), the input is interpreted as braking.
                // Otherwise (if stopped or moving backward), the input is used for reverse acceleration, applying an analogue ramp-up/down effect.
                effGasBrake = fwdSpeed > 0.1f ? rawGasBrake : this.revAnalog;
            }
            else
            {
                // No gas/brake input
                effGasBrake = 0f;
            }

            // If handbrake is active, force effective input to be non-positive for braking/sliding.
            if (handbrakeActive)
            {
                effGasBrake = math.min(effGasBrake, 0f);
            }

            // Current forward and right directions of the car derived from its rotation quaternion
            var forward = math.mul(currentTransform.m_Rotation, new float3(0, 0, 1));
            var right = math.mul(currentTransform.m_Rotation, new float3(1, 0, 0));

            // Calculate lateral speed.
            var latSpeed = math.dot(currentMoving.m_Velocity, right);

            // Friction Factors (Unified Grip System) based on settings.
            var baseLongFriction = this.Setting.OverallGrip;
            var baseLatFriction = this.Setting.OverallGrip;

            var effLongFriction = baseLongFriction;
            var effLatFriction = baseLatFriction;

            // Apply Handbrake Logic: reduces lateral and longitudinal friction.
            if (handbrakeActive)
            {
                // When handbrake is active, lateral friction is significantly reduced to simulate locked wheels.
                effLatFriction *= this.Setting.HandbrakeSlideFactor;

                // Significantly reduce longitudinal friction during handbrake to simulate loss of traction
                effLongFriction *= this.Setting.HandbrakeBrakingFactor;

                // Damp angular velocity and increase lateral friction at very low speeds during handbrake.
                // Simply Reduce excessive spinning at low speeds with handbrake
                if (math.abs(fwdSpeed) < 1.0f)
                {
                    effLatFriction *= 2.0f;
                    this.angularVelocity = math.lerp(this.angularVelocity, 0f, deltaTime * 20f);
                }

                // Apply general braking power for handbrake deceleration
                fwdSpeed = math.max(0f, fwdSpeed - (this.Setting.BrakingPower * deltaTime));
            }

            // Drifting Logic: reduces lateral friction if turning sharply and moving above activation speed.
            var absAngVel = math.abs(this.angularVelocity);
            var absFwdSpeed = math.abs(fwdSpeed);
            if (absFwdSpeed > CDriftActivationSpeed && absAngVel > 0.1f)
            {
                // Invert DriftEffectiveness: higher setting value means more drift (less grip)
                effLatFriction *= 1f - this.Setting.DriftEffectiveness;
            }

            // Acceleration/Braking (scaled by effective longitudinal friction and GasSensitivity).
            var accelSpeedFactor = math.lerp(1f, 1f, math.abs(fwdSpeed) / this.Setting.TopSpeed);
            var accelForce = effGasBrake * this.Setting.Acceleration * accelSpeedFactor;
            if (effGasBrake < 0f)
            {
                accelForce *= this.Setting.ReversePowerMultiplier; // Use setting for reverse power
            }

            // Apply acceleration/braking force, modulated by GasSensitivity AND effective longitudinal friction
            fwdSpeed += accelForce * deltaTime * this.Setting.GasSensitivity * effLongFriction;

            // Apply coasting or braking deceleration based on input.
            if (effGasBrake == 0f && math.abs(fwdSpeed) > 0.1f)
            {
                fwdSpeed = math.lerp(fwdSpeed, 0f, deltaTime * this.Setting.NaturalDeceleration); // Use setting for natural deceleration
            }
            else if (effGasBrake < 0f && fwdSpeed > 0f)
            {
                fwdSpeed = math.max(0f, fwdSpeed - (this.Setting.BrakingPower * deltaTime * effLongFriction)); // Scaled by longitudinal friction
            }
            else if (effGasBrake > 0f && fwdSpeed < 0f)
            {
                fwdSpeed = math.min(0f, fwdSpeed + (this.Setting.BrakingPower * deltaTime * effLongFriction)); // Scaled by longitudinal friction
            }

            // Clamp forward speed to limits (including reverse speed).
            fwdSpeed = math.clamp(fwdSpeed, -10f, this.Setting.TopSpeed);

            // Snap forward speed to zero if very close and no input.
            if (math.abs(fwdSpeed) < 0.1f && effGasBrake == 0f)
            {
                fwdSpeed = 0f;
            }

            // Apply lateral friction to reduce sideways movement
            latSpeed = math.lerp(latSpeed, 0f, deltaTime * effLatFriction);

            // Clamp lateral speed.
            latSpeed = math.clamp(latSpeed, -this.Setting.MaxLateralSpeed, this.Setting.MaxLateralSpeed);

            // Apply speed loss during turning.
            var turnSpeedLoss = math.abs(this.angularVelocity) * math.abs(fwdSpeed) * this.Setting.TurningSpeedLossFactor * deltaTime;
            fwdSpeed -= turnSpeedLoss;

            // Steering and Angular Velocity calculations.
            var effSteer = this.steerAnalog;

            // Apply speed-sensitive steering damping (HighSpeedTurningDamping)
            var speedNorm = math.clamp(math.abs(fwdSpeed) / this.Setting.TopSpeed, 0f, 1f);
            var speedDampFactor = math.lerp(1f, this.Setting.HighSpeedTurningDamping, speedNorm);
            var dampSteerSens = this.Setting.SteeringSensitivity * speedDampFactor;

            // Calculate base steering angle.
            var steerAngleRad = effSteer * dampSteerSens * (math.PI / 180f);

            // Calculate wheelbase-dependent turning rate.
            var wheelbaseTurnRate = 0f;
            if (this.Setting.VehicleWheelbase > 0.01f)
            {
                // This is the core realistic turning based on speed and wheelbase
                wheelbaseTurnRate = (fwdSpeed * math.sin(steerAngleRad)) / this.Setting.VehicleWheelbase;
            }

            // Apply low-speed turning boost as a multiplier to wheelbaseTurnRate
            // Boost factor is 1.0 at or above PivotTurningBlendSpeed, and increases up to LowSpeedTurningBoost as speed approaches 0.
            var lowSpeedBoost = math.lerp(this.Setting.LowSpeedTurningBoost, 1f, math.abs(fwdSpeed) / this.Setting.PivotTurningBlendSpeed);
            lowSpeedBoost = math.clamp(lowSpeedBoost, 1f, this.Setting.LowSpeedTurningBoost); // Ensure it doesn't go below 1.0

            // Apply boost to the wheelbase turn
            var desiredAngularVelocity = wheelbaseTurnRate * lowSpeedBoost;

            // Smooth and damp angular velocity.
            this.angularVelocity = math.lerp(this.angularVelocity, desiredAngularVelocity, deltaTime * CSteeringResponse);
            this.angularVelocity = math.lerp(this.angularVelocity, 0f, deltaTime * CRotationalDrag);

            // Update Vehicle State: velocity and rotation.
            currentMoving.m_Velocity = (forward * fwdSpeed) + (right * latSpeed);
            var rot = quaternion.AxisAngle(math.up(), this.angularVelocity * deltaTime);
            currentTransform.m_Rotation = math.normalize(math.mul(currentTransform.m_Rotation, rot));

            // Set updated components back to the entity
            this.EntityManager.SetComponentData(this.entity, currentTransform);
            this.EntityManager.SetComponentData(this.entity, currentMoving);

            // Mark entity as Updated to ensure game systems process this change.
            // ComponentHelper.SafeAddComponent<Game.Common.Updated>(this.EntityManager, this.entity);
        }

        /// <summary>
        /// Implements the core driving logic, applying player input to the controlled entity's physics.
        /// This method calculates forces and updates the entity's position, rotation, and velocity based on input and physics settings.
        /// It also handles camera control.
        /// </summary>
        /// <param name="inputDeps">The JobHandle representing input dependencies.</param>
        /// <returns>A JobHandle representing the completion of this update cycle's work.</returns>
        private JobHandle Drive(JobHandle inputDeps)
        {
            // Exit if target entity is null or no longer exists.
            if (this.entity == Entity.Null)
            {
                Mod.LOG.Info("[ControlSystem] Drive: Target entity is NULL. Exiting.");
                return inputDeps;
            }

            if (!this.EntityManager.Exists(this.entity))
            {
                Mod.LOG.Info($"[ControlSystem] OnUpdate: Target entity {this.entity.Index}:{this.entity.Version} no longer exists. Exiting.");
                Mod.Instance.ExitControlCleanup(false); // Trigger clean-up as entity is gone.
                return inputDeps;
            }

            // Ensure settings are available. Fallback to Mod.Instance.Setting if not set.
            if (this.Setting == null)
            {
                Mod.LOG.Warn("[ControlSystem] OnUpdate: Setting is NULL. This should not happen if initialized correctly. Attempting to get from Mod.Instance.");
                this.Setting = Mod.Instance?.Setting;
                if (this.Setting == null)
                {
                    Mod.LOG.Critical("[ControlSystem] OnUpdate: Setting is still NULL after fallback attempt. Cannot apply movement. Exiting update.");
                    return inputDeps;
                }
            }

            // Get current transform and moving components.
            var currentMoving = this.EntityManager.GetComponentData<Game.Objects.Moving>(this.entity);
            var currentTransform = this.EntityManager.GetComponentData<Game.Objects.Transform>(this.entity);

            var newMoving = currentMoving; // Work with a copy for modifications
            var newTransform = currentTransform; // Work with a copy for modifications

            if (Mod.Instance == null)
            {
                return inputDeps;
            }

            var dt = SystemAPI.Time.DeltaTime; // Delta time for frame-rate independent calculations.

            // Get raw input values from the InputHelper.
            var inputHandler = Mod.Instance.InputHelper; // Access Mod.Instance.inputHelper (now internal)
            var rawGasBrake = inputHandler.GasBrakeAction.ReadValue<float>();
            var rawSteer = inputHandler.SteerAction.ReadValue<float>();
            var handbrakeActive = inputHandler.HandbrakeAction.IsPressed();

            // Calculate forward speed.
            var fwdSpeed = math.dot(currentMoving.m_Velocity, math.mul(currentTransform.m_Rotation, new float3(0, 0, 1)));

            // Reset analogue ramp values if the car is stopped and no input is given.
            if (math.abs(fwdSpeed) < 0.1f && math.abs(rawGasBrake) < 0.01f)
            {
                this.gasAnalog = 0f;
                this.revAnalog = 0f;
            }

            // Separate gas and brake/reverse inputs.
            var rawGas = math.max(0f, rawGasBrake);
            var rawBrakeRev = math.min(0f, rawGasBrake);

            // Smooth the gas and reverse inputs using analogue ramp speeds from settings.
            this.gasAnalog = math.lerp(this.gasAnalog, rawGas, dt * (rawGas > 0.01f ? this.Setting.AnalogRampUpSpeed : this.Setting.AnalogRampDownSpeed));
            this.revAnalog = math.lerp(this.revAnalog, rawBrakeRev, dt * (rawBrakeRev < -0.01f ? this.Setting.AnalogRampUpSpeed : this.Setting.AnalogRampDownSpeed));

            // Smooth the steering input using dedicated analogue ramp speeds.
            this.steerAnalog = math.lerp(this.steerAnalog, rawSteer, dt * (math.abs(rawSteer) > 0.01f ? this.Setting.AnalogSteerRampUpSpeed : this.Setting.AnalogSteerRampDownSpeed));

            // Determine the effective gas/brake/reverse value based on current speed and input.
            float effGasBrake;
            if (rawGasBrake > 0)
            {
                effGasBrake = this.gasAnalog;
            }
            else if (rawGasBrake < 0)
            {
                // If the vehicle is moving forward (forward speed > 0.1f), the input is interpreted as braking.
                // Otherwise (if stopped or moving backward), the input is used for reverse acceleration, applying an analogue ramp-up/down effect.
                effGasBrake = fwdSpeed > 0.1f ? rawGasBrake : this.revAnalog;
            }
            else
            {
                effGasBrake = 0f; // No input
            }

            // If handbrake is active, force effective input to be non-positive for braking/sliding.
            if (handbrakeActive)
            {
                effGasBrake = math.min(effGasBrake, 0f);
            }

            // Get current forward and right directions from the car's rotation.
            var forward = math.mul(newTransform.m_Rotation, new float3(0, 0, 1));
            var right = math.mul(newTransform.m_Rotation, new float3(1, 0, 0));

            // Calculate lateral speed.
            var latSpeed = math.dot(newMoving.m_Velocity, right);

            // Friction Factors (Unified Grip System) based on settings.
            var baseLongFriction = this.Setting.OverallGrip;
            var baseLatFriction = this.Setting.OverallGrip;

            var effLongFriction = baseLongFriction;
            var effLatFriction = baseLatFriction;

            // Apply Handbrake Logic: reduces lateral and longitudinal friction.
            if (handbrakeActive)
            {
                effLatFriction *= this.Setting.HandbrakeSlideFactor;
                effLongFriction *= this.Setting.HandbrakeBrakingFactor;

                // Damp angular velocity and increase lateral friction at very low speeds during handbrake.
                if (math.abs(fwdSpeed) < 1.0f)
                {
                    effLatFriction *= 2.0f;
                    this.angularVelocity = math.lerp(this.angularVelocity, 0f, dt * 20f);
                }

                // Apply general braking power for handbrake deceleration.
                fwdSpeed = math.max(0f, fwdSpeed - (this.Setting.BrakingPower * dt));
            }

            // Drifting Logic: reduces lateral friction if turning sharply and moving above activation speed.
            var absAngVel = math.abs(this.angularVelocity);
            var absFwdSpeed = math.abs(fwdSpeed);
            if (absFwdSpeed > CDriftActivationSpeed && absAngVel > 0.1f)
            {
                effLatFriction *= 1f - this.Setting.DriftEffectiveness;
            }

            // Acceleration/Braking (scaled by effective longitudinal friction and GasSensitivity).
            var accelSpeedFactor = math.lerp(1f, 1f, math.abs(fwdSpeed) / this.Setting.TopSpeed); // Consider adjusting this
            var accelForce = effGasBrake * this.Setting.Acceleration * accelSpeedFactor;
            if (effGasBrake < 0f)
            {
                accelForce *= this.Setting.ReversePowerMultiplier;
            }

            fwdSpeed += accelForce * dt * this.Setting.GasSensitivity * effLongFriction;

            // Apply coasting or braking deceleration based on input.
            if (effGasBrake == 0f && math.abs(fwdSpeed) > 0.1f)
            {
                fwdSpeed = math.lerp(fwdSpeed, 0f, dt * this.Setting.NaturalDeceleration);
            }
            else if (effGasBrake < 0f && fwdSpeed > 0f)
            {
                fwdSpeed = math.max(0f, fwdSpeed - (this.Setting.BrakingPower * dt * effLongFriction));
            }
            else if (effGasBrake > 0f && fwdSpeed < 0f)
            {
                fwdSpeed = math.min(0f, fwdSpeed + (this.Setting.BrakingPower * dt * effLongFriction));
            }

            // Clamp forward speed to limits (including reverse speed).
            fwdSpeed = math.clamp(fwdSpeed, -10f, this.Setting.TopSpeed);

            // Snap forward speed to zero if very close and no input.
            if (math.abs(fwdSpeed) < 0.1f && effGasBrake == 0f)
            {
                fwdSpeed = 0f;
            }

            // Apply lateral friction to reduce sideways movement.
            latSpeed = math.lerp(latSpeed, 0f, dt * effLatFriction);

            // Clamp lateral speed.
            latSpeed = math.clamp(latSpeed, -this.Setting.MaxLateralSpeed, this.Setting.MaxLateralSpeed);

            // Apply speed loss during turning.
            var turnSpeedLoss = math.abs(this.angularVelocity) * math.abs(fwdSpeed) * this.Setting.TurningSpeedLossFactor * dt;
            fwdSpeed -= turnSpeedLoss;

            // Steering and Angular Velocity calculations.
            var effSteer = this.steerAnalog;

            // Apply speed-sensitive steering damping.
            var speedNorm = math.clamp(math.abs(fwdSpeed) / this.Setting.TopSpeed, 0f, 1f);
            var speedDampFactor = math.lerp(1f, this.Setting.HighSpeedTurningDamping, speedNorm);

            // Calculate damped steering sensitivity
            var dampSteerSens = this.Setting.SteeringSensitivity * speedDampFactor;

            // Calculate base steering angle.
            var steerAngleRad = effSteer * dampSteerSens * (math.PI / 180f);

            // Calculate wheelbase-dependent turning rate.
            var wheelbaseTurnRate = 0f;
            if (this.Setting.VehicleWheelbase > 0.01f)
            {
                wheelbaseTurnRate = fwdSpeed * math.sin(steerAngleRad) / this.Setting.VehicleWheelbase;
            }

            // Apply low-speed turning boost.
            var lowSpeedBoost = math.lerp(this.Setting.LowSpeedTurningBoost, 1f, math.abs(fwdSpeed) / this.Setting.PivotTurningBlendSpeed);
            lowSpeedBoost = math.clamp(lowSpeedBoost, 1f, this.Setting.LowSpeedTurningBoost);

            var desiredAngularVelocity = wheelbaseTurnRate * lowSpeedBoost;

            // Smooth and damp angular velocity.
            this.angularVelocity = math.lerp(this.angularVelocity, desiredAngularVelocity, dt * CSteeringResponse);
            this.angularVelocity = math.lerp(this.angularVelocity, 0f, dt * CRotationalDrag);

            // Update Vehicle State: velocity and rotation.
            newMoving.m_Velocity = (forward * fwdSpeed) + (right * latSpeed);
            var rot = quaternion.AxisAngle(math.up(), this.angularVelocity * dt);
            newTransform.m_Rotation = math.normalize(math.mul(newTransform.m_Rotation, rot));

            // Apply velocity to position.
            // Disabling might fix the animations
            // newTransform.m_Position += newMoving.m_Velocity * dt;

            // Set updated components back to the entity.
            this.EntityManager.SetComponentData(this.entity, newTransform);
            this.EntityManager.SetComponentData(this.entity, newMoving);

            /*
            // Write the current transform to all 4 TransformFrame slots.
            //
            // ObjectInterpolateSystem derives wheel spin from the delta between the
            // STORED InterpolatedTransform (computed and saved last rendering frame) and
            // the NEWLY computed one this frame. Since we update every rendering frame,
            // that delta equals velocity * dt — exactly the correct spin rate.
            //
            // Writing all 4 slots with the same data eliminates the reversed-slot artefact:
            // CalculateUpdateFrames cycles updateFrame1 through 0→1→2→3 every ~3 frames.
            // If [0,1]=prev and [2,3]=curr, then when updateFrame1=3 it reads frames[3]
            // (curr) as "old" and frames[0] (prev) as "new" — reversed — causing wheels
            // to spin backward and Swaying to see a velocity spike every ~12 frames.
            // All-same slots means frame1 == frame2 always, so CalculateTransform returns
            // exactly P_current, and the delta with the stored P_previous is always correct.
            //
            // The m_Velocity field is used by CalculateTransform's Bezier tangent, so
            // including the real velocity preserves interpolation quality.
            if (this.EntityManager.HasBuffer<Game.Objects.TransformFrame>(this.entity))
            {
                var transformFrames = this.EntityManager.GetBuffer<Game.Objects.TransformFrame>(this.entity);

                if (transformFrames.Length != 4)
                {
                    transformFrames.Resize(4, NativeArrayOptions.ClearMemory);
                }

                var currentFrame = new Game.Objects.TransformFrame(newTransform, newMoving);
                transformFrames[0] = currentFrame;
                transformFrames[1] = currentFrame;
                transformFrames[2] = currentFrame;
                transformFrames[3] = currentFrame;
            }*/

            // Mark entity as Updated to ensure game systems process this change.
            ComponentHelper.SafeAddComponent<Game.Common.Updated>(this.EntityManager, this.entity);

            return inputDeps;
        }
    }
}
