// <copyright file="EntityControlData.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Components
{
    using Unity.Entities;
    using VehicleDriver.Enums;

    /// <summary>
    /// A single unmanaged Component to store original state flags and data for a controlled entity.
    /// This data is used to restore the entity to its original state when manual control is released.
    /// </summary>
    public struct EntityControlData : IComponentData
    {
        /// <summary>
        /// Stores the original <see cref="EntityType"/> of the controlled entity.
        /// </summary>
        public EntityType EntityType;

        /// <summary>
        /// Stores the original <see cref="VehicleType"/> of the controlled vehicle.
        /// </summary>
        public VehicleType VehicleType;

        /// <summary>
        /// Stores the original <see cref="VehicleState"/> of the controlled vehicle.
        /// </summary>
        public VehicleState VehicleState;

        /// <summary>
        /// Stores the original <see cref="CarType"/> of the controlled car, if applicable.
        /// </summary>
        public CarType CarType;

        /// <summary>
        /// Stores the original <see cref="CarSize"/> of the controlled car, if applicable.
        /// </summary>
        public CarSize CarSize;

        /// <summary>
        /// Stores the original <see cref="TrainType"/> of the controlled train, if applicable.
        /// </summary>
        public TrainType TrainType;

        /// <summary>
        /// Stores the original <see cref="WatercraftType"/> of the controlled watercraft, if applicable.
        /// </summary>
        public WatercraftType WatercraftType;

        /// <summary>
        /// Stores the original <see cref="AircraftType"/> of the controlled aircraft, if applicable.
        /// </summary>
        public AircraftType AircraftType;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Common.Target"/> component.
        /// </summary>
        public bool HadTarget;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Pathfind.PathInformation"/> component.
        /// </summary>
        public bool HadPathInformation;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Rendering.Swaying"/> component.
        /// </summary>
        public bool HadSwaying;

        /// <summary>
        /// A flag indicating whether the entity originally had an <see cref="Game.Rendering.InterpolatedTransform"/> component.
        /// </summary>
        public bool HadInterpolatedTransform;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Vehicles.CarNavigation"/> component.
        /// </summary>
        public bool HadCarNavigation;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Vehicles.CarCurrentLane"/> component.
        /// </summary>
        public bool HadCarCurrentLane;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Pathfind.PathOwner"/> component.
        /// </summary>
        public bool HadPathOwner;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Vehicles.CarNavigationLane"/> buffer.
        /// </summary>
        public bool HadCarNavigationLane;

        /// <summary>
        /// A flag indicating whether the entity originally had a <see cref="Game.Pathfind.PathElement"/> buffer.
        /// </summary>
        public bool HadPathElement;

        /// <summary>
        /// Stores the original <see cref="Game.Vehicles.CarNavigation"/> component data.
        /// </summary>
        public Game.Vehicles.CarNavigation OriginalCarNavigation;

        /// <summary>
        /// Stores the original <see cref="Game.Vehicles.CarCurrentLane"/> component data.
        /// </summary>
        public Game.Vehicles.CarCurrentLane OriginalCarCurrentLane;

        /// <summary>
        /// Stores the original <see cref="Game.Pathfind.PathOwner"/> component data.
        /// </summary>
        public Game.Pathfind.PathOwner OriginalPathOwner;

        /// <summary>
        /// Stores the original <see cref="Game.Vehicles.CarNavigationLane"/> component data.
        /// </summary>
        public Game.Vehicles.CarNavigationLane OriginalCarNavigationLane;

        /// <summary>
        /// Stores the original <see cref="Game.Pathfind.PathElement"/> component data.
        /// </summary>
        public Game.Pathfind.PathElement OriginalPathElement;

        /// <summary>
        /// Stores the original target entity reference from the <see cref="Game.Common.Target"/> component.
        /// </summary>
        public Entity OriginalTarget;

        /// <summary>
        /// Stores the original <see cref="Game.Objects.Moving"/> component data.
        /// </summary>
        public Game.Objects.Moving OriginalMoving;
    }
}
