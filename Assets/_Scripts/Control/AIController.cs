using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using Rambler.Attributes;
using System;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

namespace Rambler.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] GameObject closestTarget = null;
        [SerializeField] GameObject footFX1;
        [SerializeField] GameObject footFX2;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float suspicionTime = 3f;       
        [SerializeField] float waypointTolerence = 1f; 
        [SerializeField] float waypointDwellTime = 1.7f;
        [SerializeField] Mover mover;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        float timeSinceArrivedAtWaypoint  
         = Mathf.Infinity;
        float timeSinceLastSawPlayer  
         = Mathf.Infinity;
        public List<GameObject> targetList
        = new List<GameObject>();

        public bool isAttacking;
        public bool isPatroling;
        public bool isReloading;
        public bool isIdle;
        public bool isDead;
        public bool isFollowingPlayer;
        public bool rocksOnGround;
        public bool caveIn;
        public bool isRocker;
        public bool standUp;
        public bool hasRock;
        public bool readyToThrow;
      
        
        BoulderGennie boulderGennie;
        public Animator animator;
        StateMachine stateMachine;        
        CapsuleCollider capsuleCol;        
        CombatTarget otherCombatTarget;     
        int currentWaypointIndex = 0;              
        float TimerForNextAttack; 
        public Vector3 nextPosition; 

        GameObject player;
        NavMeshAgent nav;
        Fighter fighter;        
        FieldOfView FOV;
        CombatTarget combatTarget;

        Boulder boulder;
        float coolDown;        
        Health health; 
        
       void OnEnable() 
       {
            Health.targetDeath += UpdateTarget;
            Health.playerDeath += PlayerDeath;  
            Health.aIHit += FindNearestTarget;
       }
 
       void OnDisable() 
       {
           Health.targetDeath -= UpdateTarget;
           Health.playerDeath -= PlayerDeath;
           Health.aIHit -= FindNearestTarget;
       }

       void Awake()
       {
           if(this.transform.name == "Rocker")
           {              
                isRocker = true;
                rocksOnGround = false;
                nav = GetComponent<NavMeshAgent>();
                boulderGennie = GetComponent<BoulderGennie>();
                animator = GetComponent<Animator>();
           }
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
           combatTarget = GetComponent<CombatTarget>();
           AssignTargetList(); 
           if(patrolPath != null) { nextPosition = patrolPath.GetWaypoint(1);}
           coolDown = 2.5f;
           TimerForNextAttack = coolDown; 
           
           //States
           var followPlayer = new FollowPlayer(this, player, mover);
           var patrol = new Patrol(this, patrolPath, mover, waypointTolerence, waypointDwellTime, patrolSpeedFraction, timeSinceArrivedAtWaypoint, currentWaypointIndex, nextPosition);
           var attack = new Attack(this, fighter, mover, FOV, TimerForNextAttack, timeSinceLastSawPlayer, suspicionTime, coolDown);
           var idle = new Idle(this, fighter);
           var causeCaveIn = new ICauseCaveIn(this, mover, animator, boulderGennie);
           var sitting = new ISitting(this, animator);
           var collectRocks = new IFindRocks(this, boulderGennie, mover);
           var hasTarget = new IThrowRock(this, boulderGennie);
           var dead = new Dead(this, combatTarget, mover, fighter);
           var reloading = new IReload(this, fighter);
            
            //Transitions           
           At(attack, patrol, HasTarget());
           At(attack, idle, HasTarget());
           At(attack, followPlayer, HasTarget());
           At(attack, reloading, Reloaded());
           At(attack, followPlayer, CompanionHasNoTarget());

           At(reloading, attack, IsReloading());
           At(followPlayer, attack, IsCompanion());
           At(idle, attack, IsIdle());
           At(idle, patrol, IsIdle());

           At(patrol, idle, HasPath());
           At(patrol, attack, HasNoTarget());           

           At(dead, patrol, IsDead());
           At(dead, idle, IsDead());
           At(dead, attack, IsDead());
           At(dead, followPlayer, IsDead());

            /////Rocker/////
           At(idle, sitting, StandUp());
           At(causeCaveIn, idle, CanCauseCaveIn());
           At(hasTarget, collectRocks, throwRocks());
           At(collectRocks, hasTarget, nextRock());
           At(causeCaveIn, collectRocks, CanCauseCaveIn());
           At(collectRocks, causeCaveIn, nextRock());

            //Initial State
           if (this.gameObject.CompareTag("Enemy") && patrolPath != null)
           {
             stateMachine.SetState(patrol);
           }
           if ((this.transform.name == "Rocker") && this.gameObject.CompareTag("Enemy"))
           {
                stateMachine.SetState(sitting);
                closestTarget = GameObject.Find("/PlayerCore/Rambler");
           }
           if (this.gameObject.CompareTag("Enemy"))
           {
             stateMachine.SetState(idle);
           } 
           else if (this.gameObject.CompareTag("Player"))
           {
             stateMachine.SetState(followPlayer);
           }        

           //Transitions
           void At(IState to, IState from, Func<bool> condition) => stateMachine.AddTransition(to, from, condition);

           Func<bool> IsDead() => () => isDead == true;
           Func<bool> IsReloading() => () => isReloading == true;
           Func<bool> Reloaded() => () => isReloading == false;
           Func<bool> HasPath() => () => patrolPath != null && isDead == false;
           Func<bool> HasTarget() => () => FOV.canSeePlayer == true && isDead == false && isReloading == false && isAttacking == true;
           Func<bool> IsIdle() => () => this.gameObject.CompareTag("Enemy") && patrolPath == null && isDead == false && FOV.canSeePlayer == false;
           Func<bool> IsCompanion() => () => this.gameObject.CompareTag("Player") && closestTarget != null && isDead == false;
           Func<bool> HasNoTarget() => () => this.gameObject.CompareTag("Enemy") && FOV.canSeePlayer == false;
           Func<bool> CompanionHasNoTarget() => () => this.gameObject.CompareTag("Player") && closestTarget == null;
           
            ////ROCKER////
           Func<bool> StandUp() => () => isRocker == true && standUp == true && caveIn == false;
           Func<bool> CanCauseCaveIn() => () => isRocker == true && rocksOnGround == false && caveIn == true; //no rocks on ground, no rocks in hand, starts jump / cave in sequence
           
           Func<bool> throwRocks() => () => isRocker == true && hasRock == true && caveIn == false && readyToThrow == true;
           Func<bool> nextRock() => () => isRocker == true && hasRock == false && rocksOnGround == true && caveIn == false && readyToThrow == false;
        }        
 
        void Update()
        {            
            stateMachine.Tick();
            Debug.Log(this.gameObject.name + " " + stateMachine._currentState);
        } 

        void FixedUpdate()
        {
            if(this.gameObject.CompareTag("Player"))
            {                
                if(FOV.canSeePlayer == false)
                {
                   fighter.TargetCap = null;              
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

        ///ROCKER///
        public void PlayFootFX1(){footFX1.SetActive(true);}
        public void DisablePlayFootFX1(){footFX1.SetActive(false);}
        public void PlayFootFX2(){footFX2.SetActive(true);}
        public void DisablePlayFootFX2(){footFX2.SetActive(false);}

        public void ReadyToThrow(){readyToThrow = true; mover.RotateTowards(closestTarget.transform); }
        public void NotReadyToThrow(){readyToThrow = false;}

        public void RockInHand() { hasRock = true; }
        public void EmptyHand() { hasRock = false; }

        public void IsPatrolling() { isPatroling = true; }
        public void NotPatrolling() { isPatroling = false; }

        public void FolllowingPlayer() { isFollowingPlayer = true; }
        public void NotFollowingPlayer() { isFollowingPlayer = false; }

        public void CaveInTrue() { caveIn = true; }
        public void CaveInFalse() { caveIn = false; }

        public void IsIdle() { isIdle = true; }
        public void NotIdle() { isIdle = false; }

        public void ReloadingTrue() { isReloading = true; }
        public void ReloadingFalse() { isReloading = false; }

        public void AttackingTrue() { isAttacking = true; }
        public void AttackingFalse() { isAttacking = false; }

        public void StandingUp(){standUp = true;}
        public void NotStandingUp(){standUp = false;}
        
        public void RocksOnGround(){rocksOnGround = true;}
        public void NoRocksOnGround() { rocksOnGround = false; }

        public void FacePlayer()
        {
            player = GameObject.Find("/PlayerCore/Rambler");            
            mover.RotateTowards(player.transform);
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
              capsuleCol = closestTarget.GetComponent<CapsuleCollider>();
              otherCombatTarget = closestTarget.GetComponent<CombatTarget>();                    
            }  
          }
        } 
    }
}