using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour
{
	public bool grabbing;
	public PowerGloves push;
	public GameObject currentgrab;
	private GameObject triggerObject;
	// Start is called before the first frame update

	// Vector Stuff
	public GameObject vectorPrefab, vectorRepresentation;
	public GameObject vectorPrefabH, vectorRepresentationH;
	public Transform vectorParent;

	private Vector3 originalScaleArrow1, originalScaleArrow2;
	private Vector3 originalScaleArrow1H, originalScaleArrow2H;

	private Transform arrow1, arrow2;
	private Transform arrow1H, arrow2H;
	private float scaleSpeed = 0.002f;

	// Clamping values
	private float minScale = 0.1f;
	private float maxScale = 0.3f;


	void Start()
	{
		push = GetComponentInParent<PowerGloves>();
		grabbing = false;
	}
	// Update is called once per frame
	void Update()
	{

		// VECTOR CREATION LOGIC
		if (grabbing)
		{
			// make vectorRepresentation when W or S and destroy the H vectorrepresentation
			if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) && vectorRepresentation == null)
			{
				vectorRepresentation = Instantiate(vectorPrefab, vectorParent);
				arrow1 = vectorRepresentation.transform.GetChild(0);
				arrow2 = vectorRepresentation.transform.GetChild(1);

				originalScaleArrow1 = arrow1.localScale;
				originalScaleArrow2 = arrow2.localScale;


				Destroy(vectorRepresentationH);
				vectorRepresentationH = null;

			}

			// make vectorRepresentation when A or D and destroy the H vectorrepresentation
			if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)) && vectorRepresentationH == null)
			{
				vectorRepresentationH = Instantiate(vectorPrefabH, vectorParent);
				arrow1H = vectorRepresentationH.transform.GetChild(0);
				arrow2H = vectorRepresentationH.transform.GetChild(1);

				originalScaleArrow1H = arrow1H.localScale;
				originalScaleArrow2H = arrow2H.localScale;


				Destroy(vectorRepresentation);
				vectorRepresentation = null;

			}

			// resizing the arrows when W or S
			if (vectorRepresentation != null && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
			{
				if (Input.GetKey(KeyCode.W))
				{
					arrow1.localScale += new Vector3(scaleSpeed, 0, scaleSpeed);
					arrow2.localScale -= new Vector3(scaleSpeed, 0, scaleSpeed);
				}
				else if (Input.GetKey(KeyCode.S))
				{
					arrow1.localScale -= new Vector3(scaleSpeed, 0, scaleSpeed);
					arrow2.localScale += new Vector3(scaleSpeed, 0, scaleSpeed);
				}

				arrow1.localScale = ClampScale(arrow1.localScale);
				arrow2.localScale = ClampScale(arrow2.localScale);
			}

			// resizing the arrows when A or D 
			if (vectorRepresentationH != null && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
			{
				if (Input.GetKey(KeyCode.A))
				{
					arrow1H.localScale += new Vector3(scaleSpeed, 0, scaleSpeed);
					arrow2H.localScale -= new Vector3(scaleSpeed, 0, scaleSpeed);
				}
				else if (Input.GetKey(KeyCode.D))
				{
					arrow1H.localScale -= new Vector3(scaleSpeed, 0, scaleSpeed);
					arrow2H.localScale += new Vector3(scaleSpeed, 0, scaleSpeed);
				}

				arrow1H.localScale = ClampScale(arrow1H.localScale);
				arrow2H.localScale = ClampScale(arrow2H.localScale);
			}
		}


		if (!grabbing)
		{
			DestroyRepresentation();
		}

		// reset scaling for the arrows
		if (vectorRepresentation != null && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
		{
			arrow1.localScale = Vector3.Lerp(arrow1.localScale, originalScaleArrow1, Time.deltaTime * 5f);
			arrow2.localScale = Vector3.Lerp(arrow2.localScale, originalScaleArrow2, Time.deltaTime * 5f);
		}

		// Smoothly reset scale for horizontal arrows
		if (vectorRepresentationH != null && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			arrow1H.localScale = Vector3.Lerp(arrow1H.localScale, originalScaleArrow1H, Time.deltaTime * 5f);
			arrow2H.localScale = Vector3.Lerp(arrow2H.localScale, originalScaleArrow2H, Time.deltaTime * 5f);
		}



		if (Input.GetKeyDown(KeyCode.X) && !push.IsGrab() && grabbing == true)
		{
			grabbing = false;
			Debug.Log("correcting");
		}

		if (Input.GetKeyDown(KeyCode.E) && triggerObject != null && !push.IsGrab() && !grabbing)
		{
			Debug.Log("PRESSED E");

			//make the pushable object a child of this object and tell PowerGloves.cs that you are grabbing.
			triggerObject.gameObject.transform.SetParent(gameObject.transform.parent, true);
			triggerObject.gameObject.GetComponent<WindObstacle>().activeController(false);

			push.GrabOn();


			if (push.IsGrab() && grabbing == false)
			{
				grabbing = true;
				Debug.Log("Both are now true");
			}
			else if (push.IsGrab() == false && grabbing == true)
			{
				grabbing = false;
				Debug.Log("Both are still false");
			}

			currentgrab = triggerObject.gameObject;


			// DEBUGGING STUFF
			if (triggerObject.gameObject.transform.IsChildOf(gameObject.transform.parent.transform))
			{
				Debug.Log("Child");
			}
			else
			{
				Debug.Log("Not Child");
			}

			if (triggerObject.gameObject.transform.IsChildOf(gameObject.transform.parent.transform) == false)
			{
				Debug.Log("Not Child 2");
			}
			else
			{
				Debug.Log("Child 2");
			}

		} //END OF IF INSIDE PRESSING E CODE

		//if it's is a pushable object and you press E and you are currently grabbing something,
		else if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q)) && grabbing && push.IsGrab())
		{

			//stop making the pushable object a child of this object and tell PowerGloves.cs that you are not grabbing.
			currentgrab.GetComponent<WindObstacle>().activeController(true);
			currentgrab.transform.SetParent(null);
			push.GrabOff();

			if (push.IsGrab() && grabbing == false)
			{
				grabbing = true;
				Debug.Log("Both are now true");
			}
			else if (push.IsGrab() == false && grabbing == true)
			{
				grabbing = false;
				Debug.Log("Both are still false");

			}

			if (currentgrab.transform.IsChildOf(gameObject.transform.parent.transform))
			{
				Debug.Log("Child");
			}
			else
			{
				Debug.Log("Not Child");
			}

			if (currentgrab.transform.IsChildOf(gameObject.transform.parent.transform) == false)
			{
				Debug.Log("Not Child 2");
			}
			else
			{
				Debug.Log("Child 2");
			}
		} //END OF ELSE IF INSIDE PRESSING E CODE

		




	}

	// Utility function to destroy both representations
	void DestroyRepresentation()
	{
		if (vectorRepresentation != null)
		{
			Destroy(vectorRepresentation);
			vectorRepresentation = null;
		}
		if (vectorRepresentationH != null)
		{
			Destroy(vectorRepresentationH);
			vectorRepresentationH = null;
		}
	}

	// Utility function for clamping scale
	Vector3 ClampScale(Vector3 scale)
	{
		return new Vector3(
			Mathf.Clamp(scale.x, minScale, 0.7f),
			scale.y,
			Mathf.Clamp(scale.z, minScale, maxScale)
		);
	}
	void OnTriggerStay(Collider other)
	{


		if (other.CompareTag("Pushable") || other.CompareTag("Battery"))
		{
			Debug.Log("Colliding with Pushable or Battery");
			//if it's a pushable object and you press E and you aren't currently grabbing something,
			triggerObject = other.gameObject;

		}
	}
	void OnTriggerExit(Collider other)
	{
		//if the collision detects that it's a pushable object and it exits the trigger somehow,
		if ((other.CompareTag("Pushable") || other.CompareTag("Battery")) && other.gameObject == triggerObject)
		{
			triggerObject = null;
		}
	}

}
