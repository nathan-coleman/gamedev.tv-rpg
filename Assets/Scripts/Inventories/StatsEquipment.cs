using System.Collections.Generic;
using RPG.Stats;

namespace RPG.Inventory
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> SetModifiers(Stat stat, SModifier modifierType)
        {
            foreach (var slot in GetAllPopulatedSlots())
            {
                var item = GetItemInSlot(slot) as IModifierProvider;
                if (item == null) { continue; }
                
                foreach (float modifier in item.SetModifiers(stat, modifierType))
                {
                    yield return modifier;
                }
            }
        }
    }
}