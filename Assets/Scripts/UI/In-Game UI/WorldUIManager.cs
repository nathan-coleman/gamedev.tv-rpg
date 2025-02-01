using UnityEngine;

namespace RPG.UI
{
    public class UIManager : MonoBehaviour
    {
        public void ToggleUI(bool enabled)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponentInChildren<HealthBar>().Hide(enabled);
            }
            
            GameObject.Find("HUD").GetComponent<CanvasGroup>().alpha = enabled ? 1f : 0f;
        }
    }
}