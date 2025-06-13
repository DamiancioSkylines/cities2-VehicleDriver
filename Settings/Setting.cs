// <copyright file="Setting.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver.Settings
{
    using System.Collections.Generic;
    using Colossal.IO.AssetDatabase;
    using Game.Input;
    using Game.Modding;
    using Game.Settings;
    using Game.UI.Widgets;

    /// <summary>
    /// Represents the mod's settings class, handling UI presentation, keybindings, and various mod-specific parameters.
    /// This class extends <see cref="ModSetting"/> to integrate with the game's settings menu.
    /// </summary>
    /// <remarks>
    /// This class defines UI groups, tab order, and keyboard/gamepad actions for various mod functionalities
    /// such as vehicle control, camera behaviour, and general mod settings.
    /// </remarks>
    // Mod settings class, handling UI and keybindings.
    [FileLocation(nameof(VehicleDriver))]

    // Define the order of groups within the tabs
    [SettingsUIGroupOrder(MainControlsGroup, KeyboardGroup, CarBehaviourGroup, MotorBikeBehaviourGroup, BikeBehaviourGroup, TrainBikeBehaviourGroup, HelicopterGroup, PlaneGroup, BoatGroup, RocketGroup, CitizenGroup, AnimalGroup, CameraGroup, CustomCameraGroup)]

    // [SettingsUIGroupOrder(MainControlsGroup, KeyboardGroup, CarBehaviourGroup, CameraGroup, CustomCameraGroup)]

    // Show group names
    // [SettingsUIShowGroupName(/*MainControlsGroup,*/ KeyboardGroup, GamepadGroup, MotorBikeBehaviourGroup, BikeBehaviourGroup, TrainBikeBehaviourGroup, HelicopterGroup, PlaneGroup, BoatGroup, RocketGroup, CitizenGroup, AnimalGroup, CustomCameraGroup)]
    [SettingsUIShowGroupName(KeyboardGroup, GamepadGroup, CustomCameraGroup)]

    // Define the order of tabs
    [SettingsUITabOrder(MainControlsTab, CarBehaviourTab, MotorBikeBehaviourTab, BikeBehaviourTab, TrainBehaviourTab, HelicopterTab, PlaneTab, BoatTab, RocketTab, CitizenTab, AnimalTab, CameraTab)]

    // [SettingsUITabOrder(MainControlsTab, CarBehaviourTab, CameraTab)]

    // No SettingsUITab attribute directly on the class. Tabs are defined by the first parameter of SettingsUISection.
    [SettingsUIKeyboardAction(Mod.ToggleControlEntityActionName, usages: new[] { "Gameplay", "VehicleControl" })]
    [SettingsUIGamepadAction(Mod.ToggleControlEntityActionName, usages: new[] { "Gameplay", "VehicleControl" })]

    // Keep these as Axis actions, as the ControlToolSystem reads them as such.
    [SettingsUIKeyboardAction(Mod.AxisGasBrakeActionName, ActionType.Axis, usages: new[] { "Gameplay", "VehicleControl" })]
    [SettingsUIKeyboardAction(Mod.AxisSteerActionName, ActionType.Axis, usages: new[] { "Gameplay", "VehicleControl" })]
    [SettingsUIKeyboardAction(Mod.ButtonHandbrakeActionName, usages: new[] { "Gameplay", "VehicleControl" })]
    [SettingsUIKeyboardAction(Mod.ButtonRespawnActionName, usages: new[] { "Gameplay", "VehicleControl" })]
    public class Setting : ModSetting
    {
        // "internal" prevents making new implied options in a mod menu when something calls this; we are using ones from InputHelper class
        // internal float GasBrake { get; set; }
        // internal float Steering { get; set; }
        // internal bool Handbrake { get; set; }

        /// <summary>
        /// Constant string for the "Main Controls" tab.
        /// </summary>
        public const string MainControlsTab = "Main Controls";

        /// <summary>
        /// Constant string for the "Main Bindings" group.
        /// </summary>
        public const string MainControlsGroup = "Main Bindings";

        /// <summary>
        /// Constant string for the "Keyboard Bindings" group.
        /// </summary>
        public const string KeyboardGroup = "Keyboard Bindings";

        /// <summary>
        /// Constant string for the "Gamepad Bindings" group.
        /// </summary>
        public const string GamepadGroup = "Gamepad Bindings";

        /// <summary>
        /// Constant string for the "Car Behaviour" tab.
        /// </summary>
        public const string CarBehaviourTab = "Car Behaviour";

        /// <summary>
        /// Constant string for the "Car Behaviour Settings" group.
        /// </summary>
        public const string CarBehaviourGroup = "Car Behaviour Settings";

        /// <summary>
        /// Constant string for the "MotorBike Behaviour" tab.
        /// </summary>
        public const string MotorBikeBehaviourTab = "MotorBike Behaviour";

        /// <summary>
        /// Constant string for the "MotorBike Behaviour Settings" group.
        /// </summary>
        public const string MotorBikeBehaviourGroup = "MotorBike Behaviour Settings";

        /// <summary>
        /// Constant string for the "Bike Behaviour" tab.
        /// </summary>
        public const string BikeBehaviourTab = "Bike Behaviour";

        /// <summary>
        /// Constant string for the "Bike Behaviour Settings" group.
        /// </summary>
        public const string BikeBehaviourGroup = "Bike Behaviour Settings";

        /// <summary>
        /// Constant string for the "Train Behaviour" tab.
        /// </summary>
        public const string TrainBehaviourTab = "Train Behaviour";

        /// <summary>
        /// Constant string for the "Train Behaviour Settings" group.
        /// </summary>
        public const string TrainBikeBehaviourGroup = "Train Behaviour Settings";

        /// <summary>
        /// Constant string for the "Helicopter Behaviour" tab.
        /// </summary>
        public const string HelicopterTab = "Helicopter Behaviour";

        /// <summary>
        /// Constant string for the "Helicopter Behaviour Settings" group.
        /// </summary>
        public const string HelicopterGroup = "Helicopter Behaviour Settings";

        /// <summary>
        /// Constant string for the "Plane Behaviour" tab.
        /// </summary>
        public const string PlaneTab = "Plane Behaviour";

        /// <summary>
        /// Constant string for the "Plane Behaviour Settings" group.
        /// </summary>
        public const string PlaneGroup = "Plane Behaviour Settings";

        /// <summary>
        /// Constant string for the "Boat Behaviour" tab.
        /// </summary>
        public const string BoatTab = "Boat Behaviour";

        /// <summary>
        /// Constant string for the "Boat Behaviour Settings" group.
        /// </summary>
        public const string BoatGroup = "Boat Behaviour Settings";

        /// <summary>
        /// Constant string for the "Rocket Behaviour" tab.
        /// </summary>
        public const string RocketTab = "Rocket Behaviour";

        /// <summary>
        /// Constant string for the "Rocket Behaviour Settings" group.
        /// </summary>
        public const string RocketGroup = "Rocket Behaviour Settings";

        /// <summary>
        /// Constant string for the "Citizen Behaviour" tab.
        /// </summary>
        public const string CitizenTab = "Citizen Behaviour";

        /// <summary>
        /// Constant string for the "Citizen Behaviour Settings" group.
        /// </summary>
        public const string CitizenGroup = "Citizen Behaviour Settings";

        /// <summary>
        /// Constant string for the "Animal Behaviour" tab.
        /// </summary>
        public const string AnimalTab = "Animal Behaviour";

        /// <summary>
        /// Constant string for the "Animal Behaviour Settings" group.
        /// </summary>
        public const string AnimalGroup = "Animal Behaviour Settings";

        /// <summary>
        /// Constant string for the "Camera" tab.
        /// </summary>
        public const string CameraTab = "Camera";

        /// <summary>
        /// Constant string for the "Camera Settings" group.
        /// </summary>
        public const string CameraGroup = "Camera Settings";

        /// <summary>
        /// Constant string for the "Custom Camera Settings" group.
        /// </summary>
        public const string CustomCameraGroup = "Custom Camera Settings";

        /// <summary>
        /// Initializes a new instance of the <see cref="Setting"/> class.
        /// </summary>
        /// <param name="mod">The mod instance associated with these settings.</param>
        public Setting(IMod mod)
            : base(mod)
        {
        }

        /// <summary>
        /// Defines the available camera modes for the mod.
        /// </summary>
        public enum CameraModeEnum
        {
            /// <summary>
            /// The camera follows the vanilla game's default behaviour.
            /// </summary>
            CameraVanillaFollow = 0,

            /// <summary>
            /// The camera uses the vanilla game's free camera movement.
            /// </summary>
            CameraVanillaFree = 1,

            /// <summary>
            /// The camera uses custom settings defined by the mod.
            /// </summary>
            CameraCustom = 2,
        }

        /// <summary>
        /// Sets a value indicating whether it gets or sets a value that, when set, triggers the reset of all key bindings to their default values.
        /// </summary>
        [SettingsUISection(MainControlsTab, MainControlsGroup)]
        public bool ResetBindings { set => this.ResetKeyBindings();  }

        /// <summary>
        /// Gets or sets the key binding for toggling control over an entity (Keyboard).
        /// </summary>
        // Keybinding for taking control over an entity
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.Enter, Mod.ToggleControlEntityActionName)]
        public ProxyBinding ToggleControlEntityKey { get; set; }

        /// <summary>
        /// Gets or sets the key binding for respawning the controlled entity (Keyboard).
        /// </summary>
        // Keybinding for Handbrake and Respawn
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.Y, Mod.ButtonRespawnActionName)]
        public ProxyBinding RespawnKey { get; set; }

        /// <summary>
        /// Gets or sets the key binding for applying the handbrake (Keyboard).
        /// This binding is hidden in the UI as it's passed to the <c>InputHelper</c> class.
        /// </summary>
        [SettingsUIHidden] // Hidden as I found no way to pass this binding to InputHelper // Hidden as I found no way to pass this binding to InputHelper
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.H, Mod.ButtonHandbrakeActionName)]
        public ProxyBinding HandbrakeKey { get; set; }

        /// <summary>
        /// Gets a note displaying the default keyboard binding for the handbrake.
        /// </summary>
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        public string NoteHandbrakeKeyboard => "H";

        // Keybindings for Accelerate and Brake (separate UI options, but map to the same axis action)

        /// <summary>
        /// Gets or sets the key binding for acceleration (Keyboard).
        /// This binding is hidden in the UI as it's passed to the <c>InputHelper</c> class.
        /// </summary>
        [SettingsUIHidden] // Hidden as I found no way to pass this binding to InputHelper
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.UpArrow, Mod.AxisGasBrakeActionName)]
        public ProxyBinding AccelerationKey { get; set; }

        /// <summary>
        /// Gets a note displaying the default keyboard binding for gas (acceleration).
        /// </summary>
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        public string NoteGasKeyboard => "Up Arrow";

        /// <summary>
        /// Gets or sets the key binding for braking (Keyboard).
        /// This binding is hidden in the UI as it's passed to the <c>InputHelper</c> class.
        /// </summary>
        [SettingsUIHidden] // Hidden as I found no way to pass this binding to InputHelper // Hidden as I found no way to pass this binding to InputHelper
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.DownArrow, Mod.AxisGasBrakeActionName)]
        public ProxyBinding BrakeKey { get; set; }

        /// <summary>
        /// Gets a note displaying the default keyboard binding for brake.
        /// </summary>
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        public string NoteBreakKeyboard => "Down Arrow";

        /// <summary>
        /// Gets or sets the key binding for a steering left (Keyboard).
        /// This binding is hidden in the UI as it's passed to the <c>InputHelper</c> class.
        /// </summary>
        [SettingsUIHidden] // Hidden as I found no way to pass this binding to InputHelper // Hidden as I found no way to pass this binding to InputHelper
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.LeftArrow, Mod.AxisSteerActionName)]
        public ProxyBinding SteerLeftKey { get; set; }

        /// <summary>
        /// Gets a note displaying the default keyboard binding for a steering left.
        /// </summary>
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        public string NoteSteerLeftKeyboard => "Left Arrow";

        /// <summary>
        /// Gets or sets the key binding for steering right (Keyboard).
        /// This binding is hidden in the UI as it's passed to the <c>InputHelper</c> class.
        /// </summary>
        [SettingsUIHidden] // Hidden as I found no way to pass this binding to InputHelper // Hidden as I found no way to pass this binding to InputHelper
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.RightArrow, Mod.AxisSteerActionName)]
        public ProxyBinding SteerRightKey { get; set; }

        /// <summary>
        /// Gets a note displaying the default keyboard binding for steering right.
        /// </summary>
        [SettingsUISection(MainControlsTab, KeyboardGroup)]
        public string NoteSteerRightKeyboard => "Right Arrow";

        // Keybindings for Gamepad

        /// <summary>
        /// Gets or sets the key binding for toggling control over an entity (Gamepad).
        /// </summary>
        [SettingsUISection(MainControlsTab, GamepadGroup)]
        [SettingsUIGamepadBinding(BindingGamepad.RightShoulder, Mod.ToggleControlEntityActionName)]
        public ProxyBinding ToggleControlEntityGamepadKey { get; set; }

        /// <summary>
        /// Gets or sets the key binding for respawning the controlled entity (Gamepad).
        /// </summary>
        [SettingsUISection(MainControlsTab, GamepadGroup)]
        [SettingsUIGamepadBinding(BindingGamepad.RightShoulder, Mod.ButtonRespawnActionName, rightStick: true)]
        public ProxyBinding RespawnGamepadKey { get; set; }

        /// <summary>
        /// Gets a note displaying the default gamepad binding for the handbrake.
        /// </summary>
        [SettingsUISection(MainControlsTab, GamepadGroup)]
        public string NoteHandbrakeGamepad => "Right Stick + Right Shoulder";

        /// <summary>
        /// Gets a note displaying the default gamepad binding for gas (acceleration).
        /// </summary>
        [SettingsUISection(MainControlsTab, GamepadGroup)]
        public string NoteGasGamepad => "Right Trigger";

        /// <summary>
        /// Gets a note displaying the default gamepad binding for brake.
        /// </summary>
        [SettingsUISection(MainControlsTab, GamepadGroup)]
        public string NoteBreakGamepad => "Right Trigger";

        /// <summary>
        /// Gets a note displaying the default gamepad binding for a steering left.
        /// </summary>
        [SettingsUISection(MainControlsTab, GamepadGroup)]
        public string NoteSteerLeftGamepad => "Left Stick";

        /// <summary>
        /// Gets a note displaying the default gamepad binding for steering right.
        /// </summary>
        [SettingsUISection(MainControlsTab, GamepadGroup)]
        public string NoteSteerRightGamepad => "Left Stick";

        /// <summary>
        /// Sets a value indicating whether it gets or sets a value that, when set, triggers the reset of car behaviour settings to their default values.
        /// </summary>
        // Settings for car behaviour
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        public bool ResetCarBehaviour { set => this.ResetCarBehaviourSettings(); }

        /// <summary>
        /// Gets or sets a value indicating whether grip physics should be enabled for cars.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        public bool EnableGripPhysics { get; set; } = true;

        /// <summary>
        /// Gets or sets the steering sensitivity for cars.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 1f, max = 10f, step = 0.1f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float SteeringSensitivity { get; set; } = 2.5f;

        /// <summary>
        /// Gets or sets the gas (acceleration) sensitivity for cars.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 1f, max = 10f, step = 0.1f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float GasSensitivity { get; set; } = 2.5f;

        /// <summary>
        /// Gets or sets the acceleration rate for cars.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.01f, max = 1f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float Acceleration { get; set; } = 0.25f;

        /// <summary>
        /// Gets or sets the maximum speed for cars.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 1f, max = 100f, step = 1f, scalarMultiplier = 1)]
        public float TopSpeed { get; set; } = 35f;

        /// <summary>
        /// Gets or sets the braking power for cars.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.01f, max = 1f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float BrakingPower { get; set; } = 0.85f;

        /// <summary>
        /// Gets or sets the wheelbase length for vehicle physics calculations.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.1f, max = 1f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float VehicleWheelbase { get; set; } = 0.1f;

        /// <summary>
        /// Gets or sets the overall grip factor for car physics.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 1f, max = 100f, step = 1f, scalarMultiplier = 1)]
        public float OverallGrip { get; set; } = 40f;

        /// <summary>
        /// Gets or sets the braking effectiveness when the handbrake is applied.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.1f, max = 1f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float HandbrakeBrakingFactor { get; set; } = 0.75f;

        /// <summary>
        /// Gets or sets the factor influencing how much a car slides when the handbrake is applied.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.01f, max = 1f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float HandbrakeSlideFactor { get; set; } = 0.5f;

        /// <summary>
        /// Gets or sets the effectiveness of drift. Setting to 1 might cause issues in the option menu.
        /// </summary>
        // setting this to 1 crashes the mod in an option menu wtf
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0f, max = 0.99f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float DriftEffectiveness { get; set; } = 0.75f;

        /// <summary>
        /// Gets or sets the ramp-up speed for analogue emulation of gas/brake input from digital sources (keyboard).
        /// </summary>
        // Analog Emulation Settings for Gas/Brake Steering
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.01f, max = 2f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float AnalogRampUpSpeed { get; set; } = 0.35f;

        /// <summary>
        /// Gets or sets the ramp-down speed for analogue emulation of gas/brake input from digital sources (keyboard).
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.01f, max = 2f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float AnalogRampDownSpeed { get; set; } = 0.18f;

        /// <summary>
        /// Gets or sets the ramp-up speed for analogue emulation of steering input from digital sources (keyboard).
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.01f, max = 2f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float AnalogSteerRampUpSpeed { get; set; } = 0.5f;

        /// <summary>
        /// Gets or sets the ramp-down speed for analogue emulation of steering input from digital sources (keyboard).
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.01f, max = 2f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float AnalogSteerRampDownSpeed { get; set; } = 1.5f;

        /// <summary>
        /// Gets or sets the boost applied to steering at low speeds.
        /// </summary>
        // Steering Specific Settings
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.1f, max = 5.0f, step = 0.1f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float LowSpeedTurningBoost { get; set; } = 1.0f;

        /// <summary>
        /// Gets or sets the damping applied to steering at high speeds.
        /// </summary>
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.0f, max = 1.0f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float HighSpeedTurningDamping { get; set; } = 0.15f;

        /// <summary>
        /// Gets or sets the blending speed for pivot turning (e.g. when stationary or at very low speeds).
        /// </summary>
        // Pivot Turning Blend Speed
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 1f, max = 100f, step = 1f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float PivotTurningBlendSpeed { get; set; } = 25.0f;

        /// <summary>
        /// Gets or sets the factor determining speed loss during turning.
        /// </summary>
        // Turning Speed Loss Factor
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.001f, max = 0.5f, step = 0.001f, scalarMultiplier = 1, unit = "floatThreeFractions")]
        public float TurningSpeedLossFactor { get; set; } = 0.15f;

        /// <summary>
        /// Gets or sets the natural deceleration rate of the vehicle when no input is applied.
        /// </summary>
        // Natural Deceleration Setting
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0.001f, max = 1f, step = 0.001f, scalarMultiplier = 1, unit = "floatThreeFractions")]
        public float NaturalDeceleration { get; set; } = 0.2f;

        /// <summary>
        /// Gets or sets the multiplier for reverse power relative to forward power.
        /// </summary>
        // Reverse Power Multiplier Locale
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 0f, max = 1f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float ReversePowerMultiplier { get; set; } = 0.5f;

        /// <summary>
        /// Gets or sets the maximum lateral speed to prevent excessive sideways drift.
        /// </summary>
        // MaxLateralSpeed to prevent runaway drift
        [SettingsUISection(CarBehaviourTab, CarBehaviourGroup)]
        [SettingsUISlider(min = 1f, max = 50f, step = 1f, scalarMultiplier = 1)] // Adjust min/max as needed
        public float MaxLateralSpeed { get; set; } = 20f; // A reasonable default

        // Camera Settings

        // IntDropdown from the official mod template if I need it later
        // [SettingsUIDropdown(typeof(Setting), nameof(GetIntDropdownItems))]
        // [SettingsUISection(CameraTab, CameraGroup)]
        // public int IntDropdown { get; set; }

        /// <summary>
        /// Sets a value indicating whether it gets or sets a value that, when set, triggers the reset of camera mode settings to their default values.
        /// </summary>
        [SettingsUISection(CameraTab, CameraGroup)]
        public bool ResetCameraMode { set => this.ResetCameraModeSettings(); }

        /// <summary>
        /// Gets or sets the currently selected camera mode.
        /// </summary>
        [SettingsUISection(CameraTab, CameraGroup)]
        public CameraModeEnum ModeDropdown { get; set; } = CameraModeEnum.CameraVanillaFollow;

        /// <summary>
        /// Sets a value indicating whether it gets or sets a value that, when set, triggers the reset of custom camera settings to their default values.
        /// This section is hidden unless <see cref="ModeDropdown"/> is set to <see cref="CameraModeEnum.CameraCustom"/>.
        /// </summary>
        [SettingsUIHideByCondition(typeof(Setting), nameof(HideCustomCameraSettings))]
        [SettingsUISection(CameraTab, CustomCameraGroup)]
        public bool ResetCustomCamera { set => this.ResetCustomCameraSettings(); }

        /// <summary>
        /// Gets or sets the vertical offset for the custom camera.
        /// This setting is hidden unless <see cref="ModeDropdown"/> is set to <see cref="CameraModeEnum.CameraCustom"/>.
        /// </summary>
        [SettingsUIHideByCondition(typeof(Setting), nameof(HideCustomCameraSettings))]
        [SettingsUISection(CameraTab, CustomCameraGroup)]
        [SettingsUISlider(min = 1f, max = 4f, step = 0.1f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float CameraOffsetY { get; set; } = 1.8f;

        /// <summary>
        /// Gets or sets the depth offset for the custom camera (distance behind the vehicle).
        /// This setting is hidden unless <see cref="ModeDropdown"/> is set to <see cref="CameraModeEnum.CameraCustom"/>.
        /// </summary>
        [SettingsUIHideByCondition(typeof(Setting), nameof(HideCustomCameraSettings))]
        [SettingsUISection(CameraTab, CustomCameraGroup)]
        [SettingsUISlider(min = 1f, max = 20f, step = 0.1f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float CameraOffsetZ { get; set; } = 10.0f;

        /// <summary>
        /// Gets or sets the interpolation speed for custom camera position smoothing.
        /// This setting is hidden unless <see cref="ModeDropdown"/> is set to <see cref="CameraModeEnum.CameraCustom"/>.
        /// </summary>
        [SettingsUIHideByCondition(typeof(Setting), nameof(HideCustomCameraSettings))]
        [SettingsUISection(CameraTab, CustomCameraGroup)]
        [SettingsUISlider(min = 0.01f, max = 10f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float CameraPositionLerpSpeed { get; set; } = 2.0f;

        /// <summary>
        /// Gets or sets the spherical linear interpolation (slerp) speed for custom camera rotation smoothing.
        /// This setting is hidden unless <see cref="ModeDropdown"/> is set to <see cref="CameraModeEnum.CameraCustom"/>.
        /// </summary>
        [SettingsUIHideByCondition(typeof(Setting), nameof(HideCustomCameraSettings))]
        [SettingsUISection(CameraTab, CustomCameraGroup)]
        [SettingsUISlider(min = 0.01f, max = 10f, step = 0.01f, scalarMultiplier = 1, unit = "floatTwoFractions")]
        public float CameraRotationLerpSpeed { get; set; } = 2.0f;

        /// <summary>
        /// Determines whether the custom camera settings section should be hidden in the UI.
        /// </summary>
        /// <returns><c>true</c> if the custom camera settings should be hidden; otherwise, <c>false</c>.</returns>
        public bool HideCustomCameraSettings()
        {
            return this.ModeDropdown != CameraModeEnum.CameraCustom;
        }

        /// <summary>
        /// Retrieves a list of dropdown items for an integer dropdown UI element.
        /// </summary>
        /// <returns>An array of <see cref="DropdownItem{T}"/> representing integer options.</returns>
        public DropdownItem<int>[] GetIntDropdownItems()
        {
            var items = new List<DropdownItem<int>>();

            for (var i = 0; i < 3; i += 1)
            {
                items.Add(new DropdownItem<int>
                {
                    value = i,
                    displayName = i.ToString(),
                });
            }

            return items.ToArray();
        }

        /// <summary>
        /// Sets all mod settings to their default values.
        /// </summary>
        public override void SetDefaults()
        {
            // IDK should this be here?
            this.ResetKeyBindings();
            this.ResetCarBehaviourSettings();
            this.ResetCameraModeSettings();
            this.ResetCustomCameraSettings();

            // throw new NotImplementedException();
        }

        /// <summary>
        /// Resets all car behaviour-related settings to their default values.
        /// </summary>
        private void ResetCarBehaviourSettings()
        {
            this.Acceleration = 0.25f;
            this.TopSpeed = 35f;
            this.SteeringSensitivity = 2.5f;
            this.BrakingPower = 0.85f;
            this.GasSensitivity = 2.5f;
            this.VehicleWheelbase = 0.1f;
            this.EnableGripPhysics = true;
            this.OverallGrip = 40f;
            this.HandbrakeBrakingFactor = 0.75f;
            this.HandbrakeSlideFactor = 0.5f;
            this.DriftEffectiveness = 0.75f;

            // Analog Emulation for keyboard
            this.AnalogRampUpSpeed = 0.35f;
            this.AnalogRampDownSpeed = 0.18f;
            this.AnalogSteerRampUpSpeed = 0.5f;
            this.AnalogSteerRampDownSpeed = 1.5f;

            // Steering Specific Settings
            this.LowSpeedTurningBoost = 1.0f;
            this.HighSpeedTurningDamping = 0.15f;
            this.PivotTurningBlendSpeed = 25.0f;
            this.TurningSpeedLossFactor = 0.15f;
            this.NaturalDeceleration = 0.2f;
            this.ReversePowerMultiplier = 0.5f;
            this.MaxLateralSpeed = 20f;
        }

        /// <summary>
        /// Resets the camera mode settings to their default values.
        /// </summary>
        private void ResetCameraModeSettings()
        {
            this.ModeDropdown = CameraModeEnum.CameraVanillaFollow;

            // ResetCustomCameraSettings();
        }

        /// <summary>
        /// Resets the custom camera offset and interpolation settings to their default values.
        /// </summary>
        private void ResetCustomCameraSettings()
        {
            this.CameraOffsetY = 1.8f;
            this.CameraOffsetZ = 10.0f;
            this.CameraPositionLerpSpeed = 2.0f;
            this.CameraRotationLerpSpeed = 2.0f;
        }
    }
}