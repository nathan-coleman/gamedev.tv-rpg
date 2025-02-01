using UnityEngine;
using RPG.Attributes;
using RPG.Audio;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [Tooltip("This gets added to the total damage")]
        public float projectileDamage;
        [SerializeField] float projectileSpeed = 15f;
        private GameObject attacker;
        private GameObject target;
        [SerializeField] bool isHoming;
        [SerializeField] GameObject hitEffect;
        [SerializeField] AudioClip hitClip;

        void Update()
        {
            if (target == null) { Destroy(gameObject, 2f); };
            if (target != null)
            {
                if (target.GetComponent<Health>().GetHealth() == 0) { target = null; return; }
                
                if (isHoming) { transform.LookAt(TargetAim()); }
            }
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        public void Shoot(float weaponDamage, GameObject target, GameObject attacker)
        {
            projectileDamage = weaponDamage;
            this.target = target;
            this.attacker = attacker;
            transform.LookAt(TargetAim());
        }

        private Vector3 TargetAim()
        {
            if (target != null && target.GetComponent<CapsuleCollider>() != null)
            {
                return target.transform.position + target.GetComponent<CapsuleCollider>().center;
            }
            else
            {
                return target.transform.position;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == target)
            {
                other.GetComponent<Health>().Attack(projectileDamage, attacker);

                if (hitEffect != null)
                {
                    Instantiate(hitEffect, TargetAim(), transform.rotation);
                }
                
                FindObjectOfType<AudioPlayerPool>().pool.Get(out SimpleAudioPlayer simpleAudioPlayer);
                simpleAudioPlayer.SetPosition(transform.position);
                simpleAudioPlayer.SetClip(hitClip);
                
                Destroy(gameObject);
            }
        }
    }
}