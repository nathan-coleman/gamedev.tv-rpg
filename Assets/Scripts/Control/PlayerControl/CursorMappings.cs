using System;
using UnityEngine;

namespace RPG.Control
{
    public enum CursorType
    {
        Default,
        Attack,
        Move,
        MoveHover,
        Quest,
        Talk,
        Buy,
        Build,
        Pickup,
        None
    }
    
    [CreateAssetMenu(fileName = "New CursorMappings", menuName = "RPG/New CursorMappings")]
    public class CursorMappings : ScriptableObject
    {
        [Serializable] public struct CursorMapping
        {
            public CursorType type;
            public Texture2D image;
            public Vector2 hotspot;
        }

        public CursorMapping[] mappings;
    }
}