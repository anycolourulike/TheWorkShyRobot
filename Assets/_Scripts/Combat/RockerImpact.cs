using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockerImpact : MonoBehaviour
{
    [SerializeField] GameObject impactFX;
    [SerializeField] GameObject bounceFX;
    [SerializeField] GameObject boulderShards;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //SFX
            meshRenderer.enabled = false;
            Instantiate(impactFX, this.transform.position, Quaternion.identity);
            Instantiate(boulderShards, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }

        if(other.CompareTag("plane"))
        {
            Vector3 pointOfIntersection = other.ClosestPoint(transform.position);
            Instantiate(bounceFX, pointOfIntersection, Quaternion.identity);
        }
    }

}
