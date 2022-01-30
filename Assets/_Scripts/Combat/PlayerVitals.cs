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
       public float SetEnergyBurnRate {set {energyBurnRate = value;}}
         
       float playerMaxEnergy = 100;              
       float playerMaxHP = 5000;      
       float playerCurEnergy;      

       float playerCurHP;
       Health health;      
       
       void Start()
       {             
           health = GameObject.FindWithTag("Player").GetComponent<Health>(); 
           MaxHealth();
           playerMaxHP = health.GetHealthPoints();
           MaxEnergy();
       }

       void FixedUpdate()
       {  
            ReduceEnergy();
            var fillHealth = playerCurHP / 100; 
            fillHealth = healthbarFill.fillAmount;              

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
                if(health.IsDead() == true)
                {
                    return;
                }
                health.Die();
            } 
        }        

        public void TakeDamage(float weaponDamage)
        {
            var fillDecrease = weaponDamage / 100;
            healthbarFill.fillAmount -= fillDecrease;
        }      

        public void MaxHealth()
        {
            playerCurHP = playerMaxHP;
        }

        public void MaxEnergy()
        {
            playerCurEnergy = playerMaxEnergy;
        }

        public void ReduceEnergy()
        {
            playerCurEnergy -= energyBurnRate * Time.deltaTime;
            var fillEnergy = playerCurEnergy / 100;
            energybarFill.fillAmount = fillEnergy;             
        }    
    }
}
