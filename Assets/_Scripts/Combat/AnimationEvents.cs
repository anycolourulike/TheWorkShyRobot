using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Control;

namespace Rambler.Combat
{
    public class AnimationEvents : MonoBehaviour
    {
        [SerializeField] Animator anim;
        [SerializeField] Fighter fighter;
        [SerializeField] PlayerController playerController;
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
        }

        public void HeadAttach()
        {
           head.transform.SetParent(playerHand);
        }
    }
}