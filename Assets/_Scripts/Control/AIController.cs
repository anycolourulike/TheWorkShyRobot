using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;
using Rambler.Movement;
using System;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
 
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
        public bool isIdle;
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
        Vector3 nextPosition; 

        GameObject player;
        NavMeshAgent nav;
        Fighter fighter;        
        FieldOfView FOV;

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
           AssignTargetList(); 
           if(patrolPath != null) { nextPosition = patrolPath.GetWaypoint(1);}
           coolDown = 2.5f;
           TimerForNextAttack = coolDown; 
           
           //States
           var followPlayer = new FollowPlayer(this, player, mover);
           var patrol = new Patrol(this, patrolPath, mover, waypointTolerence, waypointDwellTime, patrolSpeedFraction, timeSinceArrivedAtWaypoint, currentWaypointIndex, nextPosition);
           var attack = new Attack(this, mover, FOV, TimerForNextAttack, timeSinceLastSawPlayer, suspicionTime, coolDown);
           var idle = new Idle(this);
           var causeCaveIn = new ICauseCaveIn(this, mover, animator, boulderGennie);
           var sitting = new ISitting(this, animator);
           var collectRocks = new IFindRocks(this, boulderGennie, mover);
           var hasTarget = new IThrowRock(this, boulderGennie);
            
            //Transitions           
           At(attack, patrol, HasTarget());
           At(attack, idle, HasTarget());
           At(attack, followPlayer, HasTarget());
           At(followPlayer, attack, IsCompanion());
           At(attack, followPlayer, CompanionHasNoTarget());
           At(patrol, idle, HasPath());
           At(patrol, attack, HasNoTarget());
           At(idle, attack, IsIdle());
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
           else if ((this.transform.name == "Rocker") && this.gameObject.CompareTag("Enemy"))
           {
                stateMachine.SetState(sitting);
                closestTarget = GameObject.Find("/PlayerCore/Rambler");
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
           Func<bool> CompanionHasNoTarget() => () => this.gameObject.CompareTag("Player") && closestTarget == null;
           Func<bool> HasPath()   => () => patrolPath != null;
           Func<bool> HasTarget() => () => FOV.canSeePlayer == true;
           Func<bool> HasNoTarget() => () => this.gameObject.CompareTag("Enemy") && FOV.canSeePlayer == false;
           Func<bool> IsIdle() => () => this.gameObject.CompareTag("Enemy") && patrolPath == null;
            ////ROCKER////
           Func<bool> StandUp() => () => isRocker == true && standUp == true && caveIn == false;
           Func<bool> CanCauseCaveIn() => () => isRocker == true && rocksOnGround == false && caveIn == true; //no rocks on ground, no rocks in hand, starts jump / cave in sequence
           
           Func<bool> throwRocks() => () => isRocker == true && hasRock == true && caveIn == false && readyToThrow == true;
           Func<bool> nextRock() => () => isRocker == true && hasRock == false && rocksOnGround == true && caveIn == false && readyToThrow == false;
        }        
 
        void Update()
        {
            if (health.IsDead()) return;
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

        public void RockInHand(){hasRock = true;}
        public void EmptyHand(){hasRock = false;}

        public void CaveInTrue(){caveIn = true;}
        public void CaveInFalse(){caveIn = false;} 

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