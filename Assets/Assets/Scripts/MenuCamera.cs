using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
	private Vector3 startPositions;
	private Quaternion startRotation;

	private Vector3 desiredPosition;
	private Quaternion desiredRotation;

	public Transform levelWaypoint;

	private void Start()
	{
		startPositions = desiredPosition = transform.localPosition;
		startRotation = desiredRotation = transform.localRotation;
	}

	private void Update()
	{
		transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, 0.1f);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, 0.1f);
	}

	public void BackToMainMenu()
	{
		desiredPosition = startPositions;
		desiredRotation = startRotation;
	}	

	public void MoveToLevel()
	{
		desiredPosition = levelWaypoint.localPosition;
		desiredRotation = levelWaypoint.localRotation;
	}
}