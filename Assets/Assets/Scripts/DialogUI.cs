using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Dialog
{
		public string Title = "Title";
		public string Message = "Message goes here.";
		public string ButtonText = "Continue";
		public float FadeInDuration = .3f;		
		public UnityAction OnClose = null;
}

public class DialogUI : MonoBehaviour
{
	[SerializeField] GameObject canvas;
	[SerializeField] TextMeshProUGUI titleUIText;
	[SerializeField] TextMeshProUGUI messageUIText;
	[SerializeField] Button closeUIButton;

	Image closeUIButtonImage;
	Text closeUIButtonText;
	CanvasGroup canvasGroup;	

	Queue<Dialog> dialogsQueue = new Queue<Dialog>();
	Dialog dialog = new Dialog();
	Dialog tempDialog;

	[HideInInspector] public bool IsActive = false;

	//Singleton pattern
	public static DialogUI Instance;

	void Awake()
	{
		Instance = this;
		closeUIButtonImage = closeUIButton.GetComponent<Image>();
		closeUIButtonText = closeUIButton.GetComponentInChildren<Text>();
		canvasGroup = canvas.GetComponent<CanvasGroup>();

		//Add close event listener
		closeUIButton.onClick.RemoveAllListeners();
		closeUIButton.onClick.AddListener(Hide);
	}

	public DialogUI SetTitle(string title)
	{
		dialog.Title = title;
		return Instance;
	}

	public DialogUI SetMessage(string message)
	{
		dialog.Message = message;
		return Instance;
	}

	public DialogUI SetButtonText(string text)
	{
		dialog.ButtonText = text;
		return Instance;
	}	

	public DialogUI SetFadeInDuration(float duration)
	{
		dialog.FadeInDuration = duration;
		return Instance;
	}

	public DialogUI OnClose(UnityAction action)
	{
		dialog.OnClose = action;
		return Instance;
	}

	//-------------------------------------

	public void Show()
	{
		dialogsQueue.Enqueue(dialog);
		//Reset Dialog
		dialog = new Dialog();

		if (!IsActive)
			ShowNextDialog();
	}


	void ShowNextDialog()
	{
		tempDialog = dialogsQueue.Dequeue();

		titleUIText.text = tempDialog.Title;
		messageUIText.text = tempDialog.Message;
		closeUIButtonText.text = tempDialog.ButtonText.ToUpper();		

		canvas.SetActive(true);
		IsActive = true;
		StartCoroutine(FadeIn(tempDialog.FadeInDuration));
	}


	// Hide dialog
	public void Hide()
	{
		canvas.SetActive(false);
		IsActive = false;

		// Invoke OnClose Event
			if (tempDialog.OnClose != null)
				tempDialog.OnClose.Invoke();

			StopAllCoroutines();

			if (dialogsQueue.Count != 0)
				ShowNextDialog();
		}


	//-------------------------------------

	IEnumerator FadeIn(float duration)
	{
		float startTime = Time.time;
		float alpha = 0f;

		while (alpha < 1f)
		{
			alpha = Mathf.Lerp(0f, 1f, (Time.time - startTime) / duration);
			canvasGroup.alpha = alpha;

			yield return null;
		}
	}
}