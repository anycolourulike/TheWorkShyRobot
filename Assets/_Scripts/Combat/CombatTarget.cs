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
       Transform targetTransform;  
       Rigidbody rb; 
      
       void Start()
       {  
           rb = GetComponent<Rigidbody>();
           targetTransform = transform;
       }            

       public Vector3 enemyCurPos { get{ return targetTransform.position; }}

       public Vector3 velocity { get{ return rb.velocity; }}       
    }
}
