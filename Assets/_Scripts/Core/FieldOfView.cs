using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Core;

namespace Rambler.Control
{
    public class FieldOfView : MonoBehaviour
    {
      public float radius;
      [Range(0,360)]
      public float angle;
    
      [SerializeField]
      public Collider[] playerRefs; 
      public LayerMask targetMask;
      public LayerMask obstructionMask; 
      public bool canSeePlayer;

      private void Start()
      {        
        StartCoroutine(FOVRoutine());
      }

      private IEnumerator FOVRoutine()
      {
        WaitForSeconds wait = new WaitForSeconds(0.3f);

        while (true)
        {
          yield return wait;
          FieldOfViewCheck();
        }
      }      

      private void FieldOfViewCheck()
      {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
              
        if (rangeChecks.Length != 0)
        { 
          Transform target = rangeChecks[0].transform;
          Vector3 directionToTarget = (target.position - transform.position).normalized;

          if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
          { 
            float distanceToTarget = Vector3.Distance(transform.position, target.position); 
            
            if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, targetMask))
            {           
              //Debug.Log(this.gameObject.name + " " + "Can See Player True");
              canSeePlayer = true;  
              return;
            }              
            else
            {
              //Debug.Log(this.gameObject.name + " " + "Can See Player False RayCastFalse");
              canSeePlayer = false;  
              return;  
            } 
          }
          else  
          // Debug.Log(this.gameObject.name + " " + "Can See Player False Angle");              
          canSeePlayer = false; 
        } 
        else if(canSeePlayer)
        //Debug.Log("Can See Player False RangeCheck");
        canSeePlayer = false; 
      } 

      // bool PlayerDetect()
      // { 
      //   // if(this.gameObject.CompareTag("Player"))
      //       // {
      //       //   if(PlayerDetect() == true) 
      //       //   {
      //       //     canSeePlayer = false;
      //       //   }
      //       // }
      //   RaycastHit hit;
      //   if(Physics.Raycast(transform.position, this.transform.forward, out hit, 300f, playerLayer))
      //   {
      //     if(hit.transform.gameObject.name == "Rambler")
      //     {       
      //       return true;                  
      //     }    
      //     else
      //     {
      //       return false;
      //     }                
      //   } 
      //   return true;   
      // }      
  }
}