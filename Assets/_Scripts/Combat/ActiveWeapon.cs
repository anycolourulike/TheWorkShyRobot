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
        public enum WeaponType {melee, pistol, smg, shotgun, rifle, plasma, launcher, NPCWeapon};          
        [SerializeField] GameObject MuzzleFlash;       
        [SerializeField] GameObject ammoCountObj;            
        [SerializeField ] float weaponRange; 
        [SerializeField] float weaponDamage;
        [SerializeField] float destroyTime;
        [SerializeField] float speed;
        
        public Transform MuzzlePosition;  
        public Projectile projectile; 
        public WeaponType thisWeapon;
        public string weaponName;
        

        TextMeshProUGUI magDisplay;
        TextMeshProUGUI totalAmmoDisplay;

        int ammoNeededToRefillClip; 
        int maxAvailableAmmoForClip;      
        int curAmmoInClip;
        Health target;        
        
        void Start() 
        {
            if (thisWeapon == WeaponType.melee) return;
            if (thisWeapon == WeaponType.NPCWeapon) return;            

            if(gameObject.tag == "weapon")
            {               
               ammoCountObj = GameObject.Find("AmmoCounter");
               var magazineCounter = ammoCountObj.transform.Find("MagazineUI").gameObject;
               magazineCounter.SetActive(true);
               magDisplay = magazineCounter.GetComponentInChildren<TMPro.TextMeshProUGUI>();  

               var totalAmmoCounter = ammoCountObj.transform.Find("TotalAmmoUI").gameObject;
               totalAmmoCounter.SetActive(true);
               totalAmmoDisplay = totalAmmoCounter.GetComponentInChildren<TMPro.TextMeshProUGUI>();  
               FullMag();
               RefreshClipDisplay();                          
            }           
        }

        void Update() 
        {           
           AmmoDisplay();        
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetRange()
        {
            return weaponRange;
        } 

        void AmmoDisplay() 
        {
            if (thisWeapon == WeaponType.melee) return;
            if (thisWeapon == WeaponType.NPCWeapon) return;
            
            //save ammo spent for weapon switching           
            
            RefreshClipDisplay();            
            
            if(curAmmoInClip == 0)
            {
                //display mag empty flash ammo Counter & reload btn red
            }
        }

        public void Reload() 
        {                                
            if(totalAmmo >= 0)
            {  
                ammoNeededToRefillClip = magazineAmount - curAmmoInClip;  
                                       
                if (ammoNeededToRefillClip >= totalAmmo)
                {
                   ammoNeededToRefillClip = totalAmmo;                  
                }
                else if (ammoNeededToRefillClip <= totalAmmo)
                {
                    totalAmmo -= ammoNeededToRefillClip;
                }
                RefreshTotalAmmo();
                FullMag();                
            } 
            //display out of ammo
           
        }

        public void LaunchProjectile(Transform MuzzlePosition, Health target)
        {  
            if(curAmmoInClip > 0)
            {
              Projectile Firedprojectile = Instantiate(projectile, MuzzlePosition.position, Quaternion.identity);
              MuzzleFlash = Instantiate(MuzzleFlash, MuzzlePosition.position, Quaternion.LookRotation(transform.forward));
              Firedprojectile.SetTarget(target, weaponDamage);    
              curAmmoInClip --;            
            }                                          
        }

        void RefreshTotalAmmo() 
        {
            totalAmmoDisplay.text = (totalAmmo.ToString());
        } 

        void RefreshClipDisplay() 
        {
            magDisplay.text = (curAmmoInClip.ToString());
        }
       

        void FullMag()
        {
            if(thisWeapon == WeaponType.pistol)
            {
                magazineAmount = 15;
            }

            if(thisWeapon == WeaponType.smg)
            {
                magazineAmount = 30;
            }

            if(thisWeapon == WeaponType.shotgun)
            {
                magazineAmount = 8;
            }

            if(thisWeapon == WeaponType.shotgun)
            {
                magazineAmount = 30;
            }

            if(thisWeapon == WeaponType.plasma)
            {
                magazineAmount = 20;
            }

            if(thisWeapon == WeaponType.launcher)
            {
                magazineAmount = 5;
            }
            curAmmoInClip = magazineAmount;
            RefreshTotalAmmo();         
        }        
    } 
}