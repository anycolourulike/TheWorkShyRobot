using System.Collections;
using System.Collections.Generic;
using Rambler.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rambler.Combat
{
     public class PlayerVitals : MonoBehaviour
    {
       [SerializeField] TextMeshProUGUI healthText;
       [SerializeField] TextMeshProUGUI energyText;      
       public float energyBurnRate = 0.1f;  
       [SerializeField] Image healthbarFill;
       [SerializeField] Image energybarFill; 
       [SerializeField] Health health;         
       public float SetEnergyBurnRate {set {energyBurnRate = value;}}
         
       float playerMaxEnergy = 100;              
       float playerMaxHP = 100f;      
       float playerCurEnergy;  
       float playerCurHP;       
            
       
       void Start()
       {   
           playerCurHP = health.HealthPoints;
           MaxHealth();           
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
                health.Die();
            }

            if (playerCurEnergy <= 0)
            {
                playerCurEnergy = 0;
                if (health.IsDead() == true)
                {
                    return;
                }
                health.Die();
            }
        }
        
        public void Restore() 
        {
            MaxHealth();
            MaxEnergy();
        }

        public void TakeDamage(float weaponDamage)
        {
            var fillDecrease = weaponDamage / 100;
            healthbarFill.fillAmount -= fillDecrease;
        }      

        void MaxHealth()
        {
            health.HealthPoints = playerMaxHP;  
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
    }
}
