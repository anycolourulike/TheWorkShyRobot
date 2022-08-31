﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rambler.Saving;
using Rambler.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Rambler.SceneManagement
{  
    public class SavingWrapper : MonoBehaviour
    {
        const string currentSaveKey = "currentSaveName";
        int menuLevelBuildIndex = 0;        
        float fadeOutTime = 4f;
        float fadeInTime = 4f;
        

        public void ContinueGame() 
        {
            StartCoroutine("LoadLastScene");
        }

        public void LoadMenu()
        {
            StartCoroutine("LoadMenuScene");
        }

        private IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return GetComponent<SavingSystem>().LoadLastScene(currentSaveKey);
        }

        private IEnumerator LoadFirstScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);            
            LevelManager.Instance.sceneRef = 2;
            LevelManager.Instance.StartCoroutine("LoadLoading");
        }

        private IEnumerator LoadMenuScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            LevelManager.Instance.sceneRef = 0;
            LevelManager.Instance.StartCoroutine("LoadLoading");
        }
        
        public void Save()
        {
            print("Game Saved");
            GetComponent<SavingSystem>().Save(currentSaveKey);
        }

        public void Load()
        {
            print("Game Loaded");
            GetComponent<SavingSystem>().Load(currentSaveKey);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(currentSaveKey);
        }

        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }
    }
}