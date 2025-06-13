// <copyright file="InputHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver.Helpers
{
    using System.Diagnostics.CodeAnalysis;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Handles entity input from keyboard and gamepad, managing input actions and providing their current state.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Public readonly fields are necessary to allow ControlToolSystem jobs to directly access input action instances without needing to pass them as method parameters, optimizing performance in Unity ECS.")]

    public class InputHelper
    {
        /// <summary>
        /// Gets the <see cref="InputAction"/> for gas and brake input.
        /// This action is publicly accessible so the <c>ControlToolSystem</c> can read its current values.
        /// </summary>
        public readonly InputAction GasBrakeAction;

        /// <summary>
        /// Gets the <see cref="InputAction"/> for steering input.
        /// This action is publicly accessible so the <c>ControlToolSystem</c> can read its current values.
        /// </summary>
        public readonly InputAction SteerAction;

        /// <summary>
        /// Gets the <see cref="InputAction"/> for handbrake input.
        /// This action is publicly accessible so the <c>ControlToolSystem</c> can read its current values.
        /// </summary>
        public readonly InputAction HandbrakeAction;

         /// <summary>
        /// Initializes a new instance of the <see cref="InputHelper"/> class.
        /// Sets up input actions and their composite bindings for keyboard and gamepad.
        /// </summary>
        /// <remarks>
        /// Note: Handling an ESC key for exiting control might require additional logic
        /// to integrate gracefully with vanilla cancel tools and prevent crashes.
        /// </remarks>
        public InputHelper()
        {
            this.GasBrakeAction = new InputAction("GasBrake");
            this.GasBrakeAction.AddCompositeBinding("1DAxis")
                .With("Positive", "<Keyboard>/upArrow")
                .With("Negative", "<Keyboard>/downArrow");
            this.GasBrakeAction.AddCompositeBinding("1DAxis")
                .With("Positive", "<Gamepad>/rightTrigger")
                .With("Negative", "<Gamepad>/leftTrigger");

            // Register callbacks for GasBrakeAction to update the public property
            this.GasBrakeAction.performed += ctx => { this.GasBrake = ctx.ReadValue<float>(); };
            this.GasBrakeAction.canceled += ctx => { this.GasBrake = 0f; };

            this.SteerAction = new InputAction("Steer");
            this.SteerAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/leftArrow")
                .With("Positive", "<Keyboard>/rightArrow");
            this.SteerAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Gamepad>/leftStick/left")
                .With("Positive", "<Gamepad>/leftStick/right");

            // Register callbacks for SteerAction to update the public property
            this.SteerAction.performed += ctx => { this.Steering = ctx.ReadValue<float>(); };
            this.SteerAction.canceled += ctx => { this.Steering = 0f; };

            this.HandbrakeAction = new InputAction("Handbrake");
            this.HandbrakeAction.AddBinding("<Keyboard>/h");
            this.HandbrakeAction.AddBinding("<Gamepad>/leftShoulder");
            this.HandbrakeAction.started += _ => { this.Handbrake = true; };
            this.HandbrakeAction.canceled += _ => { this.Handbrake = false; };
        }

        /// <summary>
        /// Gets the current normalised value for gas/brake input.
        /// Positive for gas, negative for brake. Publicly accessible for reading by jobs.
        /// </summary>
        public float GasBrake { get; private set; }

        /// <summary>
        /// Gets the current normalised value for steering input.
        /// Negative for the left, positive for the right. Publicly accessible for reading by jobs.
        /// </summary>
        public float Steering { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the handbrake is currently active.
        /// Publicly accessible for reading by jobs.
        /// </summary>
        public bool Handbrake { get; private set; }

        /// <summary>
        /// Enables all defined input actions (GasBrake, Steer, Handbrake).
        /// </summary>
        public void Enable()
        {
            this.GasBrakeAction.Enable();
            this.SteerAction.Enable();
            this.HandbrakeAction.Enable();
        }

        /// <summary>
        /// Disables all defined input actions and resets their corresponding input values to default.
        /// </summary>
        public void Disable()
        {
            this.GasBrakeAction.Disable();
            this.SteerAction.Disable();
            this.HandbrakeAction.Disable();
            this.GasBrake = 0;
            this.Steering = 0;
            this.Handbrake = false;
        }
    }
}
