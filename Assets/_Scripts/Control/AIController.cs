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
        FieldOfView FOVCheck;
        public GameObject closestPlayer;
        public List<GameObject> playersList
        = new List<GameObject>();
        CapsuleCollider capsuleCol;        
        CombatTarget otherCombatTarget;         
        int currentWaypointIndex = 0;  
        public AIState currentState;      
        float TimerForNextAttack; 
        Fighter fighter;
        float coolDown;
        Health health;        
        Mover mover;

        public enum AIState
        {
            attack,
            patrol,
            retreat,
            searching,
            reloading,
        }
        
      
       void OnEnable() 
       {
            Health.targetDeath += SearchForPlayer;
            Health.playerDeath += PlayerDeath;
       }

       void OnDisable() 
       {
           Health.targetDeath -= SearchForPlayer;
           Health.playerDeath -= PlayerDeath;
       }

        private void Start()
        { 
            playersList.AddRange(collection: GameObject.FindGameObjectsWithTag("Player"));
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();  
            FOVCheck = GetComponent<FieldOfView>();

            SearchForPlayer();                       
            coolDown = 2.5f;
            TimerForNextAttack = coolDown;            
        }

        private void Update()
        {   
            if (health.IsDead()) return;           
           
            if(FOVCheck.canSeePlayer == true && fighter.CanAttack(combatTarget: closestPlayer))
            { 
                currentState = AIState.attack;
                if (TimerForNextAttack > 0)
                {
                    TimerForNextAttack -= Time.deltaTime;
                }
                else if (TimerForNextAttack <= 0)
                {                    
                    if(capsuleCol != null)
                    { 
                      TimerForNextAttack = coolDown;
                    }
                }                
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                currentState = AIState.searching;
            }
            else
            {
                currentState = AIState.patrol;
            }
            UpdateTimers();   

            switch(currentState)
            {
               case AIState.attack:
               {
                   AttackBehaviour();
               }   
               break;

               case AIState.patrol:
               {
                    PatrolBehaviour();
               }
               break;

               case AIState.reloading:
               {

               }
               break;

               case AIState.retreat:
               {
                   //add retreat position   
               }   
               break;
 
               case AIState.searching:
               {
                    SearchForPlayer();
               }   
               break;
            }         
        }

        public void AttackBehaviour()
        { 
            var playerHealth = closestPlayer.GetComponent<Health>();  
            bool isPlayerDead = playerHealth.isDead;             
            if(isPlayerDead == true) return;                   
            fighter.TargetCapsule = capsuleCol;
            fighter.Target = otherCombatTarget;
            timeSinceLastSawPlayer = 0;
            fighter.Attack(combatTarget: closestPlayer);
        } 

        void PlayerDeath()
        {  
            fighter.TargetCapsule = null;
            fighter.Cancel();
            SearchForPlayer();      
        }     

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }        

        private void PatrolBehaviour()
        {    
            SearchForPlayer();         
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

        void SearchForPlayer()
        {
            float distToClosestPlayer = Mathf.Infinity;
            closestPlayer = null;
            foreach(GameObject player in playersList) 
            { 
              float distanceToPlayer = (player.transform.position - this.transform.position).sqrMagnitude;
              if(distanceToPlayer < distToClosestPlayer)
              {
                distToClosestPlayer = distanceToPlayer;
                if(player == null)
                {
                    currentState = AIState.patrol;
                }
                else
                {
                    closestPlayer = player;
                }               
              }  
            } 
            capsuleCol = closestPlayer.GetComponent<CapsuleCollider>();  
            otherCombatTarget = closestPlayer.GetComponent<CombatTarget>();
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
    }
}
