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
    float minLogoTime = 3f;    
    GameObject loadingObj;
    Fader fader;
    
    bool loadingScene;    
    float loadTime;
    int scene;

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

    void Update() 
    {  
        var thisScene = SceneManager.GetActiveScene();
        if(thisScene == SceneManager.GetSceneByBuildIndex(1)) return;       
        
    }    

    public void LoadCaveLevel() 
    {   
       targetScene = "Cave";
       LoadingData.sceneToLoad = targetScene;
       SceneManager.LoadScene("Loading"); 
       
    }

    public void QuitApp()
    {
        Application.Quit();
    } 

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        AmbientMusic(); 
        if(scene == SceneManager.GetSceneByBuildIndex(1)) return;
        if(scene == SceneManager.GetSceneByName("Loading")) return;
        FindUI(); 
        fader.StartCoroutine("FadeIn", 3f);        
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

    void FindUI()
    {
        var Fader = GameObject.Find("/PlayerCore/HUD/Fader");
        fader = Fader.GetComponent<Fader>();
        loadingObj = GameObject.Find("/Canvas/LoadingRed");        
    }

    void LoadNewScene() 
    {  
        LoadingData.sceneToLoad = targetScene;
        SceneManager.LoadScene("Loading"); 
    }
}
