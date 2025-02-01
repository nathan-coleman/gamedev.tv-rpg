using System.Collections.Generic;
using System.Linq;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventory
{
    [CreateAssetMenu(menuName = "RPG/Inventory/EquippableItem")]
    public class StatsEquippableItem : EquippableItem, IModifierProvider
    {
        [System.Serializable] struct Modifier
        {
            public SModifier modifierType;
            public Stat stat;
            public float value;
        }

        [SerializeField] Modifier[] modifiers;
        public IEnumerable<float> SetModifiers(Stat stat, SModifier modifierType)
        {
            foreach (var modifier in modifiers.Where(m => m.stat == stat)
                     .Where(m => m.modifierType == modifierType))
            {
                yield return modifier.value;
            }
        }
    }
}