using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
	private Rigidbody RB;
	private Vector3 gravityDirection;
	[SerializeField] private PlayerMovement Player;
	public float gravityStrength = -9.81f; // Strength of gravity applied to the object
	public float gravityMult;

	public bool isSubmerged;
	public bool canBeGrabbed;
	public bool canBeAffectedByWind;
	private void Awake()
	{
		RB = GetComponent<Rigidbody>();

	}
	private void Start()
	{
		isSubmerged = false;
	}
	private void Update()
	{

        if (Player.getIsPlatformer())
        {
            PlatformerConstraints();
        }
        else if (!Player.getIsPlatformer())
        {
            TopDownConstraints();


        }


    }


	public bool getCanBeGrabbed()
	{
		return canBeGrabbed; 
	}
	private void FixedUpdate()
	{
		if (Player == null)
			return;

		// Update gravity direction based on the player's mode
		gravityDirection = Player.getIsPlatformer() ? Vector3.up : Vector3.back;

        // Apply custom gravity
        RB.AddForce(gravityMult * gravityStrength * gravityDirection, ForceMode.Acceleration);

    }
	#region GENERAL METHODS
	public void PlatformerConstraints()
	{
        RB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

	public void TopDownConstraints()
	{
        RB.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY  | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
	public void GrabbedPlatformerConstraints()
	{
		RB.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
	public void GrabbedTopDownConstraints()
	{
		RB.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}
	#endregion
	public bool getIsSubmerged()
	{
		return isSubmerged;
	}
	public void setSubmerged()
	{
		isSubmerged = true;
	}
}
