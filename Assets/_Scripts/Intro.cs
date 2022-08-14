using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Animator anim1;
    [SerializeField] Animator anim2;
    [SerializeField] GameObject Img;
    [SerializeField] Button next;
    [SerializeField] Button prev;
    [SerializeField] Button skip;
    

    LevelManager levelManager;
    public GameObject[] Imgs;
    GameObject nextImg;
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
        StartCoroutine("PlayImg");
    }    
        

    IEnumerator PlayImg()
    {
        anim.SetTrigger("Img"); 
        yield return new WaitForSeconds(2f);
        anim1.SetTrigger("Img");
        yield return new WaitForSeconds(0.7f);
        anim2.SetTrigger("Img");
        yield return new WaitForSeconds(1f);
        next.gameObject.SetActive(true);
        prev.gameObject.SetActive(true);
        skip.gameObject.SetActive(true);
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
            
        }
        else
        {
            curImg.gameObject.SetActive(true); 
        }
    }

    public void PlayNextScene()
    {
        if(curImg != null)
        {
          curImg.SetActive(false);
        }  
        fader.FadeOut(1);
        next.gameObject.SetActive(false);
        prev.gameObject.SetActive(false);                
        skip.gameObject.SetActive(false);
        levelManager.StartCoroutine("LoadCaveLevel");
    }

    void NextImg()
    {   
        if(counter == 7) 
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
