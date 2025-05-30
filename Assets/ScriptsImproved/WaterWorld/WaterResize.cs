using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterResize : MonoBehaviour
{
	[Header("Audio Manager")]
	private AudioManager audioManager;
	[Header("Scene")]
	public string scenename;
	[Header("Variables")]
	Vector3 fullsize;
	Vector3 lowestsize;
	public bool isplatformer;
	public bool isTouchingPlayer;
	public bool isDying;
	public float maxheight;
	public float height2;
	public float risetime;
	public bool isBG;
	private Coroutine scaleCoroutine;
	public float deathTimer;
	private Vector3 originalfullsize = Vector3.zero;
	public Collider BasePlayer;
	public bool getIsPlatformer()
	{
		return isplatformer;
	}
	private void Awake()
	{
		audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
	}
	private void Start()
	{
		fullsize = new Vector3(transform.localScale.x, maxheight, transform.localScale.z);
		lowestsize = new Vector3(transform.localScale.x, height2, transform.localScale.z);
		//Call the function giving it a target scale (Vector3) and a duration (float).
		ScaleToTarget(fullsize, risetime);

		isplatformer = true;
		if (!isBG)
		{
			originalfullsize = fullsize;
		}
	}
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			if (!isBG)
			{
				if (isplatformer)
				{
					ScaleToTarget(lowestsize, risetime * (transform.localScale.y / maxheight));
					isplatformer = false;
				}
				else if (!isplatformer)
				{
					ScaleToTarget(fullsize, risetime * (1.0f - (transform.localScale.y / maxheight)));
					isplatformer = true;
				}
			}
			if (isBG)
			{
				if (isplatformer)
				{
					ScaleToTarget(lowestsize, risetime * (1.0f - (transform.localScale.y / height2)));
					isplatformer = false;
				}
				else if (!isplatformer)
				{
					ScaleToTarget(fullsize, risetime * (transform.localScale.y / height2));
					isplatformer = true;
				}
			}


		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!isBG)
		{
			if (other.CompareTag("Pushable"))
			{
				if (!other.GetComponent<ObjectMovement>().getIsSubmerged())
				{
					other.GetComponent<ObjectMovement>().setSubmerged();
					AddMaxHeight(4.0f);
				}
			}
		}

	}
	public void OnTriggerStay(Collider other)
	{
		if (isBG) return;
		if (other.gameObject.name.Equals("BasePlayerTrigger"))
		{
			if (!isTouchingPlayer)
			{
				isTouchingPlayer = true;
				deathTimer = 0f;
			}
			deathTimer += Time.deltaTime;

			if (deathTimer >= 0.25f)
			{
				if (SceneManager.GetActiveScene().name == scenename)
				{
					if (!isDying)
					{
						audioManager.PlaySFX(audioManager.death, 0.3f);
						isDying = true;
					}
					StartCoroutine(LoadSceneWithDelay(scenename, 0.8f));
				}
			}
		}
	}
	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name.Equals("BasePlayerTrigger"))
		{
			isTouchingPlayer = false;
		}
	}

	public void ScaleToTarget(Vector3 targetScale, float duration)
	{
		// Stop the previous scaling coroutine if it's running
		if (scaleCoroutine != null)
		{
			StopCoroutine(scaleCoroutine);
		}
		// Start a new scaling coroutine and save its reference
		scaleCoroutine = StartCoroutine(ScaleToTargetCoroutine(targetScale, duration));
	}
	public IEnumerator LoadSceneWithDelay(string sceneName, float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(sceneName);
	}
	private IEnumerator ScaleToTargetCoroutine(Vector3 targetScale, float duration)
	{
		Vector3 startScale = transform.localScale;
		float timer = 0.0f;

		while (timer < duration)
		{
			timer += Time.deltaTime;
			float t = timer / duration;
			t = t * t * t * (t * (6f * t - 15f) + 10f); // smoother step
			transform.localScale = Vector3.Lerp(startScale, targetScale, t);
			yield return null;
		}

		// Ensure the target scale is reached at the end
		transform.localScale = targetScale;
		scaleCoroutine = null;
	}
	public bool getIsBG()
	{
		return isBG;
	}
	public void AddMaxHeight(float addition)
	{
		if (!isBG)
		{
			maxheight += addition;
			fullsize = new Vector3(transform.localScale.x, maxheight, transform.localScale.z);
			if (isplatformer)
			{
				ScaleToTarget(fullsize, risetime * (1.0f - (transform.localScale.y / maxheight)));
			}
		}
	}
}