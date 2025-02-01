using UnityEngine;
using UnityEngine.Playables;
using GameDevTV.Saving;
using Newtonsoft.Json.Linq;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, IJsonSaveable
    {
        [SerializeField] private bool replayable = false;
        private bool played = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (replayable || !played)
                {
                    GetComponent<PlayableDirector>().Play();
                }
                played = true;
            }
        }

        #region Saving
        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(played);
        }

        public void RestoreFromJToken(JToken state)
        {
            played = state.ToObject<bool>();
        }
        #endregion
    }
}