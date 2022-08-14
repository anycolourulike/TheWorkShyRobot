using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Control;

namespace Rambler.Combat
{
    public class AnimationEvents : MonoBehaviour
    {        
        [SerializeField] PlayerController playerController;
        [SerializeField] Fighter fighter;
        public Transform playerHand;       
        public Transform head; 
      
        public void EquipUnarmed() 
        {
            if(playerController.isHolstered == true)
            {                
              fighter.EquipUnarmed();              
              playerController.isHolstered = false;
            }  
            return;          
        }

        public void EndPickup() 
        {
            fighter.EquipPickedUpWeapon();
            playerController.EnableMover();
        }

        public void HeadAttach()
        {
           head.transform.SetParent(playerHand);
        }
    }
}