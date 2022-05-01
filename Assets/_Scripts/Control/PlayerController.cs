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


namespace Rambler.Control
{    
    public class PlayerController : MonoBehaviour  
    {              
         
        [SerializeField] PlayerVitals vitals;                
        [SerializeField] GameObject shield; 
        [SerializeField] Fighter fighter;   
        [SerializeField] Animator anim; 
        
          
        ActiveWeapon activeWeapon;         
        float holdDuration = 1f;               
        Transform handTransform;  
        Animator rigController; 
        Animator playerAnim;
        WeaponIK weaponIK;        
        Mover mover;

        public bool isHolstered; 
        Vector3 pickUpDirection;
        GameObject interact;    
        GameObject weaponPU; 
        WeaponPickUp pickUp;          
        int interactions;    
        bool shieldsUp;
              
       
        private void Start()
        {                  
           rigController = GetComponent<Fighter>().rigController;       
           handTransform = GetComponent<Fighter>().handTransform;
           playerAnim = GetComponent<Animator>();
           mover = GetComponent<Mover>();
           weaponIK = GetComponent<WeaponIK>(); 
           interact = GameObject.FindGameObjectWithTag("Interact");             
           interact.SetActive(false);                                                                  
        }

        private void Update()
        {                     
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

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget otherCombatTarget = hit.transform.GetComponent<CombatTarget>();
                Fighter fighter = GetComponent<Fighter>();
                fighter.Target = otherCombatTarget;
                CapsuleCollider targetCapsule = hit.transform.GetComponent<CapsuleCollider>(); 
                fighter.TargetCapsule = targetCapsule;

                if (otherCombatTarget == null) continue;
                if (!GetComponent<Fighter>().CanAttack(otherCombatTarget.gameObject))
                {
                    continue;
                }
                fighter.Attack(otherCombatTarget.gameObject);
                fighter.TargetCapsule = targetCapsule; 
                
                return true;
            }
            return false;
        }   

        private bool InteractWithMovement()
        {            
           RaycastHit hit;
           bool hasHit = Physics.Raycast(GetRay(), out hit);
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
                            GetComponent<Mover>().StartMoveAction(hit.point, 1f);                                                     
                            holdDuration = 1f;                           
                        }
                    }
                }
                return true;              
            }
            return false;
        }                  

        private Transform GetHandTransform(Transform handTransform)
        {
            return this.handTransform;
        }    

        private static Ray GetRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "weaponPU")
            { 
               weaponPU = other.gameObject;
               pickUpDirection = Vector3.RotateTowards(transform.position, other.transform.position, 1f * Time.deltaTime, 0.0f);
               pickUp = weaponPU.GetComponent<WeaponPickUp>();                                               
               interact.SetActive(true); 
               interactions = 1;                            
            }
        }        

        void OnTriggerExit(Collider other)
        {
            if(other.gameObject.tag == "weaponPU")
            { 
                pickUp = null;
                interact.SetActive(false);                
            }
        } 

        public void InteractPressed()
        { 
           transform.LookAt(pickUpDirection);                     
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
            }            
        }     

        public void HolsterWeapon()
        { 

            //check weapon type, holster
            if(isHolstered == true)
            {
                return;
            }
            else 
            {                        
               isHolstered = true;
               weaponIK.AimTransform = null;          
               rigController.SetTrigger("holster_weapon"); 
               var fighter = GetComponent<Fighter>();
               fighter.RigWeightToZero();  
            }                                                
        }      

       public void ToggelShields()
       {         
          shieldsUp = !shieldsUp;
       }

       public void ReloadActiveWeapon() 
       {
           anim.SetTrigger("reload");
           activeWeapon = fighter.activeWeapon;
           activeWeapon.Reload();
       }

       void ShieldsUp() 
       {  
          shield.SetActive(true);
          shieldsUp = true;
       }  

       void ShieldsDown() 
       {  
          shield.SetActive(false);
          shieldsUp = false;
       } 
    }
}