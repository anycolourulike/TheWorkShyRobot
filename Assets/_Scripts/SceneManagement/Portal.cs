using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AI;
using Rambler.SceneManagement;
using Rambler.Control;
using Rambler.Combat;

namespace Rambler.SceneManagement 
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] GameObject rambler;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Transform companionSpawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 2f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 2f;
        LevelManager levelManager;       
        PlayerController playerController; 
        public int sceneRef;

        void Start() 
        {
            levelManager = FindObjectOfType<LevelManager>();
        }        

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject ==  rambler)
            {
                StartCoroutine(Transition());
            }
        }
        
        private IEnumerator Transition()
        { 
            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            fader.FadeOut(fadeOutTime); 
            wrapper.Save();
                        
            levelManager.sceneRef = sceneRef;
            yield return levelManager.StartCoroutine("LoadLoading"); 
            Destroy(gameObject);
        }
    }
}
