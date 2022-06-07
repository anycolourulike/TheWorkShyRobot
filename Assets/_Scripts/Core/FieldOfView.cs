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
    public GameObject[] playerRefs;    

    public LayerMask targetMask;
    public LayerMask obstructionMask;

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
        
        foreach(GameObject player in playerRefs)
        {           
           Transform target = player.transform;    

           if (rangeChecks.Length != 0)
           { 
                target = rangeChecks[0].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    canSeePlayer = true;
                else
                    {canSeePlayer = false;}
            }
            else
                {canSeePlayer = false;}
        }
        else if 
              (canSeePlayer)
                canSeePlayer = false;
                

        }    
    }
  }
}
