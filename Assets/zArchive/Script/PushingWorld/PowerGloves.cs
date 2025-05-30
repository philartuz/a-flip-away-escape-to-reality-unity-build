using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerGloves : MonoBehaviour
{
	public bool grabbing;
	public PlayerController grab;
    // Start is called before the first frame update
    void Start()
    {
		grabbing = false;
		grab = GetComponentInParent<PlayerController>();
	}
	public void GrabOn()
	{

		grabbing = true;
	}
	public void GrabOff()
	{
		grabbing = false;
	}
	public bool IsGrab()
	{
		return grabbing; 
	}

	// Update is called once per frame
	void Update()
    {
		if (grabbing)
		{
			grab.Grab();
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			if (grab.getIsGravityY())
			{
				transform.localRotation = Quaternion.Euler(0, 0, 0);
			}
		}
		if (!grabbing)
		{
			grab.NotGrab();
			if (grab.getIsGravityY() == false)
			{
				if (Input.GetKeyDown(KeyCode.W))
				{
					transform.localRotation = Quaternion.Euler(0, 0, 90);
				}
				else if (Input.GetKeyDown(KeyCode.S))
				{
					transform.localRotation = Quaternion.Euler(0, 0, -90);
				}
			}

			if (Input.GetKeyDown(KeyCode.A))
			{
				transform.localRotation = Quaternion.Euler(0, 0, 180);
			}
			else if (Input.GetKeyDown(KeyCode.D))
			{
				transform.localRotation = Quaternion.Euler(0, 0, 0);
			}
		}
		
	}
}
