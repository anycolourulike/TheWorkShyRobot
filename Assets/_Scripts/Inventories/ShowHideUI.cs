using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rambler.UI
{
    public class ShowHideUI : MonoBehaviour
    {        
        [SerializeField] GameObject uiContainer = null;     

       
        void Start()
        {
            uiContainer.SetActive(false);            
        }   
      
        public void ShowInv()
        {                 
            uiContainer.SetActive(true);
        }

        public void HideInv()
        {                 
            uiContainer.SetActive(false);
        }
    }
}    
