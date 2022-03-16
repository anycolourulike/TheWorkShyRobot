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
    GameObject thisObj;   
    Vector3 target;
    Rigidbody Rb;
    Vector3 dir;  
        

    void OnEnable()
    {
      Rb = GetComponent<Rigidbody>();       
      dir = target.normalized;       
      transform.LookAt(target);     
    }   

    void Update() 
    {
      Rb.AddForce(transform.forward * Time.deltaTime * speed);
    }
  

    public void SetTarget(Vector3 target, float damage)
    {
      this.target = target;
      this.damage = damage;        
      Invoke("ObjActiveFalse", 1.3f);
    }

    public Vector3 GetAimLocation()
    {        
      return target;                      
    }        
    
    public GameObject HitEffect()
    {
      return hitEffect;
    } 

    void ObjActiveFalse() 
    {
      ObjectPooler.Instance.Deactivate(gameObject);
    }
  }
}