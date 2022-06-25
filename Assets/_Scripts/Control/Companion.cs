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
       
        [SerializeField] GameObject player; 
        public List<GameObject> enemiesList 
        = new List<GameObject>(); 
        CapsuleCollider capsuleCol;
        CombatTarget otherCombatTarget;
        public CompanionState currentState; 
        float TimerForNextAttack; 
        float followPlayerTimer = 2f; 
        FieldOfView FOVCheck;      
        NavMeshAgent agent;
        public GameObject closestEnemy; 
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
            FOVCheck = GetComponent<FieldOfView>();              
            SearchForEnemy();
            TimerForNextAttack = coolDown;
            coolDown = 2.5f;
        }

        private void Update()
        {                  
            if (health.IsDead()) return;
            DistanceToPlayer();
            switch(currentState)
            {
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
           
            if(FOVCheck.canSeePlayer == true && fighter.CanAttack(combatTarget: closestEnemy))
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
        } 

        public void AttackBehaviour()
        {    
            fighter.TargetCapsule = capsuleCol; 
            fighter.Target = otherCombatTarget;
            fighter.Attack(combatTarget: closestEnemy);             
        }    

        void UpdateTarget() 
        { 
            if(closestEnemy != null)
            {        
              var enemyHealth = closestEnemy.GetComponent<Health>();            
              bool isEnemyDead = enemyHealth.isDead;
              if(isEnemyDead == true)
              { 
                GetComponent<ActionScheduler>().CancelCurrentAction();
                StartCoroutine("RefreshEnemiesList");
              }
            }
        }         

        IEnumerator RefreshEnemiesList()
        {
            yield return new WaitForSeconds(3f);
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
                    if(FOVCheck == true)
                    closestEnemy = enemy;
                }               
              }  
            } 
            capsuleCol = closestEnemy.GetComponent<CapsuleCollider>();  
            otherCombatTarget = closestEnemy.GetComponent<CombatTarget>();
        }

        void Stop()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void DistanceToPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if(distanceToPlayer < 1.5f)
            {
                currentState = CompanionState.stop;
            }
            else if(distanceToPlayer > 3f)
            {
                currentState = CompanionState.followPlayer;
            }
        }  

        void FollowPlayer()
        {                  
            Vector3 playerDirection = player.transform.position - transform.position;
            Vector3 randPos = (player.transform.position + (transform.forward * 0.5f))
                                + new Vector3(UnityEngine.Random.Range( -5.5f, 5.5f), 0f,
                                UnityEngine.Random.Range(-3.5f, 3.5f)); 
                              

            if(playerDirection.magnitude > 4f)
            {
                mover.MoveTo(destination: randPos, speedFraction: 7f); 
            }             
        }         
    }
}

