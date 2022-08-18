using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cineCamera;
    [SerializeField] GameObject player;
    public bool zoomOut;
    
    [SerializeField] float zoomedOutValue = 47f;
    [SerializeField] float zoomedInValue  = 40f;
    float speed = 5f;
   
    void Update() 
    {           
        if(zoomOut == true)
        {
            if(cineCamera.m_Lens.FieldOfView < zoomedOutValue)
            {
              cineCamera.m_Lens.FieldOfView += speed * Time.deltaTime;
            } 
        }
        else if(zoomOut == false)
        {   
            if(cineCamera.m_Lens.FieldOfView > zoomedInValue)
            {
              cineCamera.m_Lens.FieldOfView -= speed * Time.deltaTime;
            } 
        }        
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject == player)
        {
            zoomOut = true;
        }
    }

    void OnTriggerExit(Collider other)
   {
       if(other.gameObject == player)
        {
            zoomOut = false;
        }
   }
}
