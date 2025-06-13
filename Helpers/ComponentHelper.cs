// <copyright file="ComponentHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace VehicleDriver.Helpers
{
    using System.Collections.Generic;
    using Unity.Collections;
    using Unity.Entities;

    // Helper class for safely adding, removing, and restoring Components on entities.
    // Prevents common errors by checking for Component existence before operations.

    /// <summary>
    /// Provides utility methods for safely adding, removing, and managing components in an Entity-Component-System (ECS) environment.
    /// </summary>
    /// <remarks>
    /// The ComponentHelper class ensures common errors such as attempting to add an existing component
    /// or removing a non-existent component are avoided by performing preemptive checks.
    /// These methods are designed to simplify operations within the ECS by automatically verifying
    /// component existence before taking actions.
    /// </remarks>
    public static class ComponentHelper
    {
        /// <summary>
        /// Safely adds a component of type T to the given entity if it is not already present.
        /// Prevents common errors by checking whether the component exists on the entity before attempting to add it.
        /// Intended for use with Unity's ECS (Entity Component System).
        /// This method is static and must be called on the ComponentHelper class.
        /// </summary>
        /// <typeparam name="T">The type of the component to add, which must implement the IComponentData interface and be unmanaged.</typeparam>
        /// <param name="entityManager">The EntityManager instance that controls the entity and its components.</param>
        /// <param name="entity">The entity to which the component should be added.</param>
        public static void SafeAddComponent<T>(EntityManager entityManager, Entity entity)
            where T : unmanaged, IComponentData
        {
            if (!entityManager.HasComponent<T>(entity))
            {
                entityManager.AddComponent<T>(entity);

                // Mod.LOG.Info($"Added Component {typeof(T).Name} to {entity.Index}:{entity.Version}"); // Re-enable for verbose logging
            }

            // else { Mod.LOG.Info($"Component {typeof(T).Name} already present on {entity.Index}:{entity.Version}"); } // Re-enable for verbose logging
        }

        /// <summary>
        /// Safely adds a component of type T to the given entity if it does not already exist.
        /// Ensures that the component is only added when necessary, avoiding duplicate components on the entity.
        /// Designed for use with Unity's Entity Component System (ECS).
        /// This method is static and must be called on the ComponentHelper class.
        /// </summary>
        /// <typeparam name="T">The type of the component to add, which must implement the IComponentData interface and be unmanaged.</typeparam>
        /// <param name="entityManager">The EntityManager instance that manages the entity and its components.</param>
        /// <param name="entity">The entity to which the component should be added.</param>
        public static void SafeAddComponentData<T>(EntityManager entityManager, Entity entity)
            where T : unmanaged, IComponentData
        {
            if (!entityManager.HasComponent<T>(entity))
            {
                entityManager.AddComponentData(entity, default(T));

                // Mod.LOG.Info($"Restored component: {typeof(T).Name}"); // Re-enable for verbose logging
            }

            // else { Mod.LOG.Info($"Component {typeof(T).Name} already present for restore on {entity.Index}:{entity.Version}"); } // Re-enable for verbose logging
        }

        // Safely removes a DynamicBuffer from an entity.

        /// <summary>
        /// Safely removes a DynamicBuffer of type T from the specified entity if it exists.
        /// This method ensures checks for buffer existence before attempting removal, preventing errors.
        /// Intended for use with Unity's ECS (Entity Component System).
        /// This method is static and belongs to the ComponentHelper class.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the DynamicBuffer to remove, which must implement the IBufferElementData interface and be unmanaged.
        /// </typeparam>
        /// <param name="entityManager">
        /// The EntityManager instance responsible for managing the entity and its components.
        /// </param>
        /// <param name="entity">
        /// The entity from which the buffer should be removed.
        /// </param>
        public static void SafeRemoveBuffer<T>(EntityManager entityManager, Entity entity)
            where T : unmanaged, IBufferElementData
        {
            if (entityManager.HasComponent<T>(entity))
            {
                entityManager.RemoveComponent<T>(entity);

                // Mod.LOG.Info($"Removed buffer {typeof(T).Name} from {entity.Index}:{entity.Version}"); // Re-enable for verbose logging
            }
        }

        // Safely removes a Component from an entity.

        /// <summary>
        /// Safely removes a component of type T from the given entity if it exists.
        /// Ensures the operation does not attempt to remove a component that is not present on the entity, preventing errors.
        /// Intended for use with Unity's ECS (Entity Component System).
        /// This method is static and must be called on the ComponentHelper class.
        /// </summary>
        /// <typeparam name="T">The type of the component to remove, which must implement the IComponentData interface and be unmanaged.</typeparam>
        /// <param name="entityManager">The EntityManager instance that controls the entity and its components.</param>
        /// <param name="entity">The entity from which the component should be removed.</param>
        public static void SafeRemoveComponent<T>(EntityManager entityManager, Entity entity)
            where T : unmanaged, IComponentData
        {
            if (entityManager.HasComponent<T>(entity))
            {
                entityManager.RemoveComponent<T>(entity);

                // Mod.LOG.Info($"Removed Component {typeof(T).Name} from {entity.Index}:{entity.Version}"); // Re-enable for verbose logging
            }
        }

        // Safely adds a DynamicBuffer to an entity and populates it with data.
        // Note: This method now assumes the buffer should be *cleared* if data is provided,
        // Or simply ensures the buffer exists if data is null.

        /// <summary>
        /// Never tested this code. Safely adds a DynamicBuffer of type T to the specified entity and populates it with the provided data.
        /// If the buffer is already present on the entity, it is cleared before being populated. If no data is provided,
        /// the method ensures the buffer exists but remains empty.
        /// Typically used in Unity's Entity Component System (ECS) to manage dynamic entity data in a safe and controlled manner.
        /// </summary>
        /// <typeparam name="T">The type of the buffer element to add, which must implement the IBufferElementData interface and be unmanaged.</typeparam>
        /// <param name="entityManager">The EntityManager instance used to manage the entity and its components.</param>
        /// <param name="entity">The entity to which the buffer should be added and populated.</param>
        /// <param name="data">A list of data elements to populate the buffer. If null or empty, the buffer will remain empty.</param>
        public static void SafeAddAndPopulateBuffer<T>(EntityManager entityManager, Entity entity, List<T> data)
            where T : unmanaged, IBufferElementData
        {
            if (!entityManager.HasComponent<T>(entity))
            {
                entityManager.AddBuffer<T>(entity);

                // Mod.LOG.Info($"Added buffer {typeof(T).Name} to {entity.Index}:{entity.Version}"); // Re-enable for verbose logging
            }

            var buffer = entityManager.GetBuffer<T>(entity);
            buffer.Clear();
            if (data == null || data.Count <= 0)
            {
                return;
            }

            using var nativeArray = new NativeArray<T>(data.ToArray(), Allocator.Temp);
            buffer.AddRange(nativeArray);
        }
    }
}