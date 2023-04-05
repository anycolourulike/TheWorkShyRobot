using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Saving;
using Rambler.Combat;
using Rambler.Control;
using Rambler.Core;
using Rambler.Movement;
using Rambler.Utils;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using Rambler.Stats;
using TMPro;

namespace Rambler.Attributes // To Do Stop Movement
{
    public class Health : MonoBehaviour, ISaveable
    {        
        public LazyValue<float> healthPoints; 
        LazyValue<float> HealthPoints { get{return healthPoints;} set{healthPoints = value;}}
        [SerializeField] TextMeshProUGUI displayLives;
        [SerializeField] GameObject RigLayer;
        [SerializeField] GameObject headFX;
        [SerializeField] GameObject legFX; 
        [SerializeField] GameObject armFX;
        [SerializeField] GameObject shield;
        [SerializeField] GameObject deathSplashScreen;
        [SerializeField] GameObject gameOverSplashScreen;
        [SerializeField] Mover mover;       
        public delegate void TargetDeath();
        public static event TargetDeath targetDeath;
        public delegate void PlayerDied();
        public static event PlayerDied playerDeath;
        public delegate void AIHit();
        public static event AIHit aIHit;
        UnityEngine.AI.NavMeshAgent agent;
        public float hPts;
        CombatTarget combatTarget;
        GameObject hitScreenFX;
        CapsuleCollider capCol;
        float isDeadTimer = 0f;
        PlayerVitals vitals;
        public bool isDead;
        TimbertoesFOV TFOV;
        FieldOfView FOV;        
        Fighter fighter;
        float damage;        
        Animator anim;
        int dieRanNum;        
        Rigidbody rb;
        int lives;

        void Awake() 
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            anim = GetComponent<Animator>();
        }

        void Start() 
        {   
            FOV = GetComponent<FieldOfView>();
            combatTarget = GetComponent<CombatTarget>();
            rb = GetComponent<Rigidbody>(); 
            fighter = GetComponent<Fighter>(); 
            capCol = GetComponent<CapsuleCollider>();  
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if(this.gameObject.name == "Rambler")
            {
                hitScreenFX = GameObject.Find(name: "/PlayerCore/HUD/DamageScreen");
                hitScreenFX.SetActive(false);
                vitals = GetComponent<PlayerVitals>();
                LivesUpdate();
            }
        }       

        void Update()
        {
            isDeadTimer += Time.deltaTime;
        }

        public bool IsDead()
        {
            return isDead;
        } 

        public void TakeDamage(float damage)
        {  
            if (isDead == true) return;
            if (this.gameObject.CompareTag("Enemy") || (this.gameObject.CompareTag("Companion")))
            {
                aIHit?.Invoke();
            }            

            if ((this.gameObject.CompareTag("Player")) && (vitals != null))
            {               
                StartCoroutine("HitFX");
                vitals.TakeDamage(damage);
                if (isDead == true) return;
                anim.SetTrigger("HitAnim");
            }          

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            mover.CancelNav();
            HitAnim();
            HealthCheck();
        }

        public void HealthCheck()
        {
            if (healthPoints.value <= 0)
            {
                healthPoints.value = 0f;
                if(isDeadTimer <= 2.5f)
                {
                    var enemySpawnScript = FindObjectOfType<EnemySpawn>();
                    enemySpawnScript.SetPortalsActive();
                    //activate portals
                    Die();
                    Destroy(gameObject);
                }
                else
                {
                    Die();
                }
            }            
        } 
        
