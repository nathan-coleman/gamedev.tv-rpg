using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "New Progression", menuName = "RPG/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] public List<CharacterProgression> characterProgressions;

        [Serializable] public struct CharacterProgression
        {
            [HideInInspector] public string key;
            [HideInInspector] public CharacterClass characterClass;
            public List<ProgressionStat> characterStats;
        }

        [Serializable] public struct ProgressionStat
        {
            [HideInInspector] public string key;
            [HideInInspector] public Stat stat;
            public float[] values;
        }

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = new();

        public float GetStat(CharacterClass characterClass, Stat stat, int level)
        {
            if (lookupTable.Count == 0) { BuildLookupTable(); }
            // Debug.Log("Returning " + lookupTable[characterClass][stat][level] + " for lookup of " + stat + " by " + characterClass);
            return lookupTable[characterClass][stat][level - 1];
        }

        public float[] GetStatList(CharacterClass characterClass, Stat stat)
        {
            if (lookupTable.Count == 0) { BuildLookupTable(); }
            return lookupTable[characterClass][stat];
        }

        private void BuildLookupTable()
        {
            lookupTable = new();
            foreach (var characterProgression in characterProgressions)
            {
                Dictionary<Stat, float[]> statLookupTable = new();

                foreach (var characterStat in characterProgression.characterStats)
                {
                    statLookupTable.Add(characterStat.stat, characterStat.values);
                }

                lookupTable.Add(characterProgression.characterClass, statLookupTable);
            }
        }
    }
}