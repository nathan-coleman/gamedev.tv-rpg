/*
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    /*
    [CreateAssetMenu(fileName = "New Base Progression", menuName = "RPG/Legacy/Old Base Progression", order = 0)]
    public class BaseProgression : ScriptableObject
    {
        public List<ProgressionStat> characterStats;

        [Serializable] public class ProgressionStat
        {
            public Stat stat;
            public float[] values;
        }
    }
    */

    /*
    [CreateAssetMenu(fileName = "New Progression", menuName = "RPG/Legacy/Old Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] BaseProgression baseProgression;
        [SerializeField] StatMultiplier[] statMultipliers;

        [Serializable] class StatMultiplier
        {
            public Stat stat;
            public float value;
        }

        Dictionary<Stat, float[]> lookupTable = new Dictionary<Stat, float[]>();

        public float GetStat(Stat stat, int level)
        {
            if (lookupTable.Count == 0) { BuildLookupTable(); }
            // Debug.Log("Returning " + lookupTable[characterClass][stat][level] + " for lookup of " + stat + " by " + characterClass);
            return lookupTable[stat][level];
        }

        public float[] GetStatList(Stat stat)
        {
            if (lookupTable.Count == 0) { BuildLookupTable(); }
            return lookupTable[stat];
        }

        private void BuildLookupTable()
        {
            foreach (var characterStat in baseProgression.characterStats)
            {
                float statMultiplierfloat = 1;
                foreach (var statMultiplier in statMultipliers)
                {
                    if (statMultiplier.stat == characterStat.stat)
                    {
                        statMultiplierfloat = statMultiplier.value;
                    }
                }

                float[] statTable = characterStat.values;
                for (int i = 0; i < statTable.Length; i++)
                {
                    statTable[i] *= statMultiplierfloat;
                }

                lookupTable[characterStat.stat] = characterStat.values;
            }
        }
    }
    */
//}