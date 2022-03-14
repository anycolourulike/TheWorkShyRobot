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
       Mover mover;
       Vector3 curTargetPos;
       Vector3 prevTargetPos; 
       Vector3 targetVelocity;
       public float targetSpeed;


       public float GetTargetSpeed {get{ return targetSpeed;}} 
       public Vector3 GetTargetVelocity{get{return targetVelocity;}}
       public Vector3 GetCurTargetPos{get{return curTargetPos;}}
         
       void Start() 
       {
           mover = GetComponent<Mover>();
           prevTargetPos = transform.position;
       }  

       void Update() 
       {              
            curTargetPos = transform.position;      
            var posDif = curTargetPos - prevTargetPos;
            targetVelocity = posDif/Time.deltaTime;  
            targetSpeed = targetVelocity.magnitude;           
            prevTargetPos = curTargetPos;                               
       }               
    }
}
