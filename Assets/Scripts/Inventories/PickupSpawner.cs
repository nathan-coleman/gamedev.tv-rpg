﻿using UnityEngine;
using GameDevTV.Saving;
using Newtonsoft.Json.Linq;

namespace RPG.Inventory
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, IJsonSaveable
    {
        // CONFIG DATA
        [SerializeField] InventoryItem item = null;
        [SerializeField] int number = 1;

        // LIFECYCLE METHODS
        private void Awake()
        {
            // Spawn in Awake so can be destroyed by save system after.
            SpawnPickup();
        }

        // PUBLIC

        /// <summary>
        /// Returns the pickup spawned by this class if it exists.
        /// </summary>
        /// <returns>Returns null if the pickup has been collected.</returns>
        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        /// <summary>
        /// True if the pickup was collected.
        /// </summary>
        public bool isCollected() => GetPickup() == null;

        //PRIVATE

        private void SpawnPickup()
        {
            var spawnedPickup = item.SpawnPickup(transform.position, number);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        #region Saving
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(isCollected());
        }

        public void RestoreFromJToken(JToken state)
        {
            bool shouldBeCollected = state.ToObject<bool>();

            if (shouldBeCollected && !isCollected())
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected())
            {
                SpawnPickup();
            }

        }
        #endregion
    }
}