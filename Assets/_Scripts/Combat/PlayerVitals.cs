using System.Collections;
using System.Collections.Generic;
using Rambler.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Rambler.Attributes;
using Rambler.Saving;

namespace Rambler.Combat
{
     public class PlayerVitals : MonoBehaviour, ISaveable
    {
       [SerializeField] TextMeshProUGUI healthText;
       [SerializeField] TextMeshProUGUI energyText;      
       public float energyBurnRate = 0.1f;  
       [SerializeField] Image healthbarFill;
       [SerializeField] Image energybarFill; 
       [SerializeField] Health health;         
       public float SetEnergyBurnRate {set {energyBurnRate = value;}}
         
       float playerMaxEnergy = 100;              
       float playerMaxHP;      
       float playerCurEnergy;  
       float playerCurHP;
       float fillDecrease;
       
       void Start()
       {
           playerMaxHP = health.healthPoints.value;
           playerCurHP = health.healthPoints.value;
           MaxEnergy();
       }

       void FixedUpdate()
        {
            ReduceEnergy();
           
            if (playerCurHP <= 0f)
            {
                playerCurHP = 0f;
                if (health.IsDead() == true)
                {
                    return;
                }
                health.HealthCheck();
            }

            if (playerCurEnergy <= 0)
            {
                playerCurEnergy = 0;
                if (health.IsDead() == true)
                {
                    return;
                }
                health.HealthCheck();
            }
        }
        
        public void Restore() 
        {
            MaxHealth();
            MaxEnergy();
        }

        public void TakeDamage(float weaponDamage)
        {
            fillDecrease = weaponDamage / playerMaxHP;
            healthbarFill.fillAmount -= fillDecrease;
        }      

        void MaxHealth()
        {
            health.healthPoints.value = playerMaxHP;  
            healthbarFill.fillAmount = 1;         
        }        

        void MaxEnergy()
        {
            playerCurEnergy = playerMaxEnergy;
            energybarFill.fillAmount = 1;
        }

        void ReduceEnergy()
        {
            playerCurEnergy -= energyBurnRate * Time.deltaTime;
            var fillEnergy = playerCurEnergy / 100;
            energybarFill.fillAmount = fillEnergy;             
        }

        public object CaptureState()
        {
            return healthbarFill.fillAmount;
        }

        public void RestoreState(object state)
        {
            healthbarFill.fillAmount = (float)state;
        }
    }
}
