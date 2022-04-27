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
    public class Health : MonoBehaviour, ISaveable
    {        
        public float healthPoints = 100f; 
        public float HealthPoints { get{return healthPoints;} set{healthPoints = value;}}  
        [SerializeField] AIController aIScript;  
        [SerializeField] GameObject RigLayer;
        //[SerializeField] GameObject head;
        [SerializeField] GameObject headFX;
        //[SerializeField] GameObject leg; 
        [SerializeField] GameObject legFX;        
        //[SerializeField] GameObject arm; 
        [SerializeField] GameObject armFX;
        [SerializeField] Mover mover;

        public delegate void PlayerDied();
        public static event PlayerDied playerDeath;
        PlayerController playerCont;
        CapsuleCollider capCol;
        bool isDead = false;  
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
            playerCont = GetComponent<PlayerController>(); 
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

        private void HitAnim()
        {     
            if (isDead) return; 
            anim.SetTrigger("HitAnim");
            AudioManager.PlayHumanSound(AudioManager.HumanSound.Hit1, this.transform.position);
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
              TakeDamage(damage);
              
              if(gameObject.tag == "Player")
              {
                var vitals = GetComponent<PlayerVitals>();
                vitals.TakeDamage(damage);                
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
            capCol.enabled = false;
            StopMovement();         
            dieRanNum = Random.Range(1, 4);     
            if (gameObject.name == "Rambler")
            {                
                playerDeath.Invoke();
            }            

            if(dieRanNum == 1)
            {                
                anim.SetTrigger("Die1"); 
                AudioManager.PlayHumanSound(AudioManager.HumanSound.Death1, this.transform.position);
                //arm.SetActive(false);
                armFX.SetActive(true);              
            }
            else if (dieRanNum == 2)
            {                 
                anim.SetTrigger("Die2"); 
                AudioManager.PlayHumanSound(AudioManager.HumanSound.Death2, this.transform.position);              
                //head.SetActive(false);   
                headFX.SetActive(true);                             
            }
            else if (dieRanNum == 3)
            {               
                anim.SetTrigger("Die3"); 
                AudioManager.PlayHumanSound(AudioManager.HumanSound.Death3, this.transform.position);              
                //leg.SetActive(false);
                legFX.SetActive(true);
            }          
        }

        public void HitTheFloor() 
        {
            AudioManager.PlayHumanSound(AudioManager.HumanSound.HumanHitGroundDeath, this.transform.position);
        }

        private void StopMovement()
        {  
            mover.RigWeightToZero();         
            rb.detectCollisions = false;
            rb.velocity = Vector3.zero;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;
            if (healthPoints == 0)
            {
                Die();
            }
        }
    }
}