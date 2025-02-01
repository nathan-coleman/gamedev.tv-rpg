using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using RPG.Movement;
using GameDevTV.Saving;
using RPG.Attributes;
using RPG.Stats;
using RPG.Audio;
using RPG.Inventory;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] Transform leftHand;
        [SerializeField] Transform rightHand;
        [SerializeField] Weapon defaultWeapon;
        private Weapon currentWeapon;
        private GameObject weaponPrefab;
        private float damage;
        private GameObject target = null;
        private Animator anim;
        private Mover myMover;
        private Equipment equipment;
        private bool isMidSwing = false;

        private void Awake()
        {
            myMover = GetComponent<Mover>();
            anim = GetComponent<Animator>();
            equipment = GetComponent<Equipment>();

            if (equipment) { equipment.equipmentUpdated += UpdateWeapon; }
            GetComponent<BaseStats>().OnLevelUp += UpdateFighterDamage;
        }

        private void Start()
        {
            if (currentWeapon) { EquipWeapon(currentWeapon); }
            else { EquipWeapon(defaultWeapon); }
        }

        private void Update()
        {
            if (anim.GetBool("Dead")) { return; }

            if (target == null)
            {
                anim.SetBool("Attacking", false);
                isMidSwing = false;
            }
            else if (target.GetComponent<Health>().GetHealth() <= 0)
            { // the following line exemplifies a shorthand way to do a null check
                GetComponent<Experience>()?.GainXP(target.GetComponent<BaseStats>().GetStat(Stat.XPGrant));
                target = null;
            }
            else
            {
                if (isMidSwing) { return; }

                if (currentWeapon.weaponRange > Vector3.Distance(gameObject.transform.position, target.transform.position))
                {
                    myMover.Cancel();
                    AttackBehaviour();
                }
                else
                {
                    myMover.MoveTo(target.transform.position, fighterCalling: true);
                    anim.SetBool("Attacking", false);
                }
            }
        }

        public void EquipWeapon(Weapon newWeapon)
        {
            Destroy(weaponPrefab);
            if (newWeapon.isRightHanded)
            {
                weaponPrefab = Instantiate(newWeapon.weaponEquipedPrefab, rightHand);
            }
            else
            {
                weaponPrefab = Instantiate(newWeapon.weaponEquipedPrefab, leftHand);
            }

            currentWeapon = newWeapon;
            anim.runtimeAnimatorController = currentWeapon.weaponAnimOverride;
            
            UpdateFighterDamage();
        }

        private void UpdateWeapon()
        {
            EquippableItem equipableWeapon = equipment.GetItemInSlot(EquipLocation.Weapon);
            if (equipableWeapon is Weapon weapon)
            {
                EquipWeapon(weapon);
            }
            else
            {
                EquipWeapon(defaultWeapon);
            }
        }

        private void UpdateFighterDamage()
        {
            damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
        }

        private void AttackBehaviour() // Called by Update
        {
            anim.SetBool("Attacking", true);
            anim.SetFloat("Attack Speed", GetComponent<BaseStats>().GetStat(Stat.AtkSpeed) + Random.Range(-0.1f, 0.1f));
            gameObject.transform.LookAt(target.transform);

            isMidSwing = true;
        }

        public void SetAttackTarget(GameObject attackTarget) // Called by other scripts
        {
            target = attackTarget;
        }

        public GameObject GetAttackTarget() => target;

        public bool IsInRangeOf(GameObject target)
        {
            return currentWeapon.weaponRange >
                Vector3.Distance(gameObject.transform.position, target.transform.position);
        }

        public void Hit() // Called by animator
        {
            if (target == null) { return; }
            target.GetComponent<Health>().Attack(damage, gameObject);

            isMidSwing = false;
            
            FindObjectOfType<AudioPlayerPool>().pool.Get(out SimpleAudioPlayer simpleAudioPlayer);
            simpleAudioPlayer.SetPosition(transform.position);
            simpleAudioPlayer.SetClip(currentWeapon.hitOrLaunchClip);
        }

        public void Shoot() // Called by animator
        {
            GameObject projectile;
            if (currentWeapon.isRightHanded)
            {
                projectile = Instantiate(currentWeapon.projectilePrefab, rightHand.position, transform.rotation);
            }
            else
            {
                projectile = Instantiate(currentWeapon.projectilePrefab, leftHand.position, transform.rotation);
            }
            projectile.GetComponent<Projectile>().Shoot(damage, target, gameObject);

            isMidSwing = false;
            
            FindObjectOfType<AudioPlayerPool>().pool.Get(out SimpleAudioPlayer simpleAudioPlayer);
            simpleAudioPlayer.SetPosition(transform.position);
            simpleAudioPlayer.SetClip(currentWeapon.hitOrLaunchClip);
        }

        // IEnumerable<float> IModifierProvider.SetModifiers(Stat stat, SModifier modifierType) // called by base stats
        // {
        //     if (modifierType == SModifier.Additive)
        //     {
        //         if (stat == Stat.Damage)
        //         {
        //             yield return currentWeapon.weaponAdditiveDamage;
                    
        //             if (currentWeapon.projectilePrefab != null)
        //             {
        //                 yield return currentWeapon.projectilePrefab.GetComponent<Projectile>().projectileDamage;
        //             }
        //         }
        //     }
        //     else if (modifierType == SModifier.Multiplicative)
        //     {
        //         if (stat == Stat.Damage)
        //         {
        //             yield return currentWeapon.weaponMultiplicativeDamage;
        //         }
        //     }
        // }

        // #region Saving
        // public JToken CaptureAsJToken() => JToken.FromObject(currentWeapon.name);

        // public void RestoreFromJToken(JToken state)
        // {
        //     Weapon newWeapon = Resources.Load<Weapon>(state.ToObject<string>());
        //     EquipWeapon(newWeapon);
        // }
        // #endregion
    }
}