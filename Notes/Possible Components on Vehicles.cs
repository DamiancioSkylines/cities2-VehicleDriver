// <copyright file="Possible Components on Vehicles.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// m_Prefab: Car07
// Game.Prefabs.PrefabRef - Car07

// --------------- Components --------------

// m_Blocker(entity) whats blocking me, m_Type(Flag), m_MaxSpeed(int)
// Game.Vehicles.Blocker

// m_Keeper entity that is an owner of a vehicle can be also building afaik, m_State: for example, HomeTarget
// Game.Vehicles.PersonalCar

// m_Owner: entity household or building owning the vehicle
// Game.Common.Owner

// m_Flags: StayOnRoad, probably set different for agriculture vehicles, maybe something else needs to check.
// Game.Vehicles.Car

// m_Index: m_Value: m_SubColor:(bool) I don't know yet
// Game.Objects.Color

// m_Wetness m_SnowAmount mAccumulatedWetness m_AccumulatedSnow m_Dirtyness
// Game.Objects.Surface

// m_State(flags) whats its doing, m_Resource(flag) type of resource, m_Amount(int)
// Game.Vehicles.DeliveryTruck

// m_TargetRequest(entity) m_State m_RequestCount m_Garbage m_EstimatedGarbage m_PathElementTime
// Game.Vehicles.GarbageTruck

// m_TargetRequest(entity) m_State(EdgeTarget) flags m_Maintained(int) m_MaintainEstimate(int) m_RequestCount(int) m_PathElementTime(float) m_Efficiency(float)
// Game.Vehicles.MaintenanceVehicle

// m_State(flags) m_TargetPatient(entity) m_TargetLocation(entity) m_TargetRequest(entity) m_PathElementTime(float)
// Game.Vehicles.Ambulance

// m_TargetRequest(entity) m_State(flags) m_RequestCount(int) m_PathElementTime(float) m_ShiftTime m_EstimatedShift m_PurposeMask(flags) intelligence for example
// Game.Vehicles.PoliceCar

// On FBI police cars, m_Postition(float3) m_IsValid(bool)
// Game.Common.PointOfInterest

// m_State(flags) m_TargetCorpse(entity) m_TargetRequest(entity) m_PathElementTime(float)
// Game.Vehicles.Hearse

// Example on Bus m_TargetRequest(entity) m_State(flags) m_DepartureFrame m_RequestCount(int) m_PathElementTime(float) m_MaxBoardingDistance(float) m_MinWaitingDistance (float)
// Buses Trains Subway Trams Planes Watercraft but even Rocket wtf
// Game.Vehicles.PublicTransport

// m_Route(entity - Bus Line) for example
// Game.Routes.CurrentRoute

// m_Lane(entity parking lane) m_CurvePosition(float)
// Game.Vehicles.ParkedCar

// m_TargetRequest, m_State, m_PathElementTime, m_StartDistance, m_MaxBoardingDistance, m_MinWaitingDistance, m_ExtraPathElementCount, m_NextStartingFee, m_CurrentFee
// Game.Vehicles.Taxi

// m_TargetRequest(entity) m_State(flags) m_RequestCount(int) m_PathElementTime(float) m_DeliveringMail(int) m_CollectedMail(int) m_DeliveryEstimate(int) m_CollectEstimate(int)
// Game.Vehicles.PostVan

// m_TargetRequest(entity) m_State(flags) m_RequestCount(int) m_PathElementTime(float) m_ExtinguishingAmount(300) m_Efficiency(float)
// Game.Vehicles.FireEngine

// m_Completed(float) found on Rockets
// Game.Vehicles.Produced


// m_Lane(lane entity) m_ChangeLane(lane entity) m_CurvePosition(float3) m_LaneFlags m_Duration, m_Distance, m_LanePosition(float unsigned side of the road probably)
// Game.Vehicles.CarCurrentLane

