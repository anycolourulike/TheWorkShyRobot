using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
	private CanvasGroup fadeGroup;	
	private float fadeInDuration = 3;
	private bool gameStarted;
	[SerializeField] UnityEvent Music;
	[SerializeField] UnityEvent levelComplete;

	private void Start()
	{		
		fadeGroup = FindObjectOfType<CanvasGroup>();	
		fadeGroup.alpha = 1;
		Music.Invoke();
	}
	private void Update()
	{
		if (Time.time <= fadeInDuration)
		{		
			fadeGroup.alpha = 1 - (Time.time / fadeInDuration);
		}	
		else if (!gameStarted)
		{
			fadeGroup.alpha = 0;
			gameStarted = true;
		}
	}

	public void CompleteLevel()
	{		
		DialogUI.Instance
			.SetTitle("Well Done!")
			.SetMessage("")
			.OnClose(LevelManager.Instance.LoadNextLevel)
			.Show();		
	}

	public void RocketLevelCrash()
	{
		DialogUI.Instance
			.SetTitle("Crashed!")
			.SetMessage("")
			.OnClose(LevelManager.Instance.LoadNextLevel)
			.Show();
	}

	public void RocketLevelComplete()
	{
		GameManager.Instance.playerLives += 3;
		DialogUI.Instance
			.SetTitle("Nice")
			.SetMessage("+3 Lives!")
			.OnClose(LevelManager.Instance.LoadNextLevel)
			.Show();		
	}

	public void ExitScene()
	{
		SaveManager.Instance.Save();
		SceneManager.LoadScene("Menu");
	}
}