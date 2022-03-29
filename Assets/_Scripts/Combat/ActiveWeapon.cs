using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rambler.Core;
using TMPro;

namespace Rambler.Combat
{    
    public class ActiveWeapon : MonoBehaviour
    {   
        public enum WeaponType {melee, pistol, smg, shotgun, rifle, plasma, launcher, NPCWeapon}; 
        public WeaponType weaponType;     
        [SerializeField] Transform muzzleTransform;         
        public GameObject ammoCountObj;
        [SerializeField] Transform aimTransform; 
        [SerializeField] GameObject MuzzleFlash; 
        [SerializeField] Projectile projectile;     
        [SerializeField] GameObject magazine;
        [SerializeField] float weaponRange;         
        public TextMeshProUGUI totalAmmoDisplay;  
        public TextMeshProUGUI magDisplay;
        Animator rigController;
        public Animator SetRigController {get{return rigController;} set{rigController = value; }}
        float weaponDamage;        
        int magAmount;
        public int GetMagAmount {get{return magAmount;}}
        int totalAmmo;  
        public int GetTotalAmmo {get{return totalAmmo;}}  
        bool reloading;   
        public bool GetIsReloading{get{return reloading;}}     
          
        int ammoSpent;                   
        int curClip;
                   

        void Update() 
        {   
           if ((this.tag == "NPCWeapon") || (weaponType == WeaponType.melee)) return;
           AmmoDisplay();     
        } 

        public void AmmoUIInit() 
        {  
            GetProjectileDamage();  

            var playerCore = GameObject.Find("PlayerCore");
            ammoCountObj = playerCore.transform.GetChild(0).GetChild(0).gameObject;
                
            var magazineCounter = ammoCountObj.transform.GetChild(0).gameObject;
            magazineCounter.SetActive(true);
            magDisplay = magazineCounter.GetComponentInChildren<TMPro.TextMeshProUGUI>();  

            var totalAmmoCounter = ammoCountObj.transform.GetChild(1).gameObject;
            totalAmmoCounter.SetActive(true);
            totalAmmoDisplay = totalAmmoCounter.GetComponentInChildren<TMPro.TextMeshProUGUI>(); 
            FullMag();
            UpdateTotalAmmo();
            UpdateClipDisplay();           
        } 

        public void FullAmmo()
        {
            if(weaponType == WeaponType.pistol)
            {
                totalAmmo = 90;
            }

            if(weaponType == WeaponType.smg)
            {
                totalAmmo = 120;
            }

            if(weaponType == WeaponType.shotgun)
            {
                totalAmmo = 50;
            }

            if(weaponType == WeaponType.rifle)
            {
                totalAmmo = 150;
            }

            if(weaponType == WeaponType.plasma)
            {
                totalAmmo = 50;
            }

            if(weaponType == WeaponType.launcher)
            {
                totalAmmo = 20;
            }
        }    

        public Transform AimTransform()
        {
            return aimTransform;
        } 

        public Transform MuzPos() 
        {
            return muzzleTransform;
        }

        public float GetRange()
        {
            return weaponRange;
        } 

        public void Reload() 
        {                                
            if(totalAmmo >= 0)
            { 
                reloading = true;
                rigController.SetTrigger("Reload"); 
                //set bool is reloading
                ammoSpent = magAmount - curClip;  
                                       
                if (ammoSpent >= totalAmmo)
                {
                   ammoSpent = totalAmmo;                  
                }
                else if (ammoSpent <= totalAmmo)
                {
                    totalAmmo -= ammoSpent;
                }
                UpdateTotalAmmo();
                FullMag(); 
                reloading =  false;             
            }           
        } 
       
       public void LaunchProjectile(Transform muzzleTransform, Vector3 target)
        {                      
            //MF_AutoPool.Spawn(prefab, muzzleTransform.position, muzzleTransform.rotation);  
            //projectile = prefab.GetComponent<Projectile>();
            //projectile.SetTarget(target);
            Projectile Firedprojectile = Instantiate(projectile, muzzleTransform.position, muzzleTransform.rotation);            
            Firedprojectile.SetTarget(target);

            GameObject Muzzle = Instantiate(MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation) as GameObject;            
            Muzzle.transform.parent = muzzleTransform.transform;                         
            curClip --;                                                             
        }
        // public void LaunchProjectile(Transform muzzleTransform, Vector3 target)
        // {  
        //      MF_AutoPool.Spawn(prefab, muzzleTransform.position, muzzleTransform.rotation);  
        //    projectile.SetTarget(target);  
        //
        //     Projectile Firedprojectile = Instantiate(projectile, muzzleTransform.position, muzzleTransform.rotation);            
        //     Firedprojectile.SetTarget(target);            
                          
        //     GameObject Muzzle = Instantiate(MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation) as GameObject;            
        //     Muzzle.transform.parent = muzzleTransform.transform; 
               
        //     Destroy(Firedprojectile, 2f);            
        //     curClip --;                                                             
        // }

        void AmmoDisplay() 
        {            
            UpdateClipDisplay();            
            
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

        void UpdateTotalAmmo() 
        {
            totalAmmoDisplay.text = (totalAmmo.ToString());
        } 

        void UpdateClipDisplay() 
        {
            magDisplay.text = (curClip.ToString());
        }  

        void GetProjectileDamage()
        {
            weaponDamage = projectile.GetDamage();
        }     

        void FullMag()
        {
            if(weaponType == WeaponType.pistol)
            {
                magAmount = 15;
            }

            if(weaponType == WeaponType.smg)
            {
                magAmount = 30;
            }

            if(weaponType == WeaponType.shotgun)
            {
                magAmount = 8;
            }

            if(weaponType == WeaponType.rifle)
            {
                magAmount = 30;
            }

            if(weaponType == WeaponType.plasma)
            {
                magAmount = 20;
            }

            if(weaponType == WeaponType.launcher)
            {
                magAmount = 5;
            }
            curClip = magAmount;
            UpdateTotalAmmo();         
        }    
    } 
}