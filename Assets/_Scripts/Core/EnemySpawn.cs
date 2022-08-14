using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Control;


namespace Rambler.Core
{
    public class EnemySpawn : MonoBehaviour
    {
        public static int count;
        public int npcCounterForNextWave;
        public int waveCount = 0;
        public GameObject[] waves;  
        public int numberOfAIKilled;
        public int aIKilledToShowText;
        [SerializeField] GameObject endLevelTxt;

        void OnEnable()
        {
            Health.targetDeath += UpdateTarget;
        }

        void OnDisable()
        {
            Health.targetDeath -= UpdateTarget;
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
        
        void UpdateTarget()
        {
           numberOfAIKilled++;
        }
    }      
}
