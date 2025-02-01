using RPG.Control;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine;
using RPG.Audio;

namespace RPG.Combat
{
    public class HealthPickup : MonoBehaviour, IWorldClickable
    {
        public float healAmount;
        public AudioClip pickupClip;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Health>().TakeHeal(healAmount);

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