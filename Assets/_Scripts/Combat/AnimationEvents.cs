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
      
        public void EquipUnarmed() //Called in Animator
        {
            if(playerController.isHolstered == true)
            {                
              fighter.EquipUnarmed();              
              playerController.isHolstered = false;
            }  
            return;          
        }
    }
}