using System.Collections;
using UnityEngine;

public class WaterPlatform : MonoBehaviour
{
	public bool isplatformer = true;
	private bool isFloating = false;
	private bool isSinking = false;
	private Transform currentWater; // Tracks the platform water
	public float sinkDelay = 0.5f; // Delay before sinking
	public float sinkAmount = 2f; // How much it sinks
	public float sinkTime = 2f; // Time taken to sink/rise
	public float gravitySpeed = 9.8f; // Simulated gravity speed

    [Header("Animations")]
    public Animator platformAnimator;
	public GameObject vector_bouyancy;

    public void Start()
    {
        platformAnimator = vector_bouyancy.GetComponent<Animator>();
    }

    void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			isplatformer = !isplatformer;
		}

		if (!isFloating && !isSinking && isplatformer)
		{
			// Simulated gravity when NOT in water
			transform.position += Vector3.down * gravitySpeed * Time.deltaTime;
		}
		else if (isFloating && !isSinking && currentWater != null)
		{
			// Move to match the water surface
			float waterSurfaceY = currentWater.GetComponent<Collider>().bounds.max.y;
			transform.position = new Vector3(transform.position.x, waterSurfaceY, transform.position.z);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("TriggerWater"))
		{
			WaterResize water = other.GetComponent<WaterResize>();

			if (water != null && !water.getIsBG() && isplatformer)
			{
				currentWater = other.transform;
				isFloating = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("TriggerWater") && other.transform == currentWater)
		{
			isFloating = false;
			currentWater = null;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player") && isFloating)
		{
			if (!isSinking)
			{
                platformAnimator.SetBool("isSteppedOn", true);
                StartCoroutine(SinkAndRise());

			}
		}
	}

	private IEnumerator SinkAndRise()
	{
		isSinking = true;

		yield return new WaitForSeconds(sinkDelay);

		Vector3 sinkPosition = transform.position - new Vector3(0, sinkAmount, 0);
		yield return MoveToPosition(sinkPosition, sinkTime);

		yield return new WaitForSeconds(2f);

		if (currentWater != null)
		{
			float waterSurfaceY = currentWater.GetComponent<Collider>().bounds.max.y;
			Vector3 risePosition = new Vector3(transform.position.x, waterSurfaceY, transform.position.z);
			yield return MoveToPosition(risePosition, sinkTime);
		}


		isSinking = false;
        platformAnimator.SetBool("isSteppedOn", false);
    }

	private IEnumerator MoveToPosition(Vector3 targetPos, float duration)
	{
		float elapsedTime = 0f;
		Vector3 startPos = transform.position;

		while (elapsedTime < duration)
		{
			transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		transform.position = targetPos;
	}
}
