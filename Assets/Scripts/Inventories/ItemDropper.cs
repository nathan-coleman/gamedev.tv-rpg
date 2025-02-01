using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;

namespace RPG.Inventory
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, IJsonSaveable
    {
        // STATE
        private List<Pickup> droppedItems = new();
        private List<OtherSceneDropRecord> otherSceneDrops = new();

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">
        /// The number of items contained in the pickup. Only used if the item
        /// is stackable.
        /// </param>
        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, number, GetDropLocation());
        }

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        /// <param name="number">The number of items contained in the pickup.</param>
        /// <param name="spawnLocation">The location to create the pickup.</param>
        public void SpawnPickup(InventoryItem item, int number, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation, number);
            droppedItems.Add(pickup);
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        // PRIVATE

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public Vector3 position;
            public int number;
        }

        private class OtherSceneDropRecord
        {
            public string id;
            public int number;
            public Vector3 location;
            public int scene;
        }

        List<OtherSceneDropRecord> MergeDroppedItemsWithOtherSceneDrops()
        {
            var result = new List<OtherSceneDropRecord>();
            result.AddRange(otherSceneDrops);
            foreach (var item in droppedItems)
            {
                result.Add(new OtherSceneDropRecord()
                {
                    id = item.GetItem().GetItemID(),
                    number = item.GetNumber(),
                    location = item.transform.position,
                    scene = SceneManager.GetActiveScene().buildIndex
                });
            }
            return result;
        }
        
        private void ClearExistingDrops()
        {
            foreach (var oldDrop in droppedItems)
            {
                if (oldDrop != null) Destroy(oldDrop.gameObject);
            }

            otherSceneDrops.Clear();
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
        
        #region Saving
        public JToken CaptureAsJToken()
        {
            RemoveDestroyedDrops();
            var drops = MergeDroppedItemsWithOtherSceneDrops();
            var state = new JArray();
            IList<JToken> stateList = state;
            foreach (var drop in drops)
            {
                JObject dropState = new JObject();
                IDictionary<string, JToken> dropStateDict = dropState;
                dropStateDict["id"] = JToken.FromObject(drop.id);
                dropStateDict["number"] = drop.number;
                dropStateDict["location"] = drop.location.ToToken();
                dropStateDict["scene"] = drop.scene;
                stateList.Add(dropState);
            }

            return state;
        }

        public void RestoreFromJToken(JToken state)
        {
            if (state is JArray stateArray)
            {
                int currentScene = SceneManager.GetActiveScene().buildIndex;
                IList<JToken> stateList = stateArray;
                ClearExistingDrops();
                foreach (var entry in stateList)
                {
                    if (entry is JObject dropState)
                    {
                        IDictionary<string, JToken> dropStateDict = dropState;

                        int scene = dropStateDict["scene"].ToObject<int>();
                        InventoryItem item = InventoryItem.GetFromID(dropStateDict["id"].ToObject<string>());
                        int number = dropStateDict["number"].ToObject<int>();
                        Vector3 location = dropStateDict["location"].ToVector3();

                        if (scene == currentScene)
                        {
                            SpawnPickup(item, number, location);
                        }
                        else
                        {
                            otherSceneDrops.Add(new OtherSceneDropRecord
                            {
                                id = item.GetItemID(),
                                number = number,
                                location = location,
                                scene = scene
                            });
                        }
                    }
                }
            }
        }
        #endregion
    }
}