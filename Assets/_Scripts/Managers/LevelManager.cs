using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rambler.Core;
using Rambler.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { set; get; }
    public string targetScene;    
    Fader fader;

    void OnEnable() 
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void Awake()
    {        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    } 

    public void LoadCaveLevel() 
    {   
       targetScene = "Cave";
       SceneManager.LoadScene("Cave");        
    }

    public void LoadCliffLevel() 
    {   
       targetScene = "Cliff";
       SceneManager.LoadScene("Cliff");        
    }

    public void LoadRigLevel() 
    {   
       targetScene = "Rig";
       SceneManager.LoadScene("Rig");        
    }

    public void QuitApp()
    {
        Application.Quit();
    } 

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        fader = FindObjectOfType<Fader>();
        fader.FadeIn(3);
        AmbientMusic();
    }

    void AmbientMusic()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        switch(sceneIndex)
        {
            case 1:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.ambientMusic);                           
            break;

            case 2:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);                           
            break;

            case 3:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
            break;

            case 4:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
            break;
        }  
    }
}    

    
