using System.ComponentModel.Design;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using UnityEngine.AI;
using Rambler.Movement;
using Rambler.Inventories;
using Rambler.Combat;
using Rambler.Core;
using Cinemachine;


namespace Rambler.Control
{    
    public class PlayerController : MonoBehaviour  
    {              
        [SerializeField] float maxNavMeshProjectionDistance = 1f; 
        [SerializeField] GameObject ammoCounter; 
        [SerializeField] PlayerVitals vitals;                
        [SerializeField] GameObject shield; 
        [SerializeField] Fighter fighter;          
        [SerializeField] Animator anim; 
        [SerializeField] Transform target;       
        CinemachineVirtualCamera cineMachine; 
                   
        ActiveWeapon activeWeapon;             
        float holdDuration = 1f;               
        Transform handTransform;
        Animator rigController;
         
        Animator playerAnim;        
        NavMeshAgent agent; 
        WeaponIK weaponIK;        
        Mover mover;

        Transform targetStartPos;
        public bool isHolstered; 
        Vector3 pickUpDirection;        
        GameObject interact;    
        GameObject weaponPU; 
        WeaponPickUp pickUp; 
        float shakeTimer;        
        int interactions;    
        bool shieldsUp;  
        bool isDead;   

        void OnEnable()
        {
            Health.playerDeath += PlayerDied;
        } 

        void OnDisable()
        {
            Health.playerDeath -= PlayerDied;
        }    
       
        private void Start()
        { 
           targetStartPos = target; 
           agent = GetComponent<NavMeshAgent>();         
           rigController = GetComponent<Fighter>().rigController;       
           handTransform = GetComponent<Fighter>().handTransform;
           cineMachine = FindObjectOfType<CinemachineVirtualCamera>();
           playerAnim = GetComponent<Animator>();
           mover = GetComponent<Mover>();
           weaponIK = GetComponent<WeaponIK>(); 
           interact = GameObject.FindGameObjectWithTag("Interact");             
           interact.SetActive(false);                                                                  
        }

        private void Update()
        {   
           if(shakeTimer > 0)
           { 
              shakeTimer -= Time.deltaTime;
              if(shakeTimer <= 0f) 
              {
                CinemachineBasicMultiChannelPerlin cineMachinePerlin = 
                cineMachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cineMachinePerlin.m_AmplitudeGain = 0f;
              }
           }       

           if (InteractWithCombat()) return;
           if (InteractWithMovement()) return;  
           
           if(shieldsUp == true) 
           {
            ShieldsUp();
            vitals.SetEnergyBurnRate = 1f;
           } 
           else
           {              
            ShieldsDown();
            vitals.SetEnergyBurnRate = 0.1f; 
           }
        }              

        bool InteractWithCombat()
        {           
            RaycastHit[] hits = Physics.RaycastAll(GetRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget otherCombatTarget = hit.transform.GetComponent<CombatTarget>();
                GameObject objTarget = hit.transform.gameObject;
                var thisCombatTarget = GetComponent<CombatTarget>();
                if(otherCombatTarget == thisCombatTarget) return false;
                Fighter fighter = GetComponent<Fighter>();
                fighter.CombatTarget = otherCombatTarget;
                CapsuleCollider targetCapsule = hit.transform.GetComponent<CapsuleCollider>(); 
                fighter.TargetCap = targetCapsule;

                if (otherCombatTarget == null) continue;                
                if (!GetComponent<Fighter>().CanAttack(otherCombatTarget.gameObject))
                {
                    continue;
                }
                fighter.Attack(combatTarget: otherCombatTarget.gameObject);

                if(objTarget.tag == "Enemy") 
                {
                    ShakeCamera(1.5f, 0.2f);
                }    
                
                return true;
            }
            return false;
        }   

