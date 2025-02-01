using UnityEngine;
using RPG.Attributes;
using System;
using GameDevTV.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] CharacterClass characterClass;
        [Range(1, 20)] [SerializeField] int defaultCharacterLevel;
        private LazyValue<int> currentLevel;
        [SerializeField] Progression progression;
        public event Action OnLevelUp;

        private void Awake()
        {
            Experience experience = GetComponent<Experience>();
            if (experience != null)
            {
                experience.OnExperienceGained += UpdateLevel;
            }
            currentLevel = new LazyValue<int>(GetCharacterLevel);
        }

        private void UpdateLevel()
        {
            int newLevel = GetCharacterLevel();
            if (newLevel > currentLevel.value)
            {
                Debug.Log(name + " leveled up to level " + newLevel);
                currentLevel.value = newLevel;
                OnLevelUp();
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetProgressionStat(stat) + GetModifier(stat, SModifier.Additive)) * 1 + GetModifier(stat, SModifier.Multiplicative);
        }

        private float GetProgressionStat(Stat stat)
        {
            return progression.GetStat(characterClass, stat, currentLevel.value);
        }

        private float GetModifier(Stat stat, SModifier modifierType)
        { // For an example see Fighter.cs
            float total = 0f;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.SetModifiers(stat, modifierType))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private int GetCharacterLevel()
        {
            if (GetComponent<Experience>() == null) { return defaultCharacterLevel; }

            float currentXP = GetComponent<Experience>().GetXP();
            float[] requiredXPValues = progression.GetStatList(characterClass, Stat.RequiredXP);

            for (int i = 0; i < requiredXPValues.Length; i++)
            {
                var requiredXP = requiredXPValues[i];

                if(currentXP < requiredXP)
                {
                    return i;
                }
            }
            return 0;
        }
    }
}