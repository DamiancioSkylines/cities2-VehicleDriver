// <copyright file="HidingUISystem.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Systems
{
    using Game.Input;
    using Game.Rendering;
    using Game.SceneFlow;
    using Game.Tools;
    using Game.UI;
    using Unity.Entities;
    using UnityEngine.InputSystem;
    using VehicleDriver.Settings;

    /// <summary>
    /// Represents a UI system responsible for managing the visibility of certain UI elements
    /// in response to specific user interactions or game settings.
    /// This system is primarily designed for toggling UI visibility based on input actions,
    /// with an initial state of visible UI.
    /// </summary>
    public partial class HidingUISystem : UISystemBase
    {
        private Setting setting;

        private RenderingSystem renderingSystem; // Reference to the game's rendering system to control overlays.
        private ToolRaycastSystem toolRaycastSystem; // Reference to the system managing tool and camera raycasts.
        private ToolSystem toolSystem; // Reference to base ToolSystem to get the selected entity.

        private bool uiVisible; // Last frame's state of the UI: true if hidden, false if visible.

        /// <summary>
        /// Initialises the HidingUISystem, setting up necessary dependencies and default configurations.
        /// </summary>
        /// <remarks>
        /// The method is called when the system is created and performs the following actions:
        /// - Initializes the base UISystem functionality.
        /// - Logs the creation of the HidingUISystem.
        /// - Retrieves required settings and dependencies, such as RenderingSystem, ToolRaycastSystem, and ToolSystem,
        /// within the World context.
        /// - Sets the initial state of the UI visibility to "visible."
        /// - Configures the UI toggle control action to handle interactions for toggling UI visibility.
        /// </remarks>
        protected override void OnCreate()
        {
            base.OnCreate();
            Mod.LOG.Info("[HidingUISystem] OnCreate: HidingUISystem created");

            this.setting = Mod.Instance.Setting;
            this.renderingSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<RenderingSystem>();
            this.toolRaycastSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ToolRaycastSystem>();
            this.toolSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<ToolSystem>();
            this.uiVisible = true; // Initialise the state as visible.

            var toggleControlAction = this.setting.GetAction(Mod.ToggleControlEntityActionName);
            if (toggleControlAction == null)
            {
                return;
            }

            toggleControlAction.shouldBeEnabled = true;
            toggleControlAction.onInteraction -= this.ToggleUI; // Unsubscribe first to ensure only one subscription
            toggleControlAction.onInteraction += this.ToggleUI; // Then subscribe
        }

        // Toggles the visibility of the game's UI and adjusts associated rendering and raycasting flags.
        private void ToggleUI(ProxyAction action, InputActionPhase phase)
        {
            if (phase != InputActionPhase.Canceled)
            {
                return; // Only act on Cancelled phase without this line this class will be called 3 times wtf, maybe should be on Started phase?
            }

            var entity = this.toolSystem.selected;
            var uiView = GameManager.instance.userInterface.view.View;

            // There is a case when controlled vehicle got into accident during manual control, after a while vehicle gets deleted and we can't escape Hidden UI or mod ControlToolSystem if user did not exit the vehicle. Two Part Problem here and in Mod.cs in ExitControl class todo
            if (this.uiVisible)
            {
                // Validate entity and if the UI is visible
                if (entity == Entity.Null || !this.EntityManager.Exists(entity) || !this.EntityManager.HasComponent<Game.Vehicles.Car>(entity) || this.EntityManager.HasComponent<Game.Common.Destroyed>(entity) || this.EntityManager.HasComponent<Game.Events.InvolvedInAccident>(entity))
                {
                    Mod.LOG.Info("[ToggleUI]    Blocked. Entity is invalid or destroyed or not a car");
                    return;
                }
            }

            // Toggle UI visibility except the highlighted outlines over entities, this will be handled by the ControlToolSystem itself
            if (this.uiVisible)
            {
                // Hide the rendering overlay, which can affect things like road names and notifications.
                this.renderingSystem.hideOverlay = true;

                // Disable raycasting for UI elements when in free camera mode.
                // This prevents accidental interaction with hidden UI elements and the game world.
                // The bitwise OR |= sets the FreeCameraDisable flag without changing other flags.
                this.toolRaycastSystem.raycastFlags |= Game.Common.RaycastFlags.FreeCameraDisable;

                // Execute JavaScript to hide the main UI container within the Cohtml view.
                uiView.ExecuteScript("document.querySelector('.app-container_Y5l').style.visibility = 'hidden';");

                this.uiVisible = false;
            }
            else
            {
                // The UI is currently hidden, so restore it.
                // Restore the rendering overlay visibility.
                this.renderingSystem.hideOverlay = false;

                // Re-enable raycasting for UI elements in free camera mode.
                // The bitwise AND with NOT &= ~ clears the FreeCameraDisable flag without affecting other flags.
                this.toolRaycastSystem.raycastFlags &= ~Game.Common.RaycastFlags.FreeCameraDisable;

                // Execute JavaScript to make the main UI container visible again.
                uiView.ExecuteScript("document.querySelector('.app-container_Y5l').style.visibility = 'visible';");

                this.uiVisible = true;
            }
        }
    }
}