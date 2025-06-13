// <copyright file="CameraControlSystem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Systems
{
    using Game;
    using Game.Rendering;
    using Game.UI.InGame;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using VehicleDriver.Settings;

    /// <summary>
    /// A system responsible for managing camera behavior during manual vehicle control.
    /// This includes setting custom camera positions, rotations, and restoring original camera states.
    /// </summary>
    public partial class CameraControlSystem : GameSystemBase
    {
        private Setting setting;
        private Entity controlledEntity = Entity.Null;
        private IGameCameraController originalActiveCameraController;
        private CameraUpdateSystem cameraUpdateSystem;

        /// <summary>
        /// Sets the settings instance for this camera control system.
        /// </summary>
        /// <param name="newSetting">The <see cref="Settings.Setting"/> instance from the mod.</param>
        internal void SetSetting(Setting newSetting)
        {
            this.setting = newSetting;
        }

        /// <summary>
        /// Sets the entity that the camera should follow or reference.
        /// </summary>
        /// <param name="entity">The controlled entity.</param>
        internal void SetControlledEntity(Entity entity)
        {
            this.controlledEntity = entity;
        }

        /// <summary>
        /// Determines whether the camera is currently in a custom control mode.
        /// </summary>
        /// <returns>True if the camera is in custom mode, false otherwise.</returns>
        internal bool IsInCustomCameraMode()
        {
            return this.setting.ModeDropdown == Setting.CameraModeEnum.CameraCustom;
        }

        /// <summary>
        /// Adjusts the game camera based on the selected camera mode when control is taken.
        /// </summary>
        internal void OnTakeControl()
        {
            if (Camera.main == null || SelectedInfoUISystem.s_CameraController == null || this.cameraUpdateSystem == null || this.setting == null)
            {
                Mod.LOG.Warn("[CameraControlSystem.OnTakeControl] Required camera systems or settings not available.");
                return;
            }

            if (this.setting.ModeDropdown == Setting.CameraModeEnum.CameraVanillaFollow)
            {
                SelectedInfoUISystem.s_CameraController.followedEntity = this.controlledEntity;
                SelectedInfoUISystem.s_CameraController.mode = OrbitCameraController.Mode.PhotoMode;

                // Match position to the current game camera for smooth transition
                SelectedInfoUISystem.s_CameraController.TryMatchPosition(this.cameraUpdateSystem.activeCameraController);
                this.cameraUpdateSystem.activeCameraController = SelectedInfoUISystem.s_CameraController;
                Mod.LOG.Info("[CameraControlSystem.OnTakeControl] Switched to Vanilla Follow Camera.");
            }
            else if (this.setting.ModeDropdown == Setting.CameraModeEnum.CameraCustom)
            {
                // Storing originalCameraPosition and originalCameraRotation here, though not directly used for restoration.
                // Their values are overwritten if control is retaken without exiting.
                this.originalActiveCameraController = this.cameraUpdateSystem.activeCameraController;

                if (this.originalActiveCameraController is MonoBehaviour monoBehaviour)
                {
                    monoBehaviour.enabled = false;
                    Mod.LOG.Info($"[CameraControlSystem.OnTakeControl] Disabled original active camera controller: {monoBehaviour.GetType().Name}");
                }
                else
                {
                    Mod.LOG.Warn("[CameraControlSystem.OnTakeControl] Original active camera controller is not a MonoBehaviour or couldn't be disabled.");
                }

                // Set the active camera controller to null to prevent conflicts with direct camera manipulation.
                this.cameraUpdateSystem.activeCameraController = null;

                // Disable input for other camera controllers
                if (this.cameraUpdateSystem.gamePlayController != null)
                {
                    this.cameraUpdateSystem.gamePlayController.inputEnabled = false;
                }

                Mod.LOG.Info("[CameraControlSystem.OnTakeControl] Switched to direct camera control (Custom Camera).");
            }
            else if (this.setting.ModeDropdown == Setting.CameraModeEnum.CameraVanillaFree)
            {
                this.originalActiveCameraController = this.cameraUpdateSystem.activeCameraController; // Save whatever was active
                this.cameraUpdateSystem.activeCameraController = this.cameraUpdateSystem.gamePlayController; // Set to gameplay controller

                // Ensure the gameplay controller's input is enabled for free camera movement
                if (this.cameraUpdateSystem.gamePlayController != null)
                {
                    this.cameraUpdateSystem.gamePlayController.inputEnabled = true;
                }

                // Try to set the gameplay camera to a reasonable "free" starting point above the entity.
                if (this.controlledEntity != Entity.Null && this.EntityManager.HasComponent<Game.Objects.Transform>(this.controlledEntity))
                {
                    var vehicleTransform = this.EntityManager.GetComponentData<Game.Objects.Transform>(this.controlledEntity);
                    this.cameraUpdateSystem.gamePlayController.pivot = vehicleTransform.m_Position + new float3(0, 5f, 0); // Example pivot above vehicle
                    this.cameraUpdateSystem.gamePlayController.zoom = 25f; // Example zoom
                    this.cameraUpdateSystem.gamePlayController.rotation = new Vector3(45f, 0, 0); // Example angle (looking down)
                }

                Mod.LOG.Info("[CameraControlSystem.OnTakeControl] Switched to Vanilla Free Camera.");
            }
        }

        /// <summary>
        /// Restores the camera to its original state when control is released.
        /// </summary>
        internal void OnExitControl()
        {
            if (Camera.main == null || this.cameraUpdateSystem == null)
            {
                Mod.LOG.Warn("[CameraControlSystem.OnExitControl] Required camera systems not available. Skipping camera restore.");
                return;
            }

            var currentCameraPosition = Camera.main.transform.position;
            var currentCameraRotation = Camera.main.transform.rotation;
            var desiredEulerRotation = currentCameraRotation.eulerAngles;

            Vector3 finalPivot;
            float finalZoom;

            // Determine pivot and zoom based on entity validity.
            if (this.EntityManager.Exists(this.controlledEntity))
            {
                Vector3 vehiclePosition = this.EntityManager.GetComponentData<Game.Objects.Transform>(this.controlledEntity).m_Position;
                finalZoom = Vector3.Distance(currentCameraPosition, vehiclePosition);
                finalPivot = vehiclePosition;
            }
            else
            {
                Mod.LOG.Warn($"[CameraControlSystem.OnExitControl] Controlled vehicle {this.controlledEntity.Index}:{this.controlledEntity.Version} no longer exists or has no GameTransform. Restoring camera to a default free position.");
                finalZoom = 50f; // Default zoom
                finalPivot = currentCameraPosition + (currentCameraRotation * new Vector3(0, 0, finalZoom)); // Pivot in front of the camera
            }

            if (this.originalActiveCameraController != null && this.cameraUpdateSystem != null)
            {
                if (this.originalActiveCameraController is MonoBehaviour monoBehaviour)
                {
                    monoBehaviour.enabled = true;
                }

                this.cameraUpdateSystem.activeCameraController = this.originalActiveCameraController;

                if (this.originalActiveCameraController is OrbitCameraController orbitCam)
                {
                    orbitCam.pivot = finalPivot;
                    orbitCam.zoom = finalZoom;
                    orbitCam.rotation = desiredEulerRotation;
                    orbitCam.followedEntity = this.controlledEntity; // Attempt to re-link to the entity for orbit cam
                    orbitCam.mode = OrbitCameraController.Mode.PhotoMode; // Ensure it's in follow mode
                }
                else
                {
                    this.cameraUpdateSystem.activeCameraController.pivot = finalPivot;
                    this.cameraUpdateSystem.activeCameraController.zoom = finalZoom;
                    this.cameraUpdateSystem.activeCameraController.rotation = desiredEulerRotation;
                }

                if (this.cameraUpdateSystem.gamePlayController != null)
                {
                    this.cameraUpdateSystem.gamePlayController.inputEnabled = true;
                }

                Mod.LOG.Info("[CameraControlSystem.OnExitControl] Restored original camera controller.");
            }

            // Fallback for when no originalActiveCameraController was saved, or it's invalid.
            else if (this.cameraUpdateSystem.gamePlayController != null)
            {
                if (SelectedInfoUISystem.s_CameraController is { } orbitCam)
                {
                    orbitCam.followedEntity = Entity.Null; // Ensure vanilla orbit camera stops following
                }

                this.cameraUpdateSystem.activeCameraController = this.cameraUpdateSystem.gamePlayController;
                this.cameraUpdateSystem.gamePlayController.pivot = finalPivot;
                this.cameraUpdateSystem.gamePlayController.zoom = finalZoom;
                this.cameraUpdateSystem.gamePlayController.rotation = desiredEulerRotation;

                if (this.cameraUpdateSystem.gamePlayController != null)
                {
                    this.cameraUpdateSystem.gamePlayController.inputEnabled = true;
                }

                Mod.LOG.Info("[CameraControlSystem.OnExitControl] Restored to default gameplay camera.");
            }
            else
            {
                Mod.LOG.Warn("[CameraControlSystem.OnExitControl] No camera controller available to restore.");
            }

            this.originalActiveCameraController = null; // Clear the saved reference after restoration
            this.controlledEntity = Entity.Null; // Clear the controlled entity reference
        }

        /// <summary>
        /// Called when the system is created. Initializes internal state and queries.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            this.cameraUpdateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraUpdateSystem>();
            Mod.LOG.Info("[CameraControlSystem] OnCreate: CameraControlSystem created.");
        }

        /// <summary>
        /// Called when the system is destroyed. Performs cleanup.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Mod.LOG.Info("[CameraControlSystem] OnDestroy: CameraControlSystem destroyed.");
        }

        /// <summary>
        /// Called every frame to update the system. Handles different tool states and schedules the driving job.
        /// </summary>
        protected override void OnUpdate()
        {
            // Only update the camera if we are in custom camera mode and there's a controlled entity.
            if (this.setting is { ModeDropdown: Setting.CameraModeEnum.CameraCustom } && this.controlledEntity != Entity.Null)
            {
                this.UpdateCustomCamera(SystemAPI.Time.DeltaTime);
            }
        }

        /// <summary>
        /// Updates the custom camera position and rotation.
        /// This method should be called every frame when the custom camera mode is active.
        /// </summary>
        /// <param name="dt">Delta time for frame-rate independent calculations.</param>
        private void UpdateCustomCamera(float dt)
        {
            if (this.controlledEntity == Entity.Null || !this.EntityManager.Exists(this.controlledEntity) || Camera.main == null || this.setting == null)
            {
                return;
            }

            // Ensure that the entity has a Transform component before trying to access it.
            if (!this.EntityManager.HasComponent<Game.Objects.Transform>(this.controlledEntity))
            {
                Mod.LOG.Warn($"[CameraControlSystem.UpdateCustomCamera] Controlled entity {this.controlledEntity.Index}:{this.controlledEntity.Version} missing Transform component. Cannot update custom camera.");
                return;
            }

            var vehicleTransform = this.EntityManager.GetComponentData<Game.Objects.Transform>(this.controlledEntity);
            Vector3 vehiclePosition = vehicleTransform.m_Position;
            Quaternion vehicleRotation = vehicleTransform.m_Rotation;

            var cameraOffset = new Vector3(0f, this.setting.CameraOffsetY, -this.setting.CameraOffsetZ);
            var desiredCameraPosition = vehiclePosition + (vehicleRotation * cameraOffset);

            // Camera smoothing is now directly proportional to the respective input sensitivities.
            // The CameraPositionLerpSpeed and CameraRotationLerpSpeed act as base multipliers.
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, desiredCameraPosition, this.setting.CameraPositionLerpSpeed * this.setting.GasSensitivity * dt);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, vehicleRotation, this.setting.CameraRotationLerpSpeed * this.setting.SteeringSensitivity * dt);
        }
    }
}
