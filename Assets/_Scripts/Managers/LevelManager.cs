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
    //Menu Scenes
    public AssetReference Loading;
    public AssetReference Menu; 
    public AssetReference Intro;  
    //Freighter
    public AssetReference RobotLab; //Seq
    public AssetReference Corridor;
    public AssetReference Corridor1;
    public AssetReference Armoury; //Dial
    public AssetReference Storage; //Seq
    public AssetReference Living;
    public AssetReference Living1;
    public AssetReference EngineRoom; //Seq
    public AssetReference FlightDeck;
    public AssetReference Bridge; //Boss Timbertoes
    //Surface
    public AssetReference Crash; //Dial
    public AssetReference Canyon1;
    public AssetReference Cliff1;
    public AssetReference Cliff2;
    public AssetReference Cliff3; 
    public AssetReference Canyon2; //Seq Ambush
    public AssetReference Canyon3;
    public AssetReference CaveEntrance; //Boss Player dodges 3 cannon seq fixed camera
    //Cave
    public AssetReference Tunnel1;
    public AssetReference TreeShrine; //Dial
    public AssetReference LivingQtrs;
    public AssetReference PortalRoom; //Seq
    public AssetReference Tunnel2;
    public AssetReference Rocker; //Boss Rocker
    //Outro
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
        if(sceneToLoad == CaveEntrance) return;  
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
            sceneToLoad = RobotLab;
            break;

         case 3:
            sceneToLoad = Corridor;
            break;

         case 4:
            sceneToLoad = Armoury;
            break;

         case 5:
            sceneToLoad = FlightDeck;
            break;

         case 6:
            sceneToLoad = Living;
            break;

         case 7:
            sceneToLoad = Living1;
            break;

         case 8:
            sceneToLoad = Corridor1;
            break;

         case 9:
            sceneToLoad = Storage;
            break;

         case 10:
            sceneToLoad = EngineRoom;
            break;

         case 11:
            sceneToLoad = Bridge;
            break;

         case 12:
            sceneToLoad = Crash;
            break;

         case 13:
            sceneToLoad = Canyon1;
             break;

         case 14:
            sceneToLoad = Cliff1;
            break;

         case 15:
            sceneToLoad = Cliff2;
            break;

         case 16:
            sceneToLoad = Cliff3;
            break;

         case 17:
            sceneToLoad = Canyon2;
            break;

         case 18:
            sceneToLoad = Canyon3;
            break;

         case 19:
            sceneToLoad = CaveEntrance;
            break;

         case 20:
            sceneToLoad = Tunnel1;
            break;
    
         case 21:
            sceneToLoad = TreeShrine;
            break;

         case 22:
            sceneToLoad = LivingQtrs;
            break;

         case 23:
            sceneToLoad = PortalRoom;
            break;

         case 24:
             sceneToLoad = Tunnel2;
            break;

         case 25:
            sceneToLoad = Rocker;
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
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
                break;

            case 3:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
                break;

            case 4:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);
                break;

            case 5:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);
                break;

            case 6:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
                break;

            case 7:
                 AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
                 break;

            case 8:
                 AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);
                 break;

            case 9:
                 AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);
                 break;

            case 10:
                 AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);
                 break;

            case 11:
                 AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);
                 break;

            case 12:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 13:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 14:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 15:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 16:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 17:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 18:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 19:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);
                break;

            case 20:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);
                break;

            case 21:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);
                break;

            case 22:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);
                break;

            case 23:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);
                break;

            case 24:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);
                break;

            case 25:
                AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);
                break;

        }        
    }
}  