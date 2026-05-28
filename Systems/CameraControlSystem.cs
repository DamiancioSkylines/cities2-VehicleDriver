// <copyright file="CameraControlSystem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Systems
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Colossal.Mathematics;
    using Game;
    using Game.Rendering;
    using Game.UI.InGame;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using VehicleDriver.Settings;

    /// <summary>
    /// A system responsible for managing camera behaviour during manual vehicle control.
    /// This includes setting custom camera positions, rotations, and restoring original camera states.
    /// </summary>
    [UpdateAfter(typeof(CameraUpdateSystem))]
    public partial class CameraControlSystem : GameSystemBase
    {
        /// <summary>
        /// The user settings for this mod, assigned directly from the <see cref="Mod"/> class after
        /// initialization. Marked internal because <see cref="Mod"/> sets it on startup.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Assigned directly by Mod on startup.")]
        internal Setting Setting;

        private Entity controlledEntity = Entity.Null;
        private IGameCameraController savedCameraController;
        private CameraUpdateSystem cameraUpdateSystem;

        /// <summary>
        /// Cached world-space camera position, lerped each frame in CameraCustom mode.
        /// </summary>
        private Vector3 smoothedCamPosition;

        /// <summary>
        /// Cached world-space camera rotation, slerped each frame in CameraCustom mode.
        /// </summary>
        private Quaternion smoothedCamRotation;

        /// <summary>
        /// True on the very first frame of CameraCustom control. Used to seed the
        /// smoothed values from the real camera position, preventing an initial jump.
        /// </summary>
        private bool isFirstCustomFrame = true;

        /// <summary>
        /// Sets the vehicle entity the camera should follow or reference.
        /// </summary>
        /// <param name="entity">The vehicle entity now under player control.</param>
        internal void SetControlledEntity(Entity entity)
        {
            this.controlledEntity = entity;
        }

        /// <summary>
        /// Called when the player takes control of a vehicle.
        /// Saves the current camera controller and switches to the appropriate mode.
        /// </summary>
        internal void OnTakeControl()
        {
            if (this.controlledEntity == Entity.Null || Camera.main == null || this.Setting == null)
            {
                return;
            }

            this.savedCameraController = this.cameraUpdateSystem.activeCameraController;

            switch (this.Setting.ModeDropdown)
            {
                case Setting.CameraModeEnum.CameraVanillaFollow:
                    this.ActivateVanillaFollow();
                    break;

                case Setting.CameraModeEnum.CameraCustom:
                    this.ActivateCustomCamera();
                    break;

                case Setting.CameraModeEnum.CameraVanillaFree:
                    this.ActivateVanillaFree();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Called when the player releases control of a vehicle.
        /// Restores the camera to a non-snapping state at the vehicle's current position.
        /// </summary>
        internal void OnExitControl()
        {
            if (Camera.main == null)
            {
                return;
            }

            var exitRotation = Camera.main.transform.rotation;
            var exitEulerAngles = exitRotation.eulerAngles;

            if (!this.GetVehicleCameraTarget(out Vector3 exitPivot, out float exitPureZoom, out float exitOrbitZoom))
            {
                exitPureZoom = exitOrbitZoom = 50f;
                exitPivot = Camera.main.transform.position + (exitRotation * new Vector3(0f, 0f, exitPureZoom));
            }

            bool syncPosition = this.Setting.ModeDropdown == Setting.CameraModeEnum.CameraCustom;
            var orbitCam = SelectedInfoUISystem.s_CameraController;

            if (this.EntityManager.Exists(this.controlledEntity) &&
                this.Setting.ModeDropdown != Setting.CameraModeEnum.CameraVanillaFree &&
                orbitCam != null)
            {
                // Hand off to the orbit camera following the vehicle, so the player can keep watching it drive away.
                orbitCam.pivot = exitPivot;
                orbitCam.zoom = exitOrbitZoom;
                orbitCam.rotation = exitEulerAngles;
                orbitCam.followedEntity = this.controlledEntity;
                orbitCam.mode = OrbitCameraController.Mode.Follow;
                this.cameraUpdateSystem.activeCameraController = orbitCam;
            }
            else if (this.savedCameraController != null && !(this.savedCameraController is OrbitCameraController))
            {
                this.cameraUpdateSystem.activeCameraController = this.savedCameraController;

                if (syncPosition)
                {
                    this.savedCameraController.pivot = exitPivot;
                    this.savedCameraController.zoom = exitPureZoom;
                    this.savedCameraController.rotation = exitEulerAngles;
                }
            }
            else if (this.cameraUpdateSystem.gamePlayController != null)
            {
                var gamePlayCtrl = this.cameraUpdateSystem.gamePlayController;
                this.cameraUpdateSystem.activeCameraController = gamePlayCtrl;

                if (syncPosition)
                {
                    gamePlayCtrl.pivot = exitPivot;
                    gamePlayCtrl.zoom = exitPureZoom;
                    gamePlayCtrl.rotation = exitEulerAngles;
                }
            }

            if (this.cameraUpdateSystem.gamePlayController != null)
            {
                this.cameraUpdateSystem.gamePlayController.inputEnabled = true;
            }

            this.savedCameraController = null;
            this.controlledEntity = Entity.Null;
            this.isFirstCustomFrame = true;
        }

        /// <summary>
        /// Initializes the camera update system reference.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            this.cameraUpdateSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CameraUpdateSystem>();
        }

        /// <summary>
        /// Per-frame update. Only active work happens in CameraCustom mode.
        /// </summary>
        protected override void OnUpdate()
        {
            if (this.controlledEntity == Entity.Null || this.Setting.ModeDropdown != Setting.CameraModeEnum.CameraCustom)
            {
                return;
            }

            // Ensure the gameplay controller stays active (nothing else should steal it while we're driving).
            if (!ReferenceEquals(this.cameraUpdateSystem.activeCameraController, this.cameraUpdateSystem.gamePlayController))
            {
                this.cameraUpdateSystem.activeCameraController = this.cameraUpdateSystem.gamePlayController;
            }

            this.UpdateCustomCamera(SystemAPI.Time.DeltaTime);
        }

        /// <summary>
        /// Switches to vanilla orbit-follow mode on the controlled vehicle.
        /// </summary>
        private void ActivateVanillaFollow()
        {
            var orbitCam = SelectedInfoUISystem.s_CameraController;
            if (orbitCam != null)
            {
                orbitCam.followedEntity = this.controlledEntity;
                orbitCam.mode = OrbitCameraController.Mode.Follow;
                orbitCam.TryMatchPosition(this.savedCameraController);
            }

            this.cameraUpdateSystem.activeCameraController = orbitCam;
        }

        /// <summary>
        /// Switches to the custom (scripted follow) camera mode.
        /// </summary>
        private void ActivateCustomCamera()
        {
            var gamePlayCtrl = this.cameraUpdateSystem.gamePlayController;
            if (gamePlayCtrl != null)
            {
                if (this.savedCameraController != null)
                {
                    gamePlayCtrl.TryMatchPosition(this.savedCameraController);
                }

                gamePlayCtrl.inputEnabled = true;
            }

            this.cameraUpdateSystem.activeCameraController = gamePlayCtrl;

            // Seed smoothed values from the real camera so the first frame has no position jump.
            this.smoothedCamPosition = Camera.main.transform.position;
            this.smoothedCamRotation = Camera.main.transform.rotation;

            if (SelectedInfoUISystem.s_CameraController != null)
            {
                SelectedInfoUISystem.s_CameraController.followedEntity = this.controlledEntity;
            }
        }

        /// <summary>
        /// Switches to vanilla free (player-controlled) camera mode.
        /// </summary>
        private void ActivateVanillaFree()
        {
            var gamePlayCtrl = this.cameraUpdateSystem.gamePlayController;
            if (gamePlayCtrl != null)
            {
                if (this.savedCameraController != null)
                {
                    gamePlayCtrl.TryMatchPosition(this.savedCameraController);
                }

                gamePlayCtrl.inputEnabled = true;
            }

            this.cameraUpdateSystem.activeCameraController = gamePlayCtrl;
        }

        /// <summary>
        /// Calculates the vehicle's world-space pivot point and camera zoom distances.
        /// Replicates vanilla logic to prevent snapping when handing the camera back.
        /// </summary>
        /// <param name="pivot">World-space pivot at the vehicle's bounding-box centre.</param>
        /// <param name="pureZoom">Straight-line distance from Camera.main to the pivot.</param>
        /// <param name="orbitZoom">Orbit zoom, reduced by the vehicle's bounding-box radius.</param>
        /// <returns>True if the vehicle position was resolved successfully.</returns>
        private bool GetVehicleCameraTarget(out Vector3 pivot, out float pureZoom, out float orbitZoom)
        {
            pivot = Vector3.zero;
            pureZoom = orbitZoom = 0f;
            int elementIndex = -1;

            if (this.EntityManager.Exists(this.controlledEntity) &&
                SelectedInfoUISystem.TryGetPosition(this.controlledEntity, this.EntityManager, ref elementIndex, out _, out float3 pos, out Bounds3 bounds, out _, true))
            {
                pivot = new Vector3(pos.x, MathUtils.Center(bounds.y), pos.z);
                pureZoom = Camera.main != null ? Vector3.Distance(Camera.main.transform.position, pivot) : 0f;
                orbitZoom = math.max(0.1f, pureZoom - (math.cmin(bounds.max - bounds.min) * 0.5f));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves Camera.main each frame to follow the vehicle, with position and rotation smoothing.
        /// Also keeps the vanilla controllers warm so switching modes mid-drive is seamless.
        /// </summary>
        /// <param name="dt">Delta time in seconds.</param>
        private void UpdateCustomCamera(float dt)
        {
            if (!this.EntityManager.Exists(this.controlledEntity) || Camera.main == null || this.Setting == null)
            {
                return;
            }

            if (!this.EntityManager.HasComponent<Game.Objects.Transform>(this.controlledEntity))
            {
                return;
            }

            var vehicleTransform = this.EntityManager.GetComponentData<Game.Objects.Transform>(this.controlledEntity);
            Vector3 vehiclePosition = vehicleTransform.m_Position;
            Quaternion vehicleRotation = vehicleTransform.m_Rotation;

            var localOffset = new Vector3(0f, this.Setting.CameraOffsetY, -this.Setting.CameraOffsetZ);
            var targetCamPosition = vehiclePosition + (vehicleRotation * localOffset);

            if (this.isFirstCustomFrame)
            {
                /*Mod.LOG.Debug($"[VehicleDriver CameraDebug] FIRST FRAME: " +
                             $"CameraEuler: {Camera.main.transform.rotation.eulerAngles} | " +
                             $"VehicleEuler: {vehicleRotation.eulerAngles} | " +
                             $"Difference: {Quaternion.Angle(Camera.main.transform.rotation, vehicleRotation)} degrees");*/
                this.smoothedCamPosition = Camera.main.transform.position;
                this.smoothedCamRotation = Camera.main.transform.rotation;
                this.isFirstCustomFrame = false;
            }

            this.smoothedCamPosition = Vector3.Lerp(
                this.smoothedCamPosition,
                targetCamPosition,
                this.Setting.CameraPositionLerpSpeed * this.Setting.GasSensitivity * dt);

            this.smoothedCamRotation = Quaternion.Slerp(
                this.smoothedCamRotation,
                vehicleRotation,
                this.Setting.CameraRotationLerpSpeed * this.Setting.SteeringSensitivity * dt);

            Camera.main.transform.position = this.smoothedCamPosition;
            Camera.main.transform.rotation = this.smoothedCamRotation;

            // Keep the vanilla controllers warm so switching modes mid-drive has no snap.
            if (this.GetVehicleCameraTarget(out Vector3 warmPivot, out float warmPureZoom, out float warmOrbitZoom))
            {
                Vector3 warmEulerAngles = this.smoothedCamRotation.eulerAngles;

                if (this.cameraUpdateSystem.gamePlayController != null)
                {
                    this.cameraUpdateSystem.gamePlayController.pivot = warmPivot;
                    this.cameraUpdateSystem.gamePlayController.zoom = warmPureZoom;
                    this.cameraUpdateSystem.gamePlayController.rotation = warmEulerAngles;
                }

                if (SelectedInfoUISystem.s_CameraController != null)
                {
                    SelectedInfoUISystem.s_CameraController.pivot = warmPivot;
                    SelectedInfoUISystem.s_CameraController.zoom = warmOrbitZoom;
                    SelectedInfoUISystem.s_CameraController.rotation = warmEulerAngles;
                }
            }
        }
    }
}