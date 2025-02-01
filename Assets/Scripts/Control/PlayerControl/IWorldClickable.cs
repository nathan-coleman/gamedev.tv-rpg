using UnityEngine;

namespace RPG.Control
{
    public enum ActionType // This needs to be in priority order
    {
        Attack,
        Pickup,
        Quest,
        Buy,
        Talk,
        Build,
        Move
    }

    public interface IWorldClickable
    {
        ActionType GetActionType();
        CursorType OnClick(PlayerController playerController, Vector3 hitPoint);
        CursorType OnHover(PlayerController playerController, Vector3 hitPoint);
    }
}