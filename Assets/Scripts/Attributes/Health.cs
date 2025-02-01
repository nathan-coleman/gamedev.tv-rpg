using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Control;
using GameDevTV.Saving;
using RPG.Stats;
using RPG.UI;
using RPG.Audio;

namespace RPG.Attributes
{
    [RequireComponent(typeof(BaseStats))]
    public class Health : MonoBehaviour, IJsonSaveable
    {
        private float maxHealth = -1;
        private float lostHealth = 0;
        [SerializeField] TMPro.TextMeshProUGUI debugDisplay;
        [SerializeField] HealthBar healthBar;
        [SerializeField] AudioClip[] hurtClips;
        [SerializeField] AudioClip[] dieClips;
        private DamageTextPool damageTextPool;
        private bool invulnerable;
        
        public float GetHealth() => maxHealth - lostHealth;

        private void Start()
        {
            damageTextPool = FindObjectOfType<DamageTextPool>();
            GetHealthStat();
            
            if (Mathf.Approximately(lostHealth, maxHealth))
            {
                gameObject.GetComponent<IController>().Die();
            }
        }

        internal void SetInvulnerable(bool invulnerable)
        {
            this.invulnerable = invulnerable;
        }

        private void OnEnable()
        {
            BaseStats baseStats = GetComponent<BaseStats>();
            baseStats.OnLevelUp += GetHealthStat;
            baseStats.OnLevelUp += LevelUpHealthBoost;
        }

        private void OnDisable()
        {
            BaseStats baseStats = GetComponent<BaseStats>();
            baseStats.OnLevelUp -= GetHealthStat;
            baseStats.OnLevelUp -= LevelUpHealthBoost;
        }

        private void LevelUpHealthBoost()
        {
            TakeHeal(lostHealth * 0.5f);
        }

        private void GetHealthStat()
        {
            maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
            UpdateHealthBar();
        }

        public void Attack(float damage, GameObject attacker)
        {
            if (invulnerable) { return; }
            TakeDamage(damage);

            gameObject.GetComponent<IController>().Hit(attacker);
        }

        public void TakeDamage(float damage)
        {
            lostHealth = Mathf.Clamp(lostHealth + damage, 0, maxHealth);

            AudioClip clip;
            if (lostHealth == maxHealth)
            {
                clip = dieClips[Random.Range(0, hurtClips.Length)];
                gameObject.GetComponent<IController>().Die();
            }
            else
            {
                clip = hurtClips[Random.Range(0, hurtClips.Length)];
            }
            
            damageTextPool.pool.Get(out DamageTextScript damageTextScript);
            damageTextScript.SetPosition(transform.position + Vector3.up / 2);
            damageTextScript.SetDamage(Mathf.FloorToInt(damage));
            
            FindObjectOfType<AudioPlayerPool>().pool.Get(out SimpleAudioPlayer simpleAudioPlayer);
            simpleAudioPlayer.SetPosition(transform.position);
            simpleAudioPlayer.SetClip(clip);

            UpdateHealthBar();
        }

        public void TakeHeal(float heal)
        {
            if (maxHealth < 1f) { GetHealthStat(); }
            
            lostHealth = Mathf.Clamp(lostHealth - heal, 0, maxHealth);

            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            healthBar?.SetPercentage((maxHealth - lostHealth) / maxHealth);

            debugDisplay?.SetText("Health: " + Mathf.Floor(maxHealth - lostHealth) + "/" + Mathf.Floor(maxHealth));
        }

        #region Saving
        public JToken CaptureAsJToken() => JToken.FromObject(lostHealth);

        public void RestoreFromJToken(JToken state)
        {
            lostHealth = state.ToObject<float>();
        }
        #endregion
    }
}