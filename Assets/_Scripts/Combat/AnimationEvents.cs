using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Control;
using Rambler.Attributes;

namespace Rambler.Combat
{
    public class AnimationEvents : MonoBehaviour
    {        
        [SerializeField] PlayerController playerController;
        [SerializeField] AIController aICon;
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
            fighter.SetLastWeapon = fighter.weaponConfig;
        }

        public void HeadAttach()
        {
           head.transform.SetParent(playerHand);
        }

        public void ReloadFinished()
        {
            Debug.Log("ReloadFinished");
            fighter.ReloadFin();
            aICon.ReloadingFalse();
        }        
    }
}