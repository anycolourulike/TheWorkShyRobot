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
        [SerializeField] float timeBetweenAttacks = 1.7f;    
        [SerializeField] Animator ShootAnim;          
        [SerializeField] Weapon unarmed;
        [SerializeField] Mover mover;

        [SerializeField] CapsuleCollider targetCapsule;
        public CapsuleCollider TargetCapsule {get{return targetCapsule;} set{targetCapsule = value;}}       
        public ActiveWeapon activeWeapon; 
        public Transform handTransform;        
        public Animator rigController;         
        public Weapon weaponConfig;        
        float timeSinceLastAttack = 
        Mathf.Infinity;         
        CombatTarget combatTarget;   
        public CombatTarget Target {set{combatTarget = value;}}                    
        GameObject weaponRef;               
        Transform enemyPos;  
        WeaponIK weaponIk;        
        Vector3 hitPoint;        
        Animator anim;
        
        private void Start()
        {   
           rigController = GetComponent<Fighter>().rigController;           
           weaponIk = GetComponent<WeaponIK>();          
           anim = GetComponent<Animator>();                   
           EquipWeapon(weaponConfig);   
           ActiveWeaponInit();                                          
        }

        private void Update()
        {                    
            timeSinceLastAttack += Time.deltaTime; 
            if (enemyPos == null) return;  
            if (targetCapsule == null) return;                       
             
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(enemyPos.transform.position, 1f);
            }
            else
            {
                if(enemyPos.tag == "AIConversant") return;
                GetComponent<Mover>().Cancel(); 
                AttackBehaviour(); 
                if(this.gameObject.tag == "Player")
                {
                    Cancel();
                }             
            }
        } 
        
        private void AttackBehaviour()
        {                        
            transform.LookAt(enemyPos.transform);            
            AssignTarget();
            if(gameObject.tag == "Player" && enemyPos.gameObject.tag == "Player") return;          
            
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
                        Vector3 TargetPoint = GetEnemyLocation() + Vector3.up / 1.1f;                                    
                        activeWeapon.LaunchProjectile(activeWeapon.MuzPos(), TargetPoint);  
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

        void AssignTarget()
        {
            weaponIk.targetTransform.position = GetEnemyLocation() + Vector3.up / 1.1f;
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
                StartCoroutine(AimInit());             
           }
           else
           {                             
                RigWeightToOne();
                rigController.Play("equip_" + weapon.weaponTitle);  
                mover.RigWeaponEquipped(); 
                StartCoroutine(AimInit());
           }         
        }

        IEnumerator AimInit()
        {
            yield return new WaitForSeconds(0.8f);            
            var aimTransform = activeWeapon.AimTransform();
            weaponIk.AimTransform = aimTransform;         
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
                MeleeAttack(handTransform, enemyPos);
            }
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

        //Animation Event
        void EndMelee()
        {

        }               

        private void MeleeAttack(Transform muzzleFX, Transform target)
        {
            if (target == null) { return; }
                        
            if (HasProjectile())
            {
                activeWeapon.LaunchProjectile(muzzleFX, GetEnemyLocation());                
            }
            else
            {
                Debug.Log("Target took Melee Damage");                
                //target.TakeDamage(activeWeapon.GetDamage());
            }
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
            enemyPos = combatTarget.GetComponent<Transform>();           
        }       

        public void Cancel()
        {
            StopAttack();
            enemyPos = null;
        }

        public void StopAttack()
        {
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("stopAttack");
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, enemyPos.position) < activeWeapon.GetRange();
        }

        public void MeleeAttack()
        {
           Debug.Log("MeleeAttack");
           anim.ResetTrigger("meleeAttack");
           anim.SetTrigger("meleeAttack"); 
           StopAttack();           
        } 
        
        //Projectile Prediciton Logic
        public Vector3 GetEnemyLocation()
        { 
           float time = 0;       
           hitPoint = GetHitPoint(combatTarget.enemyCurPos, combatTarget.LastSpeed, transform.position, 150f, out time);
           return hitPoint;                                 
        }

        Vector3 GetHitPoint(Vector3 targetPosition, Vector3 targetSpeed, Vector3 attackerPosition, float bulletSpeed, out float time)
        {             
            Vector3 q = targetPosition - attackerPosition;
            
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

            //Debug.Log("t1: " + t1 + " t2: " + t2);

            time = Mathf.Max(t1, t2);

            Vector3 ret = targetPosition + targetSpeed * time;
            return ret;
       }       
   }
}