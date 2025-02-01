using Newtonsoft.Json.Linq;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Control;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IJsonSaveable
    {
        private NavMeshAgent navMeshAgent;
        private Animator anim;
        private float defaultSpeed;

        void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            defaultSpeed = navMeshAgent.speed;
        }

        private void Update()
        {
            if (anim.GetBool("Dead")) { return; }
            UpdateAnimator();
        }

        public bool CanMoveToLocation(Vector3 hitPoint, bool requiresCompletePath)
        {
            NavMeshPath path = new();
            bool pathFound = NavMesh.CalculatePath(transform.position, hitPoint, NavMesh.AllAreas, path);
            if (!pathFound) { return false; }

            if (requiresCompletePath && path.status != NavMeshPathStatus.PathComplete)
            { return false; }
            
            float pathLength = 0f;

            for (int i = 1; i < path.corners.Length; i++)
            {
                pathLength += Vector3.Distance(path.corners[i-1], path.corners[i]);
            }

            if (pathLength > 40f) { return false; }
            
            else { return true; }
        }


        public void MoveTo(Vector3 destination, float speedFraction = 1f, bool fighterCalling = false)
        {
            if (!fighterCalling) { GetComponent<Fighter>().SetAttackTarget(null); }
            
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = defaultSpeed * speedFraction;
            navMeshAgent.destination = destination;
        }

        public void RotateTo(Quaternion Direction)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Direction, 0.1f);
        }

        public void Cancel()
        {
            if(navMeshAgent == null) { return; }
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 Velocity = gameObject.GetComponent<NavMeshAgent>().velocity;
            Vector3 LocalVelocity = transform.InverseTransformDirection(Velocity);

            float Speed = LocalVelocity.z + LocalVelocity.x + LocalVelocity.y;
            anim.SetFloat("Movement Speed", Speed);
        }
        #region Saving
        public JToken CaptureAsJToken()
        {
            Dictionary<string, JToken> saveData = new Dictionary<string, JToken>();
            saveData["position"] = transform.position.ToToken();
            saveData["rotation"] = transform.eulerAngles.ToToken();
            return JToken.FromObject(saveData);
        }

        public void RestoreFromJToken(JToken state)
        {
            Dictionary<string, JToken> saveData = state.ToObject<Dictionary<string, JToken>>();
            navMeshAgent.Warp(saveData["position"].ToVector3());
            transform.eulerAngles = saveData["rotation"].ToVector3();
            // if(gameObject.tag == "Player") { Debug.Log("Loaded position: " + saveData["position"].ToVector3()); }
        }
        #endregion
    }
}