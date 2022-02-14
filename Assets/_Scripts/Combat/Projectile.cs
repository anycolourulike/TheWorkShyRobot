using Rambler.Core;
using Rambler.Movement;
using Rambler.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rambler.Combat
{
  public class Projectile : MonoBehaviour
  {    
    [SerializeField] GameObject hitEffect = null;   
    [SerializeField] float maxLifeTime = 1.5f;  
    [SerializeField] float speed = 100f;    
    [SerializeField] float aim = 1.35f; 

    CombatTarget combatTarget;     
    public float damage = 0; 
   
    Health target; 
    public Health Target {set{target = value;}}   
    Rigidbody rb;    
    

    void Start()
    {       
      combatTarget = target.GetComponent<CombatTarget>();     
      transform.LookAt(GetAimLocation());            
      rb = GetComponent<Rigidbody>();             
    }

    void Update()
    {      
      rb.AddRelativeForce(Vector3.forward * speed * Time.deltaTime);            
    }

    public void SetTarget(Health target, float damage)
    {
        this.target = target;
        this.damage = damage;        
        Destroy(gameObject, maxLifeTime);
    }

    public Vector3 GetAimLocation()
    {        
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>(); 
        
        if (targetCapsule == null)
        {                         
          return target.transform.position;              
        }        
        return target.transform.position + combatTarget.velocity + Vector3.up * targetCapsule.height / aim;                      
    }        
    
    public GameObject HitEffect()
    {
       return hitEffect;
    } 
  }
}