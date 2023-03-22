﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rambler.Core;
using Rambler.Control;
using TMPro;
using Rambler.Saving;

namespace Rambler.Combat
{    
    public class ActiveWeapon : MonoBehaviour, ISaveable
    {   
        public enum WeaponType {melee, pistol, smg, shotgun, rifle, plasma, launcher}; 
        public WeaponType weaponType;     
        [SerializeField] Transform muzzleTransform;         
        public GameObject ammoCountObj;
        [SerializeField] Transform aimTransform; 
        [SerializeField] GameObject MuzzleFlash; 
        [SerializeField] Projectile projectile; 
        [SerializeField] float weaponRange;         
        public TextMeshProUGUI totalAmmoDisplay;  
        public TextMeshProUGUI magDisplay;
        Animator rigController;
        public Animator SetRigController { get{return rigController;} set{rigController = value;} }  
        bool reloadAmmoSFX;
        float weaponDamage;        
        int magAmount;
        int totalAmmo;
        bool isAmmo = true;
        public bool IsAmmo { get {return isAmmo;} set {isAmmo = value;} }
        bool reloading;  
        public int maxClip;  
        int ammoSpent;                   
        public int curClip;
        public int CurClip { get {return curClip;} private set{ ;} }

        void Start() 
        { 
            if(this.gameObject.CompareTag("NPCWeapon"))
            {                
                FullMag();
            }
        }          

        void Update() 
        {  
            if(weaponType == WeaponType.melee) return;  
            if(this.CompareTag ("weapon"))
            {
               AmmoDisplay();  
            }
            else if(this.CompareTag("NPCWeapon"))
            {
               if(curClip == 0)
               {
                    isAmmo = false;
               }
            }            
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
            
            if(this.CompareTag ("NPCWeapon")) return;
            UpdateTotalAmmoDisplay();
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
            if(curClip == maxClip)  return;                          
            if(totalAmmo >= 0)
            { 
                reloading = true;
                ReloadAnimCheck();
                
                ammoSpent = magAmount - curClip;  
                                       
                if (ammoSpent >= totalAmmo)
                {
                   ammoSpent = totalAmmo;  //Store Total Ammo for when Weapon is requipped                
                }
                else if (ammoSpent <= totalAmmo)
                {
                    totalAmmo -= ammoSpent;
                }
                FullMag(); 
                reloading = false; 
                if(this.gameObject.CompareTag ("NPCWeapon")) return;
                UpdateTotalAmmoDisplay();                                              
            }           
        } 
       
       public void LaunchProjectile(Transform muzzleTransform, Vector3 target)
        {     
            if((curClip > 0)  && (reloading = true))
            { 
            ShootAnim();          
                
            Projectile Firedprojectile = Instantiate(projectile, muzzleTransform.position, muzzleTransform.rotation);            
            Firedprojectile.SetTarget(target);

            GameObject Muzzle = Instantiate(MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation) as GameObject;            
            Muzzle.transform.parent = muzzleTransform.transform;  
                                   
            curClip --; 
            }                                                            
        }

        [System.Serializable]
        public struct SavingStruct
        {
            public int totalAmmo;
            public int magAmount;
        }

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

        void UpdateTotalAmmoDisplay() 
        {
            totalAmmoDisplay.text = (totalAmmo.ToString());
        } 

        void UpdateClipDisplay() 
        {
            magDisplay.text = (curClip.ToString());
        }  

        void GetProjectileDamage()
        {
            if(weaponType == WeaponType.melee)
            {
                weaponDamage = 50;
            }
            else
            {
              weaponDamage = projectile.GetDamage();
            }
        }   

        void ReloadAnimCheck() 
        {            
            if(reloadAmmoSFX == false)
            {
              AudioManager.PlayWeaponSound(weaponSFX: AudioManager.WeaponSound.weaponReload, this.transform.position);
              reloadAmmoSFX = true;
            }  
            
            if(this.gameObject.CompareTag("NPCWeapon"))
            {
                return;//AI Reload Animation Played In Fighter
            }
            else
            {                
                switch(this.weaponType)
               {
                case WeaponType.pistol:
                 rigController.SetTrigger("ReloadPistol");  
                 break;
                case WeaponType.smg:
                 rigController.SetTrigger("ReloadSMG");
                 break;
                case WeaponType.rifle:
                 rigController.SetTrigger("ReloadRifle");
                 break;
                //  case WeaponType.shotgun:
                //  rigController.SetTrigger("ReloadShotgun");
                //  break;                 
                //  case WeaponType.launcher:
                //  rigController.SetTrigger("ReloadLauncher");
                //  break;
                //  case WeaponType.plasma:
                //  rigController.SetTrigger("ReloadPlasma");
                //  break;
               } 
            }
        }  

        void FullMag()
        {
            switch(this.weaponType)
               {
                case WeaponType.pistol:
                 magAmount = 15; 
                 maxClip = 45;                     
                 break;
                case WeaponType.smg:
                 magAmount = 30;
                 maxClip = 90;
                 break;
                case WeaponType.rifle:
                 magAmount = 25;
                 maxClip = 75;
                 break;
                 case WeaponType.shotgun:
                 magAmount = 5;
                 maxClip = 15;
                 break;                 
                 case WeaponType.launcher:
                 magAmount = 5;
                 maxClip = 15;
                 break;
                 case WeaponType.plasma:
                 magAmount = 7;
                 maxClip = 21;
                 break;
               } 
           
            curClip = magAmount;
            reloadAmmoSFX = false;
            if(this.gameObject.CompareTag("NPCWeapon")) return;
            UpdateTotalAmmoDisplay();         
        }  

        void ShootAnim() 
        {
            if(this.gameObject.tag == "NPCWeapon")
            {                
                rigController.SetTrigger("Shoot");                
            }
            else
            {
               switch(this.weaponType)
               {
                case WeaponType.pistol:
                rigController.SetTrigger("ShootPistol");                           
                 break;
                case WeaponType.smg:
                rigController.SetTrigger("ShootSMG");
                 break;
                case WeaponType.rifle:
                rigController.SetTrigger("ShootRifle");
                 break;
               }
                             
            }                    
        }

        public object CaptureState()
        {
            SavingStruct savingStruct = new SavingStruct();
            savingStruct.totalAmmo = totalAmmo;
            savingStruct.magAmount = magAmount;
            return savingStruct;            
        }

        public void RestoreState(object state)
        {
            magAmount = (int)state;
            totalAmmo = (int)state;
        }
    } 
}