using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System;
using RPG.Combat;
using RPG.Movement;
using RPG.Attributes;
using RPG.Inventory;

namespace RPG.Control
{
    enum ActionStates
    {
        AttackingPlayer,
        FollowingBuddy,
        Patrolling,
        ReturningToPatrol
    }
    
    public class EnemyController : MonoBehaviour, IController
    {
        private Animator anim;
        private GameObject player;
        private Fighter myFighter;
        private Mover myMover;

        [SerializeField] float viewDist;
        [Tooltip("If this is null the enemy stands in one place")]
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] GameObject patrolPathPrefab;
        [Tooltip("How far the enemy will chase the player before retruning to patrol")]
        [SerializeField] float backToPatrolDist = 10f;
        [SerializeField] [Range(0f, 1f)] float patrolSpeed = 0.5f;
        [Tooltip("How long the enemy will chase the player after getting hit whether or not the player is in range")]
        [SerializeField] float hitChaseTime = 4f;
        [SerializeField] LootTable lootTable;
        private float hitChaseTimer = 0f;
        private int currentPatrolIndex;
        [SerializeField] bool debug;
        private ActionStates currentAction;

        private void Awake()
        {
            myFighter = GetComponent<Fighter>();
            myMover = GetComponent<Mover>();
            anim = GetComponent<Animator>();
            player = GameObject.FindGameObjectWithTag("Player");

            if (patrolPath == null)
            {
                patrolPath = Instantiate(patrolPathPrefab, transform.position, new()).GetComponent<PatrolPath>();
            }
        }

        private void FixedUpdate()
        {
            if (anim.GetBool("Dead")) { return; }
            
            AICalculations();

            if (debug) { Debug.Log("CurrentAction: " + currentAction); }
        }

        private void AICalculations()
        {
            if (WasHitRecently())
            {
                myFighter.SetAttackTarget(player);

                currentAction = ActionStates.AttackingPlayer;
            }
            else if (ShouldReturnToPatrol())
            {
                myFighter.SetAttackTarget(null);
                myMover.MoveTo(NearestPatrolPoint(), 0.8f);

                currentAction = ActionStates.ReturningToPatrol;
            }
            else if (ShouldAttack())
            {
                myFighter.SetAttackTarget(player);

                currentAction = ActionStates.AttackingPlayer;
            }
            else if (ShouldFollowBuddy(out GameObject buddy))
            {
                myMover.MoveTo(buddy.transform.position);

                currentAction = ActionStates.FollowingBuddy;
            }
            else // (ShouldPatrol())
            {
                myFighter.SetAttackTarget(null);

                Vector3 nextPatrolPoint = NextPatrolPoint();
                if (nextPatrolPoint != new Vector3())
                {
                    myMover.MoveTo(nextPatrolPoint, speedFraction: patrolSpeed);
                }
                else if (currentAction != ActionStates.Patrolling)
                {
                    myMover.MoveTo(NearestPatrolPoint(), speedFraction: patrolSpeed);
                }
                currentAction = ActionStates.Patrolling;
            }
        }

