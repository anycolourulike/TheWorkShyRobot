using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rambler.Core 
{  
  
  public class EnemyDeathCounter : MonoBehaviour
  {
    public int enemyWaveCount;
    public Collider col;
    public int numberToTriggerDialogue;

    void Start()
    {
      col.enabled = false;  
    }

    void Update()
    {
      var enemiesKilled = EnemySpawn.count;
      if(enemiesKilled == 3)
      {
        enemyWaveCount ++;
      }   
      if(enemyWaveCount == numberToTriggerDialogue)
      {
        col.enabled = true;
      } 
    }
  }  

}
