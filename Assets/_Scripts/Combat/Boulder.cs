using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rambler.Control;

public class Boulder : MonoBehaviour
{

    [SerializeField] GameObject landFx;
    public Rigidbody rb;
    bool FXStarted;

    void Start()
    {
        rb = GetComponent<Rigidbody>();          
    }

       public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("plane"))
        {
            landFx.SetActive(true);
            StartCoroutine("DisableFX");
        }
        if (other.CompareTag("Player"))
        {
            landFx.SetActive(true);            
        }
        if (FXStarted == false)
        {
            StartCoroutine("DisableFX");
        }
        
    }

    IEnumerator DisableFX()
    {
        FXStarted = true;
        yield return new WaitForSeconds(7);
        landFx.SetActive(false);    
    }    

    public void DestroyThisObj()
    {
        Destroy(this.gameObject);
    }
}
