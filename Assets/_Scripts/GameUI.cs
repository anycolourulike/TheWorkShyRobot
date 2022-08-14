using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rambler.Saving;
using Rambler.SceneManagement;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour
{
    LevelManager levelManager;
    

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>(); 
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void LoadSavedGame() 
    {
        levelManager.PlayerWeaponCheck();
        levelManager.StartCoroutine("LoadSavedGame");   
    }

    public void LoadMenu() 
    {
        levelManager.StartCoroutine("LoadMenu");
    }

    public void QuitApp()
    {
        levelManager.StartCoroutine("QuitApp");        
    } 
}
