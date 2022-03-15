using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rambler.Saving;
using Rambler.SceneManangement;

namespace Rambler.SceneManagement
{  
    public class SavingWrapper : MonoBehaviour
    {
        GameObject saveButton;
        Button loadButton;
        Button deleteButton;
        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.2f;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }       

        IEnumerator LoadLastScene() 
        {
            //Fader fader = FindObjectOfType<Fader>();
            //fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            //yield return fader.FadeIn(fadeInTime);
        }
        
        public void Save()
        {
            print("Game Saved");
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            print("Game Loaded");
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}