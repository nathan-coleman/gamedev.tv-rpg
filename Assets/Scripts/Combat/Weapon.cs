using RPG.Inventory;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "RPG/New Weapon", order = 0)]
    public class Weapon : StatsEquippableItem
    {
        // Weapons are not allowed to have start functions
        
        [Header("Weapon Visuals and Audios")]
        public AnimatorOverrideController weaponAnimOverride;
        public AudioClip hitOrLaunchClip;
        public GameObject weaponEquipedPrefab;
        [Tooltip("We will assume a melee weapon without this.")]
        public bool isRightHanded = true;
        [Tooltip("If there is no projectile this will not get used.")]
        public GameObject projectilePrefab;

        [Header("Weapon Stats")]
        public float weaponRange = 2f;
    }
}