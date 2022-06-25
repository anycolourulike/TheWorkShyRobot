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
    public LayerMask playerLayer;
    public bool canSeePlayer;

    private void Start()
    {        
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

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
          if(this.gameObject.CompareTag("Player"))
          {
            if(PlayerDetect() == true) 
            {
              canSeePlayer = false;
            }
          }
          if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
          
          canSeePlayer = true;
              
          else
                
          canSeePlayer = false;    
        }                    
          
          else
                
          canSeePlayer = false; 

        }
          else if(canSeePlayer)
            canSeePlayer = false;       
    } 

    bool PlayerDetect()
    {
      RaycastHit hit;
      if(Physics.Raycast(transform.position, Vector3.forward, out hit, Mathf.Infinity, playerLayer))
      {
        if(hit.transform.gameObject.tag == "Player")
        {       
          return true;                  
        }    
        else
        {
          return false;
        }                
      } 
      return true;   
    }      
  }
}