using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Rambler.Core;
using Rambler.SceneManagement;
using Rambler.Saving;
using Rambler.Combat;
using UnityEngine.AI;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { set; get; }
    AsyncOperationHandle<SceneInstance> handle;
    public AssetReference sceneToLoad;
    public int sceneRef;
    public AssetReference Loading;
    public AssetReference Menu;   
    public AssetReference Intro; 
    public AssetReference Cave; 
    public AssetReference Cliff;
    public AssetReference Rig;
    public AssetReference Inter;
    public AssetReference Living;
    public AssetReference Flight;
    public AssetReference Outro;
    Fader fader;    

    void Awake()
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

    public IEnumerator LoadLoading() 
    {  
       FindAssetPath();
       fader = FindObjectOfType<Fader>();
       fader.FadeOut(3);      
       yield return new WaitForSeconds(2);
       Addressables.LoadSceneAsync(Loading, LoadSceneMode.Single).Completed += OnSceneLoadComplete; 
    } 
    
    public IEnumerator LoadMenu() 
    { 
       SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
       yield return new WaitForSeconds(1);
       wrapper.LoadMenu();       
    }

    public IEnumerator LoadSavedGame() 
    {          
        yield return StartCoroutine("LoadLoading");
    } 

    public IEnumerator LoadSavedFile() 
    {                                  
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        yield return new WaitForSeconds(0.5f);
        wrapper.Load(); 
    }        

    public IEnumerator QuitApp()
    {   
        yield return new WaitForSecondsRealtime(2);     
        Application.Quit();
    }    

    public void OnLevelFinishedLoading()
    {    
        FindAssetPath();     
        System.GC.Collect();                
        fader = FindObjectOfType<Fader>();
        fader.FadeIn(3);
        AmbientMusic();                 
        if(sceneToLoad == Loading) return;
        if(sceneToLoad == Menu) return;
        if(sceneToLoad == Intro) return; 
        if(sceneToLoad == Cave) return;      
        if(sceneToLoad == Inter) return;        
        Time.timeScale = 1; 
        StartCoroutine("LoadSavedFile");
    } 

    void OnSceneLoadComplete(AsyncOperationHandle<SceneInstance>previousScene)
    {  
        if(previousScene.Status == AsyncOperationStatus.Succeeded)
        {
          handle = previousScene;
          UnloadScene();
        }  
    }

    void UnloadScene()
    {   
       Addressables.UnloadSceneAsync(handle, true).Completed += op =>
       {
          if(op.Status == AsyncOperationStatus.Succeeded)
          {                                 
              
          }
       };
    }  

    public void FindAssetPath()
    {
       switch (sceneRef)
       {
         case 0:
         sceneToLoad = Menu;
         break;

         case 1:
         sceneToLoad = Intro;
         break;

         case 2:
         sceneToLoad = Cave;
         break;

         case 3:
         sceneToLoad = Cliff;
         break;

         case 4:
         sceneToLoad = Rig;
         break;

         case 5:
         sceneToLoad = Inter;
         break;

         case 6:
         sceneToLoad = Living;
         break;

         case 7:
         sceneToLoad = Flight;
         break;

         case 8:
         sceneToLoad = Outro;
         break;

         default:
         sceneToLoad = Menu;
         break;
       }       
    }       

    void AmbientMusic()
    {
        switch(sceneRef)
        {
            case 0:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.ambientMusic);                           
            break;

            case 1:                       
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

            case 5:                       
            break;

            case 6:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
            break;

            case 7:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
            break;
        }        
    }
}  