// m_TargetPosition(float3) m_TargetRotation(quaternion) m_NaxSpeed(float)
// Game.Vehicles.CarNavigation

// Distance traveled?
// Game.Vehicles.Odometer

// m_Target(entity - Bus Line) final destination
// Game.Common.Target

// m_ElementIndex m_State I don't know yet
// Game.Pathfind.PathOwner

// Example GarbageTruck or Bus, m_Origin(entity - Bus Line), m_Destination(entity - Bus Line), m_Distance(float), m_Duration(float), m_TotalCost(float), m_Methods(Road), m_State(flags?)
// Game.Pathfind.PathInformation

// m_Position(float3) m_Rotation(quaternion) m_Flags backend of interpolated position?
// Game.Rendering.InterpolatedTransform

// m_Bounds(min(float3,max(float3) boundingbox size I guess, m_Radius(float) m_CullingIndex(int) m_Mask(flags) m_MinLod(int) m_PassedCulling(bool or int)
// Game.Rendering.CullingInfo

// m_LastVelocity(float3) m_SwayPosition(float3) m_SwayVelocity(float3)
// Game.Rendering.Swaying

// m_Velocity float3, m_AngularVelocity float3 this is a main Component that allows vehicle move but needs also TransformFrame to visually see that
// Game.Objects.Moving

// m_Position(float3) m_Rotation(quaternion) this is the main Component that stores vehicle position
// Game.Objects.Transform

// I don't know yet, probably some utility thing
// Game.Common.PseudoRandomSeed

// Car Trailer stuff

// m_Lane(entity lane) m_NextLane(entity next lane) m_CurvePosition(float2) m_NextPosition(float2) NextLane and NextPosition is exact value for pulling vehicle
// Game.Vehicles.CarTrailerLane

// m_Controller(entity) that can pull the vehicle like trailer train car train engine
// Game.Vehicles.Controller

// m_Front m_Rear m_FrontCache m_RearCache m_Duration m_Distance, found on trains trams
// Game.Vehicles.TrainCurrentLane

// m_Flags(reversed) well trains can go both ways I guess or its about which engine pulls now
// Game.Vehicles.Train

// m_ParkingLocation(entity train spawn location) m_FrontLane(entity lane) m_RearLane(entity lane) m_CurvePosition(float2)
// Game.Vehicles.ParkedTrain

// m_Front m_Rear m_Speed
// Game.Vehicles.TrainNavigation

// m_Lane(entity lane)  m_CurvePosition(float2) m_LaneFlags(flags) m_Duration(float) m_Distance (float) m_LanePostition
// Game.Vehicles.AircraftCurrentLane

// m_Flags(StayOn Taxiway) for example, planes rockets helicopters
// Game.Vehicles.Aircraft

// m_TargetPosition(float3) m_TargetDirection(float3) m_MaxSpeed(float) m_MinClimbAngle(unsigned float) for example, planes rockets helicopters
// Game.Vehicles.AircraftNavigation

// m_Color(RGBA) custom Route color
// Game.Routes.Color

// m_Flags(multiple flags like StayOnWaterway, DeckLights, LightsOff)
// Game.Vehicles.Watercraft

// m_TargetPosition(float3) m_TargetDirection(float3) m_MaxSpeed(float)
// Game.Vehicles.WatercraftNavigation

// m_Lane(entity lane) m_ChangeLane(entity lane) m_CurvePosition(float3) m_LaneFlags(flags FixedStart for example) m_ChangeProgress(float) m_Duration(float) m_Distance (float) m_LanePosition
// Game.Vehicles.WatercraftCurrentLane

// m_TargetRequest(entity) m_State(flags like EnRoute, RouteSource) m_DepartureFrame(int) m_RequestCount(int) m_PathElementTime(float?)
// Watercraft, Plane, Train
// Game.Routes.CargoTransport

// m_TargetRequest(entity) m_State(flags like EnRoute, RouteSource) m_DepartureFrame(int) m_RequestCount(int) m_PathElementTime(float?)
// Watercraft, Plane, Train
// Game.Vehicles.CargoTransport

