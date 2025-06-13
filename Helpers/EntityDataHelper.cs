// <copyright file="EntityDataHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace VehicleDriver.Helpers
{
    using Colossal.Entities;
    using Unity.Entities;
    using VehicleDriver.Components;
    using VehicleDriver.Enums;

    /// <summary>
    /// Provides utility methods for determining and storing the original state and types of controlled entities.
    /// </summary>
    public static class EntityDataHelper
    {
        // Reference to the main mod logger for consistent logging.
        // private static readonly ILog LOG = Mod.LOG;

        /// <summary>
        /// Determines and compiles the original state and type information of an entity into an <see cref="EntityControlData"/> struct.
        /// This method queries various components to identify the entity's type, vehicle state, and specific vehicle sub-types (e.g., CarType, TrainType).
        /// </summary>
        /// <param name="entityManager">The <see cref="EntityManager"/> instance to query entity components.</param>
        /// <param name="entity">The <see cref="Entity"/> for which to determine control data.</param>
        /// <param name="prefabSystem">The <see cref="Game.Prefabs.PrefabSystem"/> to get prefab names for detailed type determination.</param>
        /// <returns>A populated <see cref="EntityControlData"/> struct containing the entity's original state and type information.</returns>
        public static EntityControlData DetermineEntityControlData(EntityManager entityManager, Entity entity, Game.Prefabs.PrefabSystem prefabSystem)
        {
            var originalEntityType = EntityType.None;
            var originalVehicleState = VehicleState.None;
            var originalVehicleType = Enums.VehicleType.None;
            var originalCarType = CarType.None;
            var originalCarSize = CarSize.None;
            var originalTrainType = TrainType.None;
            var originalWatercraftType = WatercraftType.None;
            var originalAircraftType = AircraftType.None;

            entityManager.TryGetComponent(entity, out Game.Prefabs.PrefabRef prefabRef);
            string prefabName = prefabSystem.GetPrefabName(prefabRef.m_Prefab);

            if (prefabName == null)
            {
                Mod.LOG.Warn($"[EntityDataHelper.DetermineEntityControlData] Failed to find prefabName for entity: {entity.Index}. Defaulting to None types.");

                // Return a default EntityControlData if prefabName is null to avoid null reference issues.
                return default;
            }

            if (entityManager.HasComponent<Game.Vehicles.Vehicle>(entity))
            {
                originalEntityType = EntityType.Vehicle;

                if (entityManager.HasComponent<Game.Vehicles.Car>(entity))
                {
                    originalVehicleType = Enums.VehicleType.Car;

                    if (entityManager.HasComponent<Game.Vehicles.PersonalCar>(entity) && !entityManager.HasComponent<Game.Vehicles.CarTrailer>(entity))
                    {
                        originalCarType = CarType.Car;
                        originalCarSize = CarSize.Small;

                        if (prefabName.Contains("Motorcycle"))
                        {
                            originalCarType = CarType.Motorcycle;
                        }
                    }
                    else if ((entityManager.HasComponent<Game.Vehicles.DeliveryTruck>(entity) || entityManager.HasComponent<Game.Vehicles.GarbageTruck>(entity)) && !entityManager.HasComponent<Game.Vehicles.CarTrailer>(entity))
                    {
                        originalCarType = CarType.Truck;
                        originalCarSize = CarSize.Large;
                    }
                    else if (entityManager.HasComponent<Game.Vehicles.MaintenanceVehicle>(entity))
                    {
                        originalCarType = CarType.Truck;
                        originalCarSize = CarSize.Medium;
                    }
                    else if (entityManager.HasComponent<Game.Vehicles.CarTrailer>(entity))
                    {
                        originalCarType = CarType.Trailer;
                        originalCarSize = entityManager.HasComponent<Game.Vehicles.DeliveryTruck>(entity) ? CarSize.Large : CarSize.Small;
                    }
                    else if (entityManager.HasComponent<Game.Vehicles.PublicTransport>(entity))
                    {
                        originalCarType = CarType.Bus;
                        originalCarSize = CarSize.Large;
                    }
                    else if (entityManager.HasComponent<Game.Vehicles.Ambulance>(entity))
                    {
                        originalCarType = CarType.Ambulance;
                        originalCarSize = CarSize.Small;
                    }
                    else if (entityManager.HasComponent<Game.Vehicles.PoliceCar>(entity))
                    {
                        originalCarType = CarType.PoliceCar;
                        originalCarSize = CarSize.Small;
                    }
                    else if (entityManager.HasComponent<Game.Vehicles.Taxi>(entity))
                    {
                        originalCarType = CarType.Taxi;
                        originalCarSize = CarSize.Small;
                    }
                }
                else if (entityManager.HasComponent<Game.Vehicles.Train>(entity))
                {
                    originalVehicleType = Enums.VehicleType.Train;
                    if (prefabName.Contains("Engine"))
                    {
                        if (prefabName.Contains("Passenger"))
                        {
                            originalTrainType = TrainType.PassengerEngine;
                        }

                        if (prefabName.Contains("Subway"))
                        {
                            originalTrainType = TrainType.SubwayEngine;
                        }

                        if (prefabName.Contains("Tram"))
                        {
                            originalTrainType = TrainType.TramEngine;
                        }

                        if (prefabName.Contains("Cargo"))
                        {
                            originalTrainType = TrainType.CargoEngine;
                        }
                    }
                    else if (prefabName.Contains("Car"))
                    {
                        if (prefabName.Contains("Passenger"))
                        {
                            originalTrainType = TrainType.PassengerCar;
                        }

                        if (prefabName.Contains("Subway"))
                        {
                            originalTrainType = TrainType.SubwayCar;
                        }

                        if (prefabName.Contains("Tram"))
                        {
                            originalTrainType = TrainType.TramCar;
                        }

                        if (prefabName.Contains("Cargo"))
                        {
                            originalTrainType = TrainType.CargoCar;
                        }
                    }
                }
                else if (entityManager.HasComponent<Game.Vehicles.Watercraft>(entity))
                {
                    originalVehicleType = Enums.VehicleType.Watercraft;
                    if (prefabName.Contains("Passenger"))
                    {
                        originalWatercraftType = WatercraftType.Passenger;
                    }
                    else if (prefabName.Contains("Cargo"))
                    {
                        originalWatercraftType = WatercraftType.Cargo;
                    }
                }
                else if (entityManager.HasComponent<Game.Vehicles.Aircraft>(entity))
                {
                    originalVehicleType = Enums.VehicleType.Aircraft;
                    if (entityManager.HasComponent<Game.Vehicles.Helicopter>(entity))
                    {
                        if (entityManager.HasComponent<Game.Vehicles.PoliceCar>(entity) || entityManager.HasComponent<Game.Vehicles.FireEngine>(entity))
                        {
                            originalAircraftType = AircraftType.Helicopter;
                        }
                        else
                        {
                            originalAircraftType = AircraftType.Rocket; // Assuming rocket for non-police/fire helicopters
                        }
                    }
                    else if (entityManager.HasComponent<Game.Vehicles.Airplane>(entity))
                    {
                        originalAircraftType = AircraftType.PassengerAirplane; // Default to passenger
                        if (entityManager.HasComponent<Game.Vehicles.CargoTransport>(entity))
                        {
                            originalAircraftType = AircraftType.CargoAirplane;
                        }
                    }
                }

                // Determine VehicleState
                if (entityManager.HasComponent<Game.Events.InvolvedInAccident>(entity))
                {
                    originalVehicleState = VehicleState.InvolvedInAccident;
                }
                else if (entityManager.HasComponent<Game.Vehicles.ParkedCar>(entity) || entityManager.HasComponent<Game.Vehicles.ParkedTrain>(entity))
                {
                    originalVehicleState = VehicleState.Parked;
                }
                else if (entityManager.HasComponent<Game.Objects.Moving>(entity))
                {
                    originalVehicleState = VehicleState.Moving;
                }
            }

            return new EntityControlData
            {
                EntityType = originalEntityType,
                VehicleType = originalVehicleType,
                VehicleState = originalVehicleState,
                CarType = originalCarType,
                CarSize = originalCarSize,
                TrainType = originalTrainType,
                WatercraftType = originalWatercraftType,
                AircraftType = originalAircraftType,

                HadCarNavigation = entityManager.HasComponent<Game.Vehicles.CarNavigation>(entity),
                HadCarCurrentLane = entityManager.HasComponent<Game.Vehicles.CarCurrentLane>(entity),
                HadPathOwner = entityManager.HasComponent<Game.Pathfind.PathOwner>(entity),
                HadCarNavigationLane = entityManager.HasBuffer<Game.Vehicles.CarNavigationLane>(entity),
                HadPathElement = entityManager.HasBuffer<Game.Pathfind.PathElement>(entity),
                HadPathInformation = entityManager.HasComponent<Game.Pathfind.PathInformation>(entity),
                HadSwaying = entityManager.HasComponent<Game.Rendering.Swaying>(entity),
                HadTarget = entityManager.HasComponent<Game.Common.Target>(entity),
                OriginalTarget = entityManager.HasComponent<Game.Common.Target>(entity) ? entityManager.GetComponentData<Game.Common.Target>(entity).m_Target : Entity.Null,
                OriginalMoving = entityManager.HasComponent<Game.Objects.Moving>(entity) ? entityManager.GetComponentData<Game.Objects.Moving>(entity) : default,
                HadInterpolatedTransform = entityManager.HasComponent<Game.Rendering.InterpolatedTransform>(entity),
            };
        }
    }
}
