using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rambler.Core;
using TMPro;

namespace Rambler.Combat
{
    public class ActiveWeapon : WeaponBehaviour
    {        
        [SerializeField] GameObject MuzzleFlash;       
        [SerializeField] GameObject ammoCountObj;            
        [SerializeField ] float weaponRange; 
        [SerializeField] float weaponDamage;
        [SerializeField] float destroyTime;
        [SerializeField] float speed;
        public Transform MuzzlePosition;  
        public Projectile projectile; 
        public string weaponName;

        TextMeshProUGUI ammoDisplay;
        Weapon weaponConfig;
        Health target;  
        int ammoLeft;  
        int ammoSpent;    
        
        void Start() 
        {
            if(gameObject.tag == "Player")
            {
               ammoCountObj = GameObject.Find("AmmoCounter");
               ammoDisplay = ammoCountObj.GetComponent<TMPro.TextMeshProUGUI>();
               totalAmmo = 100;
            }
        }

        void Update() 
        {           
            ammoLeft = totalAmmo - ammoSpent;
            ammoDisplay.text = ammoLeft.ToString();            
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetRange()
        {
            return weaponRange;
        } 

        public void LaunchProjectile(Transform MuzzlePosition, Health target)
        {  
            Projectile Firedprojectile = Instantiate(projectile, MuzzlePosition.position, Quaternion.identity);
            MuzzleFlash = Instantiate(MuzzleFlash, MuzzlePosition.position, Quaternion.LookRotation(transform.forward));
            Firedprojectile.SetTarget(target, weaponDamage);    
            ammoSpent ++;                                          
        }         
    } 
}