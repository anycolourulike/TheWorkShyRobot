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

public class Loading : MonoBehaviour
{    
   AsyncOperationHandle<SceneInstance> handle;   
   [SerializeField] float fadeInTime = 2f;
   [SerializeField] float fadeOutTime = 2f;
   public AssetReference sceneToLoad;
   SavingWrapper wrapper;
   Fader fader;

   void Start()
   {
      fader = FindObjectOfType<Fader>();
      fader.FadeIn(fadeInTime);
      sceneToLoad = LevelManager.Instance.sceneToLoad;
      StartCoroutine("LoadScene");
   }

   public IEnumerator LoadScene() 
   { 
      fader.FadeOut(fadeOutTime); 
      yield return new WaitForSeconds(4);
      Addressables.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single).Completed += OnSceneLoadComplete; 
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
           LevelManager.Instance.OnLevelFinishedLoading();
         }
      };
   } 
}
