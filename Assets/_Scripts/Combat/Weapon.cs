using Rambler.Core;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Rambler.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/new", order = 0)]
    public class Weapon : ScriptableObject, ISerializationCallbackReceiver
    {       
        [SerializeField] AnimatorOverrideController animatorOverride = null;  
        public ActiveWeapon activeWeapon;        
        public GameObject equippedPrefab;
        public Projectile projectile;
        public string weaponTitle; 
        public bool isPistol;
        public bool isRifle;        
        public bool isMelee;
        public bool isEMP;  
         
        // CONFIG DATA
        [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
        [SerializeField] string itemID = null;
        [Tooltip("Item name to be displayed in UI.")]
        [SerializeField] string displayName = null;
        [Tooltip("Item description to be displayed in UI.")]
        [SerializeField] [TextArea] string description = null;
        [Tooltip("The UI icon to represent this item in the inventory.")]
        [SerializeField] public Sprite icon = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        [SerializeField] WeaponPickUp pickup = null;
        [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
        [SerializeField] bool stackable = false;
        
        // STATE
        static Dictionary<string, Weapon> itemLookupCache;

        void Start()
        {
            activeWeapon = equippedPrefab.GetComponentInChildren<ActiveWeapon>();
        }           

        public static Weapon GetFromID(string itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, Weapon>();
                var itemList = Resources.LoadAll<Weapon>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                        continue;
                    }
                    itemLookupCache[item.itemID] = item;
                }
            }
            if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
            return itemLookupCache[itemID];
        }   

        public WeaponPickUp SpawnPickUp(Vector3 position, int number)
        {
            var pickup = Instantiate(this.pickup);
            pickup.transform.position = position;
            pickup.Setup(this, number);
            return pickup;
        }     

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        } 

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            // Generate and save a new UUID if this is blank.
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            // Required by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }       
    }
}