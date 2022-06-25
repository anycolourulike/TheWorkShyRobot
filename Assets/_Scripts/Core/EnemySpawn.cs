using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Control;


namespace Rambler.Core
{
    public class EnemySpawn : MonoBehaviour
    {
        public static int count;
        public int npcCounter;
        public int waveCount = 0;
        public GameObject[] waves;  

        void Start()
        {
            count = 0;
        }   

        void Update()
        {
            if(count == npcCounter)
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
    }      
}
