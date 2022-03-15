using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using System;
using UnityEngine.AI;

namespace Rambler.Control
{
    public class AIController : MonoBehaviour
    { 
        [SerializeField] Vector3 nextPosition; 
        [SerializeField] PatrolPath patrolPath;             
        [SerializeField] float suspicionTime = 3f;        
        [SerializeField] float waypointTolerence = 1f;
        [SerializeField] float waypointDwellTime = 1.7f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;  
        float timeSinceArrivedAtWaypoint
         = Mathf.Infinity;
        float timeSinceLastSawPlayer
         = Mathf.Infinity;
        [SerializeField] 
        float chaseDistance = 5f;
        public float ChaseDistance {get{return chaseDistance;} set{chaseDistance = value;}}

        CapsuleCollider capsuleCollider;
        int currentWaypointIndex = 0;        
        float TimerForNextAttack;               
        NavMeshAgent agent;        
        GameObject player;        
        Fighter fighter;
        float coolDown;
        Health health;        
        Mover mover;
        
      
       void OnEnable() 
       {
            Health.playerDeath += PlayerDeath;
       }

       void OnDisable() 
       {
           Health.playerDeath -= PlayerDeath;
       }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");  //Change to array for multiple player characters 
            agent = GetComponent<NavMeshAgent>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();  

            capsuleCollider = player.GetComponent<CapsuleCollider>();  
            CombatTarget otherCombatTarget = player.GetComponent<CombatTarget>(); 
            fighter.Target = otherCombatTarget;                     
            TimerForNextAttack = coolDown;
            coolDown = 2.5f;
        }

        private void Update()
        {                       
            if (health.IsDead()) return;            

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                if (TimerForNextAttack > 0)
                {
                    TimerForNextAttack -= Time.deltaTime;
                }
                else if (TimerForNextAttack <= 0)
                {
                    
                    if(capsuleCollider != null)
                    {
                      AttackBehaviour();
                      TimerForNextAttack = coolDown;
                    }
                }                
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();           
        }

        void PlayerDeath()
        {            
            capsuleCollider = null;
            fighter.TargetCapsule = null;
            fighter.Cancel();
            PatrolBehaviour();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {  
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }            
        }     

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerence;
        }       

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }      

        public Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {            
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public void AttackBehaviour()
        {            
            fighter.TargetCapsule = capsuleCollider; 
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
           float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
           return distanceToPlayer < chaseDistance;
        }     

        //Called by Unity
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);            
        } 

        void FootStep() 
        {
            
        }      
    }
}
