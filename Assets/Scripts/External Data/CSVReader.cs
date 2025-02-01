using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RPG.ExternalData
{
    public static class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };
    
        public static List<Dictionary<string, object>> Read(string file)
        {
            Debug.Log("Running CSVProgressionReader.Load()");

            var masterList = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load(file) as TextAsset; // This gets the raw text from the file
    
            string[] rows = Regex.Split(data.text, LINE_SPLIT_RE); // This splits by newlines

            if (rows.Length <= 1) return masterList; // If there is no text return the empty master list
    
            string[] header = Regex.Split(rows[0], SPLIT_RE); // Split the header row

            for (int i = 1; i < rows.Length; i++) // For each row...
            {
                string[] values = Regex.Split(rows[i], SPLIT_RE); // Split this row into cells
                if (values.Length == 0 || values[0] == "") { continue; } // If this line is empty try the next one
    
                var entry = new Dictionary<string, object>();
                
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }
                    entry[header[j]] = finalvalue;
                }
                masterList.Add(entry);
            }
            return masterList;
        }
    }
}