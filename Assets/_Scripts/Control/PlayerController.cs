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
        
        public bool isHolstered;  

        ActiveWeapon activeWeapon;         
        float holdDuration = 1f;               
        Transform handTransform;  
        Animator rigController;
        public Vector3 nextPosition;
        public Vector3 NextPosition {get{return nextPosition;}}
        
        GameObject interact;    
        GameObject weaponPU; 
        WeaponPickUp pickUp;  
        int interactions;    
        bool shieldsUp;
              
       
        private void Start()
        {                  
           rigController = rigController = GetComponent<Fighter>().rigController;       
           handTransform = GetComponent<Fighter>().handTransform;  
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
                CombatTarget target = hit.transform.GetComponent<CombatTarget>(); 
                fighter.Target = target;
                CapsuleCollider targetCapsule = hit.transform.GetComponent<CapsuleCollider>();                 
                if (target == null) continue;
                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }

                if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
                {
                    if(target == gameObject) return false;                                       
                    Fighter fighter = GetComponent<Fighter>();
                    fighter.Attack(target.gameObject);
                    fighter.TargetCapsule = targetCapsule; 
                }
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
                            nextPosition = hit.point;                            
                            holdDuration = 1f;                           
                        }
                    }
                }
                return true;              
            }
            return false;
        }                  

        private Transform GetTransform(Transform handTransform)
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
           Interact();
           interact.SetActive(false);
           pickUp = null;
        } 

        public void Interact()
        {
            switch(interactions)
            {
                case 1:
                pickUp.PickUpItem();
                break;
            }
        }      

        public void HolsterWeapon()
        {  
            isHolstered = true;
            rigController.ResetTrigger("holster_weapon");
            rigController.SetTrigger("holster_weapon"); 
            var fighter = GetComponent<Fighter>();
            fighter.RigWeightToZero();                                                  
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