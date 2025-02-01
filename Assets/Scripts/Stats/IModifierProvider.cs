using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> SetModifiers(Stat stat, SModifier modifierType);
    }
}