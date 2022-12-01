using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateFirstWave : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject[] enemy;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        foreach(var obj in enemy)
        {
            obj.SetActive(true);
        }
    }
}
