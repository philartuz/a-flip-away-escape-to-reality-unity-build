using System.Collections;
using UnityEngine;

public class WindBall : MonoBehaviour
{
	public bool isplatformer = true;
	private bool isFloating = false;
	private bool isSinking = false;
	private Transform currentWindZone; // Tracks the platform water
	public float sinkDelay = 0.5f; // Delay before sinking
	public float sinkAmount = 2f; // How much it sinks
	public float sinkTime = 2f; // Time taken to sink/rise
	public float gravitySpeed = 9.8f; // Simulated gravity speed
	public bool affectedbyumbrella = false;
	[Header("Animations")]
	public Animator platformAnimator;
	public GameObject vector_bouyancy;
	public bool istouchingSensor = false;
	public bool islocked = false;

	public float targetZPosition = -2f; // Set your desired world Z position
	public float zMoveSpeed = 5f; // Speed of smoothing

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			isplatformer = !isplatformer;
			if (isplatformer && istouchingSensor)
			{
				islocked = true;
				// Start moving to fixed Z when switching
				StartCoroutine(MoveToFixedZ(targetZPosition, 0.5f));
			}
		}

		if (!isFloating && !isSinking && isplatformer && !islocked)
		{
			// Simulated gravity when NOT in wind
			transform.position += Vector3.down * gravitySpeed * Time.deltaTime;
		}
		else if (isFloating && !isSinking && currentWindZone != null)
		{
			// Move to match the wind surface
			float waterSurfaceY = currentWindZone.GetComponent<Collider>().bounds.max.y;
			transform.position = new Vector3(transform.position.x, waterSurfaceY, transform.position.z);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("WindZone") && !islocked)
		{
			if (!affectedbyumbrella)
			{
				WindArea windzone = other.GetComponent<WindArea>();

				if (isplatformer)
				{
					currentWindZone = other.transform;
					isFloating = true;
				}
			}
		}
		if (other.CompareTag("Sensor"))
		{
			istouchingSensor = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("WindZone") && other.transform == currentWindZone)
		{
			isFloating = false;
			currentWindZone = null;
		}
	}

	private IEnumerator MoveToFixedZ(float targetZ, float duration)
	{
		float elapsedTime = 0f;
		Vector3 startPos = transform.position;
		Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, targetZ);

		while (elapsedTime < duration)
		{
			transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.position = targetPos; // Ensure it reaches the exact target
	}

	public void isUmbrellad(bool umbrellad)
	{
		affectedbyumbrella = umbrellad;
		if (umbrellad == true)
		{
			isFloating = false;
		}
	}
}
