using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Saving;
using Rambler.SceneManagement;
using Rambler.Core;

public class MenuScene : MonoBehaviour
{
    LevelManager levelManager;
    [SerializeField] GameObject mainCam;
    Fader fader;
    

    void Start()
    {        
        fader = FindObjectOfType<Fader>();
        fader.FadeOutImmediate();
        levelManager = FindObjectOfType<LevelManager>(); 
        fader.FadeIn(3);
    }  

    public void LoadNextLevel()
    {
        FindMusic();
        fader.FadeOut(3);
        levelManager.StartCoroutine("LoadIntro");
    }

    public void LoadSavedGame() 
    {
        FindMusic();
        fader.FadeOut(3);
        levelManager.StartCoroutine("LoadSavedGame");
    }

    public void QuitApp()
    {
        FindMusic();
        fader.FadeOut(3);
        levelManager.StartCoroutine("QuitApp");
    } 

    void FindMusic()
    {
        var music = GameObject.Find("AmbientSFX");
        music.SetActive(false); 
        AudioManager.PlayHumanSound(AudioManager.HumanSound.Death4, mainCam.transform.position);
    }       
}
