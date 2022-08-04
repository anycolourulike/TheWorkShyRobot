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
    public class Companion : MonoBehaviour
    {   
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f; 
        [SerializeField] float waypointDwellTime = 1.7f;  
        [SerializeField] float waypointTolerence = 1f;
        [SerializeField] PatrolPath patrolPath;     
        [SerializeField] GameObject player; 
        float timeSinceArrivedAtWaypoint
         = Mathf.Infinity;
        public List<GameObject> enemiesList 
        = new List<GameObject>(); 
        CapsuleCollider capsuleCol;
        CombatTarget otherCombatTarget;
        public CompanionState currentState; 
        float followPlayerTimer = 2f; 
        int currentWaypointIndex = 0; 
        float TimerForNextAttack;                  
        GameObject closestEnemy;             
        Vector3 nextPosition; 
        NavMeshAgent agent;
        FieldOfView FOV;
        Fighter fighter;
        float coolDown;        
        Health health;        
        Mover mover;         

        void OnEnable() 
        {
            Health.targetDeath += UpdateTarget;            
        }   

        void OnDisable() 
        {
            Health.targetDeath -= UpdateTarget;
        }   

        public enum CompanionState
        {    
            patrolBehaviour,        
            followPlayer,
            attackEnemy,
            stop,
        } 

        void Start()
        {  
            enemiesList.AddRange(collection: GameObject.FindGameObjectsWithTag(tag: "Enemy"));
            agent = GetComponent<NavMeshAgent>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();  
            FOV = GetComponent<FieldOfView>();              
            SearchForEnemy();
            TimerForNextAttack = coolDown;
            coolDown = 2.5f;
        }

        private void Update()
        {                  
            if (health.IsDead()) return;
            DistanceToPlayer();            
           
            if(FOV.canSeePlayer == true)// && fighter.CanAttack(combatTarget: closestEnemy))
            {    
                currentState = CompanionState.attackEnemy;              
                if (TimerForNextAttack > 0)
                {
                    TimerForNextAttack -= Time.deltaTime;
                }
                else if (TimerForNextAttack <= 0)
                {                                                        
                    currentState = CompanionState.attackEnemy;
                    TimerForNextAttack = coolDown;                    
                }                
            }
            else
            {
               currentState = CompanionState.followPlayer;
            }

            switch(currentState)
            {
                case CompanionState.patrolBehaviour:
                {
                    PatrolBehaviour();
                    break;
                }

                case CompanionState.followPlayer:
                {                    
                    followPlayerTimer += Time.deltaTime;
                    if (followPlayerTimer >= 1f) 
                    {
                      followPlayerTimer = followPlayerTimer % 1f;         
                      FollowPlayer();
                    }
                }
                break;

                case CompanionState.attackEnemy:
                {              
                    AttackBehaviour();                    
                }
                break;

                case CompanionState.stop:
                {
                    Stop();
                }
                break;              
            }
        } 

        public void AttackBehaviour()
        {  
            var enemyHealth = closestEnemy.GetComponent<Health>();            
            bool isEnemyDead = enemyHealth.isDead;
            if(isEnemyDead == true) return;
            fighter.TargetCap = capsuleCol; 
            fighter.Target = otherCombatTarget;            
            mover.Cancel();
            fighter.Attack(combatTarget: closestEnemy);             
        }    

        void UpdateTarget() 
        { 
            if(closestEnemy != null)
            { 
                GetComponent<ActionScheduler>().CancelCurrentAction();
                StartCoroutine("RefreshEnemiesList");              
            }
            else
            {
                SearchForEnemy();
            }
        }         

        IEnumerator RefreshEnemiesList()
        {
            yield return new WaitForSeconds(0.1f);
            enemiesList.Clear();
            enemiesList.AddRange(collection: GameObject.FindGameObjectsWithTag("Enemy"));
            SearchForEnemy();
        }

        void SearchForEnemy()
        {
            float distToClosestPlayer = Mathf.Infinity;
            closestEnemy = null;
            foreach(GameObject enemy in enemiesList) 
            { 
              float distanceToPlayer = (enemy.transform.position - this.transform.position).sqrMagnitude;
              if(distanceToPlayer < distToClosestPlayer)
              {
                distToClosestPlayer = distanceToPlayer;
                if(enemy == null)
                {
                    currentState = CompanionState.followPlayer;
                }
                else
                {
                    currentState = CompanionState.attackEnemy;
                    closestEnemy = enemy;
                    capsuleCol = closestEnemy.GetComponent<CapsuleCollider>();  
                    otherCombatTarget = closestEnemy.GetComponent<CombatTarget>();                   
                }               
              }  
            }            
        }

        void Stop()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void DistanceToPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if(distanceToPlayer < 2f)
            {
                currentState = CompanionState.stop;
            }
            else if(distanceToPlayer > 7f)
            {
                currentState = CompanionState.followPlayer;
            }
        }  

        void FollowPlayer()
        {                  
            Vector3 playerDirection = player.transform.position - transform.position;
            Vector3 randPos = (player.transform.position + (transform.forward * 10.5f))
                                + new Vector3(UnityEngine.Random.Range( -1.5f, 5.5f), 0f,
                                UnityEngine.Random.Range(-1.5f, 3.5f)); 
                              

            if(playerDirection.magnitude > 4f)
            {
                mover.MoveTo(destination: randPos, speedFraction: 7f); 
            }             
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
                mover.StartMoveAction(destination: nextPosition, speedFraction: patrolSpeedFraction);
            }            
        }
        void UpdateTimers()
        {
            timeSinceArrivedAtWaypoint += Time.deltaTime;
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

        public Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }         
    }
}

