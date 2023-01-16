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

    //add damage to player if hit


    void InstantiateLandFX(Vector3 FXPoint)
    {
        Instantiate(landFx, FXPoint, Quaternion.identity);
    }
    
    public void DestroyThisObj()
    {
        Destroy(this.gameObject);
    }
}
