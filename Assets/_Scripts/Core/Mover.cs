using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Rambler.Core;
using Rambler.Control;
using Rambler.Saving;
using Rambler.Combat;
using UnityEngine.Animations.Rigging;
using System.Collections;

namespace Rambler.Movement
{
    public class Mover : MonoBehaviour, IAction
    { 
        [SerializeField] float maxSpeed = 6f;  
        [SerializeField] WeaponIK weaponIK;         
        [SerializeField] Rig mainRig;        
        NavMeshAgent navMeshAgent;  
        public bool rigWeaponEquipped;
        Health health;        

        public void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();       
        }

        void Update()
        {            
            navMeshAgent.enabled = !health.IsDead();            
            UpdateAnimator();
            if(rigWeaponEquipped == true)
            { 
                if(navMeshAgent.velocity.magnitude > 0.15f)
                {
                   mainRig.weight = 0.7f;                   
                } 
                else
                {
                   mainRig.weight = 1f;                                                        
                }               
            } 
            WeaponIKWeight();                     
        }

        void WeaponIKWeight()
        {           
            if(navMeshAgent.velocity.magnitude > 0.15f)
            {                   
                weaponIK.AimWeight = 0f;
            } 
            else
            {                   
                weaponIK.AimWeight = 1f;                  
            }
                    
        }

        public void RigWeaponEquipped()
        {            
            rigWeaponEquipped = true;
        }   

        public void RigWeaponUnequipped()
        {            
            rigWeaponEquipped = false;
        } 

        public void RigWeightToZero() 
        {
            mainRig.weight = 0;
            RigWeaponUnequipped();
        }   

        public void RotateTowards(Transform target)
        {
            int rotSpeed = 120;
            var targetToLook = Quaternion.LookRotation(target.transform.position - this.transform.position);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetToLook, rotSpeed * Time.deltaTime);            
        }       

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {   
            navMeshAgent.isStopped = false;      
            navMeshAgent.destination = destination; 
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
        }

        public void CancelNav()
        {            
            navMeshAgent.isStopped = true;
        }
               
        private void UpdateAnimator()
        {
            Animator anim = GetComponent<Animator>();
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            anim.SetFloat("forwardSpeed", speed);            
        }

        // public object CaptureState()
        // {   
        //     return new SerializableVector3(transform.position);
        // }

        // public void RestoreState(object state)
        // {
        //     SerializableVector3 position = (SerializableVector3)state;
        //     GetComponent<NavMeshAgent>().enabled = false;
        //     transform.position = position.ToVector();
        //     NavMeshDelay();
        // }

        IEnumerator NavMeshDelay()
        {
            yield return new WaitForSeconds(1f);
            GetComponent<NavMeshAgent>().enabled = true;
        }        
    }
}