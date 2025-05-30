using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class ModelRotation : MonoBehaviour
{
	[Header("Rotation Settings")]
	public PlayerMovement Player;
	
	//Player Orientation Checks
	public bool IsFacingRight { get; private set; }
	public bool IsPlatformer;
	public bool IsFacingUp { get; private set; }
	public float rotationSpeed = 10f; // Speed of rotation adjustments
	public Vector3 LPlatform = new Vector3(0, 90, 0);  // First rotation state
	public Vector3 RPlatform = new Vector3(0, -90, 0); // Second rotation state

	public Vector3 UTopDown = new Vector3(90, 90, -90);  // First rotation state
	public Vector3 DTopDown = new Vector3(270, 90, -90);  // First rotation state
	public Vector3 LTopDown = new Vector3(0, 90, -90);  // First rotation state
	public Vector3 RTopDown = new Vector3(180, 90, -90);  // First rotation state
	private bool isRotationA = true;  // Tracks current rotation state
	/*
	XYZ
	PLATFORMER
	LEFT 0, 90, 0
	RIGHT 0, -90, 0
	TOPDOWN
	UP    90, 90, -90,
	DOWN  270, 90, -90
	LEFT   0, 90, -90
	RIGHT  180, 90, -90

	 */
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {

	}

	// Update is called once per frame
	void Update()
    {
        
    }
	public void PlatformTurn(Vector3 moveInput)
	{
		if(moveInput.x < 0)
		{
			transform.eulerAngles = LPlatform;
			Player.DirectionChange(0);
		}
		else if(moveInput.x > 0)
		{
			transform.eulerAngles = RPlatform;
			Player.DirectionChange(1);
		}
	}
	public void TopDownTurn(Vector3 moveInput)
	{
		if (moveInput.x < 0)
		{
			transform.eulerAngles = LTopDown;
			Player.DirectionChange(0);
		}
		else if (moveInput.x > 0)
		{
			transform.eulerAngles = RTopDown;
			Player.DirectionChange(1);
		}
		else if (moveInput.y < 0)
		{
			transform.eulerAngles = DTopDown;
			Player.DirectionChange(2);
		}
		else if (moveInput.y > 0)
		{
			transform.eulerAngles = UTopDown;
			Player.DirectionChange(3);
		}
	}
	#region ROTATION METHODS
	public void ToggleRotation()
	{
		// Swap between rotationA and rotationB
		isRotationA = !isRotationA;

		Vector3 targetRotation = isRotationA ? LPlatform: RPlatform;
		StartCoroutine(SmoothRotation(targetRotation));
	}
	#endregion

	private System.Collections.IEnumerator SmoothRotation(Vector3 targetRotation)
	{
		Quaternion startRotation = transform.rotation;
		Quaternion endRotation = Quaternion.Euler(targetRotation);

		float elapsedTime = 0f;
		float duration = 1f / rotationSpeed;  // Adjust duration by speed

		while (elapsedTime < duration)
		{
			transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.rotation = endRotation;  // Ensure final rotation is exact
	}
}
