using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Rambler.SceneManagement;
using Rambler.Control;

namespace Rambler.SceneManagement 
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;
        LevelManager levelManager;    

        GameObject player;
        GameObject newPlayer;  
        PlayerController playerController; 

        void Start() 
        {
            levelManager = FindObjectOfType<LevelManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }
        // add BuddyCrew to positioning
        private IEnumerator Transition()
        {

            if(sceneToLoad < 0)
            {
                Debug.LogError("Scene To Load not Set");
                yield break;
            }
           
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            GameObject HUD = GameObject.FindWithTag("HUD");
            HUD.SetActive(false); 

            List<GameObject> playersList = new List<GameObject>();
            playersList.AddRange(collection: GameObject.FindGameObjectsWithTag("Player"));

            foreach(var Item in playersList)
            {
                if(Item.name == "Rambler")
                {
                    player = Item;
                    playerController = player.GetComponent<PlayerController>(); 
                }
            }         
            
            playerController.enabled = false;
            yield return fader.FadeOut(fadeOutTime);            
            
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);  
            PlayerController newPlayerController = GameObject.Find("/PlayerCore/Rambler").GetComponent<PlayerController>();          
            newPlayerController.enabled = false; 
            wrapper.Load();                

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal: otherPortal); 

            wrapper.Save();
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);            
            newPlayerController.enabled = true;

            Destroy(gameObject);
        }

           

        private void UpdatePlayer(Portal otherPortal)
        {
            List<GameObject> newPlayersList = new List<GameObject>();            
            newPlayersList.AddRange(collection: GameObject.FindGameObjectsWithTag("Player"));
                        
            foreach(var Item in newPlayersList)
            {
                if(Item.name == "Rambler")
                {
                    newPlayer = Item;
                }
            } 

            var navMeshAgent = newPlayer.GetComponent<NavMeshAgent>();           
            navMeshAgent.enabled = false;
            newPlayer.transform.position = otherPortal.spawnPoint.position;
            newPlayer.transform.rotation = otherPortal.spawnPoint.rotation;
            navMeshAgent.enabled = true;
            
        }

        private Portal GetOtherPortal()
        {           
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                Debug.Log(portal.name);
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;                
            }
            
            return null;
        }
    }
}
