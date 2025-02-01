using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
{
    [CreateAssetMenu(fileName = "Loot Table", menuName = "RPG/New Loot Table")]
    public class LootTable : ScriptableObject
    {
        [System.Serializable] public struct LootDrop
        {
            public InventoryItem item;
            [HideInInspector] public int Number { get
                {
                    return Random.Range(dropNumberRange.x, dropNumberRange.y + 1);
                } }
            [SerializeField] Vector2Int dropNumberRange;
        }

        [SerializeField] private List<LootDrop> lootDrops;

        public LootDrop GetRandomLoot()
        {
            int index = Random.Range(0, lootDrops.Count);
            return lootDrops[index];
        }
    }
}