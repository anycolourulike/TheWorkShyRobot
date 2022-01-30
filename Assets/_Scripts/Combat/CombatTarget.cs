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
       Transform enemyTrans;
       AIController aIController;
       Vector3 nextWayPoint;
       Transform enemyAim;       
       float speed = 6f;
       Vector3 charDest;      
       Rigidbody rb;     
              

       void Start()
       {
           rb = GetComponent<Rigidbody>();
           aIController = GetComponent<AIController>();
           enemyAim = transform;
       } 

       public void NextWayPoint()
       {
          nextWayPoint = aIController.nextPos;
       }

       public Vector3 enemyDest { get{ return nextWayPoint; }}

       public Vector3 enemyAimPos { get{ return enemyAim.position; }}

       public Vector3 velocity { get{ return rb.velocity; }}       
    }
}
