using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Stats;

namespace RPG.ExternalData
{
    [CreateAssetMenu(fileName = "New CSVImporterWrapper", menuName = "RPG/New CSVImporterWrapper", order = 0)]
    public class CSVProgressionImporter : ScriptableObject
    {
        [SerializeField] private string fileName;
        [SerializeField] private Progression progression;
        [SerializeField] private bool import;

        private void OnValidate()
        {
            if (import)
            {
                import = false;
                ImportData();
            }
        }
        
        private void ImportData()
        {
            List<Dictionary<string, object>> importedData = CSVReader.Read(fileName);
            DealWithImportedData(importedData);
        }

        private void DealWithImportedData(List<Dictionary<string, object>> importedData)
        {
            Dictionary<string, List<Progression.ProgressionStat>> characterProgressionDict = new();

            foreach (var dict in importedData) // each dict is another stat
            {
                // string[] dictKeys = dict.Keys.ToArray();
                object[] dictValues = dict.Values.ToArray();

                if (dictValues[0].ToString() == "" || dictValues[1].ToString() == "")
                { continue; } // if we are missing either  of the first two columns skip this row

                Enum.TryParse(dictValues[1].ToString(), out Stat stat);
                Progression.ProgressionStat progressionStat = new()
                {
                    key = stat.ToString(),
                    stat = stat,
                    values = ConvertDictValues(dictValues)
                };

                if (characterProgressionDict.ContainsKey(dictValues[0].ToString()))
                { // if we have the key in the dictionary already just add to it
                    characterProgressionDict[dictValues[0].ToString()].Add(progressionStat);
                }
                else
                { // if we don't have this key in the dictionary yet then add it with the proper value
                    characterProgressionDict.Add(dictValues[0].ToString(),
                    new() { progressionStat } );
                }
            }

            progression.characterProgressions = new List<Progression.CharacterProgression>();
            string[] cPDictKeys = characterProgressionDict.Keys.ToArray();
            foreach (var key in cPDictKeys)
            {
                Enum.TryParse(key, out CharacterClass tempCharacterClass);
                progression.characterProgressions.Add(new Progression.CharacterProgression
                {
                    key = key,
                    characterClass = tempCharacterClass,
                    characterStats = characterProgressionDict[key]
                });
            }

        }

        private float[] ConvertDictValues(object[] dictValues)
            {
                List<float> output = new();

                for (int i = 3; i < dictValues.Length; i++)
                {
                    float temp;
                    float.TryParse(dictValues[i].ToString(), out temp);
                    output.Add(temp);
                }

                return output.ToArray();
            }
    }
}