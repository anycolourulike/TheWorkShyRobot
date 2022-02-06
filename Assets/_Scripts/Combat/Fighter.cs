using System;
using Rambler.Movement;
using Rambler.Core;
using Rambler.Control;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Rambler.Combat
{  
    public class Fighter : MonoBehaviour, IAction 
    {
        [SerializeField] AnimatorOverrideController animatorOverride;
        [SerializeField] float timeBetweenAttacks = 1.7f;    
        [SerializeField] Weapon unarmed;
        [SerializeField] Mover mover;

        
        public ActiveWeapon activeWeapon;        
        public Transform handTransform;        
        public Animator rigController;         
        public Weapon weaponConfig;               
        
        
        float timeSinceLastAttack = 
        Mathf.Infinity; 
        CombatTarget combatTarget;
        GameObject weaponRef;  
        Vector3 hitPoint;
        Vector3 aimPoint; 
        Animator anim;
        Health target;          
             
        
        private void Start()
        {    
           rigController = GetComponent<Fighter>().rigController; 
           combatTarget = GetComponent<CombatTarget>();           
           anim = GetComponent<Animator>();           
           EquipWeapon(weaponConfig);   
           ActiveWeaponInit();                                          
        }

        private void Update()
        {   
            timeSinceLastAttack += Time.deltaTime; 
            if (target == null) return;  
            if (target.IsDead()) return;                       
             
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                if(target.tag == "AIConversant") return;
                GetComponent<Mover>().Cancel();  
                AttackBehaviour();
            }
        } 
        
        private void AttackBehaviour()
        {                 
            transform.LookAt(target.transform); 
            if(gameObject.tag == "Player" && target.gameObject.tag == "Player") return;          
            
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                if (HasProjectile())
                {                    
                    if (weaponConfig.isEMP == true)
                    {
                        anim.ResetTrigger("EMPAttack");
                        anim.SetTrigger("EMPAttack");                                             
                    }
                    else
                    {
                        if (timeSinceLastAttack < 1.5f) return;                       
                        activeWeapon.LaunchProjectile(activeWeapon.MuzzlePosition, target);                       
                        timeSinceLastAttack = 0f;                                            

                        switch (weaponConfig.weaponTitle)
                        {
                            case "pistol":
                            SoundManager.PlayProjectileSound(SoundManager.WeaponSound.pistolShoot);
                                break;
                            case "mpistol":
                            SoundManager.PlayProjectileSound(SoundManager.WeaponSound.mPistolShoot);
                                break;
                            case "rifle":
                            SoundManager.PlayProjectileSound(SoundManager.WeaponSound.rifleShoot);
                                break;
                            case "shotgun":
                            SoundManager.PlayProjectileSound(SoundManager.WeaponSound.shotgunShoot);
                                break;
                        }                        
                    }                    
                }
                else
                {
                    //triggers Melee Event.
                    MeleeAttack();
                    timeSinceLastAttack = 0f;
                }
                if(this.tag == "Player")
                {
                    Cancel();
                }
                else
                {
                    return;
                }
            }
        }         

         public void EquipWeapon(Weapon weapon)
        {                  
            Animator animator = GetComponent<Animator>();    
            weaponConfig = weapon;                     
            Spawn(handTransform, animator);  
                        
            if(weapon.isMelee == true)
            { 
                mover.RigWeaponUnequipped();
                RigWeightToZero();
                rigController.Play("equip_" + weapon.weaponTitle);                
           }
           else
           {                             
                RigWeightToOne();
                rigController.Play("equip_" + weapon.weaponTitle);  
                mover.RigWeaponEquipped(); 
           }         
        }

        public void EquipUnarmed()
        {
            RigWeightToZero();
            EquipWeapon(unarmed);                                      
        }

        public void RigWeightToZero() 
        {    
            mover.RigWeightToZero(); 
        }

        public void RigWeightToOne() 
        {
            mover.RigWeaponEquipped();            
        }

        public void Spawn(Transform handTransform, Animator animator)
        {   
            DestroyOldWeapon(handTransform);                                 

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

        private void DestroyOldWeapon(Transform handTransform)
        {            
            var oldWeapon =  GameObject.FindWithTag("weapon");
            if (oldWeapon == null) return;            
            Debug.Log("weapon destroyed");                    
            Destroy(oldWeapon);                            
        }        

        public bool HasProjectile()
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

        void ActiveWeaponInit()
        {            
            activeWeapon = weaponRef.GetComponentInChildren<ActiveWeapon>();
        }     

        //Animation Event
        void MeleeEvent()
        {
            if (HasProjectile() != true)
            {
                LaunchProjectile(handTransform, target);
            }
        }

        void EMPAttack()
        {
            if (HasProjectile() != true)
            {
                activeWeapon.LaunchProjectile(handTransform, target);                
            }
        }

        void EMPEnd()
        {
                      
        }

        //Animation Event
        void EndMelee()
        {

        }               

        private void LaunchProjectile(Transform muzzleFX, Health target)
        {
            if (target == null) { return; }
                        
            if (HasProjectile())
            {
                activeWeapon.LaunchProjectile(muzzleFX, target);                
            }
            else
            {
                Debug.Log("Target took Melee Damage");                
                target.TakeDamage(activeWeapon.GetDamage());
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);           
            target = combatTarget.GetComponent<Health>();
        }       

        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        public void StopAttack()
        {
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("stopAttack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < activeWeapon.GetRange();
        }

        public void MeleeAttack()
        {
           Debug.Log("MeleeAttack");
           anim.ResetTrigger("meleeAttack");
           anim.SetTrigger("meleeAttack"); 
           StopAttack();           
        } 

        //Projectile Prediciton Logic Unused...for now
        public Vector3 GetEnemyLocation()
        {        
           CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>(); 
        
           if (targetCapsule == null)
           {                         
              return target.transform.position;              
           }
           float time = 0;       
           hitPoint = GetHitPoint(combatTarget.enemyDest, combatTarget.velocity, transform.position, 500f, out time);
           aimPoint = hitPoint - transform.position; 
           return aimPoint + Vector3.up * targetCapsule.height / 1.35f;
           //return target.transform.position + combatTarget.velocity + Vector3.up * targetCapsule.height / aim;                      
        }

        Vector3 GetHitPoint(Vector3 targetPosition, Vector3 targetSpeed, Vector3 attackerPosition, float bulletSpeed, out float time)
        { 
             Vector3 q = targetPosition - attackerPosition;
             //Ignoring Y for now. Add gravity compensation later, for more simple formula and clean game design around it
             q.y = 0;
             targetSpeed.y = 0;

             //solving quadratic ecuation from t*t(Vx*Vx + Vy*Vy - S*S) + 2*t*(Vx*Qx)(Vy*Qy) + Qx*Qx + Qy*Qy = 0

            float a = Vector3.Dot(targetSpeed, targetSpeed) - (bulletSpeed * bulletSpeed); //Dot is basicly (targetSpeed.x * targetSpeed.x) + (targetSpeed.y * targetSpeed.y)
            float b = 2 * Vector3.Dot(targetSpeed, q); //Dot is basicly (targetSpeed.x * q.x) + (targetSpeed.y * q.y)
            float c = Vector3.Dot(q, q); //Dot is basicly (q.x * q.x) + (q.y * q.y)

            //Discriminant
            float D = Mathf.Sqrt((b * b) - 4 * a * c);

            float t1 = (-b + D) / (2 * a);
            float t2 = (-b - D) / (2 * a);

            Debug.Log("t1: " + t1 + " t2: " + t2);

            time = Mathf.Max(t1, t2);

            Vector3 ret = targetPosition + targetSpeed;
            return ret;
       }       
   }
}