        bool InteractWithMovement()
        {  
           if(isDead == true) return false;          
           Vector3 target;
           bool hasHit = RaycastNavMesh(out target);
           
            if (Input.touchCount > 0 && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            {
                return false;
            }
            if (hasHit)
            {              
                if (Input.touchCount > 0)
                {
                    Touch touchInfo = Input.GetTouch(0);

                    if (touchInfo.phase == TouchPhase.Stationary)
                    {
                        var holdTime = holdDuration -= Time.deltaTime;
                        if (holdTime < .6f)
                        {                                                  
                            GetComponent<Mover>().StartMoveAction(target, 1f);                                                     
                            holdDuration = 1f;                           
                        }
                    }
                }
                return true;              
            }
            return false;
        }              

        Transform GetHandTransform(Transform handTransform)
        {
            return this.handTransform;
        }    

        static Ray GetRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
            hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            return true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("weaponPU"))
            { 
               weaponPU = other.gameObject;
              // pickUpDirection = Vector3.RotateTowards(transform.position, other.transform.position, 1f * Time.deltaTime, 0.0f);
               pickUp = weaponPU.GetComponent<WeaponPickUp>();                                               
               interact.SetActive(true);
               interactions = 1;                            
            }

            if (other.gameObject.CompareTag("headPickUP"))
            {
               interactions = 2;
               weaponPU = other.gameObject;
               pickUp = weaponPU.GetComponent<WeaponPickUp>();
               interact.SetActive(true);
            }

            if (other.gameObject.CompareTag("UsePC"))
            {
                interactions = 3;
                weaponPU = other.gameObject;                                                              
                interact.SetActive(true);
            }
        } 

        public void InteractPressed()
        {            
           agent.enabled = false; 
           mover.enabled = false;
           Interact();
           interact.SetActive(false);
           pickUp = null;
        }

        public void Interact()
        {
            switch(interactions)
            {
                case 1: 
                fighter.SetLastWeapon = fighter.weaponConfig; 
                fighter.EquipUnarmed(); 
                playerAnim.SetTrigger("pickUp");
                pickUp.PickUpItem();
                break;

                case 2:
                fighter.SetLastWeapon = fighter.weaponConfig; 
                fighter.EquipUnarmed();  
                playerAnim.SetTrigger("PicKUPHead");
                pickUp.PickUpItem(); 
                break;

                case 3:
                target.position = weaponPU.transform.position;
                fighter.SetLastWeapon = fighter.weaponConfig; 
                fighter.EquipUnarmed();                
                playerAnim.SetTrigger("use");
                DoorOpen.doorUnlocked();
                //notify doors to open
                //play SFX
                //reassign target
                break;
            }            
        }

        public void EnableMover()
        { 
           agent.enabled = true;
           mover.enabled = true;
        } 

        void PlayerDied()
        {
            isDead = true;
        }

        void OnTriggerExit(Collider other)
        {
            if(other.gameObject.CompareTag("weaponPU"))
            { 
                pickUp = null;
                interact.SetActive(false);      
            }

            if(other.gameObject.CompareTag("headPickUP"))
            { 
                pickUp = null;
                interact.SetActive(false);      
            }

            if(other.gameObject.CompareTag("UsePC"))
            { 
                pickUp = null;
                interact.SetActive(false);      
            }
        }         

        public void HolsterWeapon()
        { 
            if(isHolstered == true)
            {
                return;
            }
            else 
            {  
               ammoCounter.SetActive(false);                                    
               isHolstered = true;
               weaponIK.AimTransform = null;          
               rigController.SetTrigger("holster_weapon"); 
               var fighter = GetComponent<Fighter>();
               fighter.RigWeightToZero();  
            }                                                
        }

        public void ActivateAmmoCounter()
       {         
          ammoCounter.SetActive(true);        
       }

        public void DeactivateAmmoCounter()
       {
          ammoCounter.SetActive(false);
       }

        public void ReloadActiveWeapon() 
       {
           anim.SetTrigger("Reload");
           activeWeapon = fighter.activeWeapon;
           activeWeapon.Reload();
       }

        void ShieldsUp() 
       {           
          shield.SetActive(true);
          shieldsUp = true;
       } 

        public void ToggelShields()
       { 
        if(shieldsUp == false)
        {
          AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.ShieldUp, transform.position);
        }        
          shieldsUp = !shieldsUp;
       } 

        void ShieldsDown() 
       {  
          shield.SetActive(false);
          shieldsUp = false;
       } 

        void ShakeCamera(float intensity, float time)
       {
          if(fighter.activeWeapon.weaponType == ActiveWeapon.WeaponType.melee) return;
          CinemachineBasicMultiChannelPerlin cineMachinePerlin = 
          cineMachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

          cineMachinePerlin.m_AmplitudeGain = intensity;
          shakeTimer = time;
       }
    }
}