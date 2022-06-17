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
        [SerializeField] Companion companion;
        [SerializeField] GameObject deathSplashScreen;
        [SerializeField] GameObject hud;
        
        public delegate void TargetDeath();
        public static event TargetDeath targetDeath;

        public delegate void PlayerDied();
        public static event PlayerDied playerDeath;
        CapsuleCollider capCol;
        public bool isDead;
        public bool CharacterIsDead {get{return isDead;}}        
        Fighter fighter;
        PlayerVitals vitals;
        WeaponIK weaponIK;
        float damage; 
        public float SetDamage {set{damage = value;}}
        CombatTarget combatTarget;
        Animator anim;        
        int dieRanNum; 
        int hitRanNum;        
        Rigidbody rb;   
        GameObject hitScreenFX;
        UnityEngine.AI.NavMeshAgent agent;                
       
        void Start() 
        {   
            anim = GetComponent<Animator>(); 
            combatTarget = GetComponent<CombatTarget>();
            rb = GetComponent<Rigidbody>(); 
            fighter = GetComponent<Fighter>(); 
            capCol = GetComponent<CapsuleCollider>();  
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();    
            if(this.gameObject.name == "Rambler")
            {
                hitScreenFX = GameObject.Find(name: "/PlayerCore/HUD/DamageScreen");
                hitScreenFX.SetActive(value: false);
                vitals = GetComponent<PlayerVitals>();
            }
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

        public void HealthCheck()
        {
            if (healthPoints <= 0)
            {
                healthPoints = 0;
                isDead = true;                
                Die();
            }            
        }
       
        void HitAnim()
        {  
            if(isDead) return;
            if(this.gameObject.name == "Rambler") return;
            aIScript.SuspicionBehaviour();
            anim.SetTrigger("HitAnim");  
            HealthCheck();          
            AudioManager.PlayHumanSound(humanSound: AudioManager.HumanSound.Hit1, position: this.transform.position);
            
            if (gameObject.tag == "Enemy")
            {               
               aIScript.AttackBehaviour();
            }                                    
        }
        
        void OnParticleCollision(GameObject particleProj)
        {           
            var proj = particleProj.GetComponent<Projectile>();
            damage = proj.GetDamage(); 
                                     
            if (proj.HitEffect() != null)
            {                    
              var cloneProjectile = Instantiate(proj.HitEffect(), proj.GetAimLocation(), particleProj.transform.rotation); 
              TakeDamage(damage: damage);              

              if((this.gameObject.name == "Rambler") && vitals != null)
              {
                StartCoroutine("HitFX");
                vitals.TakeDamage(damage);  
                if(!isDead)
                { 
                   anim.SetTrigger("HitAnim");
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
            StopMovement();  
            EnemyDeath();
            targetDeath.Invoke();             
            capCol.enabled = false;
            isDead = true; 
                    
            dieRanNum = Random.Range(1, 4);     
            if (this.gameObject.name == "Rambler")
            {              
                playerDeath.Invoke();
                LevelManager.playerDied = true;
                LevelManager.Instance.PlayerDeathCheck();
                mover = GetComponent<Mover>();
                mover.enabled = false;
                shield.SetActive(false);                
            }            

            if(dieRanNum == 1)
            {                
                anim.SetTrigger("Die1"); 
                armFX.SetActive(true);
                if(this.CompareTag("Enemy"))
                {
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death1, this.transform.position);
                  this.tag = "DeadCharacter";
                }
                else
                {
                   PlayerDeath(); 
                }                              
            }
            else if (dieRanNum == 2)
            {                 
                anim.SetTrigger("Die2"); 
                headFX.SetActive(true);
                if(this.CompareTag("Enemy"))
                {
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death2, this.transform.position); 
                  this.tag = "DeadCharacter";
                } 
                else
                {
                   PlayerDeath(); 
                }                                            
            }
            else if (dieRanNum == 3)
            {               
                anim.SetTrigger("Die3");
                legFX.SetActive(true); 
                if(this.CompareTag("Enemy"))
                {
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death3, this.transform.position); 
                  this.tag = "DeadCharacter";
                }
                else
                {
                   PlayerDeath(); 
                }
            }          
        }

        void EnemyDeath()
        {            
            if(enemyspawn != null)
            {
                enemyspawn.count --;
                if(companion != null)
                {
                  companion.RemoveDeadAI(enemyToRemove: this.gameObject);
                }  
            }
        }

        void PlayerDeath() 
        {
            deathSplashScreen.SetActive(true);
            AudioManager.PlayHumanSound(AudioManager.HumanSound.Death4, this.transform.position);             
        }

        public void HitTheFloor() 
        {
            AudioManager.PlayHumanSound(AudioManager.HumanSound.HumanHitGroundDeath, this.transform.position);
        }

        private void StopMovement()
        {  
            combatTarget.enabled = false;
            mover.RigWeaponUnequipped();
            var rigWeight = RigLayer.GetComponent<Rig>();
            rigWeight.weight = 0;
            GetComponent<ActionScheduler>().CancelCurrentAction(); 
            agent.enabled = false;
            rb.detectCollisions = false;
            rb.velocity = Vector3.zero;            
        }

        IEnumerator HitFX()
        {
            hitScreenFX.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            hitScreenFX.SetActive(false);
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