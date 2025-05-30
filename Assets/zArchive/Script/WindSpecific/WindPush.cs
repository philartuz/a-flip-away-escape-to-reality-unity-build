using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPush : MonoBehaviour
{
	public float windScale;
	public string winddirection = "";
	// Start is called before the first frame update
	void Start()
    {
		windScale = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay(Collider other)
    {
		if(other.gameObject.tag == "Player")
		{
			other.GetComponent<PlayerController>().WindAssign(winddirection);
		}
		else if(other.gameObject.tag == "Pushable")
		{
			other.GetComponent<WindObstacle>().WindAssign(winddirection);
		}
	}
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			if (winddirection == "left" || winddirection == "right")
			{
				other.GetComponent<PlayerController>().NotHitByHWind();
			}
			else if (winddirection == "up" || winddirection == "down")
			{
				other.GetComponent<PlayerController>().NotHitByVWind();
			}
		}
		else if (other.gameObject.tag == "Pushable")
		{
			if (winddirection == "left" || winddirection == "right")
			{
				other.GetComponent<WindObstacle>().NotHitByHWind();
			}
			else if (winddirection == "up" || winddirection == "down")
			{
				other.GetComponent<WindObstacle>().NotHitByVWind();
			}
		}

	}
}