// ----------- Shared Components -----------
// m_Index this is the frame index when game simulation AI updates the vehicle, visually everything is interpolated with TransformFrame
// Game.Simulation.UpdateFrame

// -------- Buffers/ Managed Components ----

// CarNavigationLane[0...] Current 8 curve segments vehicle is consuming
// Game.Vehicles.CarNavigationLane (8)

// PathElement[0...] Number of curve segments in pathfind?
// Game.Pathfind.PathElement (x)

// m_Request: Entity that requested the current service we are delivering
// Train, Watercraft, Plane, Maintenance, Bus, GarbageTruck, Hearse, Police, Fire, Ambulance can have but not always maybe not after initial build
// Game.Simulation.ServiceDispatch

// MeshColor[0] > m_ColorSet (Game.Rendering.ColorSet) m_Channel0 RGBA
// Game.Rendering.MeshColor

// I don't know yet
// Game.Rendering.MeshBatch

// I don't know yet
// Game.Rendering.Emissive

// Number of toggle lights
// Game.Rendering.LightStat

// I don't know yet, implying it's for animation
// Game.Rendering.Skeleton [1]

// Number of bones used in animation
// Game.Rendering.Bone (5)

// I think it's for road wear, noise pollution and other effects like that
// Game.Effects.EnabledEffect (1)

// This is related to interpolated animation
// Game.Objects.TransformFrame (4) this is related to interpolated animation

// Passenger [0..] Count of passengers 23 in the Bus for example, 1 in taxi
// Game.Vehicles.Passenger ()

// I don't know yet
// Game.Objects.BlockedLane

// Maybe it's about pantograph animation
// Game.Vehicles.TrainBogieFrame(4)

// AircraftNavigationLane[0...] Current 8 curve segments vehicle is consuming
// Game.Vehicles.AircraftNavigationLane(8)

// WatercraftNavigationLane[0...] Current 8 curve segments vehicle is consuming
// Game.Vehicles.WatercraftNavigationLane(8)

// ------------------ Tag ------------------ // Empty components, most likely used as markers or base archetypes
// I guess a simple tag for all vehicles
// Game.Vehicles.Vehicle

// This is something for tools, a very likely essential for highlighting outline on hover or selection
// Game.Tools.Highlighted

// Game.Objects.Object

// Game.Objects.ObjectGeometry

// I think it's a tag to the entity that this can at some point participate in simulation, Vehicles always have it.
// Unity.Entities.Simulate [Enabled:True]

// Game.Vehicles.CarTrailer

// Game.Vehicles.Airplane

// Game.Objects.ParkMaintenanceVehicle

// helicopters and rocket
// Game.Objects.Stopped

// Fire Helicopter, Police, Helicopters, even Rocket wtf
// Game.Vehicles.Helicopter

// Found on Subway, Trains, even Rocket
// Game.Vehicles.PassengerTransport


// ----------------- Other ----------------

// Vehicles, Citizens, Pets?
// Game.Events.InvolvedInAccident

// Find out if there is something unique about resque buses or prison vehicles etc.

// Tag for entities that are destroyed and will be deleted from the game after a period of frames not sure how long.
// Game.Common.Destroyed

// this component marks the entity for deleting or something
// Game.Tools.Temp

// I dont know
// Game.Objects.TripSource

// I think even if the entity id deleted from the game it stills stays a bit as obsolete entity?
// Game.Common.Deleted

// hides entity from AI mostly but not completely, allows for collisions, and physics simulation, when trying to control position velocity etc. of OutOfControl vehicle the physics will be fighting your input tho.
// Game.Vehicles.OutOfControl

// hides the visuals of an entity not sure what else it does, but it fucks with everything
// Game.Tools.Hidden

// lets the game know entity state changes and the game needs to know?
// <Game.Common.Updated