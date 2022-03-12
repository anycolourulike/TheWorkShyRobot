using System.Collections;
using System.Collections.Generic;
using Rambler.Movement;
using UnityEngine;
using Rambler.Core;
using Rambler.Control;

namespace Rambler.Combat 
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {
       PlayerController playerController;
       Vector3 lastSpeed = new Vector3();
       Transform targetTransform;        
       AIController aIScript;
       Vector3 nextPosition;           
       Rigidbody rb; 
       Mover mover; 
       float speed;
      
       void Start()
       {  
           if(this.gameObject.tag == "Enemy")
           {
               aIScript = GetComponent<AIController>();
           }
           else
           {
               playerController = GetComponent<PlayerController>();
           }
           mover = GetComponent<Mover>();
           rb = GetComponent<Rigidbody>();
           targetTransform = transform;
       }  

       void Update() 
       {   
            speed = mover.MaxSpeed();        
            if(this.gameObject.tag == "Enemy")
            {
               nextPosition = aIScript.NextPosition;
            }
            else
            {
               nextPosition = playerController.NextPosition;
            }  

            Vector3 aim = nextPosition - transform.position;

            if(aim.magnitude > 0.5f)
            {
                lastSpeed = aim.normalized * speed;
            }
            else
            {
                lastSpeed = new Vector3(0,0,0);
            }
       }          

       public Vector3 enemyCurPos { get{ return targetTransform.position; }}

       public Vector3 LastSpeed { get{ return lastSpeed; }}       
    }
}
