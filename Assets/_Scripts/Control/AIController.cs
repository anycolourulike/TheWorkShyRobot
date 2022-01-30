﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using System;

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
        public float chaseDistance = 5f;
        int currentWaypointIndex = 0;        
        float TimerForNextAttack;
        Vector3 guardPosition;
        
        GameObject player;
        Fighter fighter;
        Health health;        
        Mover mover;
        float Cool;  

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();            
            mover = GetComponent<Mover>();
            guardPosition = transform.position;            
            TimerForNextAttack = Cool;
            Cool = 2.5f;
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
                    AttackBehaviour();
                    TimerForNextAttack = Cool;
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

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {            
            nextPosition = guardPosition;

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

        public Vector3 nextPos {get{ return nextPosition; }}

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
    }
}
