using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rambler.Control;
using Rambler.Inventories;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;

namespace Rambler.Combat
{
   /// Set the vital data after creating the prefab.
   /// </summary>
   /// <param name="item">The type of item this prefab represents.</param>
   /// <param name="number">The number of items represented.</param>
   /// <summary/>
    public class WeaponPickUp : MonoBehaviour
    {  
        PickUpSpawner pickupSpawner;      
        public UnityAction PickUp; 
        Weapon weaponConfig; 
        Inventory inventory;   
        GameObject interact;  
        Fighter fighter;
        GameObject player; 
        int number = 1; 

        void OnEnable() 
        {            
            PickUp += PickUpItem;
        }   

        void OnDisable() 
        {
            PickUp -= PickUpItem;
        }    

         private void Awake()
        {       
            interact = GameObject.FindGameObjectWithTag("Interact");                                         
            player = GameObject.Find("/PlayerCore/Rambler");  
            fighter = player.GetComponent<Fighter>();   
            pickupSpawner = GetComponent<PickUpSpawner>();            
            inventory = player.GetComponent<Inventory>(); 
        }                

        public void Setup(Weapon item, int number)
        {
            this.weaponConfig = item;
            if (!item.IsStackable())
            {
                number = 0;
            }
            this.number = number;                        
        }  
    
        public Weapon GetItem()
        {
            return weaponConfig;
        }

        public int GetNumber()
        {
            return number;
        } 

        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(weaponConfig);
        }  
       
        public void PickUpItem()
        {             
            bool foundSlot = inventory.AddToFirstEmptySlot(weaponConfig, number);
            if (foundSlot)
            {      
                fighter.weaponPickedUp = weaponConfig;
                Destroy(gameObject, 0.1f);                                           
            } 
        }     
    }        
}