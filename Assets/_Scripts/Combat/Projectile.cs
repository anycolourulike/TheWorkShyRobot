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
    [SerializeField] float speed;    
    public float damage = 0;    
    Vector3 target;
    Rigidbody Rb;
    Vector3 dir;  
    

    void Awake()
    {
      Rb = GetComponent<Rigidbody>();       
      dir = transform.position - target; 
      dir = dir.normalized; 
      //transform.LookAt(dir);
      // Rb.velocity = dir * Time.deltaTime* speed; 
    }   

    void Update() 
    {
      Rb.AddForce(dir * Time.deltaTime * speed);
       //transform.forward += (dir * (speed * Time.deltaTime));
    } 
  

    public void SetTarget(Vector3 target, float damage)
    {
      this.target = target;
      this.damage = damage;        
      Destroy(gameObject, maxLifeTime);
    }

    public Vector3 GetAimLocation()
    {        
      return target;                      
    }        
    
    public GameObject HitEffect()
    {
      return hitEffect;
    } 
  }
}