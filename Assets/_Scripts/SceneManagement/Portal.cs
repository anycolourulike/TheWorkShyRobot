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
        public enum DestinationIdentifier
        {
            A, B, C, D, E
        }        

        [SerializeField] Transform companionSpawnPoint;
        [SerializeField] float fadeOutTime = 2f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 2f;

        PlayerController playerController;
        LevelManager levelManager;       
        
        public DestinationIdentifier destination;
        public Transform spawnPoint;
        public GameObject rambler;
        public int sceneRef;
        public int introNum;


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
            levelManager.destinationID = destination;
            levelManager.sceneRef = sceneRef;
            levelManager.introNum = introNum;

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            fader.FadeOut(fadeOutTime); 
            wrapper.Save();
           
            yield return levelManager.StartCoroutine("LoadLoading"); 
            Destroy(gameObject);
        }
    }
}
