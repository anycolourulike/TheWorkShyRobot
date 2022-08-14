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
        float HealthPoints { get{return healthPoints;} set{healthPoints = value;}}  
        [SerializeField] GameObject RigLayer;
        [SerializeField] GameObject headFX;
        [SerializeField] GameObject legFX; 
        [SerializeField] GameObject armFX;
        [SerializeField] GameObject shield;        
        [SerializeField] GameObject deathSplashScreen;
        [SerializeField] Animator weaponAnim; 
        [SerializeField] Mover mover;       
        public delegate void TargetDeath();
        public static event TargetDeath targetDeath;
        public delegate void PlayerDied();
        public static event PlayerDied playerDeath;
        UnityEngine.AI.NavMeshAgent agent;
        CombatTarget combatTarget;
        GameObject hitScreenFX;
        CapsuleCollider capCol;
        PlayerVitals vitals;
        public bool isDead; 
        FieldOfView FOV;        
        Fighter fighter;        
        float damage;        
        Animator anim;
        int dieRanNum;        
        Rigidbody rb;        

        void Start() 
        {   
            FOV = GetComponent<FieldOfView>();
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
            mover.CancelNav();
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
 
        public void Die()
        {            
            targetDeath?.Invoke();          
            //capCol.enabled = false;
            isDead = true; 
            StopMovement();  
            EnemySpawn.count ++;          
                   
            dieRanNum = Random.Range(1, 4);     
            if (this.gameObject.name == "Rambler")
            {              
                playerDeath?.Invoke();
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
                  weaponAnim.SetTrigger("isDead");
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
                  weaponAnim.SetTrigger("isDead");
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
                  weaponAnim.SetTrigger("isDead");
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death3, this.transform.position); 
                  this.tag = "DeadCharacter";
                }
                else
                {
                   PlayerDeath(); 
                }
            }                     
        }

        public void HitTheFloor() 
        {
            AudioManager.PlayHumanSound(AudioManager.HumanSound.HumanHitGroundDeath, this.transform.position);
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

        void HitAnim()
        {  
            if(isDead) return;
            if(this.gameObject.name == "Rambler") return;
            anim.SetTrigger("HitAnim");
            if(gameObject.CompareTag("Player")) return;            
            FOV.radius = 40f;
            AudioManager.PlayHumanSound(humanSound: AudioManager.HumanSound.Hit1, position: this.transform.position); 
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

        void PlayerDeath() 
        {
            deathSplashScreen.SetActive(true);
            AudioManager.PlayHumanSound(AudioManager.HumanSound.Death4, transform.position); 
            AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.DeathScreen, transform.position);            
            if(this.gameObject.name == "Rambler") {LevelManager.Instance.PlayerWeaponCheck();}
        }

        IEnumerator HitFX()
        {
            hitScreenFX.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            hitScreenFX.SetActive(false);
        }

        void StopMovement()
        {  
            combatTarget.enabled = false;
            mover.RigWeaponUnequipped();
            var rigWeight = RigLayer.GetComponent<Rig>();
            fighter.enabled = false;
            rigWeight.weight = 0;
            agent.enabled = false;
            rb.detectCollisions = false;
            rb.velocity = Vector3.zero;            
        }
    }
}