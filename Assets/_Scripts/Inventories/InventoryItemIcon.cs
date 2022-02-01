using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rambler.Inventories;
using Rambler.Combat;
using TMPro;
 
namespace Rambler.Inventories
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] Button equipBtn;
        [SerializeField] Button dropBtn;                
        Inventory inventory;                 
        GameObject player;  
        Weapon thisWeapon; 
        int slotIndex;  
 
        void Start() 
        {
            player = GameObject.FindWithTag("Player");
            inventory = player.GetComponent<Inventory>();                 
        }
        
        // PUBLIC
        public void SetItem(Weapon weaponConfig, int number)
        {            
            var iconImage = GetComponent<Image>();
            if (weaponConfig == null)
            {
                iconImage.enabled = false;
            }
            else
            {                
                iconImage.enabled = true;
                iconImage.sprite = weaponConfig.GetIcon();    
                equipBtn.onClick.AddListener(() => OnEquipBtnClick(weaponConfig));
                dropBtn.onClick.AddListener(() => OnDropBtnClick(weaponConfig, number));
                thisWeapon = weaponConfig;            
            }
        }        
 
        public void OnEquipBtnClick(Weapon equipWeapon)
        {
            player.GetComponent<Fighter>().EquipWeapon(equipWeapon);
        }
 
        public void OnDropBtnClick(Weapon dropWeapon, int number)
        {           
            player.GetComponent<ItemDropper>().DropItem(thisWeapon, number);  
            player.GetComponent<Fighter>().EquipUnarmed(); 
            slotIndex = inventory.FindSlot(thisWeapon);          
            inventory.RemoveFromSlot(slotIndex, number);
        }
    }
}