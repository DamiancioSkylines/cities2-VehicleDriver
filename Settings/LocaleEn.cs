// <copyright file="LocaleEn.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver.Settings
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Provides English locale entries for the Vehicle Driver mod settings.
    /// This class implements <see cref="IDictionarySource"/> to supply localised strings
    /// for UI elements, tooltips, and other mod-specific text.
    /// </summary>
    public class LocaleEn : IDictionarySource
    {
        private readonly Setting setting;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleEn"/> class.
        /// </summary>
        /// <param name="setting">The mod's settings instance, used to generate locale IDs.</param>
        public LocaleEn(Setting setting)
        {
            this.setting = setting;
        }

        /// <summary>
        /// Reads and provides a collection of English locale entries.
        /// </summary>
        /// <param name="errors">A list to which any errors encountered during reading can be added.</param>
        /// <param name="indexCounts">A dictionary to track the count of each index, used internally by the localization system.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/> representing the locale entries.</returns>
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
        {
            { this.setting.GetSettingsLocaleID(), "Vehicle Driver" },
            { this.setting.GetOptionTabLocaleID(Setting.MainControlsTab), "Main Controls" },
            { this.setting.GetOptionGroupLocaleID(Setting.MainControlsGroup), "Reset" },
            { this.setting.GetOptionGroupLocaleID(Setting.KeyboardGroup), "Keyboard" },
            { this.setting.GetOptionGroupLocaleID(Setting.GamepadGroup), "Gamepad" },

            // Key bindings

            // ResetBindings
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset Shortcuts" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), "Reset Shortcuts to default" },

            // ToggleControlEntityKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ToggleControlEntityKey)), "Toggle manual control" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ToggleControlEntityKey)), "First select Vehicle and press to toggle manual control." },

            // HandbrakeKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.HandbrakeKey)), "Handbrake" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.HandbrakeKey)), "Handbrake description" },

            // AccelerationKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.AccelerationKey)), "Accelerate" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.AccelerationKey)), "Assign key to accelerate (positive axis input)." },

            // BrakeKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.BrakeKey)), "Decelerate" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.BrakeKey)), "Assign key to decelerate, brake or reverse (negative axis input).." },

            // SteerLeftKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.SteerLeftKey)), "Steer Left" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.SteerLeftKey)), "Assign key to steer left (negative axis input)." },

            // SteerRightKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.SteerRightKey)), "Steer Right" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.SteerRightKey)), "Assign key to steer right (positive axis input)." },

            // NoteGasKeyboard
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteGasKeyboard)), "Accelerate" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteGasKeyboard)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteBreakKeyboard
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteBreakKeyboard)), "Decelerate" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteBreakKeyboard)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteSteerLeftKeyboard
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteSteerLeftKeyboard)), "Steer Left" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteSteerLeftKeyboard)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteSteerRightKeyboard
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteSteerRightKeyboard)), "Steer Right" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteSteerRightKeyboard)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteHandbrakeKeyboard
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteHandbrakeKeyboard)), "Handbrake" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteHandbrakeKeyboard)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // Gamepad bindings

            // ToggleControlEntityGamepadKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ToggleControlEntityGamepadKey)), "Toggle manual control" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ToggleControlEntityGamepadKey)), "First select Vehicle and press to toggle manual control." },

            // RespawnGamepadKey
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.RespawnGamepadKey)), "Respawn" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.RespawnGamepadKey)), "Press respawn , stops movement." },

            // NoteGasGamepad
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteGasGamepad)), "Accelerate" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteGasGamepad)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteBreakGamepad
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteBreakGamepad)), "Decelerate" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteBreakGamepad)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteSteerLeftGamepad
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteSteerLeftGamepad)), "Steer Left" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteSteerLeftGamepad)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteSteerRightGamepad
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteSteerRightGamepad)), "Steer Right" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteSteerRightGamepad)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // NoteHandbrakeGamepad
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NoteHandbrakeGamepad)), "Handbrake" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NoteHandbrakeGamepad)), "Changing keybinding for this use not implemented, due to technical limitation" },

            // Car Behaviour
            { this.setting.GetOptionTabLocaleID(Setting.CarBehaviourTab), "Car Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.CarBehaviourGroup), "Car Behaviour Settings" },

            // ResetCarBehaviour
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ResetCarBehaviour)), "Reset Behaviour" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ResetCarBehaviour)), "Reset Behaviour to default" },

            // EnableGripPhysics
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.EnableGripPhysics)), "Enable better Grip Physics" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.EnableGripPhysics)), "This option is not implemented" },

            // Acceleration
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.Acceleration)), "Acceleration" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.Acceleration)), "Adjust how quickly the vehicle gains speed when accelerating." },

            // TopSpeed
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.TopSpeed)), "Top Speed" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.TopSpeed)), "Set the maximum speed the vehicle can reach." },

            // SteeringSensitivity
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.SteeringSensitivity)), "Steering Sensitivity" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.SteeringSensitivity)), "Adjust how sharply the vehicle's wheels can turn." },

            // BrakingPower
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.BrakingPower)), "Braking Power" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.BrakingPower)), "Adjust how quickly the vehicle slows down when the brake (reverse) input is actively pressed." },

            // GasSensitivity
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.GasSensitivity)), "Gas Sensitivity" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.GasSensitivity)), "Adjust the responsiveness of the accelerator pedal." },

            // VehicleWheelbase
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.VehicleWheelbase)), "Vehicle Wheelbase" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.VehicleWheelbase)), "The virtual distance between the vehicle's front and rear axles (affects turning radius)." },

            // OverallGrip
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.OverallGrip)), "Overall Tire Grip" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.OverallGrip)), "Adjust the overall friction of the tires (affects acceleration, braking, and lateral sliding)." },

            // HandbrakeBrakingFactor
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.HandbrakeBrakingFactor)), "Handbrake Braking Factor" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.HandbrakeBrakingFactor)), "Multiplier for how much 'Overall Grip' is reduced longitudinally when the handbrake is engaged (lower value means less braking)." },

            // HandbrakeSlideFactor
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.HandbrakeSlideFactor)), "Handbrake Slide Factor" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.HandbrakeSlideFactor)), "Multiplier for how much 'Overall Grip' is reduced laterally when the handbrake is engaged (lower value means more slide)." },

            // DriftEffectiveness
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.DriftEffectiveness)), "Drift Effectiveness" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.DriftEffectiveness)), "Multiplier for how much lateral grip is reduced during sharp turns (higher value means more drift)." },

            // DriftEffectiveness
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.AnalogRampUpSpeed)), "Analog Ramp Up Speed (Gas/Brake)" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.AnalogRampUpSpeed)), "How quickly keyboard input ramps up to its full value for acceleration/braking." },

            // AnalogRampDownSpeed
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.AnalogRampDownSpeed)), "Analog Ramp Down Speed (Gas/Brake)" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.AnalogRampDownSpeed)), "How quickly keyboard input ramps down to zero for acceleration/braking." },

            // AnalogSteerRampUpSpeed
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.AnalogSteerRampUpSpeed)), "Analog Ramp Up Speed (Steering)" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.AnalogSteerRampUpSpeed)), "How quickly keyboard input ramps up to its full value for steering." },

            // AnalogSteerRampDownSpeed
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.AnalogSteerRampDownSpeed)), "Analog Ramp Down Speed (Steering)" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.AnalogSteerRampDownSpeed)), "How quickly keyboard input ramps down to zero for steering." },

            // LowSpeedTurningBoost
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.LowSpeedTurningBoost)), "Low Speed Turning Boost" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.LowSpeedTurningBoost)), "Increases turning responsiveness at very low speeds (e.g., parking, reversing)." },

            // HighSpeedTurningDamping
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.HighSpeedTurningDamping)), "High Speed Turning Damping" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.HighSpeedTurningDamping)), "Reduces steering effectiveness as vehicle approaches top speed (0 = no turning, 1 = no damping)." },

            // PivotTurningBlendSpeed
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.PivotTurningBlendSpeed)), "Low Speed Turn Boost Blend Speed" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.PivotTurningBlendSpeed)), "The vehicle speed at which the low speed turning boost blends out." },

            // TurningSpeedLossFactor
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.TurningSpeedLossFactor)), "Turning Speed Loss Factor" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.TurningSpeedLossFactor)), "Controls how much speed is lost when turning at high speeds." },

            // shut up
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.NaturalDeceleration)), "Coast Deceleration" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.NaturalDeceleration)), "How quickly the vehicle slows down when no gas or brake input is applied (higher value means faster stopping)." },

            // shut up
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ReversePowerMultiplier)), "Reverse Speed Multiplier" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ReversePowerMultiplier)), "Multiplier for how fast the vehicle can accelerate and reach its top speed in reverse (1.0 means same as forward)." },

            // shut up
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.MaxLateralSpeed)), "Max Lateral Speed" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.MaxLateralSpeed)), "Caps the vehicle's sideways movement speed to prevent excessive drift or instability." },

            // MotorBike
            { this.setting.GetOptionTabLocaleID(Setting.MotorBikeBehaviourTab), "MotorBike Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.MotorBikeBehaviourGroup), "MotorBike Behaviour Settings" },

            // Bike
            { this.setting.GetOptionTabLocaleID(Setting.BikeBehaviourTab), "Bike Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.BikeBehaviourGroup), "Bike Behaviour Settings" },

            // Train
            { this.setting.GetOptionTabLocaleID(Setting.TrainBehaviourTab), "Train Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.TrainBikeBehaviourGroup), "Train Behaviour Settings" },

            // Helicopter
            { this.setting.GetOptionTabLocaleID(Setting.HelicopterTab), "Helicopter Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.HelicopterGroup), "Helicopter Behaviour Settings" },

            // Plane
            { this.setting.GetOptionTabLocaleID(Setting.PlaneTab), "Plane Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.PlaneGroup), "Plane Behaviour Settings" },

            // Boat
            { this.setting.GetOptionTabLocaleID(Setting.BoatTab), "Boat Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.BoatGroup), "Boat Behaviour Settings" },

            // Rocket
            { this.setting.GetOptionTabLocaleID(Setting.RocketTab), "Rocket Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.RocketGroup), "Rocket Behaviour Settings" },

            // Citizen
            { this.setting.GetOptionTabLocaleID(Setting.CitizenTab), "Citizen Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.CitizenGroup), "Citizen Behaviour Settings" },

            // Animal
            { this.setting.GetOptionTabLocaleID(Setting.AnimalTab), "Animal Behaviour" },
            { this.setting.GetOptionGroupLocaleID(Setting.AnimalGroup), "Animal Behaviour Settings" },

            // Camera
            { this.setting.GetOptionTabLocaleID(Setting.CameraTab), "Camera" },
            { this.setting.GetOptionGroupLocaleID(Setting.CameraGroup), "Camera Settings" },
            { this.setting.GetOptionGroupLocaleID(Setting.CustomCameraGroup), "Custom Camera" },

            // Int Dropdown from vanilla mod template
            // { _setting.GetOptionLabelLocaleID(nameof(Setting.IntDropdown)), "Int dropdown" },
            // { _setting.GetOptionDescLocaleID(nameof(Setting.IntDropdown)), $"Use int property with getter and setter and [{nameof(SettingsUIDropdownAttribute)}(typeof(SomeType), nameof(SomeMethod))] to get int dropdown: Method must be static or instance of your setting class with 0 parameters and returns {typeof(DropdownItem<int>).Name}" },
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ResetCameraMode)), "Reset Camera Mode" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ResetCameraMode)), "Reset only Camera mode to default, Custom Camera settings unaffected" },

            // ModeDropdown
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ModeDropdown)), "Camera Mode" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ModeDropdown)), "Choose camera mode, that will be used for camera control during manual control" },

            // CameraModeEnum
            { this.setting.GetEnumValueLocaleID(Setting.CameraModeEnum.CameraVanillaFollow), "Vanilla Follow Camera" },
            { this.setting.GetEnumValueLocaleID(Setting.CameraModeEnum.CameraVanillaFree), "Vanilla Free Camera" },
            { this.setting.GetEnumValueLocaleID(Setting.CameraModeEnum.CameraCustom), "Custom Camera" },

            // ResetCustomCamera
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.ResetCustomCamera)), "Reset Custom Camera" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.ResetCustomCamera)), "Reset Custom Camera to default" },

            // CameraOffsetY
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.CameraOffsetY)), "Camera Height Offset" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.CameraOffsetY)), "Adjusts the custom camera's height." },

            // CameraOffsetZ feels so wrong
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.CameraOffsetZ)), "Camera Distance" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.CameraOffsetZ)), "Adjusts the custom camera's distance." },

            // CameraPositionLerpSpeed
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.CameraPositionLerpSpeed)), "Camera Position Smoothing" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.CameraPositionLerpSpeed)), "Smoothing for custom camera position (higher value = more responsive)." },

            // CameraRotationLerpSpeed
            { this.setting.GetOptionLabelLocaleID(nameof(Setting.CameraRotationLerpSpeed)), "Camera Rotation Smoothing" },
            { this.setting.GetOptionDescLocaleID(nameof(Setting.CameraRotationLerpSpeed)), "Smoothing for custom camera rotation (higher value = more responsive)." },

            // I dont know what is this
            { this.setting.GetBindingMapLocaleID(), "VehicleControl Bindings" },
        };

        /// <summary>
        /// Called when the mod's locale is unloaded.
        /// This method is part of the <see cref="IDictionarySource"/> interface and can be used to
        /// release any resources if they were allocated by this locale source.
        /// </summary>
        public void Unload()
        {
        }
    }
}
