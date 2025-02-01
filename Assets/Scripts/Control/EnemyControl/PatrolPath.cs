using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        enum PatrolType
        {
            Loop,
            Line
        }
        [SerializeField] PatrolType patrolType;

        public Vector3[] GetPoints()
        {
            List<Vector3> childTransforms = new();

            for (int i = 0; i < transform.childCount; i++)
            {
                childTransforms.Add(transform.GetChild(i).position);
            }

            if (patrolType == PatrolType.Line)
            {
                childTransforms.AddRange(childTransforms.AsEnumerable().Reverse());
            }
            
            return childTransforms.ToArray();
        }

        public void OnDrawGizmosSelected()
        {
            OnDrawGizmos();
            Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(transform.GetChild(i).position, 0.3f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            List<Vector3> childrenPos = new();

            for (int i = 0; i < transform.childCount; i++)
            {
                childrenPos.Add(transform.GetChild(i).position);
                childrenPos.Add(transform.GetChild(i).position);

                // if (i == transform.childCount - 1)
                // {
                //     if (patrolType == PatrolType.Loop)
                //     {
                //         Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(0).position);
                //     }
                // }
                // else
                // {
                //     Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
                // }
            }
            childrenPos.Remove(transform.GetChild(0).position);
            if (patrolType == PatrolType.Loop)
            {
                childrenPos.Add(transform.GetChild(0).position);
            }
            else
            {
                childrenPos.Remove(transform.GetChild(transform.childCount-1).position);
            }

            Gizmos.DrawLineList(childrenPos.ToArray());
        }
    }
}