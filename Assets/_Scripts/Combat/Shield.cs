using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Combat;
using Rambler.Control;
using Rambler.Core;


namespace Rambler.Combat
{   
    public class Shield : MonoBehaviour
    {       

       void OnParticleCollision(GameObject particleProj) 
       {
           if(gameObject.tag == "Shield")
           {             
             var proj = particleProj.GetComponent<Projectile>();
             var pool = proj.GetComponent<GameObject>();
             Instantiate(proj.HitEffect(), particleProj.transform.position, particleProj.transform.rotation);
             pool.SetActive(false);
           }
       } 
    }    
} 

