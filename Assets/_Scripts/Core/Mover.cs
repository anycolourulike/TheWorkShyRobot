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

namespace Rambler.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    { 
        [SerializeField] float maxSpeed = 6f;         
        [SerializeField] Rig mainRig;        
        NavMeshAgent navMeshAgent;   
        Health health;    
        bool rigWeaponEquipped;
        public Vector3 nextDestination;  
        public Vector3 NextDestination {get{return nextDestination;}} 
        


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

        public float MaxSpeed()
        {
            return  maxSpeed;
        }          

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {    
            nextDestination = destination;        
            navMeshAgent.destination = destination; 
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
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

        public object CaptureState()
        {   
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}