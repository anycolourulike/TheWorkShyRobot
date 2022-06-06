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
        [SerializeField] GameObject player;
        public List<GameObject> enemiesList 
        = new List<GameObject>(); 
        CapsuleCollider capsuleCol;
        CombatTarget otherCombatTarget;
        bool primaryTargetSet;   
        float TimerForNextAttack;     
        FieldOfView FOVCheck;          
        public GameObject enemy;                     
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
            enemiesList.AddRange(collection: GameObject.FindGameObjectsWithTag("Enemy"));
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

            enemy = enemiesList[index: 1];          

            TimerForNextAttack = coolDown;
            coolDown = 2.5f;
            
            InvokeRepeating("FollowPlayer", 3f, 6f);
        }

        private void Update()
        {                  
            if (health.IsDead()) return; 
           
            if(FOVCheck.canSeePlayer == true && fighter.CanAttack(combatTarget: enemy))
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
        } 

        public void AttackBehaviour()
        {                  
            fighter.TargetCapsule = capsuleCol; 
            fighter.Target = otherCombatTarget; 
            fighter.Attack(combatTarget: enemy);
        }

        public void RemoveDeadAI(GameObject enemyToRemove)
        {
            for(int i = 0; i < enemiesList.Count; i++)
            {
                if(enemiesList[index: i] == enemyToRemove)
                {
                   enemiesList.Remove(item: enemyToRemove);                  
                }
            }
        }

        void UpdateTarget() 
        {           
            var enemyHealth = enemy.GetComponent<Health>();
            if(enemyHealth.CharacterIsDead == true)
            { 
                NextTarget();
            }
        }         

        IEnumerator RefreshEnemiesList()
        {
            yield return new WaitForSeconds(1f);
            enemiesList.AddRange(collection: GameObject.FindGameObjectsWithTag("Enemy"));
        }

        void NextTarget()
        {        
           int currentTarget = 0;
           for(int i = 0; i < enemiesList.Count; ++i)
           {
               if(enemiesList[index: i] == enemy)
               {
                   currentTarget = i;
               }
           }
           if(enemiesList.Count == 0) return;
           currentTarget = (currentTarget + 1) % enemiesList.Count;                    
           enemy = enemiesList[index: currentTarget]; 
                    
           capsuleCol = enemy.GetComponent<CapsuleCollider>();  
           otherCombatTarget = enemy.GetComponent<CombatTarget>();
        }      

        void FollowPlayer()
        {   
            Vector3 playerDirection = player.transform.position - transform.position;
            Vector3 newPos = RandomNavSphere(origin: player.transform.position, dist: 6f, layermask: 1);
            if(playerDirection.magnitude > 1f)
            {
                mover.MoveTo(destination: newPos, speedFraction: 6);
            }
        } 

        static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
            randDirection += origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
            return navHit.position;
        }               
    }
}

