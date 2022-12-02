using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{

    [SerializeField] GameObject landFx;    


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "plane")
        {
            landFx.SetActive(true);
            StartCoroutine("DisableFX");
        }

        if (other.tag == "Player")
        {
            landFx.SetActive(true);
            StartCoroutine("DisableFX");
        }
    }

    IEnumerator DisableFX()
    {
        yield return new WaitForSeconds(7);
        landFx.SetActive(false);
    }

    //boulder disentagrates when hits player
}
