using UnityEngine;
using RPG.Attributes;
using RPG.Control;
using RPG.Movement;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IWorldClickable
    {
        ActionType IWorldClickable.GetActionType() => ActionType.Attack;

        CursorType IWorldClickable.OnClick(PlayerController playerController, Vector3 hitPoint)
        {
            if (playerController.GetComponent<Mover>().CanMoveToLocation(hitPoint, requiresCompletePath: false))
            {
                playerController.GetComponent<Fighter>().SetAttackTarget(gameObject);
                return CursorType.Attack;
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
                return CursorType.Attack;
            }
            else
            {
                return CursorType.Default;
            }
        }
    }
}