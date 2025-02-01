using UnityEngine;
using GameDevTV.Saving;
using Newtonsoft.Json.Linq;
using System;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, IJsonSaveable
    {
        private float currentXP = 0;
        [SerializeField] TMPro.TextMeshProUGUI debugDisplay;
        public Action OnExperienceGained;

        private void Start()
        {
            if (debugDisplay == null) { return; }
            debugDisplay.text = "XP: " + Mathf.Floor(currentXP);
        }

        public void GainXP(float gain)
        {
            currentXP += gain;
            OnExperienceGained();
            
            if (debugDisplay == null) { return; }
            debugDisplay.text = "XP: " + Mathf.Floor(currentXP);
        }

        public float GetXP() => currentXP;

        #region Saving
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentXP);
        }

        public void RestoreFromJToken(JToken state)
        {
            currentXP = state.ToObject<float>();

            if (debugDisplay == null) { return; }
            debugDisplay.text = "XP: " + Mathf.Floor(currentXP);
        }
        #endregion
    }
}