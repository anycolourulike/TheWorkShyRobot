using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rambler.Core 
{
    public class GameManager : MonoBehaviour
    {
        
    
        void Start()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            switch(sceneIndex)
            {
              case 0:
              AudioManager.PlayAmbientSound(AudioManager.AmbientSound.ambientMusic);                           
              break;
              case 1:
              AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);                           
              break;
              case 2:
              AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
              break;
              case 3:
              AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
              break;
            }
        }
    }
}
