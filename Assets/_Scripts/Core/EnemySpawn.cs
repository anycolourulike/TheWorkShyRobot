using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Control;
using Rambler.Attributes;


namespace Rambler.Core
{
    public class EnemySpawn : MonoBehaviour
    {         
        public static int count; 
        public int waveCount = 0;
        public GameObject[] waves;  
        public int numberOfAIKilled;
        public int aIKilledToShowText;
        public int npcCounterForNextWave;
        [SerializeField] GameObject endLevelTxt;
        public List<GameObject> portals = new List<GameObject>();

        void OnEnable()
        {
            Health.targetDeath += UpdateTarget;
            LevelManager.disablePortal += SetFXInactive;
        }

        void OnDisable()
        {
            Health.targetDeath -= UpdateTarget;
            LevelManager.disablePortal -= SetFXInactive;
        }

        void Start()
        {
            count = 0;
        }   

        void Update()
        {
            if(aIKilledToShowText == numberOfAIKilled)
            { 
                endLevelTxt.SetActive(true);
                SetFXActive();  
            }
            if(count == npcCounterForNextWave)
            {                  
                for(int i = waveCount; i < waves.Length;)
                {
                  waves[i].SetActive(true);
                  waveCount ++;
                  count = 0;
                  break;     
                }                
            }
        }

        public void SetFXInactive()
        {
            GameObject[] taggedPortals = GameObject.FindGameObjectsWithTag("Portal");
            foreach(GameObject obj in taggedPortals)
            {
                portals.Add(obj);
                foreach( GameObject taggedObj in portals)
                {
                    taggedObj.SetActive(false);
                }
            }
        }

        void SetFXActive()
        {            
            foreach (GameObject taggedobj in portals)
            {
                if (taggedobj != null)
                {
                    taggedobj.SetActive(true);
                }
            }
            
        }
        
        void UpdateTarget()
        {
           numberOfAIKilled++;
        }
    }      
}
