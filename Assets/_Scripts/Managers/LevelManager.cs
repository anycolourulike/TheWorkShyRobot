using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rambler.Core;
using Rambler.SceneManagement;
using Rambler.Combat;

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

    public IEnumerator LoadCaveLevel() 
    {   
       targetScene = "Cave";
       yield return new WaitForSeconds(3);
       SceneManager.LoadScene("Cave"); 
    }

    public IEnumerator LoadIntro() 
    { 
       SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
       yield return new WaitForSeconds(3);
       SceneManager.LoadScene("IntroComic"); 
    }

    public IEnumerator LoadMenu() 
    { 
       SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
       yield return new WaitForSeconds(3);
       wrapper.LoadMenu();       
    }

    public IEnumerator LoadSavedGame() 
    {   
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        yield return new WaitForSeconds(3);
        wrapper.ContinueGame();   
    }    

    public IEnumerator QuitApp()
    {   
        yield return new WaitForSeconds(3);     
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

            case 3:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);                           
            break;

            case 4:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
            break;

            case 5:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
            break;
        }  
    }
}    

    
