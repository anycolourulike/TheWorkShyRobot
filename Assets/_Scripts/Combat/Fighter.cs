using System;
using Rambler.Movement;
using Rambler.Core;
using Rambler.Control;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;


namespace Rambler.Combat
{  
    public class Fighter : MonoBehaviour, IAction 
    {
        [SerializeField] AnimatorOverrideController animatorOverride;
        [SerializeField] Animator AIRigLayer;
        [SerializeField] float timeBetweenAttacks = 1.3f;    
        [SerializeField] Animator ShootAnim;          
        [SerializeField] Weapon unarmed;
        [SerializeField] ParticleSystem punchImpact; 
        [SerializeField] CapsuleCollider targetCapsule;
        public CapsuleCollider TargetCap {get{return targetCapsule;} set{targetCapsule = value;}}       
        public ActiveWeapon activeWeapon; 
        public Transform handTransform;        
        public Animator rigController;  
        public Weapon weaponPickedUp;       
        public Weapon weaponConfig;  
        Weapon lastWeaponUsed;     
        public Weapon SetLastWeapon{set{lastWeaponUsed = value;} get {return lastWeaponUsed;}}      
        float timeSinceLastAttack = 
        Mathf.Infinity;         
        CombatTarget otherCombatTarget;  
        public CombatTarget CombatTarget {get{return otherCombatTarget;} set{otherCombatTarget = value;}} 
        PlayerController playerController; 
        Vector3 hitPointVector;  
        GameObject weaponRef;
        Health targetHealth;                       
        Transform enemyPos; 
        WeaponIK weaponIk;    
        Animator anim;          
        Mover mover;        
        
        void OnEnable()
        {
            Health.targetDeath += CancelTarget;
            Health.playerDeath += StopMovement;
        }

        void OnDisable()
        {
            Health.targetDeath -= CancelTarget;
            Health.playerDeath -= StopMovement;
        }
        
        void Start()
        { 
           if(this.gameObject.name == "Rambler")
           {
              playerController = GetComponent<PlayerController>();
           }
           mover = GetComponent<Mover>();
           rigController = GetComponent<Fighter>().rigController;  
           weaponIk = GetComponent<WeaponIK>();          
           anim = GetComponent<Animator>();             
           EquipWeapon(weapon: weaponConfig);   
           ActiveWeaponInit();                                          
        }

        void Update()
        {                    
            timeSinceLastAttack += Time.deltaTime; 
            if (TargetCap != null)                      
            //if (enemyPos.CompareTag("AIConversant")) return; 
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(enemyPos.transform.position, 1f);
            }
            else
            {                
                //GetComponent<Mover>().Cancel();  Blocks Multiple shots from player? 
                AttackBehaviour();
            }
        } 
        
