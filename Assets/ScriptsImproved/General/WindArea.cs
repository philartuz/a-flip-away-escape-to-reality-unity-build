using UnityEngine;

public class WindArea : MonoBehaviour
{
	[Header("Wind Settings")]
	public Vector3 windDirection = Vector3.zero;
	public float windStrength = 7.5f;
	public bool isHorizontalWind = false;
	public bool isVerticalWind = false;
	public Animator windAnimator;
	private AudioManager audioManager;
	private Transform playerTransform;

	[Header("Wind Animation Positions")]
	public Transform idleWindPosition; // Fixed position when no player is inside
	public Vector3 windOffset = new Vector3(0, 2, 0); // Offset when following player

	[Header("Z Positions for Different Directions")]
	public float zFront = -1f;
	public float zBack = 1f;
	public float zLeft = -2f;
	public float zRight = -5f;

	private bool playerInWindZone = false; // Track if player is inside

	private void Start()
	{
		if (windAnimator == null)
		{
			windAnimator = GetComponent<Animator>();
		}

		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

		// Ensure wind is playing from the start
		windAnimator.SetBool("isWindDefault", true);
	}

	private void Update()
	{
		Vector3 targetPosition;

		if (playerInWindZone && playerTransform != null)
		{
			// Follow the player when inside the wind area
			targetPosition = playerTransform.position + windOffset;
		}
		else
		{
			// Move to fixed idle position when no player is inside
			targetPosition = idleWindPosition.position;
		}

		// Adjust Z position based on wind direction
		if (windDirection.x > 0)
		{
			targetPosition.z = zRight;
		}
		else if (windDirection.x < 0)
		{
			targetPosition.z = zLeft;
		}
		else if (windDirection.y > 0)
		{
			targetPosition.z = zBack;
		}
		else if (windDirection.y < 0)
		{
			targetPosition.z = zFront;
		}

		// Move the wind animation smoothly
		windAnimator.transform.position = Vector3.Lerp(
			windAnimator.transform.position,
			targetPosition,
			Time.deltaTime * 5f
		);
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovement player = other.GetComponent<PlayerMovement>();
			if (player != null)
			{
				playerTransform = player.transform;
				player.EnterWindZone(this);
				playerInWindZone = true;
				HandleWindAnimation(player);
				audioManager.StartWind();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerMovement player = other.GetComponent<PlayerMovement>();
			if (player != null)
			{
				player.ExitWindZone();
			}
		}

		playerInWindZone = false;
		ResetWindAnimation();
		audioManager.StopWind();
		playerTransform = null;
	}

	private void HandleWindAnimation(PlayerMovement player)
	{
		if (player.isUmbrellaEquipped() && player.umbrelladirection == windDirection)
		{
			windAnimator.SetBool("isWindDefault", false);
			windAnimator.SetBool("isWindAgainst", false);
		}
		else
		{
			float windDir = windDirection.x;
			bool isAgainstWind = (windDir > 0 && player.getMoveInput().x < 0) || (windDir < 0 && player.getMoveInput().x > 0);

			windAnimator.SetBool("isWindDefault", !isAgainstWind);
			windAnimator.SetBool("isWindAgainst", isAgainstWind);
		}
	}

	private void ResetWindAnimation()
	{
		windAnimator.SetBool("isWindDefault", true); // Keep wind running
		windAnimator.SetBool("isWindAgainst", false);
	}
}
