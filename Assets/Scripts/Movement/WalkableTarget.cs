using RPG.Control;
using UnityEngine;

namespace RPG.Movement
{
    public class WalkableTarget : MonoBehaviour, IWorldClickable
    {
        ActionType IWorldClickable.GetActionType() => ActionType.Move;

        CursorType IWorldClickable.OnClick(PlayerController playerController, Vector3 hitPoint)
        {
            if (Input.GetButton("LeftShift") == false &&
                playerController.GetComponent<Mover>().CanMoveToLocation(hitPoint, requiresCompletePath: true))
            {
                playerController.GetComponent<Mover>().MoveTo(hitPoint);
                return CursorType.Move;
            }
            else
            {
                return CursorType.Default;
            }
        }

        CursorType IWorldClickable.OnHover(PlayerController playerController, Vector3 hitPoint)
        {
            if (Input.GetButton("LeftShift") == false &&
                playerController.GetComponent<Mover>().CanMoveToLocation(hitPoint, requiresCompletePath: true))
            {
                return CursorType.MoveHover;
            }
            else
            {
                return CursorType.Default;
            }
        }
    }
}