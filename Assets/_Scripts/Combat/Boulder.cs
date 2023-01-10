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

    //Find land point and instantiate landFX
    public void OnTriggerEnter(Collider other)
    {
        Vector3 pointOfIntersection = other.ClosestPoint(transform.position);

        if (other.CompareTag("plane"))
        {
            InstantiateLandFX(pointOfIntersection);
        }
        else if (other.CompareTag("Player"))
        {
            InstantiateLandFX(pointOfIntersection);
        } 
    }


    void InstantiateLandFX(Vector3 FXPoint)
    {
        Instantiate(landFx, FXPoint, Quaternion.identity);
    }

    //Add DisableFX Script to landFX object
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
