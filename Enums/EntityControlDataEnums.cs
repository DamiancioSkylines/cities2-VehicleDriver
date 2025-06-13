// <copyright file="EntityControlDataEnums.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver.Enums
{
    /// <summary>
    /// Defines the types of entities that can be controlled by the mod.
    /// </summary>
    public enum EntityType
    {
        /// <summary>
        /// The entity is a vehicle.
        /// </summary>
        Vehicle,

        /// <summary>
        /// The entity is a citizen.
        /// </summary>
        Citizen,

        /// <summary>
        /// The entity type is not determined or is none.
        /// </summary>
        None,
    }

    /// <summary>
    /// Defines the types of vehicles that can be controlled.
    /// </summary>
    public enum VehicleType
    {
        /// <summary>
        /// The vehicle is a car.
        /// </summary>
        Car,

        /// <summary>
        /// The vehicle is a train.
        /// </summary>
        Train,

        /// <summary>
        /// The vehicle is an aircraft.
        /// </summary>
        Aircraft,

        /// <summary>
        /// The vehicle is a watercraft.
        /// </summary>
        Watercraft,

        /// <summary>
        /// The vehicle type is not determined or is none.
        /// </summary>
        None,
    }

    /// <summary>
    /// Defines the states a vehicle can be in.
    /// Only Cars and Trains can be parked so far; this simply determines the state for all vehicles.
    /// </summary>
    public enum VehicleState
    {
        /// <summary>
        /// The vehicle is currently moving.
        /// </summary>
        Moving,

        /// <summary>
        /// The vehicle is currently parked.
        /// </summary>
        Parked,

        /// <summary>
        /// The vehicle is involved in an accident.
        /// </summary>
        InvolvedInAccident,

        /// <summary>
        /// The vehicle state is not determined or is none.
        /// </summary>
        None,
    }

    /// <summary>
    /// Defines specific types of cars.
    /// </summary>
    public enum CarType
    {
        /// <summary>
        /// A standard car.
        /// </summary>
        Car,

        /// <summary>
        /// A motorcycle.
        /// </summary>
        Motorcycle,

        /// <summary>
        /// A truck.
        /// </summary>
        Truck,

        /// <summary>
        /// A trailer.
        /// </summary>
        Trailer,

        /// <summary>
        /// A bus.
        /// </summary>
        Bus,

        /// <summary>
        /// An ambulance.
        /// </summary>
        Ambulance,

        /// <summary>
        /// A police car.
        /// </summary>
        PoliceCar,

        /// <summary>
        /// A taxi.
        /// </summary>
        Taxi,

        /// <summary>
        /// The car type is not determined or is none.
        /// </summary>
        None,
    }

    /// <summary>
    /// Defines the size categories for cars.
    /// </summary>
    public enum CarSize
    {
        /// <summary>
        /// A small-sized car.
        /// </summary>
        Small,

        /// <summary>
        /// A medium-sized car.
        /// </summary>
        Medium,

        /// <summary>
        /// A large-sized car.
        /// </summary>
        Large,

        /// <summary>
        /// The car size is not determined or is none.
        /// </summary>
        None,
    }

    /// <summary>
    /// Defines specific types of trains.
    /// </summary>
    public enum TrainType
    {
        /// <summary>
        /// A passenger train engine.
        /// </summary>
        PassengerEngine,

        /// <summary>
        /// A passenger train car.
        /// </summary>
        PassengerCar,

        /// <summary>
        /// A cargo train engine.
        /// </summary>
        CargoEngine,

        /// <summary>
        /// A cargo train car.
        /// </summary>
        CargoCar,

        /// <summary>
        /// A tram engine.
        /// </summary>
        TramEngine,

        /// <summary>
        /// A tram car.
        /// </summary>
        TramCar,

        /// <summary>
        /// A subway engine.
        /// </summary>
        SubwayEngine,

        /// <summary>
        /// A subway car.
        /// </summary>
        SubwayCar,

        /// <summary>
        /// The train type is not determined or is none.
        /// </summary>
        None,
    }

    /// <summary>
    /// Defines specific types of watercraft.
    /// </summary>
    public enum WatercraftType
    {
        /// <summary>
        /// A passenger watercraft.
        /// </summary>
        Passenger,

        /// <summary>
        /// A cargo watercraft.
        /// </summary>
        Cargo,

        /// <summary>
        /// The watercraft type is not determined or is none.
        /// </summary>
        None,
    }

    /// <summary>
    /// Defines specific types of aircraft.
    /// </summary>
    public enum AircraftType
    {
        /// <summary>
        /// A cargo airplane.
        /// </summary>
        CargoAirplane,

        /// <summary>
        /// A passenger airplane.
        /// </summary>
        PassengerAirplane,

        /// <summary>
        /// A helicopter.
        /// </summary>
        Helicopter,

        /// <summary>
        /// A rocket.
        /// </summary>
        Rocket,

        /// <summary>
        /// The aircraft type is not determined or is none.
        /// </summary>
        None,
    }
}
