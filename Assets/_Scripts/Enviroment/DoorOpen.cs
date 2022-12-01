using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    public delegate void DoorUnlocked();
    public static DoorUnlocked doorUnlocked;    
    public bool doorIsUnlocked;
    public GameObject player;
    public float doorTimer = 7f;
    public bool doorIsOpen;
    bool doorOpened;
    Animator anim;

    void OnEnable()
    {
        doorUnlocked += DoorLock;
    }
    
    void OnDisable()
    {
        doorUnlocked -= DoorLock;
    }

    void Start() 
    {
        anim = GetComponent<Animator>();      
    } 

    void Update()
    {
        DoorTimer();
        if(doorIsUnlocked == true && doorOpened == false)
        {         
            OpenDoor();
            doorOpened = true;
        }         

        if(doorTimer <= 0f)
        {
            doorTimer = 7f;
            doorIsUnlocked = false;  
            doorIsOpen = false; 
            doorOpened = false;         
            anim.SetTrigger("Close");
        } 
    } 

    void DoorLock()
    {
       doorIsUnlocked = true;
    }  

    void DoorTimer()
    {
        if(doorIsOpen == true)
        {
          doorTimer -= Time.deltaTime;
        }  
    }

    void OnTriggerEnter(Collider other)
    {
        if((other.gameObject.CompareTag("Enemy")) || (other.gameObject == player))
        {
            anim.SetTrigger("Open");
            doorIsOpen = true;
        }
    }

    void OpenDoor()
    {
        anim.SetTrigger("Open");
        doorIsOpen = true;
    }
}
