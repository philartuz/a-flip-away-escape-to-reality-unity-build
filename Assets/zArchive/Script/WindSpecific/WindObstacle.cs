	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindObstacle : MonoBehaviour
{
	public CharacterController controller;
	public float moveSpeed;
	public float specificZPosition = 0f;
    public float setInBgZPosition;
    public float gravityScale;
	public bool isGravityY = true;
	public Vector3 moveDirection;
	public string winddirection = "";
	public float hwindSpeed;
	public float vwindSpeed;
	public bool isHitByHWind;
	public bool isHitByVWind;
	public bool umbrellaActive = false;
	public bool controlleractivity = true;
	public bool BGStay;
	float ygrav;

	public void activeController(bool activity)
	{
		controller.enabled = activity;
		controlleractivity = activity;
	}

	public void SetInBG()
	{
		specificZPosition = setInBgZPosition;
		Vector3 position = transform.position;
		position.z = specificZPosition;
		transform.position = position;
	}
	public void WindAssign(string wind)
	{
		winddirection = wind;
		if (!umbrellaActive)
		{
			if (wind == "left")
			{
				isHitByHWind = true;
				hwindSpeed = -10f;
			}
			else if (wind == "right")
			{
				isHitByHWind = true;
				hwindSpeed = 10f;
			}
			else if (wind == "down")
			{
				isHitByHWind = true;
				vwindSpeed = -10f;
			}
			else if (wind == "up")
			{
				isHitByVWind = true;
				vwindSpeed = 10f;
			}
		}
	}
	
	public void NotHitByVWind()
	{
		isHitByVWind = false;
		vwindSpeed = 0f;
	}
	public void NotHitByHWind()
	{
		isHitByHWind = false;
		hwindSpeed = 0f;
	}

	// Start is called before the first frame update
	void Start()
    {
		BGStay = false;
		ygrav = 0f;
		isHitByVWind = false;
		isHitByHWind = false;
		controller = GetComponent<CharacterController>();
		gravityScale = 9.8f;
	}

	// Update is called once per frame
	void Update()
	{
		// Toggle umbrella state with key (for example, the "U" key)
		if (Input.GetKeyDown(KeyCode.U))
		{
			umbrellaActive = !umbrellaActive;
			Debug.Log("Umbrella is " + (umbrellaActive ? "active" : "inactive"));
			vwindSpeed = 0f;
			hwindSpeed = 0f;
		}
		// Handle switching gravity direction
		if (Input.GetKeyDown(KeyCode.Q))
		{

			isGravityY = !isGravityY;
			if (isGravityY)
			{
				Debug.Log("Top Down");
			}
			else
			{
				Debug.Log("Platformer");
			}
			// Reset moveDirection to prevent residual velocity from affecting the switch
			moveDirection = Vector3.zero;
		}
		// Rotate the player GameObject based on current gravity direction
		if (controlleractivity)
		{
			if (isGravityY)
			{
				// Movement input remains the same
				transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

				if (controller.isGrounded)
				{
					moveDirection.y = 0;

				}

				if (winddirection == "up" && isHitByVWind)
				{
					ygrav = 10f;
				}
				else
				{
					ygrav = -10f;
				}
				moveDirection = new Vector3(hwindSpeed, ygrav + vwindSpeed, moveDirection.z);

				// Checks if player is touching the ground

			}
			else
			{
				moveDirection = new Vector3(hwindSpeed, vwindSpeed, moveDirection.z);

				// Adds Gravity
				moveDirection.z -= Physics.gravity.y * gravityScale * Time.deltaTime;

			}

			// Move the controller based on current gravity direction
			controller.Move(moveDirection * Time.deltaTime);
			if (isGravityY)
			{
				Vector3 position = transform.position;
				position.z = specificZPosition;
				transform.position = position;
			}
		}
	}
}
