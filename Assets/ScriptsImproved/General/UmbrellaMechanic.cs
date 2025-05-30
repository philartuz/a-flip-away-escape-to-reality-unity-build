using System.Collections.Specialized;
using UnityEngine;

public class UmbrellaMechanic : MonoBehaviour
{
	public PlayerMovement Player;
	public Vector3 directionnegated;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Player.isUmbrellaEquipped() && Player.getUmbrellaAnchored() == false)
		{
			// Get the current direction from the PlayerMovement script
			int currentDirection = Player.getCurrentDirection();
			if (currentDirection == 1)
			{
				directionnegated = new Vector3(-1.0f, 0.0f, 0.0f);
			}
			else if (currentDirection == 0)
			{
				directionnegated = new Vector3(1.0f, 0.0f, 0.0f);
			}
			else if (currentDirection == 3)
			{
				directionnegated = new Vector3(0.0f, -1.0f, 0.0f);
			}
			else if (currentDirection == 2)
			{
				directionnegated = new Vector3(0.0f, 1.0f, 0.0f);
			}
			Player.AffectedByUmbrella(directionnegated);
		}
	}
	private void OnTriggerStay(Collider other)
	{

		if (other.CompareTag("Player"))
		{
			if (Player != null)
			{
				Player.AffectedByUmbrella(directionnegated);
			}
		}
		if (other.gameObject.name == "WindBall")
		{
			other.gameObject.GetComponent<WindBall>().isUmbrellad(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if (Player != null)
			{
				Player.AffectedByUmbrella(Vector3.zero);
			}
		}
		if (other.gameObject.name == "WindBall")
		{
			other.gameObject.GetComponent<WindBall>().isUmbrellad(false);
		}
	}
}
