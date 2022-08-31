using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Saving;
using Rambler.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Rambler.Core;

public class MenuScene : MonoBehaviour
{    
    [SerializeField] AssetReference sceneToLoad;
    [SerializeField] GameObject mainCam;
    LevelManager levelManager;
    Fader fader;    
    int sceneRef = 1;

    void Start()
    {         
        fader = FindObjectOfType<Fader>();
        fader.FadeOutImmediate();
        levelManager = FindObjectOfType<LevelManager>(); 
        levelManager.OnLevelFinishedLoading();
        fader.FadeIn(3);
    }  

    public void LoadNextLevel()
    {
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        wrapper.Delete();
        FindMusic();
        fader.FadeOut(3);
        levelManager.sceneRef = sceneRef;
        levelManager.StartCoroutine("LoadLoading");  
    }

    public void LoadSavedGame() 
    {
        fader.FadeOut(3);
        FindMusic();         
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();  
        wrapper.ContinueGame(); 
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
