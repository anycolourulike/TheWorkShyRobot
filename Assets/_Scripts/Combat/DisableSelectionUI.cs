using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSelectionUI : MonoBehaviour
{
    float timeToDisable = 3f;
    float timer;
  

    void Update()
    {
        timer += Time.deltaTime;
        if (timer <= timeToDisable)
        {
            var selectionUI = this.gameObject;
            selectionUI.SetActive(false);
        }
    }
}