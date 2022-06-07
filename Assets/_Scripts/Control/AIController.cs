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
        public GameObject player;
        public List<GameObject> playersList
        = new List<GameObject>();
        CapsuleCollider capsuleCol;        
        CombatTarget otherCombatTarget;        
        bool primaryTargetSet;
        Transform playerPos;
        int currentWaypointIndex = 0;        
        float TimerForNextAttack; 
        Fighter fighter;
        float coolDown;
        Health health;        
        Mover mover;
        
      
       void OnEnable() 
       {
            Health.targetDeath += NextTarget;
            Health.playerDeath += PlayerDeath;
       }

       void OnDisable() 
       {
           Health.targetDeath -= NextTarget;
           Health.playerDeath -= PlayerDeath;
       }

        private void Start()
        { 
            playersList.AddRange(collection: GameObject.FindGameObjectsWithTag("Player"));
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();  
            FOVCheck = GetComponent<FieldOfView>();

            foreach(GameObject player in playersList) 
            { 
              capsuleCol = player.GetComponent<CapsuleCollider>();  
              otherCombatTarget = player.GetComponent<CombatTarget>(); 
            }            

            TimerForNextAttack = coolDown;
            coolDown = 2.5f;
        }

        private void Update()
        {                  
            if (health.IsDead()) return;
           
            if(FOVCheck.canSeePlayer == true && fighter.CanAttack(combatTarget: player))
            { 
                if (TimerForNextAttack > 0)
                {
                    TimerForNextAttack -= Time.deltaTime;
                }
                else if (TimerForNextAttack <= 0)
                {
                    
                    if(capsuleCol != null)
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

        public void AttackBehaviour()
        {                       
            fighter.TargetCapsule = capsuleCol;
            fighter.otherCombatTarget = otherCombatTarget;
            timeSinceLastSawPlayer = 0;
            fighter.Attack(combatTarget: player);
        }   

        void PlayerDeath()
        {  
            fighter.TargetCapsule = null;
            fighter.Cancel();
            PatrolBehaviour();            
        }

       

        void NextTarget()
        {          
           int currentTarget = 0;
           for(int i = 0; i < playersList.Count; ++i)
           {
               if(playersList[index: i] == player)
               {
                   currentTarget = i;
               }
           }
           if(playersList.Count == 0) return;
           currentTarget = (currentTarget + 1) % playersList.Count;
           player = playersList[index: currentTarget];           
           capsuleCol = player.GetComponent<CapsuleCollider>();  
           otherCombatTarget = player.GetComponent<CombatTarget>();
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
                mover.StartMoveAction(destination: nextPosition, speedFraction: patrolSpeedFraction);
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
    }
}