        public void RestoreHealth()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            vitals.Restore();
        }
 
        public void Die()
        {   
            isDead = true;
            if (this.CompareTag("Enemy"))
            {
                var aiCon = GetComponent<AIController>();
                aiCon.isDead = true;
                EnemySpawn.count++;
                Transform selectionUI = transform.Find("SelectionIcon");
                var selObj = selectionUI.gameObject;
                selObj.SetActive(false);
            }

            var rigBuilder = GetComponent<RigBuilder>();
            rigBuilder.enabled = false;
            var capCol = GetComponent<CapsuleCollider>();
            capCol.enabled = false;
            fighter.enabled = false;

            anim.SetBool("HitAnim", false);
            dieRanNum = Random.Range(1, 4);     
            if (this.gameObject.name == "Rambler")
            {              
                playerDeath?.Invoke();
                shield.SetActive(false);
                lives--;
                displayLives.text = lives.ToString();
            }

            
            transform.position += new Vector3(0,0,-2f) * 10f * Time.deltaTime;
            if(dieRanNum == 1)
            {
                anim.speed = 1.5f;
                anim.SetTrigger("Die1");
                armFX.transform.SetParent(null);
                armFX.SetActive(true);
                if(this.CompareTag("Enemy"))
                {                  
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death1, this.transform.position);
                  this.tag = "Dead";                  
                  targetDeath?.Invoke();
                }
                else
                {
                   PlayerDeath(); 
                }                              
            }
            else if (dieRanNum == 2)
            {
                anim.speed = 1.5f;
                anim.SetTrigger("Die2");
                //headFX.transform.SetParent(null);
                headFX.SetActive(true);
                if(this.CompareTag("Enemy"))
                {                  
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death2, this.transform.position); 
                  this.tag = "Dead";
                  targetDeath?.Invoke();                  
                } 
                else
                {
                   PlayerDeath(); 
                }                                            
            }
            else if (dieRanNum == 3)
            {
                anim.speed = 1.5f;
                anim.SetTrigger("Die3");
                //legFX.transform.SetParent(null);
                legFX.SetActive(true); 
                if(this.CompareTag("Enemy"))
                {
                  AudioManager.PlayHumanSound(AudioManager.HumanSound.Death3, this.transform.position); 
                  this.tag = "Dead";
                  targetDeath?.Invoke();
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

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["healthPoints.value"] = healthPoints.value;
            if (this.gameObject.name == "Rambler")
            {
                data["lives"] = lives;
            }
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            healthPoints.value = (float)data["healthPoints.value"];
            if (this.gameObject.name == "Rambler")
            {
                lives = (int)data["lives"];
                LivesUpdate();
            }
            
            HealthCheck();

        }

        public void DeathSpeedNormal()
        {
            Debug.Log("healthSpeedNormal");
            anim.speed = 1f;
        }

        public void StartingLives()
        {
            lives = 10;
            displayLives.text = lives.ToString();
        }
       
        public void LivesUpdate()
        {
            displayLives.text = lives.ToString();
        }

        float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        void HitAnim()
        {  
            if(isDead) return;
            anim.SetTrigger("HitAnim");
            if(gameObject.CompareTag("Player")) return; 
            FOV.radius = 40f;
            AudioManager.PlayHumanSound(AudioManager.HumanSound.Hit1, position: this.transform.position); 
        }
        
        void OnParticleCollision(GameObject particleProj)
        {           
            var proj = particleProj.GetComponent<Projectile>();
            damage = proj.GetDamage();
            TakeDamage(damage);

            if (proj.HitEffect() != null)
            {                    
              var cloneProjectile = Instantiate(proj.HitEffect(), proj.GetAimLocation(), particleProj.transform.rotation);                      
              
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
            
            this.tag = "Dead";
            fighter.enabled = false;
            if (lives > 0)
            {
                deathSplashScreen.SetActive(true);
            }
            else
            {
                gameOverSplashScreen.SetActive(true);
            }
                AudioManager.PlayHumanSound(AudioManager.HumanSound.Death4, transform.position); 
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.DeathScreen);            
            //if(this.gameObject.name == "Rambler") {LevelManager.Instance.PlayerWeaponCheck();}
        }

        IEnumerator HitFX()
        {
            hitScreenFX.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            hitScreenFX.SetActive(false);
        }
       
    }
}