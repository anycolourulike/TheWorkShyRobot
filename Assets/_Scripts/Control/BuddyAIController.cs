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
    public class BuddyAIController : MonoBehaviour
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

        FieldOfView FOVCheck;
        public GameObject[] enemies;   
        public GameObject enemy;

        public List<CapsuleCollider> capsuleColliderList
        = new List<CapsuleCollider>();

        public List<CombatTarget> otherCombatTargetList
        = new List<CombatTarget>();

        Transform enemyPos;
        

        
        int currentWaypointIndex = 0;        
        float TimerForNextAttack;               
        NavMeshAgent agent;        
               
        Fighter fighter;
        float coolDown;
        Health health;        
        Mover mover;            

        private void Start()
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");  //Change to array for multiple player characters 
            agent = GetComponent<NavMeshAgent>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();  
            FOVCheck = GetComponent<FieldOfView>();

            foreach(GameObject enemy in enemies) 
            { 
              capsuleColliderList.Add(enemy.GetComponent<CapsuleCollider>());  
              otherCombatTargetList.Add(enemy.GetComponent<CombatTarget>()); 
            }  

            TimerForNextAttack = coolDown;
            coolDown = 2.5f;
        }

        private void Update()
        {                       
            if (health.IsDead()) return;   
            UpdateTarget();      
           
            if(FOVCheck.canSeePlayer == true && fighter.CanAttack(enemy))
            {  
                if (TimerForNextAttack > 0)
                {
                    TimerForNextAttack -= Time.deltaTime;
                }
                else if (TimerForNextAttack <= 0)
                {
                    
                    if(capsuleColliderList != null)
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
            capsuleColliderList = null;
            fighter.TargetCapsule = null;
            fighter.Cancel();
            PatrolBehaviour();
        }

        void UpdateTarget() 
        {             
            enemyPos = enemy.transform; 
            enemy = enemies[0];
            fighter.Target = otherCombatTargetList[0];  
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

            //Check if player is out of view, if no move to Player
            //mover.MoveTo(playerPos.position, 7f);
        }

        public void AttackBehaviour()
        {            
            fighter.TargetCapsule = capsuleColliderList[0]; 
            timeSinceLastSawPlayer = 0;
            fighter.Attack(enemy);
        }  
    }
}

