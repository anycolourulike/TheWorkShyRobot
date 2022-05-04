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
        public GameObject enemy;        
        public List<GameObject> enemiesList 
        = new List<GameObject>(); 
        CapsuleCollider capsuleCol;
        CombatTarget otherCombatTarget;
        bool primaryTargetSet;
        Transform enemyPos;
        int currentWaypointIndex = 0;        
        float TimerForNextAttack;               
        NavMeshAgent agent;  
        Fighter fighter;
        float coolDown;
        Health health;        
        Mover mover;  

        void OnEnable() 
        {
            Health.targetDeath += NextTarget;
            
        }   

        void OnDisable() 
        {
            Health.targetDeath -= NextTarget;
        }       

        void Start()
        {  
            enemiesList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
            agent = GetComponent<NavMeshAgent>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();  
            FOVCheck = GetComponent<FieldOfView>();

            foreach(GameObject enemy in enemiesList) 
            { 
              capsuleCol = enemy.GetComponent<CapsuleCollider>();  
              otherCombatTarget = enemy.GetComponent<CombatTarget>(); 
            }

            enemy = enemiesList[1];          

            TimerForNextAttack = coolDown;
            coolDown = 2.5f;
        }

        private void Update()
        {                  
            if (health.IsDead()) return; 
           
            if(FOVCheck.canSeePlayer == true && fighter.CanAttack(enemy))
            {  
                UpdateTarget(); 
                if (TimerForNextAttack > 0)
                {
                    TimerForNextAttack -= Time.deltaTime;
                }
                else if (TimerForNextAttack <= 0)
                {                                                        
                    AttackBehaviour();
                    TimerForNextAttack = coolDown;                    
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

        public void AttackBehaviour()
        {                  
            fighter.TargetCapsule = capsuleCol; 
            fighter.Target = otherCombatTarget;                     
            timeSinceLastSawPlayer = 0;
            fighter.Attack(enemy);
        }

        void UpdateTarget() 
        {           
            var enemyHealth = enemy.GetComponent<Health>();
            if(enemyHealth.IsDead())
            { 
                NextTarget();
            } 
            else
            { 
               enemy = enemiesList[1];
               enemyPos = enemy.transform; 
               fighter.Target = otherCombatTarget; 
            }   
        }  

        void NextTarget()
        {
            Debug.Log("NextTarget Called");
           int currentTarget = 0;
           for(int i = 0; i < enemiesList.Count; ++i)
           {
               if(enemiesList[i] == enemy)
               {
                   currentTarget = i;
               }
           }
           currentTarget = (currentTarget + 1) % enemiesList.Count;
           enemy = enemiesList[currentTarget];
        } 

        void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }        

        void PatrolBehaviour()
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

        bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerence;
        }       

        void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }      

        Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        void SuspicionBehaviour()
        {            
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }        
    }
}

