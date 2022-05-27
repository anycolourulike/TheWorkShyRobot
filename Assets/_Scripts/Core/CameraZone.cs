using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZone : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cineCamera;
    public bool zoomOut;
    
    [SerializeField] float zoomedOut = 47f;
    [SerializeField] float zoomedIn  = 40f;
    float speed = 5f;
   
    void Update() 
    {           
        if(zoomOut == true)
        {
            if(cineCamera.m_Lens.FieldOfView < zoomedOut)
            {
              cineCamera.m_Lens.FieldOfView += speed * Time.deltaTime;
            } 
        }
        else if(zoomOut == false)
        {   
            if(cineCamera.m_Lens.FieldOfView > zoomedIn)
            {
              cineCamera.m_Lens.FieldOfView -= speed * Time.deltaTime;
            } 
        }        
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            zoomOut = true;
        }
    }

    void OnTriggerExit(Collider other)
   {
       if(other.CompareTag("Player"))
        {
            zoomOut = false;
        }
   }
}
