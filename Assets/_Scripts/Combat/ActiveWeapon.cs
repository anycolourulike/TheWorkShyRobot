using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rambler.Core;
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
        [SerializeField] GameObject magazine;
        [SerializeField] float weaponRange;         
        public TextMeshProUGUI totalAmmoDisplay;  
        public TextMeshProUGUI magDisplay;
        Animator rigController;
        public Animator SetRigController {get{return rigController;} set{rigController = value; }}
        float weaponDamage;        
        public int magAmount;
        public int GetMagAmount {get{return magAmount;}}
        public int totalAmmo;  
        public int GetTotalAmmo {get{return totalAmmo;}}  
        bool reloading;   
        public bool GetIsReloading{get{return reloading;}}     

        int maxClip;  
        int ammoSpent;                   
        int curClip;

        void Start() 
        {
            if(this.gameObject.tag == ("NPCWeapon"))
            {
                FullMag();
            }
        }          

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
            
            if(this.tag == "NPCWeapon") return;
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
            if(totalAmmo == maxClip)  return;                          
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
                if(this.gameObject.tag == "NPCWeapon") return;
                UpdateTotalAmmoDisplay();
                                              
            }           
        } 
       
       public void LaunchProjectile(Transform muzzleTransform, Vector3 target)
        {     
            if((curClip > 0)  && (reloading = true))
            {              
            //MF_AutoPool.Spawn(prefab, muzzleTransform.position, muzzleTransform.rotation);  
            //projectile = prefab.GetComponent<Projectile>();
            //projectile.SetTarget(target);
            ShootAnim();          
                
            Projectile Firedprojectile = Instantiate(projectile, muzzleTransform.position, muzzleTransform.rotation);            
            Firedprojectile.SetTarget(target);

            GameObject Muzzle = Instantiate(MuzzleFlash, muzzleTransform.position, muzzleTransform.rotation) as GameObject;            
            Muzzle.transform.parent = muzzleTransform.transform;  
                                   
            curClip --; 
            }                                                            
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
            if(this.gameObject.tag == "NPCWeapon")
            {
                rigController.SetTrigger("Reload");
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
                 maxClip = 15;                     
                 break;
                case WeaponType.smg:
                 magAmount = 30;
                 maxClip = 30;
                 break;
                case WeaponType.rifle:
                 magAmount = 25;
                 maxClip = 25;
                 break;
                 case WeaponType.shotgun:
                 magAmount = 5;
                 maxClip = 5;
                 break;                 
                 case WeaponType.launcher:
                 magAmount = 5;
                 maxClip = 5;
                 break;
                 case WeaponType.plasma:
                 magAmount = 10;
                 maxClip = 10;
                 break;
               } 
           
            curClip = magAmount;
            if(this.gameObject.tag == "NPCWeapon") return;
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
            return magAmount;
        }

        public void RestoreState(object state)
        {
            magAmount = (int)state;
        }
    } 
}