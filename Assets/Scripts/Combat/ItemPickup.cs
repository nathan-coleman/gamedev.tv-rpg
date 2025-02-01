using RPG.Inventory;
using RPG.Audio;
using RPG.Control;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class ItemPickup : MonoBehaviour, IWorldClickable
    {
        [SerializeField] AudioClip pickupClip;
        // private Weapon weaponPickup;
        // [SerializeField] int pickupNum = 1;

        // private void Start()
        // {
        //     Instantiate(weaponPickup.weaponEquipedPrefab, transform);
        //     if (weaponPickup != null)
        //     {
        //         GetComponent<Pickup>().Setup(weaponPickup, pickupNum);
        //     }
        // }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                // other.GetComponent<Fighter>().EquipWeapon(weaponPickup);
                GetComponent<Pickup>().PickupItem();

                FindObjectOfType<AudioPlayerPool>().pool.Get(out SimpleAudioPlayer simpleAudioPlayer);
                simpleAudioPlayer.SetPosition(transform.position);
                simpleAudioPlayer.SetClip(pickupClip);

                Destroy(gameObject);
            }
        }

        ActionType IWorldClickable.GetActionType() => ActionType.Pickup;

        CursorType IWorldClickable.OnClick(PlayerController playerController, Vector3 hitPoint)
        {
            if (playerController.GetComponent<Mover>().CanMoveToLocation(hitPoint, requiresCompletePath: false))
            {
                playerController.GetComponent<Mover>().MoveTo(transform.position);
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.Default;
            }
        }

        CursorType IWorldClickable.OnHover(PlayerController playerController, Vector3 hitPoint)
        {
            if (playerController.GetComponent<Mover>().CanMoveToLocation(hitPoint, requiresCompletePath: false))
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.Default;
            }
        }
    }
}