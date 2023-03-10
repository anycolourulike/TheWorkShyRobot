using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.AI;
using Rambler.Movement;
using Rambler.Combat;
using Rambler.Attributes;
using Rambler.Core;
using Cinemachine;
using Rambler.SceneManagement;
using System.Collections;

namespace Rambler.Control
{
    //[RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] GameObject ammoCounter;
        [SerializeField] PlayerVitals vitals;
        [SerializeField] GameObject shield;
        [SerializeField] Fighter fighter;
        [SerializeField] Transform target;
        CinemachineVirtualCamera cineMachine;

        GameObject selectionUIObj;
        ActiveWeapon activeWeapon;             
        float holdDuration = 1f;               
        Transform handTransform;
        Animator rigController;         
        Animator anim;
       
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
       
        void Start()
        {           
           interact = GameObject.FindGameObjectWithTag("Interact");      
           interact.SetActive(false); 
           targetStartPos = target;

           cineMachine = FindObjectOfType<CinemachineVirtualCamera>();
           rigController = GetComponent<Fighter>().rigController;
           handTransform = GetComponent<Fighter>().handTransform;
           lineRenderer = GetComponent<LineRenderer>();
           lineRenderer.positionCount = 0;
           lineRenderer.startWidth = 0.1f;
           lineRenderer.endWidth = 0.1f;

           agent = GetComponent<NavMeshAgent>();
           weaponIK = GetComponent<WeaponIK>();
           anim = GetComponent<Animator>();
           mover = GetComponent<Mover>();

           var sceneRef = LevelManager.Instance.sceneRef;
           if(sceneRef == 0) return;
           if(sceneRef == 1) return;
           if(sceneRef == 2) return;
           if(sceneRef == 5) return;
        }

        void Update()
        { 
            if (shakeTimer > 0)
            { 
              shakeTimer -= Time.deltaTime;
              if(shakeTimer <= 0f) 
              {
                CinemachineBasicMultiChannelPerlin cineMachinePerlin = 
                cineMachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cineMachinePerlin.m_AmplitudeGain = 0f;
              }
            }            
            
            if (shieldsUp == true) 
            {
                ShieldsUp();
                vitals.SetEnergyBurnRate = 1f;
            } 
            else
            {              
                ShieldsDown();
                vitals.SetEnergyBurnRate = 0.1f; 
            }
            
            if (InteractWithCombat() || InteractWithMovement()) return;
        }

        void FixedUpdate()
        {
            if (agent.isStopped)
            {
                RemoveMovementLine();
            }
            else
            {
                DisplayMovementLine();
            }            
        }


        bool InteractWithCombat()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    CombatTarget otherCombatTarget = hit.transform.GetComponent<CombatTarget>();
                    GameObject objTarget = hit.transform.gameObject;

                    Transform selectionUI = objTarget.transform.Find("SelectionIcon");
                    if ((selectionUI != null) && (objTarget.CompareTag("Enemy")))
                    {                        
                        selectionUIObj = selectionUI.gameObject;
                        selectionUIObj.SetActive(true);
                    }

                    var thisCombatTarget = GetComponent<CombatTarget>();
                    if (otherCombatTarget == thisCombatTarget) return false;
                    Fighter fighter = GetComponent<Fighter>();
                    fighter.CombatTarget = otherCombatTarget;
                    CapsuleCollider targetCapsule = hit.transform.GetComponent<CapsuleCollider>();
                    fighter.TargetCap = targetCapsule;

                    if (otherCombatTarget == null) return false; // continue;                
                    if (!GetComponent<Fighter>().CanAttack(otherCombatTarget.gameObject))
                    {
                        return false;
                    }
                    
                    fighter.Attack(otherCombatTarget.gameObject);

                    if ((objTarget.tag == "Enemy") || (!this.CompareTag("Dead")))
                    {
                        ShakeCamera(1.5f, 0.2f);
                    }

                    return true;
                }
            }
            return false;
            
        }   

        bool InteractWithMovement()
        {          
           if(isDead == true) return false;
           Fighter fighter = GetComponent<Fighter>();           
           fighter.CancelTarget();          
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

        void DisplayMovementLine()
        {           
            lineRenderer.enabled = true;
            lineRenderer.material.color = Color.red;
            lineRenderer.positionCount = agent.path.corners.Length;
            lineRenderer.SetPosition(0, transform.position);

            if(agent.path.corners.Length < 2)
            {
                return;
            }

            for(int i = 1; i < agent.path.corners.Length; i++)
            {
              Vector3 pointPosition = new Vector3(agent.path.corners[i].x, agent.path.corners[i].y, agent.path.corners[i].z);
              lineRenderer.SetPosition(i, pointPosition);
            }            
        }

        void RemoveMovementLine()
        {
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }

        void OnTriggerEnter(Collider other)
        {
            GC.Collect(); 
            if (other.gameObject.CompareTag("weaponPU"))
            { 
               weaponPU = other.gameObject;
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
           GC.Collect();         
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
                fighter.EquipUnarmed();  
                anim.SetTrigger("pickUp");
                pickUp.PickUpItem();
                break;

                case 2:
                fighter.SetLastWeapon = fighter.weaponConfig; 
                fighter.EquipUnarmed();  
                anim.SetTrigger("PicKUPHead");
                pickUp.PickUpItem(); 
                break;

                case 3:
                target.position = weaponPU.transform.position;
                fighter.SetLastWeapon = fighter.weaponConfig; 
                fighter.EquipUnarmed();                
                anim.SetTrigger("use");
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
            if(ammoCounter == true)
           {
               ammoCounter.SetActive(false);
           }  
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