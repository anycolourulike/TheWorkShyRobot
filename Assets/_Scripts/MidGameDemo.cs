using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.SceneManagement;
using UnityEngine.UI;

public class MidGameDemo : MonoBehaviour
{
    [SerializeField] GameObject Img;
    [SerializeField] Button next;
    [SerializeField] Button prev;
    [SerializeField] Button skip;
    LevelManager levelManager;
    public GameObject[] Imgs;
    GameObject nextImg;
    public int numberOfImgs = 0;
    GameObject curImg;    
    int counter = 0;
    Fader fader;

   
    void Start()
    {
        fader = FindObjectOfType<Fader>();
        fader.FadeOutImmediate();
        levelManager = FindObjectOfType<LevelManager>(); 
        fader.FadeIn(2);
        curImg = Img;
        Img.SetActive(true);
    }  

    public void PlayNextImg()
    {
        curImg.SetActive(false);
        NextImg();
 
        if ( curImg == null )
        {
            PlayNextScene();
        }
        else
        {
           curImg.gameObject.SetActive(true); 
        }
    }

    public void PlayPrevImg()
    {
        curImg.SetActive(false);
        PrevImg();
 
        if ( curImg == null )
        {
            return;
        }
        else
        {
            curImg.gameObject.SetActive(true); 
        }
    }

    public void PlayNextScene()
    {
        curImg.SetActive(false);
        fader.FadeOut(1);
        next.gameObject.SetActive(false);
        prev.gameObject.SetActive(false);                
        skip.gameObject.SetActive(false);
        levelManager.LoadNextScene();
    }

    void NextImg()
    {   
        if(counter == numberOfImgs) 
        {
            PlayNextScene();
            curImg = null;
            return;
        }             
        nextImg = Imgs[counter + 1];
        counter ++;
        curImg = nextImg;
    }

    void PrevImg()
    {    
        if(counter == 0)
        {
           counter = 0;
           return;
        }   
        else
        { 
           nextImg = Imgs[counter - 1];
           counter -- ;
           curImg = nextImg;
        }   
    }    
}
