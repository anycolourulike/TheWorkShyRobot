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
       public float targetSpeed;
       public Vector3 curTargetPos;
       public Vector3 prevTargetPos; 
       public Vector3 targetVelocity;            
         
       void Start() 
       {          
           prevTargetPos = transform.position;
       }  

       void Update() 
       {              
            curTargetPos = transform.position; 
            //Debug.Log(this.name + " " + "curTargetPos" + " " + curTargetPos);   

            Vector3 posDif = curTargetPos - prevTargetPos;
            //Debug.Log("posDif" + "" + posDif); 

            targetVelocity = posDif/Time.deltaTime; 
            //Debug.Log(s"targetVelocity" + " " + targetVelocity); 

            targetSpeed = targetVelocity.magnitude; 
           // Debug.Log("targetSpeed" + " " + targetSpeed);   

            prevTargetPos = curTargetPos;
            //Debug.Log("prevTargetPos" + " " + prevTargetPos);                               
       } 

       public Vector3 TargetFuturePos(Vector3 ShooterPos) 
       {
           var hitPointVector = GetHitPointVector(curTargetPos, targetVelocity, ShooterPos, 10f);
           return hitPointVector;
       }

        Vector3 GetHitPointVector(Vector3 targetPosition, Vector3 targetVelocity, Vector3 shooterPosition, float projectileSpeed)
        {
          Vector3 targetShooterVector = targetPosition - shooterPosition;
          float targetMoveAngle = Vector3.Angle(-targetShooterVector, targetVelocity) * Mathf.Deg2Rad;
          //if the target is stopping or if it is impossible for the projectile to catch up with the target (Sine Formula)
          
         if (targetVelocity.magnitude == 0 || targetVelocity.magnitude > projectileSpeed && Mathf.Sin(targetMoveAngle)
               / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
           {
             Debug.Log("Target standing Still");              
             return targetPosition;
           }
         //also Sine Formula
          float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetSpeed / projectileSpeed);
          return targetPosition + targetVelocity * targetShooterVector.magnitude 
                / Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle)
                / targetVelocity.magnitude;
        }
  }
}
