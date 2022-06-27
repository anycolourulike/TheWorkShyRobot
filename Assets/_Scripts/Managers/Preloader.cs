using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeGroup;
    float minLogoTime = 3f;
    float loadTime;   


    void Start() 
    {
        fadeGroup.alpha = 1;

        if(Time.time < minLogoTime)
           loadTime = minLogoTime;
        else
            loadTime = Time.time;   
    }

    void Update() 
    {
        if(Time.time < minLogoTime)
        {
            fadeGroup.alpha = 1 - Time.time;
        }

        if(Time.time > minLogoTime && loadTime != 0)
        {
            fadeGroup.alpha = Time.time - minLogoTime;
            if(fadeGroup.alpha >= 1)
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
