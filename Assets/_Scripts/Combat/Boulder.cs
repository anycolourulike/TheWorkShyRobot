using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Rambler.Control;

public class Boulder : MonoBehaviour
{

    [SerializeField] GameObject landFx;
    Rigidbody rb;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("plane"))
        {
            landFx.SetActive(true);
        }

        if (other.CompareTag("Player"))
        {
            landFx.SetActive(true);
        }
        StartCoroutine("DisableFX");
    }

    IEnumerator DisableFX()
    {
        yield return new WaitForSeconds(7);
        landFx.SetActive(false);
        rb.constraints = RigidbodyConstraints.FreezePosition;
        AIController.rocksHaveLanded.Invoke();
    }

    public void DestroyThisObj()
    {
        Destroy(this.gameObject);
    }
}
