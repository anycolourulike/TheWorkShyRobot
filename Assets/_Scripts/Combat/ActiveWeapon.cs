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
        [SerializeField] Transform muzzleTransform; 
        [SerializeField] ObjectPooler projObjPool;
        [SerializeField] GameObject ammoCountObj;
        [SerializeField] Transform aimTransform; 
        [SerializeField] GameObject MuzzleFlash; 
        [SerializeField] Projectile projectile;           
        [SerializeField] float weaponDamage;   
        [SerializeField] float weaponRange;
        TextMeshProUGUI totalAmmoDisplay;  
        TextMeshProUGUI magDisplay;              
        WeaponType thisWeapon;             
        string weaponName; 
        int ammoSpent;                   
        int curClip;
        
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

        public Transform AimTransform()
        {
            return aimTransform;
        }    

        public Transform MuzPos() 
        {
            return muzzleTransform;
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
                       
            RefreshClipDisplay();            
            
            if(curClip == 0)
            {
                magDisplay.color = Color.red;
            }
            else 
            {
                magDisplay.color = Color.white;
            }

            if(totalAmmo == 0)
            {
                totalAmmoDisplay.color = Color.red;
            }
            else 
            {
                totalAmmoDisplay.color = Color.white;
            }
        }

        public void Reload() 
        {                                
            if(totalAmmo >= 0)
            {  
                ammoSpent = magazineAmount - curClip;  
                                       
                if (ammoSpent >= totalAmmo)
                {
                   ammoSpent = totalAmmo;                  
                }
                else if (ammoSpent <= totalAmmo)
                {
                    totalAmmo -= ammoSpent;
                }
                RefreshTotalAmmo();
                FullMag();                
            }           
        }
        
        int id = 0;

       
       public void LaunchProjectile(Transform muzzleTransform, Vector3 target)
        {                      
            int id = 1;
            Vector3 position = muzzleTransform.position;
            Quaternion rotation = Quaternion.LookRotation(target - position);
            var proj = ObjectPooler.Instance.Activate(id, position, rotation);
            var projObj = GetComponent<Projectile>();            
            projObj.SetTarget(target, projObj.damage);  

            GameObject Muzzle = Instantiate(MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation) as GameObject;            
            Muzzle.transform.parent = muzzleTransform.transform;                         
            curClip --;                                                             
        }
        // public void LaunchProjectile(Transform muzzleTransform, Vector3 target)
        // {  
        //     Projectile Firedprojectile = Instantiate(projectile, muzzleTransform.position, muzzleTransform.rotation);            
        //     Firedprojectile.SetTarget(target, weaponDamage);            
                          
        //     GameObject Muzzle = Instantiate(MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation) as GameObject;            
        //     Muzzle.transform.parent = muzzleTransform.transform; 
               
        //     Destroy(Firedprojectile, 2f);            
        //     curClip --;                                                             
        // }

        void RefreshTotalAmmo() 
        {
            totalAmmoDisplay.text = (totalAmmo.ToString());
        } 

        void RefreshClipDisplay() 
        {
            magDisplay.text = (curClip.ToString());
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
            curClip = magazineAmount;
            RefreshTotalAmmo();         
        }     
    } 
}