using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Saving;
using Rambler.Combat;
using Rambler.Control;
using Rambler.Core;
using Rambler.Movement;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

namespace Rambler.Core // To Do Stop Movement
{
    public class Health : MonoBehaviour
    {        
        public float healthPoints = 100f; 
        public float HealthPoints { get{return healthPoints;} set{healthPoints = value;}}  
        [SerializeField] AIController aIScript;  
        [SerializeField] GameObject RigLayer;
        [SerializeField] GameObject headFX;
        [SerializeField] GameObject legFX; 
        [SerializeField] GameObject armFX;
        [SerializeField] GameObject shield;
        [SerializeField] Mover mover;
        [SerializeField] EnemySpawn enemyspawn;
        [SerializeField] BuddyAIController buddyCon;
        
        public delegate void TargetDeath();
        public static event TargetDeath targetDeath;

        public delegate void PlayerDied();
        public static event PlayerDied playerDeath;
        CapsuleCollider capCol;
        bool isDead;
        public bool CharacterIsDead {get{return isDead;}}        
        Fighter fighter;
        float damage; 
        public float SetDamage {set{damage = value;}}
        Animator anim;
        int dieRanNum; 
        int hitRanNum;        
        Rigidbody rb;                     
       
        void Start() 
        {            
            anim = GetComponent<Animator>(); 
            rb = GetComponent<Rigidbody>(); 
            fighter = GetComponent<Fighter>(); 
            capCol = GetComponent<CapsuleCollider>();      
        }

        void Update() 
        {
            if(isDead == true) return;
            HealthCheck();
        } 

        public bool IsDead()
        {
            return isDead;
        } 

        public void TakeDamage(float damage)
        {
            if (isDead == true) return;
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            HealthCheck();
            HitAnim();
        }

        private void HealthCheck()
        {
            if (healthPoints <= 0)
            {
                healthPoints = 0;
                isDead = true;
                Die();
            }            
        }

        private
        
         void HitAnim()
        {     
            if (isDead) return; 
            anim.SetTrigger("HitAnim");
            AudioManager.PlayHumanSound(humanSound: AudioManager.HumanSound.Hit1, position: this.transform.position);
            if (gameObject.tag == "Enemy")
            {               
               aIScript.AttackBehaviour();
            }                                    
        }
        
        private void OnParticleCollision(GameObject particleProj)
        {           
            var proj = particleProj.GetComponent<Projectile>();
            damage = proj.GetDamage(); 
                                     
            if (proj.HitEffect() != null)
            {                    
              var cloneProjectile = Instantiate(proj.HitEffect(), proj.GetAimLocation(), particleProj.transform.rotation); 
              TakeDamage(damage: damage);
              
              if(gameObject.CompareTag("Player"))
              {
                var vitals = GetComponent<PlayerVitals>();
                if((this.gameObject.name == "Rambler") && vitals != null)
                {
                  vitals.TakeDamage(damage);  
                }                
              }
              Destroy(proj.gameObject);
               //MF_AutoPool.Despawn(gameObject);     
            }
            else
            {
                Destroy(proj.gameObject);
               //MF_AutoPool.Despawn(gameObject);
            }       
        }        
 
        public void Die()
        { 
            isDead = true;  
            EnemyDeath();
            targetDeath.Invoke();             
            capCol.enabled = false;
            StopMovement();         
            dieRanNum = Random.Range(1, 4);     
            if (gameObject.name == "Rambler")
            {                
                playerDeath.Invoke();
                fighter.enabled = false;
                shield.SetActive(false);                
            }            

            if(dieRanNum == 1)
            {                
                anim.SetTrigger("Die1"); 
                armFX.SetActive(true);
                if(this.CompareTag("Enemy"))
                {
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death1, this.transform.position);
                }
                else
                {
                   PlayerDeathAudio(); 
                }                              
            }
            else if (dieRanNum == 2)
            {                 
                anim.SetTrigger("Die2"); 
                headFX.SetActive(true);
                if(this.CompareTag("Enemy"))
                {
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death2, this.transform.position);              
                } 
                else
                {
                   PlayerDeathAudio(); 
                }                                            
            }
            else if (dieRanNum == 3)
            {               
                anim.SetTrigger("Die3");
                legFX.SetActive(true); 
                if(this.CompareTag("Enemy"))
                {
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death3, this.transform.position);              
                }
                else
                {
                   PlayerDeathAudio(); 
                }
            }          
        }

        void EnemyDeath()
        {            
            if(enemyspawn != null)
            {
                this.gameObject.tag = "Player";
                enemyspawn.count --;
                if(buddyCon != null)
                {
                  buddyCon.RemoveDeadAI(this.gameObject);
                }  
            }
        }

        void PlayerDeathAudio() 
        {
             if (gameObject.name == "Rambler")
             {
               AudioManager.PlayHumanSound(AudioManager.HumanSound.Death4, this.transform.position);
             }
             else
             {
                AudioManager.PlayHumanSound(AudioManager.HumanSound.Death3, this.transform.position); 
             }  
        }

        public void HitTheFloor() 
        {
            AudioManager.PlayHumanSound(AudioManager.HumanSound.HumanHitGroundDeath, this.transform.position);
        }

        private void StopMovement()
        {  
            mover.RigWeightToZero(); 
            mover.enabled = false;      
            rb.detectCollisions = false;
            rb.velocity = Vector3.zero;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        // public object CaptureState()
        // {
        //     return healthPoints;
        // }

        // public void RestoreState(object state)
        // {
        //     healthPoints = (float)state;
        //     if (healthPoints <= 0)
        //     {
        //         Die();
        //     }
        // }
    }
}