        void AttackBehaviour()
        {                    
            transform.LookAt(enemyPos.transform);            
            AssignIKTarget();
            if(gameObject.tag == "Player" && enemyPos.gameObject.tag == "Player") return;          
            
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                if (HasProjectile())
                {                    
                    if (weaponConfig.isEMP == true)
                    {                       
                        anim.SetTrigger("EMPAttack");                                             
                    }
                    else
                    {
                        if (timeSinceLastAttack < 0.5f) return; 
                        if(activeWeapon.CurClip > 0)
                        {                        
                          Vector3 targetVector = GetEnemyLocation() + Vector3.up / 1.1f; 
                          activeWeapon.LaunchProjectile(activeWeapon.MuzPos(), targetVector);  
                          timeSinceLastAttack = 0f; 

                            switch (weaponConfig.weaponTitle)
                            {
                                case "pistol":
                                AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.pistolShoot, activeWeapon.transform.position);                           
                                   break;
                                case "SMG":
                                AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.SMGShoot, activeWeapon.transform.position);
                                   break;
                                case "rifle":
                                AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.RifleShoot, activeWeapon.transform.position);
                                   break;
                                case "shotgun":
                                AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.ShotgunShoot, activeWeapon.transform.position);
                                   break;
                            } 
                        }
                        else
                        {
                            activeWeapon.Reload();
                            AIRigLayer.SetTrigger("Reload");
                        }
                        // else
                        // {                                  
                        //     if(Time.time > outOfAmmo && outOfAmmoCalled == false)
                        //     {
                        //        outOfAmmo += period;
                        //        outOfAmmoCalled = true;
                        //        AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.outOfAmmo, activeWeapon.transform.position);
                        //        outOfAmmoCalled = false;
                        //        if(this.gameObject.name == "Rambler") return;
                        //        ShootAnim.SetTrigger("Reload");                                   
                        //     }   
                        // }                                         
                    }                    
                }
                else
                {                    
                    MeleeAttack(target: enemyPos);
                    timeSinceLastAttack = 0f;
                }
            }            
        }        

        public void EquipWeapon(Weapon weapon)
        { 
            DestroyOldWeapon(handTransform: handTransform);
            weaponConfig = weapon;                                
            Spawn(handTransform: handTransform, animator: anim);  
            activeWeapon.SetRigController = rigController;  
            AssignAmmo();
            if(this.gameObject.CompareTag("Enemy"))
            {
                activeWeapon.FullAmmo();
            }              
           if(weaponConfig.isMelee)
           {                
                mover.RigWeaponUnequipped();
                RigWeightToZero();
                rigController.Play("equip_" + weapon.weaponTitle);   
                StartCoroutine(AimInit());             
           }
           else
           {                                                   
                RigWeightToOne();
                rigController.Play("equip_" + weapon.weaponTitle);  
                mover.RigWeaponEquipped(); 
                StartCoroutine(AimInit());
                if(this.gameObject.name == "Rambler")                 
                {  
                  playerController.ActivateAmmoCounter();                  
                  activeWeapon.AmmoUIInit();
                }
            }        
        } 

        public void EquipUnarmed()
        {   
            playerController.DeactivateAmmoCounter();                  
            activeWeapon.AmmoUIInit();
            RigWeightToZero();
            EquipWeapon(weapon: unarmed);               
        }

        public void EquipLastWeapon() 
        {
            EquipWeapon(weapon: lastWeaponUsed);
        }

        public void EquipPickedUpWeapon()
        {
            weaponConfig = weaponPickedUp;
            EquipWeapon(weaponConfig);
        }

        public void RigWeightToZero() 
        {    
            mover.RigWeightToZero(); 
        } 

        public void RigWeightToOne() 
        {
            mover.RigWeaponEquipped();            
        }
      
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            var targetToTest = combatTarget.GetComponent<CapsuleCollider>();
            return targetToTest != null; 
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this); 
            if(combatTarget != null)
            {
              enemyPos = combatTarget.GetComponent<Transform>();    
            }         
        }       

        public void CancelTarget()
        { 
            CombatTarget = null; 
            TargetCap = null;      
            enemyPos = null; 
        }   

        public void AssignAmmo() 
        {
            activeWeapon.FullAmmo();
        } 

         public void CancelNav()
        {
            mover.CancelNav();
        }

        public object CaptureState()
        {
            return lastWeaponUsed;
        }

        public void RestoreState(object state)
        {
            lastWeaponUsed = (Weapon)state;
        } 
        
        Vector3 GetEnemyLocation()
        {              
           return hitPointVector = CombatTarget.TargetFuturePos(activeWeapon.AimTransform().position);
        }        

        void Spawn(Transform handTransform, Animator animator)
        {   
            DestroyOldWeapon(handTransform: handTransform);                                 

            if (weaponConfig != null)
            {
                weaponRef = Instantiate(weaponConfig.equippedPrefab, handTransform);   
                ActiveWeaponInit();                                                          
            }
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
        } 
        
        bool HasProjectile()
        {
            if(weaponConfig.projectile != false)
            {
              return true;
            }
            else
            {
              return false;
            }
        } 

        IEnumerator AimInit()
        {
            yield return new WaitForSeconds(0.3f);            
            var aimTransform = activeWeapon.AimTransform();
            weaponIk.AimTransform = aimTransform;         
        }

        void DestroyOldWeapon(Transform handTransform)
        { 
            var oldWeapon =  FindChildWithTag();
            if (oldWeapon == null) return;                   
            Destroy(oldWeapon);                            
        } 

        GameObject FindChildWithTag() 
        {            
            Transform hand = handTransform;
            foreach(Transform transform in hand) 
            {
                if(transform.tag == "weapon")
                {
                    var childObj = transform.gameObject;
                    return childObj;                    
                }
            }
            return null;
        }

        void AssignIKTarget()
        {
            weaponIk.targetTransform.position = GetEnemyLocation() + (Vector3.up / 1.1f);
        }

        bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, enemyPos.position) < activeWeapon.GetRange();
        } 

        void MeleeAttack(Transform target)
        {
            if (target == null) { return; }
            targetHealth = target.GetComponent<Health>();
            anim.SetTrigger("meleeAttack");
        } 

        void MeleeEvent()
        {
            Instantiate(punchImpact, handTransform.position, handTransform.rotation);
            AudioManager.PlayHumanSound(AudioManager.HumanSound.Hit2, this.transform.position);
            targetHealth.TakeDamage(50f);
        }

        void EMPAttack()
        {
            if (HasProjectile() != true)
            {
                activeWeapon.LaunchProjectile(handTransform, GetEnemyLocation());                
            }
        }

        void EMPEnd()
        {
                      
        }

        void EndMelee()
        {

        }
        
        void ActiveWeaponInit()
        {            
            activeWeapon = weaponRef.GetComponentInChildren<ActiveWeapon>();
        }

        void StopMovement()
        {
            CancelNav();
            mover.enabled = false;
            var aICon = GetComponent<AIController>();
            aICon.enabled = false;
        }              
    }
}