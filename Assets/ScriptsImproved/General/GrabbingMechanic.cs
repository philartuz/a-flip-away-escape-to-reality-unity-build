using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class GrabbingMechanic : MonoBehaviour
{
    
    Rigidbody rb;

    [Header("Grabbing Mechanic")]
    public PlayerMovement Player;
	public bool isGrabbing;
	public GameObject currentgrab;
	public GameObject triggerObject;

    [Space(5)]
    [Header("Vector Creation")]
    public GameObject vectorRepresentation;
    public GameObject verticalPrefabPull, verticalPrefabPush;
    public GameObject horizontalPrefabPull, horizontalPrefabPush, vectorRepresentationH;
    public Transform vectorParent;

    private Vector3 originalScaleArrow1, originalScaleArrow2, originalPositionArrow1Main, originalPositionArrow2Main; 
    private Vector3 originalScaleArrow1H, originalScaleArrow2H, originalPositionArrow1MainH, originalPositionArrow2MainH;

    private Transform arrow1Main, arrow2Main, arrow1Body, arrow2Body;
    private Transform arrow1MainH, arrow2MainH, arrow1BodyH, arrow2BodyH;
    private float scaleSpeed = 0.01f;
    private float posSpeed = 0.01f;

    // Clamping values
    private float minScale = 1.0f;
    private float maxScale = 4.0f;
    private float minPos = -2.0f;
    private float maxPos = 6.0f;

    [Header("Not Platform Offset")]
    public Vector3 verticalMovingLeft;
    public Vector3 verticalMovingRight;
    public Vector3 verticalMovingUp;
    public Vector3 verticalMovingDown;

    public Vector3 horizMovingLeft;
    public Vector3 horizMovingRight;
    public Vector3 horizMovingUp;
    public Vector3 horizMovingDown;

    [Header("Platform Offset")]
    public Vector3 movingLeft;
    public Vector3 movingRight;



    void Start()
    {
		isGrabbing = false;
	}

    // Update is called once per frame
    void Update()
    {

        #region Grabbing Code
        if (Input.GetKeyDown(KeyCode.E) && triggerObject != null && !isGrabbing)
		{
			Player.Resize(false);
			//make the pushable object a child of this object and tell PowerGloves.cs that you are grabbing.
			currentgrab = triggerObject.gameObject;
			if(Player.getIsPlatformer() == true)
			{
				currentgrab.gameObject.GetComponent<ObjectMovement>().GrabbedPlatformerConstraints();

                // Left = 0,
                // Right = 1,
                // Down = 2,
                // Up = 3

                // moving to the right
                if ((int)Player.direction == 1) {
                    currentgrab.gameObject.transform.position = new Vector3(transform.position.x + 0.7f,
                        transform.position.y,
                        transform.position.z - 0.5f);
                }
                // moving to the left 
                else if ((int)Player.direction == 0)
                {
                    currentgrab.gameObject.transform.position = new Vector3(transform.position.x - 0.7f,
                        transform.position.y,
                        transform.position.z - 0.5f);

                }

       


            }
            else if (Player.getIsPlatformer() == false)
			{
				currentgrab.gameObject.GetComponent<ObjectMovement>().GrabbedTopDownConstraints();

                // moving to the right
                if ((int)Player.direction == 1)
                {
                    currentgrab.gameObject.transform.position = new Vector3(transform.position.x + 0.7f,
                        transform.position.y,
                        transform.position.z - 0.5f);
                }
                // moving to the left 
                else if ((int)Player.direction == 0)
                {
                    currentgrab.gameObject.transform.position = new Vector3(transform.position.x - 0.7f,
                        transform.position.y,
                        transform.position.z - 0.5f);

                }
                // moving to the down
                else if ((int)Player.direction == 2)
                {
                    currentgrab.gameObject.transform.position = new Vector3(transform.position.x,
                        transform.position.y - 0.7f,
                        transform.position.z - 0.5f);
                }
                // moving to the up 
                else if ((int)Player.direction == 3)
                {
                    currentgrab.gameObject.transform.position = new Vector3(transform.position.x,
                        transform.position.y + 0.7f,
                        transform.position.z - 0.5f);

                }


            }

			currentgrab.gameObject.GetComponent<Rigidbody>().isKinematic = true;
			currentgrab.gameObject.GetComponent<Rigidbody>().detectCollisions = false;
			currentgrab.gameObject.GetComponent<ObjectMovement>().enabled = false;
			currentgrab.gameObject.transform.SetParent(gameObject.transform.parent, true);

			isGrabbing = true;



		} //END OF IF INSIDE PRESSING E CODE

		//if it's is a pushable object and you press E and you are currently grabbing something,
		else if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q)) && isGrabbing)
		{
            Player.Resize(true);
			//stop making the pushable object a child of this object and tell PowerGloves.cs that you are not grabbing.
			currentgrab.gameObject.GetComponent<ObjectMovement>().enabled = true;
			if (Player.getIsPlatformer() == true)
			{
				currentgrab.gameObject.GetComponent<ObjectMovement>().PlatformerConstraints();
			}
			else if (Player.getIsPlatformer() == false)
			{
				currentgrab.gameObject.GetComponent<ObjectMovement>().TopDownConstraints();
			}
			currentgrab.transform.SetParent(null);
			currentgrab.gameObject.GetComponent<Rigidbody>().isKinematic = false;
			currentgrab.gameObject.GetComponent<Rigidbody>().detectCollisions = true;
			isGrabbing = false;
			rb = null;
			currentgrab = null;
		} //END OF ELSE IF INSIDE PRESSING E CODE

        #endregion

        #region Vector Creation

        if (isGrabbing && Player.getIsPlatformer())
        {
       

            // make vectorRepresentation when A or D and destroy the H vectorrepresentation
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {

                // Destroy the previous representation before creating a new one
                if (vectorRepresentationH != null)
                {
                    Destroy(vectorRepresentationH);
                    vectorRepresentationH = null;
                }

                if (Input.GetKeyDown(KeyCode.D)) {
                    vectorRepresentationH = Instantiate(horizontalPrefabPush, vectorParent);
                }

                if (Input.GetKeyDown(KeyCode.A)) {
                    vectorRepresentationH = Instantiate(horizontalPrefabPull, vectorParent);
                }

                // float angle = Player.transform.eulerAngles.y; // Adjust based on movement direction

                // moving to the right 
                if ((int)Player.direction == 1)
                {
                    vectorRepresentationH.transform.position = vectorRepresentationH.transform.position + movingRight;

                }

                // moving to the left 
                else if ((int)Player.direction == 0) 
                {
                    vectorRepresentationH.transform.position = vectorRepresentationH.transform.position + movingLeft;
                }




                arrow1MainH = vectorRepresentationH.transform.GetChild(0);
                arrow2MainH = vectorRepresentationH.transform.GetChild(1);

                arrow1BodyH = arrow1MainH.transform.GetChild(0);
                arrow2BodyH = arrow2MainH.transform.GetChild(0);

                originalScaleArrow1H = arrow1BodyH.localScale;
                originalScaleArrow2H = arrow2BodyH.localScale;

                originalPositionArrow1MainH = arrow1MainH.localPosition;
                originalPositionArrow2MainH = arrow2MainH.localPosition;

                Destroy(vectorRepresentation);
                vectorRepresentation = null;

            }


            // resizing the arrows when A or D 
            if (vectorRepresentationH != null && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    arrow1BodyH.localScale += new Vector3(0, scaleSpeed, 0);
                    //arrow1MainH.localPosition += new Vector3(0, posSpeed, 0);

                    arrow2MainH.localPosition -= new Vector3(0, posSpeed, 0);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    arrow1MainH.localPosition += new Vector3(0, posSpeed, 0);

                    arrow2BodyH.localScale += new Vector3(0, scaleSpeed, 0);
                    //arrow2MainH.localPosition -= new Vector3(0, posSpeed, 0);
                }

                arrow1BodyH.localScale = ClampScale(arrow1BodyH.localScale);
                arrow2BodyH.localScale = ClampScale(arrow2BodyH.localScale);

                arrow1MainH.localPosition = ClampPosition(arrow1MainH.localPosition);
                arrow2MainH.localPosition = ClampPosition(arrow2MainH.localPosition);


            }
        }

        else if (isGrabbing && !(Player.getIsPlatformer()))
        {
            // make vectorRepresentation when W or S and destroy the H vectorrepresentation
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)))
            {

                // Destroy the previous representation before creating a new one
                if (vectorRepresentation != null)
                {
                    Destroy(vectorRepresentation);
                    vectorRepresentation = null;
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    vectorRepresentation = Instantiate(verticalPrefabPull, vectorParent);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    vectorRepresentation = Instantiate(verticalPrefabPush, vectorParent);
                }

                //moving to the right
                if ((int)Player.direction == 1)
                {
                    vectorRepresentation.transform.position = vectorRepresentation.transform.position + verticalMovingRight;
                }

                //moving to the left
                else if ((int)Player.direction == 0)
                {
                    vectorRepresentation.transform.position = vectorRepresentation.transform.position + verticalMovingLeft;
                }

                //moving up
                else if ((int)Player.direction == 3)
                {
                    vectorRepresentation.transform.position = vectorRepresentation.transform.position + verticalMovingUp;
                }

                //moving down
                else if ((int)Player.direction == 2)
                {
                    vectorRepresentation.transform.position = vectorRepresentation.transform.position + verticalMovingDown;
                }

                // float angle = Player.transform.eulerAngles.y; // Adjust based on movement direction

                vectorRepresentation.transform.rotation = Quaternion.Euler(0, 0, 0);

                arrow1Main = vectorRepresentation.transform.GetChild(0);
                arrow2Main = vectorRepresentation.transform.GetChild(1);

                arrow1Body = arrow1Main.transform.GetChild(0);
                arrow2Body = arrow2Main.transform.GetChild(0);

                originalScaleArrow1 = arrow1Body.localScale;
                originalScaleArrow2 = arrow2Body.localScale;

                originalPositionArrow1Main = arrow1Main.localPosition;
                originalPositionArrow2Main = arrow2Main.localPosition;

                Destroy(vectorRepresentationH);
                vectorRepresentationH = null;

            }

            // make vectorRepresentation when A or D and destroy the H vectorrepresentation
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {

                // Destroy the previous representation before creating a new one
                if (vectorRepresentationH != null)
                {
                    Destroy(vectorRepresentationH);
                    vectorRepresentationH = null;
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    vectorRepresentationH = Instantiate(horizontalPrefabPush, vectorParent);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    vectorRepresentationH = Instantiate(horizontalPrefabPull, vectorParent);
                }


                //moving to the right
                if ((int)Player.direction == 1)
                {
                    vectorRepresentationH.transform.position = vectorRepresentationH.transform.position + horizMovingRight;
                }

                //moving to the left
                else if ((int)Player.direction == 0)
                {
                    vectorRepresentationH.transform.position = vectorRepresentationH.transform.position + horizMovingLeft;
                }

                //moving up
                else if ((int)Player.direction == 3)
                {
                    vectorRepresentationH.transform.position = vectorRepresentationH.transform.position + horizMovingUp;
                }

                //moving down
                else if ((int)Player.direction == 2)
                {
                    vectorRepresentationH.transform.position = vectorRepresentationH.transform.position + horizMovingDown;
                }
                
                vectorRepresentationH.transform.rotation = Quaternion.Euler(0, 0, 90);

                arrow1MainH = vectorRepresentationH.transform.GetChild(0);
                arrow2MainH = vectorRepresentationH.transform.GetChild(1);

                arrow1BodyH = arrow1MainH.transform.GetChild(0);
                arrow2BodyH = arrow2MainH.transform.GetChild(0);

                originalScaleArrow1H = arrow1BodyH.localScale;
                originalScaleArrow2H = arrow2BodyH.localScale;

                originalPositionArrow1MainH = arrow1MainH.localPosition;
                originalPositionArrow2MainH = arrow2MainH.localPosition;

                Destroy(vectorRepresentation);
                vectorRepresentation = null;

            }

            // resizing the arrows when W or S
            if (vectorRepresentation != null && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)))
            {
                if (Input.GetKey(KeyCode.W))
                {
                    arrow1Body.localScale += new Vector3(0, scaleSpeed, 0);
                    arrow2Main.localPosition -= new Vector3(0, posSpeed, 0);

                }

                else if (Input.GetKey(KeyCode.S))
                {
                    arrow2Main.localPosition += new Vector3(0, scaleSpeed, 0);
                    arrow1Body.localScale += new Vector3(0, scaleSpeed, 0);
                }

                arrow1Body.localScale = ClampScale(arrow1Body.localScale);
                arrow2Body.localScale = ClampScale(arrow2Body.localScale);

                arrow1Main.localPosition = new Vector3(0,
                                                       Mathf.Clamp(arrow1Main.localPosition.y, -1.0f, 4),
                                                       0);

                arrow2Main.localPosition = new Vector3(0,
                                                       Mathf.Clamp(arrow2Main.localPosition.y, -1.0f, 4),
                                                      0);





            }

            // resizing the arrows when A or D 
            if (vectorRepresentationH != null && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                if (Input.GetKey(KeyCode.A))
                {
                    arrow1BodyH.localScale += new Vector3(0, scaleSpeed, 0);
                    //arrow1MainH.localPosition += new Vector3(0, posSpeed, 0);

                    arrow2MainH.localPosition -= new Vector3(0, posSpeed, 0);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    arrow1MainH.localPosition += new Vector3(0, posSpeed, 0);

                    arrow2BodyH.localScale += new Vector3(0, scaleSpeed, 0);
                    //arrow2MainH.localPosition -= new Vector3(0, posSpeed, 0);
                }

                arrow1BodyH.localScale = ClampScale(arrow1BodyH.localScale);
                arrow2BodyH.localScale = ClampScale(arrow2BodyH.localScale);

                arrow1MainH.localPosition = ClampPosition(arrow1MainH.localPosition);
                arrow2MainH.localPosition = ClampPosition(arrow2MainH.localPosition);


            }
        }


        if (!isGrabbing)
        {
            DestroyRepresentation();
        }

        

        // reset scaling for the arrows
        if (vectorRepresentation != null && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            arrow1Body.localScale = Vector3.Lerp(arrow1Body.localScale, originalScaleArrow1, Time.deltaTime * 5f);
            arrow2Body.localScale = Vector3.Lerp(arrow1Body.localScale, originalScaleArrow2, Time.deltaTime * 5f);

            arrow1Main.localPosition = Vector3.Lerp(arrow1Main.localPosition, originalPositionArrow1Main, Time.deltaTime * 5f);
            arrow2Main.localPosition = Vector3.Lerp(arrow2Main.localPosition, originalPositionArrow2Main, Time.deltaTime * 5f);
        }

        // Smoothly reset scale for horizontal arrows
        if (vectorRepresentationH != null && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            arrow1BodyH.localScale = Vector3.Lerp(arrow1BodyH.localScale, originalScaleArrow1H, Time.deltaTime * 5f);
            arrow2BodyH.localScale = Vector3.Lerp(arrow1BodyH.localScale, originalScaleArrow2H, Time.deltaTime * 5f);

            arrow1MainH.localPosition = Vector3.Lerp(arrow1MainH.localPosition, originalPositionArrow1MainH, Time.deltaTime * 5f);
            arrow2MainH.localPosition = Vector3.Lerp(arrow2MainH.localPosition, originalPositionArrow2MainH, Time.deltaTime * 5f);

        }

        #endregion


    }

    #region Vector Utility Functions
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

    
    Vector3 ClampScale(Vector3 scale)
    {
        return new Vector3(
            Mathf.Clamp(scale.x, minScale, maxScale),
            Mathf.Clamp(scale.y, minScale, maxScale),
            Mathf.Clamp(scale.z, minScale, maxScale)
        );
    }

    Vector3 ClampPosition(Vector3 position)
    {
        return new Vector3(
            0,
            Mathf.Clamp(position.y, minPos, maxPos),
            0
        );
    }

    #endregion

    void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Pushable") || other.CompareTag("Battery"))
		{           //if it's a pushable object and you press E and you aren't currently grabbing something,
			if (other.gameObject.GetComponent<ObjectMovement>().getCanBeGrabbed() == true)
			{           //if it's a pushable object and you press E and you aren't currently grabbing something,
				triggerObject = other.gameObject;

			}
		}
	}
	void OnTriggerExit(Collider other)
	{

		if (!isGrabbing && triggerObject != null && other.gameObject == triggerObject)
		{
			triggerObject = null;
		}

	}
	public bool getIsGrabbing()
	{
		return isGrabbing;
	}
}
