using UnityEngine;
using Rambler.Combat;

namespace Rambler.Inventories
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class PickUp : MonoBehaviour
    {
        // STATE
        Weapon weaponConfig;
        int number = 1;

        // CACHED REFERENCE
        Inventory inventory;

        // LIFECYCLE METHODS

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        // PUBLIC

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        /// <param name="number">The number of items represented.</param>
        public void Setup(Weapon weaponConfig, int number)
        {
            this.weaponConfig = weaponConfig;
            if (!weaponConfig.IsStackable())
            {
                number = 1;
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

        public void PickupItem()
        {
            bool foundSlot = inventory.AddToFirstEmptySlot(weaponConfig, number);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(weaponConfig);
        }
    }
}