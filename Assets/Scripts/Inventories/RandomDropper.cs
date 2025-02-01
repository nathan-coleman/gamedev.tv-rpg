using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventory
{
    public class RandomDropper: ItemDropper
    {
        [Tooltip("How far pickups can be scattered from the dropper.")]
        [SerializeField] float scatterDistance = 1f;
        protected override Vector3 GetDropLocation()
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;

            for (int i = 0; i < 30; i++)
            {
                if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            return transform.position;
        }
    }
}