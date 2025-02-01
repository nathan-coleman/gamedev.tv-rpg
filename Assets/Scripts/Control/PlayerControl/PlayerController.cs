using System.Collections.Generic;
using System.Linq; // For sorted dictionary
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using RPG.Movement;
using RPG.Combat;


namespace RPG.Control
{
    [RequireComponent(typeof(Fighter))]
    [RequireComponent(typeof(Mover))]
    public class PlayerController : MonoBehaviour, IController
    {
        [SerializeField] private GameObject cameraTarget;
        [SerializeField] private CursorMappings cursorMappings;
        private bool canControl = true;
        private Fighter myFighter;
        private Mover myMover;
        private Animator Anim;

        private struct ClickInfo
        {
            public IWorldClickable click;
            public Vector3 hitPoint;
        }

        void Awake()
        {
            myFighter = GetComponent<Fighter>();
            myMover = GetComponent<Mover>();
            Anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (canControl)
            {
                SetCursor(InteractAtCursor());
            }
            else
            {
                SetCursor(CursorType.None);
            }
        }

        private CursorType InteractAtCursor()
        {
            if (EventSystem.current.IsPointerOverGameObject()) // This checks if we are over a UI
            {
                return CursorType.Default;
            }

            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(cameraRay); // sorted from furthest to closest
            
            SortedDictionary<int, List<ClickInfo>> allClickables = new();

            foreach (RaycastHit hit in hits)
            {
                IWorldClickable clickable = hit.transform.gameObject.GetComponent<IWorldClickable>();

                if (clickable != null)
                {
                    if (!allClickables.ContainsKey((int)clickable.GetActionType()))
                    {
                        allClickables.Add((int)clickable.GetActionType(), new());
                    }

                    allClickables[(int)clickable.GetActionType()].Add(
                        new ClickInfo { click = clickable, hitPoint = hit.point });
                }
            }

            if (allClickables.Count == 0) { return CursorType.Default; }

            foreach (ClickInfo clickInfo in allClickables.First().Value)
            {
                CursorType cursorType = (Input.GetMouseButton(1) || Input.GetMouseButton(1)) ?
                    clickInfo.click.OnClick(this, clickInfo.hitPoint) :
                    clickInfo.click.OnHover(this, clickInfo.hitPoint);
                
                if (cursorType != CursorType.Default)
                {
                    return cursorType;
                }
            }

            return CursorType.Default;
        }

        private void SetCursor(CursorType cursorType)
        {
            // if (cursorType == CursorType.None)
            // {
                Cursor.visible = !(cursorType == CursorType.None);
            // }
            foreach (var cursorMapping in cursorMappings.mappings)
            {
                if (cursorMapping.type == cursorType)
                {
                    Cursor.SetCursor(cursorMapping.image, cursorMapping.hotspot, CursorMode.Auto);
                    return;
                }
            }
            Debug.LogWarning("Cursor type not found");
        }

        void IController.Die()
        {
            SetControl(false);
            Anim.SetBool("Dead", true);
            if (GetComponent<NavMeshAgent>() != null) { GetComponent<NavMeshAgent>().enabled = false; }
            if (GetComponent<Collider>() != null) { GetComponent<Collider>().enabled = false; }
            // Show UI Screen and do a bunch of other stuff will come in the future
        }

        void IController.Hit(GameObject attacker) // Called by Health.cs
        {
            // flash screen red or something

            if (myFighter.GetAttackTarget() == null &&
                myFighter.IsInRangeOf(attacker))
            {
                myFighter.SetAttackTarget(attacker);
            }
        }

        public void SetControl(bool control)
        {
            canControl = control;
            if (canControl == false)
            {
                myFighter.SetAttackTarget(null);
                myMover.Cancel();
            }
        }
    }
}