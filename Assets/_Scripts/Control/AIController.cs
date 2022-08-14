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
        public PatrolPath patrolPath;    //Patrol & Idle  
        [SerializeField] float suspicionTime = 3f;      //Attack  
        [SerializeField] float waypointTolerence = 1f; //Patrol
        [SerializeField] float waypointDwellTime = 1.7f; //Patrol
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;  //Patrol
        float timeSinceArrivedAtWaypoint  //Patrol
         = Mathf.Infinity;
        float timeSinceLastSawPlayer  //Attack
         = Mathf.Infinity;
        public List<GameObject> targetList
        = new List<GameObject>();
        public GameObject closestTarget = null; //Attack
        CapsuleCollider capsuleCol;        //Attack
        CombatTarget otherCombatTarget;  //Attack   
        int currentWaypointIndex = 0; //Patrol
        StateMachine stateMachine;        
        float TimerForNextAttack; //Attack
        Vector3 nextPosition; //Patrol
        GameObject player;
        Fighter fighter; //Attack        
        FieldOfView FOV; //Attack
        float coolDown;  //Attack
        Health health;        
        Mover mover; //Attack & Patrol
        
       void OnEnable() 
       {
            Health.targetDeath += UpdateTarget;
            Health.playerDeath += PlayerDeath;  
       }
 
       void OnDisable() 
       {
           Health.targetDeath -= UpdateTarget;
           Health.playerDeath -= PlayerDeath;
       }

       void Awake()
       {
           if(this.gameObject.tag == "Player")
           {
              var playerCore = GameObject.Find("PlayerCore");
              player = playerCore.transform.GetChild(1).gameObject;
           }
           else if(this.gameObject.tag == "Enemy")
           {
              player = null;
           }
           stateMachine = new StateMachine();
           fighter = GetComponent<Fighter>();
           health = GetComponent<Health>();            
           mover = GetComponent<Mover>();  
           FOV = GetComponent<FieldOfView>();   
           AssignTargetList(); 
           if(patrolPath != null) { nextPosition = patrolPath.GetWaypoint(1);}
           coolDown = 2.5f;
           TimerForNextAttack = coolDown; 
           //States
           var followPlayer = new FollowPlayer(this, player, mover);
           var patrol = new Patrol(this, patrolPath, mover, waypointTolerence, waypointDwellTime, patrolSpeedFraction, timeSinceArrivedAtWaypoint, currentWaypointIndex, nextPosition);
           var attack = new Attack(this, mover, FOV, TimerForNextAttack, timeSinceLastSawPlayer, suspicionTime, coolDown);
           var idle = new Idle(this);
           //Transitions
           
           At(attack, patrol, HasTarget());
           At(attack, idle, HasTarget());
           At(attack, followPlayer, HasTarget());
           At(followPlayer, attack, IsCompanion());
           At(patrol, idle, HasPath());
           At(patrol, attack, HasNoTarget());
           At(idle, attack, IsIdle());
           
        
           //Initial State
           if(this.gameObject.CompareTag("Enemy") && patrolPath != null)
           {
             stateMachine.SetState(patrol);
           }
           else if (this.gameObject.CompareTag("Enemy") && patrol == null)
           {
             stateMachine.SetState(idle);
           } 
           else if (this.gameObject.CompareTag("Player"))
           {
             stateMachine.SetState(followPlayer);
           }        

           //Transitions
           void At(IState to, IState from, Func<bool> condition) => stateMachine.AddTransition(to, from, condition);

           Func<bool> IsCompanion() => () => this.gameObject.CompareTag("Player") && closestTarget != null;
           Func<bool> HasPath()   => () => patrolPath != null;
           Func<bool> HasTarget() => () => FOV.canSeePlayer == true;
           Func<bool> HasNoTarget() => () => this.gameObject.CompareTag("Enemy") && FOV.canSeePlayer == false;
           Func<bool> IsIdle() => () => this.gameObject.CompareTag("Enemy") && patrolPath == null;
       }        
 
        void Update()
        {
            if (health.IsDead()) return;
            stateMachine.Tick();
        } 

        void FixedUpdate()
        {
            if(this.gameObject.CompareTag("Player"))
            {
                // if(FOV.PlayerDetect() == true) 
                // {
                //   FOV.canSeePlayer = false;
                // }
                if(FOV.canSeePlayer == false)
                {
                   fighter.TargetCap = null;
                }
                if(fighter.TargetCap == null)
                {
                   UpdateTarget();
                }
            }
        }

        public void TargetHealthCheck()
        { 
           if(closestTarget == null) return;
           var targetIsDead = closestTarget.GetComponent<Health>().isDead; 
           if(targetIsDead == true) {UpdateTarget();};  
        }

        public void AssignTarget()
        {
            fighter.TargetCap = capsuleCol;
            fighter.CombatTarget = otherCombatTarget;         
            fighter.Attack(combatTarget: closestTarget);
        }

        public void AttackBehaviour()
        {  
            var targetAlive = closestTarget.GetComponent<Health>().isDead;
            if(targetAlive == false) {return;}
            fighter.Attack(combatTarget: closestTarget);
        }
 
        void PlayerDeath()
        {  
            fighter.TargetCap = null;
            fighter.CancelTarget();
            UpdateTarget();    
        } 

        void UpdateTarget() 
        {     
            StartCoroutine("RefreshEnemiesList"); 
        }   

        IEnumerator RefreshEnemiesList()
        {
            targetList.Clear();
            AssignTargetList();
            FindNearestTarget();
            yield return new WaitForSeconds(0.01f);
        }             

        void AssignTargetList()
        {
            if(this.gameObject.tag == "Player") 
            {
                targetList.AddRange(collection: GameObject.FindGameObjectsWithTag("Enemy"));
            }
            else if(this.gameObject.tag == "Enemy")
            {
                targetList.AddRange(collection: GameObject.FindGameObjectsWithTag("Player"));
            }
        } 
 
        public void FindNearestTarget()
        {
          float distToClosestTarget = Mathf.Infinity;
          closestTarget = null;
          foreach(GameObject target in targetList) 
          { 
            float distanceToTarget = (target.transform.position - this.transform.position).sqrMagnitude;
            if(distanceToTarget < distToClosestTarget)
            {
              distToClosestTarget = distanceToTarget;
              closestTarget = target;
              if(closestTarget == null) return;
              capsuleCol = closestTarget.GetComponent<CapsuleCollider>();  
              otherCombatTarget = closestTarget.GetComponent<CombatTarget>();  
            }  
          }
        } 
    }
}