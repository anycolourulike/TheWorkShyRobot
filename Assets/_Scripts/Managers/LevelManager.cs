using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rambler.Core;
using Rambler.SceneManagement;
using Rambler.Combat;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { set; get; }
    public Weapon lastEquippedWeapon;     
    GameObject companionSpawnPoint;
    GameObject playerSpawnPoint;
    public string targetScene;
    GameObject companion;
    GameObject player;
    Fighter fighter;
    Fader fader;

    void OnEnable() 
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() 
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void Awake()
    {        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    } 

    public IEnumerator LoadCaveLevel() 
    {   
       targetScene = "Cave";
       yield return new WaitForSecondsRealtime(3);
       SceneManager.LoadScene("Cave"); 
    }

    public IEnumerator LoadLivingQuarters() 
    {   
       targetScene = "LivingQuarters";
       yield return new WaitForSecondsRealtime(3);
       SceneManager.LoadScene("LivingQuarters"); 
    }

    public IEnumerator LoadIntro() 
    { 
       SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
       wrapper.Delete();
       yield return new WaitForSecondsRealtime(3);
       SceneManager.LoadScene("IntroComic"); 
    }

    public IEnumerator LoadMenu() 
    { 
       SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
       yield return new WaitForSecondsRealtime(3);
       wrapper.LoadMenu();       
    }

    public IEnumerator LoadSavedGame() 
    {  
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        yield return new WaitForSecondsRealtime(3);
        wrapper.ContinueGame();   
    } 

    public IEnumerator LoadNextScene() 
    {   
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        yield return new WaitForSecondsRealtime(3);
        wrapper.Save();
        var currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene + 1);  
    }    

    public IEnumerator QuitApp()
    {   
        yield return new WaitForSecondsRealtime(3);     
        Application.Quit();
    } 

    public void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        fader = FindObjectOfType<Fader>();
        fader.FadeIn(3);
        AmbientMusic();
        Time.timeScale = 1;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if(sceneIndex > 3)
        {
            if(sceneIndex == 6) return;
            if(sceneIndex == 7)
            {
                playerSpawnPoint = GameObject.FindWithTag("PlayerSpawn");
                companionSpawnPoint = GameObject.FindWithTag("CompanionSpawn");
            }
            PlayerAssignWeapons();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>(); 
            wrapper.Load(); 
            if(sceneIndex == 7) {PlayerStartPosition();}

            if(lastEquippedWeapon != null)
            {
               fighter.EquipWeapon(lastEquippedWeapon);
            }  
            if(lastEquippedWeapon == null)
            {
               fighter.EquipUnarmed();
            }
        } 
    } 

    public void PlayerWeaponCheck()
    {  
        var player = GameObject.Find("PlayerCore/Rambler");
        fighter = player.GetComponent<Fighter>();
        lastEquippedWeapon = fighter.weaponConfig;           
    } 

    void PlayerAssignWeapons()
    {
        var player = GameObject.Find("PlayerCore/Rambler");
        fighter = player.GetComponent<Fighter>();
        fighter.weaponConfig = lastEquippedWeapon;                
    }   

    void PlayerStartPosition()
    {  
        player = GameObject.Find("/PlayerCore/Rambler");
        companion = GameObject.Find("/Companion");
        var navMeshAgent = player.GetComponent<NavMeshAgent>();
        var navMeshAgentCompanion = companion.GetComponent<NavMeshAgent>(); 
        navMeshAgent.enabled = false;
        navMeshAgentCompanion.enabled = false;
        player.transform.position = playerSpawnPoint.transform.position;
        player.transform.rotation = playerSpawnPoint.transform.rotation;
        companion.transform.position = companionSpawnPoint.transform.position;
        companion.transform.rotation = companionSpawnPoint.transform.rotation;
        navMeshAgent.enabled = true;
        navMeshAgentCompanion.enabled = true;   
    }

    void AmbientMusic()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        switch(sceneIndex)
        {
            case 1:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.ambientMusic);                           
            break;

            case 3:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.CaveBackground);                           
            break;

            case 4:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
            break;

            case 5:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.SurfaceBackground);                           
            break;

            case 7:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
            break;

            case 8:
            AudioManager.PlayAmbientSound(AudioManager.AmbientSound.RigBackground);                           
            break;
        }  
    }
}  