        private bool WasHitRecently() // Called when deciding whether to fight player
        {
            if (hitChaseTimer > 0)
            {
                hitChaseTimer -= Time.deltaTime;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ShouldReturnToPatrol() // Called when deciding whether to return to patrol path
        {
            if (currentAction == ActionStates.Patrolling)
            { return false; }

            // float tempBackToPatrolDist = Mathf.Clamp(
            //     currentAction == ActionStates.ReturningToPatrol ? backToPatrolDist - viewDist - 1f : backToPatrolDist,
            //     2f, Mathf.Infinity);
            // // if we are going back then we need to go back further, this is to stop the enemies from glitch walking in one spot
            
            Vector3[] patrolPoints = patrolPath.GetPoints();
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Vector3 lineStartPoint = patrolPoints[i];

                Vector3 lineEndPoint;

                if (i == patrolPoints.Length - 1)
                { lineEndPoint = patrolPoints[0]; }
                else
                { lineEndPoint = patrolPoints[i+1]; }

                float distToLine = DistanceToLine(transform.position, lineStartPoint, lineEndPoint);

                if (distToLine < backToPatrolDist)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ShouldAttack() // Called when deciding whether to attack
        {
            if (player.GetComponent<Health>().GetHealth() == 0)
            { return false; } // Player is dead

            if (Vector3.Distance(transform.position, player.transform.position) <= viewDist)
            { return true; } // I can see the player so I will attack him
            
            return false;
        }

        private bool ShouldFollowBuddy(out GameObject buddy) // Called when deciding whether to follow buddy
        {
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, viewDist);

            foreach (Collider nearbyObject in nearbyObjects)
            {
                if (nearbyObject.gameObject != this && // if it is not me
                    nearbyObject.GetComponent<EnemyController>() != null && // and it is an enemy
                    nearbyObject.GetComponent<EnemyController>().currentAction == ActionStates.AttackingPlayer) // and he can see the player
                {
                    buddy = nearbyObject.gameObject;
                    return true;
                }
            }
            buddy = null;
            return false;
        }

        private Vector3 NearestPatrolPoint()
        {
            SortedDictionary<float, int> patrolPointDists = new();
            Vector3[] patrolPoints = patrolPath.GetPoints();
            if (patrolPoints.Length == 1)
            {
                return patrolPoints[0];
            }
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                var dist = Vector3.Distance(transform.position, patrolPoints[i]);
                if (!patrolPointDists.ContainsKey(dist))
                {
                    patrolPointDists.Add(dist, i);
                }
            }
            currentPatrolIndex = patrolPointDists.First().Value;
            return patrolPoints[patrolPointDists.First().Value];
        }

        private Vector3 NextPatrolPoint()
        {
            Vector3[] patrolPoints = patrolPath.GetPoints();
            if(debug) { Debug.Log(currentPatrolIndex); }
            if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex]) < 2f)
            {
                if (currentPatrolIndex == patrolPoints.Length - 1)
                {
                    currentPatrolIndex = 0;
                }
                else
                {
                    currentPatrolIndex += 1;
                }
                return patrolPoints[currentPatrolIndex];
            }
            return new();
        }
        
        private float DistanceToLine(Vector3 p, Vector3 l1, Vector3 l2)
        {
            // I got this from Stack Overflow and adapted it to my needs
            // https://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment/20059470#20059470
            float line_dist = Mathf.Pow(Vector3.Distance(l1, l2), 2);
            if (Mathf.Approximately(line_dist, 0f))
            { return Mathf.Pow(Vector3.Distance(p, l1), 2); }
            float t = ((p.x - l1.x) * (l2.x - l1.x) + (p.y - l1.y) * (l2.y - l1.y) + (p.z - l1.z) * (l2.z - l1.z)) / line_dist;
            t = Mathf.Clamp01(t);
            Vector3 c = new(l1.x + t * (l2.x - l1.x), l1.y + t * (l2.y - l1.y), l1.z + t * (l2.z - l1.z));
            return Vector3.Distance(p, c);
        }

        private void DropLoot()
        {
            var drop = lootTable.GetRandomLoot();
            GetComponent<RandomDropper>().DropItem(drop.item, drop.Number);
        }

        void IController.Die() // Called by Health.cs
        {
            anim.SetBool("Dead", true);
            if (GetComponent<CombatTarget>() != null) { GetComponent<CombatTarget>().enabled = false; }
            if (GetComponent<Collider>() != null) { GetComponent<Collider>().enabled = false; }
            if (GetComponent<NavMeshAgent>() != null) { GetComponent<NavMeshAgent>().enabled = false; }
            if (GetComponent<Fighter>() != null) { GetComponent<Fighter>().enabled = false; }
            if (GetComponent<Mover>() != null) { GetComponent<Mover>().enabled = false; }
            DropLoot();
        }

        void IController.Hit(GameObject attacker) // Called by Health.cs
        {
            hitChaseTimer = hitChaseTime;
        }

        private void OnDrawGizmosSelected() // Called by Unity at EditorTime
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, viewDist);
            
            if (patrolPath != null)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
                Gizmos.DrawLine(transform.position, NearestPatrolPoint());
                patrolPath.OnDrawGizmosSelected();
            }
        }
    }
}