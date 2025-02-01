using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDevTV.Core.UI.Dragging;
using RPG.Inventory;
using UnityEngine.EventSystems;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// Handles spawning pickups when item dropped into the world.
    /// 
    /// Must be placed on the root canvas where items can be dragged. Will be
    /// called if dropped over empty space. 
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(Image))]
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        Image image;
        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) // This checks if we are over a UI
            {
                if (Input.GetMouseButtonDown(0))
                {
                    image.raycastTarget = true;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                image.raycastTarget = false;
            }
        }

        public void AddItems(InventoryItem item, int number)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<ItemDropper>().DropItem(item, number);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return int.MaxValue;
        }
    }
}