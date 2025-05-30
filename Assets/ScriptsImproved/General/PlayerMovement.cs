using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
	//RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	//Component and Script References
	[Header("References")]
	public Rigidbody RB { get; private set; }
	public ModelRotation Model;
	public GrabbingMechanic grab;
	private WindArea activeWindZone = null;
	public BoxCollider box;
    [Header("Equipment GameObjects")]
	public GameObject powerGloves;  // Reference to the PowerGloves GameObject
	public GameObject umbrella;

    #region Basic Variables
    //Player Orientation Checks
    public bool IsPlatformer;
	public enum Direction
	{
		Left = 0,
		Right = 1,
		Down = 2,
		Up = 3
	}
	public Direction direction;
	
	// wont use anymore
	public bool IsFacingRight { get; private set; }
	public bool IsFacingUp { get; private set; }

	//Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime;
	public float LastPressedJumpTime;

	//Jump
	public bool IsJumping;
	public bool IsJumpFalling;

	//Movement Input
	private Vector3 moveInput;

	//Manual Gravity
	public float globalGravity = -9.81f;
	//Set all of these up in the inspector
	[Header("Checks")]
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector3 _groundCheckSize = new Vector3(1.0f, 0.03f, 1.0f);
	[Space(5)]
	[SerializeField] private Transform _leftWallCheck;
	[SerializeField] private Transform _rightWallCheck;
	[SerializeField] private Transform _topWallCheck;
	[SerializeField] private Transform _downWallCheck;
	[SerializeField] private Vector3 _wallCheckSize = new Vector3(0.1f, 5.0f, 0.1f);

	[Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	#endregion

	#region Adjustable Variables


	[Header("Gravity")]
	[HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
	[HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).

	public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
	public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
	[Space(5)]
	public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
									  //Seen in games such as Celeste, lets the player fall extra fast if they wish.
	public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

	[Space(20)]

	[Header("Run")]
	public float runMaxSpeed; //Target speed we want the player to reach.
	public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
	private float currentSpeed = 0f; // Tracks the player's current horizontal speed
	private Vector2 tpcurrentSpeed = Vector2.zero;
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	[Space(5)]
	[Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	[Range(0f, 1)] public float deccelInAir;
	[Space(5)]
	public bool doConserveMomentum = true;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; //Height of the player's jump
	public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
	[HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.

	[Header("Both Jumps")]
	public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
	[Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
	public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	[Space(0.5f)]
	public float jumpHangAccelerationMult;
	public float jumpHangMaxSpeedMult;

	[Space(20)]

	[Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Space(20)]

	[Header("Rotation Settings")]
	public float rotationSpeed = 10f; // Speed of rotation adjustments
	public Vector3 rotationA = new Vector3(0, 0, 0);  // First rotation state
	public Vector3 rotationB = new Vector3(-90, 0, 0); // Second rotation state
	private bool isRotationA = true;  // Tracks current rotation state
	#endregion

	#region Game Mechanics Variables
	[Header("Game Mechanics")]
	//Grabbing Mechanics
	

	//Wind Mechanics
	[SerializeField] private float windspeedaddition;
	[SerializeField] private float windacceladdtion;

	public bool umbrellaEquipped = false;
	public Vector3 umbrelladirection;

	public enum Equipment
	{
		None,
		PowerGloves,
		Umbrella
	}
	[Header("Equipment")]
	public Equipment currentEquipment = Equipment.None;
	private bool isEquipped = false;
	private bool isUmbrellaAnchored = false;
	private bool isFrozen = false;
	private float freezeTimer = 0f;

	[Header("Grabbing Movement Modifiers")]
	public float grabRunMaxSpeed = 2f;       // Reduced max speed when grabbing
	public float grabRunAcceleration = 1f;  // Slower acceleration when grabbing
	public float grabRunDecceleration = 2f; // Slower deceleration when grabbing

	#endregion
	

	[Header("Animations")]
	public Animator characterAnimator;
    public Animator windAnimator;
	public Animator umbrellaAnimator;
	public Animator umbrellaBuildAnimator;
    public GameObject umbrellaVector;
	public GameObject umbrellaObject;
	public GameObject targetAnimatorWind;


    private GameObject characterPlatform;
	private GameObject characterTopDown;

    [Header("Audio Manager")]
	private AudioManager audioManager;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
		SetEquipment(Equipment.Umbrella);
	}


    private void Start()
    {
		IsPlatformer = true;

        //targetAnimatorWind = GameObject.FindGameObjectWithTag("airconVector");
        //windAnimator = targetAnimatorWind.GetComponent<Animator>();


        umbrellaAnimator = umbrellaVector.GetComponent<Animator>();

        umbrellaBuildAnimator = umbrellaObject.GetComponent<Animator>();

		characterPlatform = GameObject.Find("Character");
        characterTopDown = GameObject.Find("CharacterOrange");

		// Start as Top Down instead of Platformer
		if (SceneManager.GetActiveScene().name == "World1_1_A")
		{
			IsPlatformer = false;
			ToggleRotation();
			TopDownConstraints();
			characterPlatform.SetActive(false);
			characterTopDown.SetActive(true);
		}

		else {
            characterPlatform.SetActive(true);
            characterTopDown.SetActive(false);
        }
        
    }


	// Update is called once per frame
	void Update()
	{

		// GameObject targetObjectAnimator = GameObject.Find("Character");
		GameObject targetObjectAnimator = GameObject.FindGameObjectWithTag("Character");
		characterAnimator = targetObjectAnimator.GetComponent<Animator>();

        


        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER
		moveInput.x = Input.GetAxisRaw("Horizontal");
		moveInput.y = Input.GetAxisRaw("Vertical");

		//Character Direction Facing Handling
		if (!grab.getIsGrabbing())
		{
			if (moveInput.x != 0 && IsPlatformer)
			{
				Model.PlatformTurn(moveInput);
			}
			else if ((moveInput.x != 0 || moveInput.y != 0) && !IsPlatformer)
			{
				Model.TopDownTurn(moveInput);
			}
		}

		/*
		if (!IsPlatformer && moveInput.y != 0)
			TopDownCheckVerticalDirectionToFace(moveInput.y > 0);*/

		//Jump
		if (Input.GetKeyDown(KeyCode.Space) && !grab.getIsGrabbing())
		{
			OnJumpInput();
		}

		// State Switcher Handling
		if (Input.GetKeyDown(KeyCode.Q)
			&& SceneManager.GetActiveScene().name != "World1_0_A"
			&& SceneManager.GetActiveScene().name != "World1_1_A")
		{


			ToggleRotation();
			if (IsPlatformer)
			{
				TopDownConstraints();
				characterPlatform.SetActive(false);
				characterTopDown.SetActive(true);

			}
			else
			{
				PlatformerConstraints();
				characterPlatform.SetActive(true);
				characterTopDown.SetActive(false);

			}
			IsPlatformer = !IsPlatformer;
		}

		// Reset button
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		#endregion

		#region COLLISION CHECKS
		if (!IsJumping)
		{
			//Ground Check
			if (Physics.OverlapBox(_groundCheckPoint.position, _groundCheckSize / 2, Quaternion.identity, _groundLayer).Length > 0 && !IsJumping) //checks if set box overlaps with ground
			{
				LastOnGroundTime = coyoteTime; //if so sets the lastGrounded to coyoteTime
			}
		}
		#endregion

		#region JUMP CHECKS
		if (IsPlatformer)
		{
			if (IsJumping && RB.linearVelocity.y < 0)
			{
				characterAnimator.SetBool("isJumping", false);
				IsJumping = false;
				IsJumpFalling = true; // Transition to falling state after peak


			}

			if (LastOnGroundTime > 0 && !IsJumping && IsJumpFalling == true)
			{
				IsJumpFalling = false; // Reset falling state if 
				audioManager.PlaySFX(audioManager.landing, 0.5f);
			}

			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
                audioManager.PlaySFX(audioManager.jump, 2.0f);
				characterAnimator.SetBool("isJumping", true);
                IsJumping = true;
				IsJumpFalling = false;
				Jump();

				

			}
		}
		if (!IsPlatformer)
		{
			if (IsJumping && RB.linearVelocity.z > 0)
			{
				characterAnimator.SetBool("isJumping", false);
				IsJumping = false;
				IsJumpFalling = true; // Transition to falling state after peak


			}

			if (LastOnGroundTime > 0 && !IsJumping && IsJumpFalling == true)
			{
				IsJumpFalling = false; // Reset falling state if grounded
				audioManager.PlaySFX(audioManager.landing, 0.5f);

			}

			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
				characterAnimator.SetBool("isJumping", true);
				IsJumping = true;
				IsJumpFalling = false;
				Jump();

				audioManager.PlaySFX(audioManager.jump, 2.0f);
			}
		}
		#endregion

		#region EQUIPMENT SELECTION



		// Toggle Equipment Visibility/Equipping
		if (Input.GetKeyDown(KeyCode.I) && 
			(SceneManager.GetActiveScene().name == "World3_1_A" || SceneManager.GetActiveScene().name == "World3_2_A") && !isFrozen)
		{
			
			ToggleEquipped();
			if (isEquipped)
			{
				FreezeMovement(4f);
			}

			characterAnimator.SetTrigger("isBuilding");
			umbrellaBuildAnimator.SetTrigger("BuildWall");
		}
		if (Input.GetKeyDown(KeyCode.U) && 
			(SceneManager.GetActiveScene().name == "World3_1_A") || (SceneManager.GetActiveScene().name == "World3_2_A"))
		{
			UseEquipped();
		}

		#endregion
		/*
		
		#region GRAVITY
		// Adjust gravity and fall speeds based on player state
		if (RB.linearVelocity.y < 0 && moveInput.y < 0)
		{
			// Apply much higher gravity if holding down for a fast fall
			SetGravityScale(gravityScale * fastFallGravityMult);
			// Cap maximum fall speed to prevent excessive acceleration
			RB.linearVelocity = new Vector3(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -maxFastFallSpeed), RB.linearVelocity.z);
		}
		else if (IsJumpCut)
		{
			// Apply higher gravity if the jump button is released mid-jump
			SetGravityScale(gravityScale * jumpCutGravityMult);
			RB.linearVelocity = new Vector3(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -maxFallSpeed), RB.linearVelocity.z);
			IsJumpCut = false; // Reset after applying
		}
		else if ((IsJumping || IsJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < jumpHangTimeThreshold)
		{
			// Apply lower gravity for a "floaty" feel during jump hang time
			SetGravityScale(gravityScale * jumpHangGravityMult);
		}
		else if (RB.linearVelocity.y < 0)
		{
			// Apply higher gravity when falling
			SetGravityScale(gravityScale * fallGravityMult);
			// Cap maximum fall speed to prevent excessive acceleration
			RB.linearVelocity = new Vector3(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -maxFallSpeed), RB.linearVelocity.z);
		}
		else
		{
			// Default gravity for standard movement
			SetGravityScale(gravityScale);
		}

		#endregion
		*/

		#region Character Animation and Audio
		// moving
		if ((moveInput.x != 0 || moveInput.y != 0) && (IsJumpFalling == false && IsJumping == false))
		{
			characterAnimator.SetBool("isMoving", true);
			audioManager.StartFootsteps();

		}

		// idle default
		else
		{
			characterAnimator.SetBool("isMoving", false);
            audioManager.StopFootsteps();

        }
	
        // not moving but grabbing
        if (grab.getIsGrabbing())
		{
			characterAnimator.SetBool("isPushing", false);
            characterAnimator.SetBool("isPulling", false);
            characterAnimator.SetBool("isGrabbing", true);

            audioManager.StopFootsteps();
           
        }

		// Left = 0,
		// Right = 1,
		// Down = 2,
		// Up = 3

		// if moving to the right
		if ((int)direction == 1) {

			// moving and grabbing (pushing)
			if (grab.getIsGrabbing() && (currentSpeed > 0 || tpcurrentSpeed.x > 0 || tpcurrentSpeed.y > 0))
            {
				characterAnimator.SetBool("isPushing", true);
				characterAnimator.SetBool("isPulling", false);
				characterAnimator.SetBool("isMoving", false);

				audioManager.StartPushing();
				audioManager.StopPulling();
			}

			else
			{
				audioManager.StopPushing();
			}

			// moving and grabbing (pulling)
			if (grab.getIsGrabbing() && (currentSpeed < 0  || tpcurrentSpeed.x < 0 || tpcurrentSpeed.y < 0))
			{
				characterAnimator.SetBool("isPulling", true);
				characterAnimator.SetBool("isPushing", false);
				characterAnimator.SetBool("isMoving", false);

				audioManager.StartPulling();
				audioManager.StopPushing();
			}

			else {
				audioManager.StopPulling();
			}

		}

		// if moving to the left 
		else if ((int)direction == 0)
		{

			// moving and grabbing (pushing)
			if (grab.getIsGrabbing() && (currentSpeed < 0 || tpcurrentSpeed.x < 0 || tpcurrentSpeed.y < 0))
			{
				characterAnimator.SetBool("isPushing", true);
				characterAnimator.SetBool("isPulling", false);
				characterAnimator.SetBool("isMoving", false);

				audioManager.StartPushing();
				audioManager.StopPulling();
			}

			// moving and grabbing (pulling)
			if (grab.getIsGrabbing() && (currentSpeed > 0 || tpcurrentSpeed.x > 0 || tpcurrentSpeed.y > 0))
            {
                characterAnimator.SetBool("isPulling", true);
                characterAnimator.SetBool("isPushing", false);
                characterAnimator.SetBool("isMoving", false);

                audioManager.StartPulling();
                audioManager.StopPushing();
            }
        }

        // if moving down 
        else if ((int)direction == 2)
        {

            // moving and grabbing (pushing)
            if (grab.getIsGrabbing() && (tpcurrentSpeed.y < 0 || tpcurrentSpeed.x < 0))
            {
                characterAnimator.SetBool("isPushing", true);
                characterAnimator.SetBool("isPulling", false);
                characterAnimator.SetBool("isMoving", false);

                audioManager.StartPushing();
                audioManager.StopPulling();
            }

            // moving and grabbing (pulling)
            if (grab.getIsGrabbing() && (tpcurrentSpeed.y > 0 || tpcurrentSpeed.x > 0))
            {
                characterAnimator.SetBool("isPulling", true);
                characterAnimator.SetBool("isPushing", false);
                characterAnimator.SetBool("isMoving", false);

                audioManager.StartPulling();
                audioManager.StopPushing();
            }
        }

        // if moving down 
        else if ((int)direction == 3)
        {

            // moving and grabbing (pushing)
            if (grab.getIsGrabbing() && (tpcurrentSpeed.y > 0 || tpcurrentSpeed.x > 0))
            {
                characterAnimator.SetBool("isPushing", true);
                characterAnimator.SetBool("isPulling", false);
                characterAnimator.SetBool("isMoving", false);

                audioManager.StartPushing();
				audioManager.StopPulling();
            }

            // moving and grabbing (pulling)
            if (grab.getIsGrabbing() && (tpcurrentSpeed.y < 0 || tpcurrentSpeed.x < 0))
            {
                characterAnimator.SetBool("isPulling", true);
                characterAnimator.SetBool("isPushing", false);
                characterAnimator.SetBool("isMoving", false);

                audioManager.StartPulling();
                audioManager.StopPushing();
            }
        }




        // return to default idle when not grabbing/pushing/pulling
        if (!(grab.getIsGrabbing())) {
            characterAnimator.SetBool("isGrabbing", false);
            characterAnimator.SetBool("isPulling", false);
            characterAnimator.SetBool("isPushing", false);

            audioManager.StopPushing();
            audioManager.StopPulling();
        }


		











        #endregion
    }

    void FixedUpdate()
	{
		if (isFrozen)
		{
			freezeTimer -= Time.deltaTime;
			if (freezeTimer <= 0f)
			{
				isFrozen = false;
			}
			return; // Stop here so movement is disabled while frozen
		}

		if (IsPlatformer)
		{
			Vector3 gravity = globalGravity * gravityScale * Vector3.up;

			HandleGravityWithWind(gravity); // Apply vertical wind
			PlatformRun(2); // Apply horizontal movement
		}
		else
		{
			TopDownRun(2); // Apply horizontal movement
			Vector3 gravity = globalGravity * gravityScale * Vector3.back;
			HandleGravityWithWind(gravity); // Apply vertical wind

		}
	}
	#region ALL METHODS


	#region INPUT CALLBACKS
	//Methods which whandle input detected in Update()
	public void OnJumpInput()
	{
		LastPressedJumpTime = jumpInputBufferTime;
	}
	#endregion

	#region GENERAL METHODS
	public void DirectionChange(int num)
	{
		switch (num)
		{
			case 0:
				direction = Direction.Left;
				break;
			case 1:
				direction = Direction.Right;
				break;
			case 2:
				direction = Direction.Down;
				break;
			case 3:
				direction = Direction.Up;
				break;
		}
	}
	public void SetGravityScale(float scale)
	{
		gravityScale = scale;
	}

	public void PlatformerConstraints()
	{
		RB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}

	public void TopDownConstraints()
	{
		RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
	#endregion

	#region MOVEMENT METHODS

	private void PlatformRun(float lerpAmount)
	{
		// Base movement values
		float maxSpeed = grab.getIsGrabbing() ? grabRunMaxSpeed : runMaxSpeed;
		float accel = grab.getIsGrabbing() ? grabRunAcceleration : runAccelAmount;
		float deccel = grab.getIsGrabbing() ? grabRunDecceleration : runDeccelAmount;

		float targetSpeed = moveInput.x * maxSpeed;

		// Handle horizontal wind effect
		float hWindEffect = 0f;
		if (activeWindZone != null && activeWindZone.isHorizontalWind)
		{

			audioManager.StartWind();
			if (umbrelladirection != activeWindZone.windDirection )
			{
				umbrellaAnimator.SetBool("isUmbrellaActive", false);
				float windDirection = activeWindZone.windDirection.x; // Horizontal wind
				bool isPressingAgainstWind = (windDirection > 0 && moveInput.x < 0) || (windDirection < 0 && moveInput.x > 0);
				bool isMovingWithWind = (windDirection > 0 && moveInput.x > 0) || (windDirection < 0 && moveInput.x < 0);

				if (isPressingAgainstWind)
				{
					// Cancel all movement against the wind
					targetSpeed = 0f;
					currentSpeed = 0f;
                    
                }
				else if (isMovingWithWind)
				{
					// Boost speed and acceleration when moving with the wind
					maxSpeed += windspeedaddition; // Increase max speed slightly
					accel += windacceladdtion;    // Increase acceleration slightly
				}
				else if (moveInput.x == 0)
				{
					// Apply wind effect when standing still
					hWindEffect = windDirection * activeWindZone.windStrength;
					targetSpeed = hWindEffect;
					
				}
			}

			else if (umbrellaEquipped && umbrelladirection == activeWindZone.windDirection)
			{
				//windAnimator.SetBool("isWindDefault", false);
				//windAnimator.SetBool("isWindAgainst", false);
				umbrellaAnimator.SetBool("isUmbrellaActive", true);
			}
			

		}

		// if outside wind area 
		else {
			audioManager.StopWind();
            //windAnimator.SetBool("isWindDefault", false);
            //windAnimator.SetBool("isWindAgainst", false);
            umbrellaAnimator.SetBool("isUmbrellaActive", false);

            if (isUmbrellaAnchored)
            {
                umbrellaAnimator.SetBool("isUmbrellaActive", true);
            }
            else
            {
                umbrellaAnimator.SetBool("isUmbrellaActive", false);
            }
        }


		// Prevent movement if touching walls
		if ((targetSpeed < 0 && IsTouchingWallLeft()) || (targetSpeed > 0 && IsTouchingWallRight()) && CanJump() == false)
		{
			RB.linearVelocity = new Vector3(0, RB.linearVelocity.y, RB.linearVelocity.z);
			currentSpeed = 0f; // Reset current speed to avoid "ghost momentum"
			return;
		}
		// Check for input direction change and snap to the new direction
		if (Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeed) && moveInput.x != 0)
		{
			currentSpeed = 0f; // Reset horizontal speed when changing directions
		}
		targetSpeed = moveInput.x * maxSpeed;
		// Smoothly adjust current speed towards target speed
		float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? accel : deccel;
		currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);

		// Apply horizontal wind effect
		RB.linearVelocity = new Vector3(currentSpeed + hWindEffect, RB.linearVelocity.y, RB.linearVelocity.z);
		UnityEngine.Debug.Log($"Current Speed: {currentSpeed}, Target Speed: {targetSpeed}, Horizontal Wind: {hWindEffect}, Is Grounded: {LastOnGroundTime > 0}");
	}

	private void TopDownRun(float lerpAmount)
	{
		float maxSpeed = grab.getIsGrabbing() ? grabRunMaxSpeed : runMaxSpeed;
		float accel = grab.getIsGrabbing() ? grabRunAcceleration : runAccelAmount;
		float deccel = grab.getIsGrabbing() ? grabRunDecceleration : runDeccelAmount;

		Vector2 targetSpeed = new Vector2(moveInput.x, moveInput.y) * maxSpeed;

		// Clamp velocity magnitude to max speed
		if (targetSpeed.magnitude > maxSpeed)
			targetSpeed = targetSpeed.normalized * maxSpeed;

		// Wind and umbrella mechanics
		Vector2 windEffect = Vector2.zero;
		if (activeWindZone != null)
		{
			audioManager.StartWind();
			if (umbrelladirection != activeWindZone.windDirection)
			{
				umbrellaAnimator.SetBool("isUmbrellaActive", false);
				Vector2 windDirection = new Vector2(activeWindZone.windDirection.x, activeWindZone.windDirection.y);
				bool isPressingAgainstWind = Vector2.Dot(moveInput, windDirection) < 0;
				bool isMovingWithWind = Vector2.Dot(moveInput, windDirection) > 0;

				if (isPressingAgainstWind)
				{
					// Cancel all movement against the wind
					targetSpeed = Vector2.zero;
					tpcurrentSpeed = Vector2.zero;
				}
				else if (isMovingWithWind)
				{
					// Boost speed and acceleration when moving with the wind
					maxSpeed += windspeedaddition;
					accel += windacceladdtion;
				}
				else
				{
					// Apply wind effect when standing still
					windEffect = windDirection * activeWindZone.windStrength;
					if (moveInput == Vector3.zero)
					{
						targetSpeed = windEffect;
					}
					//windAnimator.SetBool("isWindDefault", true);
				}
			}
			else if (umbrellaEquipped && umbrelladirection == activeWindZone.windDirection)
			{
				//windAnimator.SetBool("isWindDefault", false);
				//windAnimator.SetBool("isWindAgainst", false);
				umbrellaAnimator.SetBool("isUmbrellaActive", true);
			}
		}
		// if outside wind area 
		else
		{
			audioManager.StopWind();
			//windAnimator.SetBool("isWindDefault", false);
			//windAnimator.SetBool("isWindAgainst", false);
			umbrellaAnimator.SetBool("isUmbrellaActive", false);

			if (isUmbrellaAnchored)
			{
				umbrellaAnimator.SetBool("isUmbrellaActive", true);
			}
			else
			{
				umbrellaAnimator.SetBool("isUmbrellaActive", false);
			}
		}
		// Prevent movement if touching walls
		if (CanJump() == false)
		{
			if ((targetSpeed.x < 0 && IsTouchingWallLeft()) ||
				(targetSpeed.x > 0 && IsTouchingWallRight()) ||
				(targetSpeed.y < 0 && IsTouchingWallDown()) ||
				(targetSpeed.y > 0 && IsTouchingWallUp()))
			{
				RB.linearVelocity = new Vector3(0, 0, RB.linearVelocity.z);
				tpcurrentSpeed = Vector2.zero; // Reset movement speed
				return;
			}
		}
		// Check for input direction change and snap to the new direction
		if (Mathf.Sign(targetSpeed.x) != Mathf.Sign(tpcurrentSpeed.x) && moveInput.x != 0)
			tpcurrentSpeed.x = 0f; // Reset horizontal speed when changing directions
		if (Mathf.Sign(targetSpeed.y) != Mathf.Sign(tpcurrentSpeed.y) && moveInput.y != 0)
			tpcurrentSpeed.y = 0f; // Reset vertical speed when changing directions

		// Determine acceleration or deceleration rate
		float accelRate = (LastOnGroundTime > 0)
			? (targetSpeed.magnitude > 0.01f ? accel : deccel)
			: (targetSpeed.magnitude > 0.01f ? accel * accelInAir : deccel * deccelInAir);

		// Smoothly adjust current speed towards target speed
		tpcurrentSpeed = Vector2.MoveTowards(tpcurrentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);

		// Apply the calculated speed to the Rigidbody while preserving z velocity
		RB.linearVelocity = new Vector3(tpcurrentSpeed.x + windEffect.x, tpcurrentSpeed.y + windEffect.y, RB.linearVelocity.z);

		UnityEngine.Debug.Log($"Current Speed: {tpcurrentSpeed}, Target Speed: {targetSpeed}, Accel Rate: {accelRate}, Wind Effect: {windEffect}");

		// Clamp final velocity to max speed
		if (tpcurrentSpeed.magnitude > maxSpeed)
		{
			tpcurrentSpeed = tpcurrentSpeed.normalized * maxSpeed;
		}
	}

	private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Platformer Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
		if(IsPlatformer)
		{
			float force = jumpForce;
			if (RB.linearVelocity.y < 0)
				force -= RB.linearVelocity.y;

			RB.AddForce(Vector3.up * force, ForceMode.Impulse);
		}
		#endregion
		#region Perform TopDown Jump
		//TopDown Version
		else 
		{
			float force = jumpForce;
			if (RB.linearVelocity.z > 0)
				force -= RB.linearVelocity.z;

			RB.AddForce(Vector3.back * force, ForceMode.Impulse);
			IsJumping = true;
		}
		#endregion
	}


	#endregion

	#region CHECK METHODS
	private bool IsTouchingWallLeft()
	{
		// Retrieve all colliders in the overlap box
		Collider[] colliders = Physics.OverlapBox(_leftWallCheck.position, _wallCheckSize / 2, Quaternion.identity, ~0); // Use ~0 to include all layers

		// Check if any collider matches the criteria
		foreach (Collider collider in colliders)
		{
			// Check if the collider is on the ground layer or has the "Pushable" tag
			if (((1 << collider.gameObject.layer) & _groundLayer) != 0 || collider.gameObject.CompareTag("Pushable"))
			{
				return true;
			}
		}

		return false;
	}

	private bool IsTouchingWallRight()
	{
		// Retrieve all colliders in the overlap box
		Collider[] colliders = Physics.OverlapBox(_rightWallCheck.position, _wallCheckSize / 2, Quaternion.identity, ~0); // Use ~0 to include all layers

		// Check if any collider matches the criteria
		foreach (Collider collider in colliders)
		{
			// Check if the collider is on the ground layer or has the "Pushable" tag
			if (((1 << collider.gameObject.layer) & _groundLayer) != 0 || collider.gameObject.CompareTag("Pushable"))
			{
				return true;
			}
		}

		return false;
	}

	private bool IsTouchingWallUp()
	{
		Collider[] colliders = Physics.OverlapBox(_topWallCheck.position + Vector3.forward, _wallCheckSize / 2, Quaternion.identity, ~0);
		foreach (Collider collider in colliders)
		{
			if (((1 << collider.gameObject.layer) & _groundLayer) != 0 || collider.gameObject.CompareTag("Pushable"))
				return true;
		}
		return false;
	}

	private bool IsTouchingWallDown()
	{
		Collider[] colliders = Physics.OverlapBox(_downWallCheck.position + Vector3.back, _wallCheckSize / 2, Quaternion.identity, ~0);
		foreach (Collider collider in colliders)
		{
			if (((1 << collider.gameObject.layer) & _groundLayer) != 0 || collider.gameObject.CompareTag("Pushable"))
				return true;
		}
		return false;
	}

	private bool CanJump()
	{
		return LastOnGroundTime > 0 && !IsJumping;
	}
	public bool getIsPlatformer()
	{
		return IsPlatformer;
	}
	#endregion

	#region ROTATION METHODS
	private void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
	private void ToggleRotation()
	{
		// Swap between rotationA and rotationB
		isRotationA = !isRotationA;

		Vector3 targetRotation = isRotationA ? rotationA : rotationB;
		StartCoroutine(SmoothRotation(targetRotation));
	}
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
    #endregion

    #region UNIQUE GAME MECHANIC METHODS

    #region Grabbing
	public void Resize(bool isResized)
	{
		/*
		Left = 0,
		Right = 1,
		Down = 2,
		Up = 3
		*/
		float centerx = 0.0f;
		float centerz = 0.0f;
		float sizex = 1.0f;
		float sizez = 1.0f;
		if (!isResized)
		{
			if ((int)direction == 0)
			{
				sizex = 2.9f;
				centerx = -0.927f;
			}
			else if ((int)direction == 1)
			{
				sizex = 2.9f;
				centerx = 0.927f;
			}
			else if ((int)direction == 2)
			{
				sizez = 2.67f;
				centerz = -0.8f;
				//down
			}
			else if ((int)direction == 3)
			{
				sizez = 2.67f;
				centerz = 0.8f;
			}

			box.center = new Vector3(centerx, 0.1806138f, centerz);
			box.size = new Vector3(sizex, 1.361628f, sizez);
		}
		else if (isResized)
		{
			box.center = new Vector3(0.0f, 0.1806138f, 0.0f);
			box.size = new Vector3(1.0f, 1.361628f, 1.0f);
		}
		
		
	}
    #endregion

    #region Wind




    private void HandleGravityWithWind(Vector3 gravity)
	{
		
		if(gravity.z == 0f)
		{
			if (activeWindZone != null && activeWindZone.isVerticalWind)
			{
				if (activeWindZone.windDirection != umbrelladirection)
				{
					float vWindDirection = activeWindZone.windDirection.y; // Vertical wind direction
					if (vWindDirection > 0) // Upward wind
					{
						LastOnGroundTime = -0.01f; // Simulate floating
						gravity = Vector3.up * activeWindZone.windStrength * 2f; // Weaker upward force for floating
					}
					else if (vWindDirection < 0) // Downward wind
					{
						gravity += Vector3.down * activeWindZone.windStrength; // Stronger downward force for sinking
					}
				}
			}
		}
		// Apply the calculated gravity
		RB.AddForce(gravity, ForceMode.Acceleration);
	}

	public void EnterWindZone(WindArea windZone)
	{	
		activeWindZone = windZone;
	}

	public void ExitWindZone()
	{
		activeWindZone = null;
	}

	public void AffectedByUmbrella(Vector3 directionnegated)
	{
		umbrelladirection = directionnegated;
	}
	public bool isUmbrellaEquipped()
	{
		return umbrellaEquipped;
	}
	public int getCurrentDirection()
	{
		return (int)direction;
	}
	public void AnchorUmbrella()
	{
		if (umbrella.transform.parent != null)
		{
			umbrella.transform.SetParent(null);
			isUmbrellaAnchored = true;
		}
	}
	public bool getUmbrellaAnchored()
	{
		return isUmbrellaAnchored;
	}

	public Vector3 getMoveInput()
	{
		return moveInput;
	}
	#endregion

	#region EQUIPMENT METHODS
	private void SetEquipment(Equipment newEquipment)
	{
		if (currentEquipment == newEquipment)
			return; // If the same equipment is selected, do nothing

		// Unequip current equipment
		UnequipCurrent();

		// Update selected equipment
		currentEquipment = newEquipment;
		isEquipped = false; // Equipment starts unequipped
	}
	private void ToggleEquipped()
	{
		if (currentEquipment == Equipment.None)
			return; // No equipment to toggle

		isEquipped = !isEquipped; // Toggle equip state

		// Enable or disable the corresponding GameObject
		switch (currentEquipment)
		{
			case Equipment.PowerGloves:
				powerGloves.SetActive(isEquipped);
				break;
			case Equipment.Umbrella:
				if(!isUmbrellaAnchored) umbrella.SetActive(isEquipped);
				break;
		}
		if(currentEquipment == Equipment.Umbrella && isEquipped)
		{
			umbrellaEquipped = true;
		}
		else
		{
			umbrellaEquipped = false;
			AffectedByUmbrella(Vector3.zero);

        }
		// Log for debugging
		UnityEngine.Debug.Log($"{currentEquipment} is now {(isEquipped ? "equipped" : "unequipped")}.");
	}
	private void UseEquipped()
	{
		if (currentEquipment == Equipment.None)
			return; // No equipment to toggle

		// Enable or disable the corresponding GameObject
		if (isEquipped)
		{
			switch (currentEquipment)
			{
				case Equipment.PowerGloves:
					powerGloves.SetActive(isEquipped);
					break;
				case Equipment.Umbrella:
					AnchorUmbrella();
					break;
			}
		}
		// Log for debugging
		UnityEngine.Debug.Log("Anchored Umbrella");
	}
	private void UnequipCurrent()
	{
		if (currentEquipment == Equipment.None)
			return;

		// Disable the current equipment GameObject
		switch (currentEquipment)
		{
			case Equipment.PowerGloves:
				powerGloves.SetActive(false);
				break;
			case Equipment.Umbrella:
				umbrella.SetActive(false);
				break;
		}

		isEquipped = false;
	}
	public void FreezeMovement(float duration)
	{
		isFrozen = true;
		freezeTimer = duration;

		if (RB != null)
		{
			if (IsPlatformer)
			{
				// Keep Y velocity (like gravity/falling), zero out horizontal
				RB.linearVelocity = new Vector3(0f, RB.linearVelocity.y, 0f);
				RB.angularVelocity = Vector3.zero; // Optional: stops any spin
			}
			else
			{
				// Keep Y velocity (like gravity/falling), zero out horizontal
				RB.linearVelocity = new Vector3(0f, 0f, RB.linearVelocity.z);
				RB.angularVelocity = Vector3.zero; // Optional: stops any spin
			}
		}
	}

	#endregion
	#endregion

	#endregion
	#region Animation
	#endregion
	private void OnValidate()
	{
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);

		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
		#endregion
	}
}
