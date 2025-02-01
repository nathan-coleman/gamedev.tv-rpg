using UnityEngine;
using UnityEngine.Playables;
using RPG.Control;
using RPG.UI;
using RPG.Attributes;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private void OnEnable()
        {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        private void OnDisable()
        {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector _)
        {
            ToggleControl(false);
        }

        private void EnableControl(PlayableDirector _)
        {
            ToggleControl(true);
        }

        public void ToggleControl(bool enable)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerController>().SetControl(enable);
            player.GetComponent<Health>().SetInvulnerable(!enable);

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponentInChildren<HealthBar>().Hide(enable);
            }

            GameObject.Find("HUD").GetComponent<CanvasGroup>().alpha = enable ? 1f : 0f;
        }
    }
}