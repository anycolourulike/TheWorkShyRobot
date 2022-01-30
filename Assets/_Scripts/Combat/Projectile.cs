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
    [SerializeField] float lifeAfterImpact = 0.1f;  
    [SerializeField] GameObject hitEffect = null;   
    [SerializeField] float maxLifeTime = 1.5f;   
    [SerializeField] bool isHoming = true;
    [SerializeField] float aim = 1.35f; 
    [SerializeField] float speed = 30f;

    
    CombatTarget combatTarget;
    ActiveWeapon activeWeapon;    
    public float damage = 0;    
    Vector3 hitPoint;
    Vector3 aimPoint;
    Fighter fighter;    
    Health target;        
    Rigidbody rb;    

    void Start()
    { 
      combatTarget = target.GetComponent<CombatTarget>();
      activeWeapon = GetComponent<ActiveWeapon>(); 
      transform.LookAt(GetAimLocation());
      fighter = GetComponent<Fighter>();       
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