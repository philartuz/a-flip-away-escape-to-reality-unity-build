using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed;
	public float jumpForce;
	public CharacterController controller;
	public GameObject player;
	private Vector3 moveDirection;
	public float gravityScale;
	public bool isGravityY = true; // Tracks if gravity is applied in Y direction
	public float specificZPosition = 0f;
	public float grabmultiplier;

	public string winddirection = "";
	public float hwindSpeed;
	public float vwindSpeed;
	public bool isHitByHWind;
	public bool isHitByVWind;
	public bool grabbing;
	public bool umbrellaActive = false;

	public bool getIsGravityY()
	{
		return isGravityY;
	}
	public void FlipGravity()
	{
		isGravityY = !isGravityY;
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

	public bool Grabbing()
	{
		return grabbing;
	}

	public void Grab()
	{
		grabbing = true;
	}
	public void NotGrab()
	{
		grabbing = false;
	}
	// Start is called before the first frame update
	void Start()
	{
		grabbing = false;
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

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (Input.GetKeyDown(KeyCode.Q) && currentSceneName != "World1_0_A" && currentSceneName != "World1_1_A")
		{
			isGravityY = !isGravityY;
			if (isGravityY)
			{
				Debug.Log("Platformer");
			}
			else
			{
				Debug.Log("Top Down");
			}
			// Reset moveDirection to prevent residual velocity from affecting the switch
			moveDirection = Vector3.zero;
		}


		// Reset Level
		if (Input.GetKeyDown(KeyCode.R))
		{
			Scene currentScene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(currentScene.name);
		}


        if (grabbing)
		{
			grabmultiplier = 0.5f;
		}
		else
		{
			grabmultiplier = 1f;
		}
		if (isGravityY)
		{
			// Movement input remains the same
			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

			moveDirection = new Vector3(((Input.GetAxis("Horizontal") * moveSpeed) + hwindSpeed) * grabmultiplier, moveDirection.y, moveDirection.z);

			// Checks if player is touching the ground
			if (controller.isGrounded)
			{
				moveDirection.y = 0;

				if (Input.GetButtonDown("Jump"))
				{
					moveDirection.y = jumpForce;
				}
			}
			// Adds Gravity
			if (isHitByVWind && winddirection == "up")
			{
				moveDirection.y = vwindSpeed/2;
			}
			else
			{
				moveDirection.y += Physics.gravity.y * gravityScale * Time.deltaTime;
			}
		}
		else
		{
			moveDirection = new Vector3(((Input.GetAxis("Horizontal") * moveSpeed) + hwindSpeed) * grabmultiplier, ((Input.GetAxis("Vertical") * moveSpeed) + vwindSpeed) * grabmultiplier, moveDirection.z);

			// Checks if player is touching the ground
			//if (controller.isGrounded)
			//{
			//moveDirection.z = 0;
			

			if (Input.GetButtonDown("Jump"))
			{
				moveDirection.z = -jumpForce; // Adjusted to jump against Z-axis gravity
			}
			//}
			if (Input.GetKeyDown(KeyCode.A))
			{
				player.transform.localScale = new Vector3(1.0f, 1f, 1.0f);
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				player.transform.localScale = new Vector3(1.0f, 1f, 1.0f);
			}
			// Adds Gravity
			moveDirection.z -= Physics.gravity.y * gravityScale * Time.deltaTime;
			if (moveDirection.z > 100.0f) moveDirection.z = 0.0f;
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
