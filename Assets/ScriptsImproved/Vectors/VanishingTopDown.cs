using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingTopDown : MonoBehaviour
{
    public GameObject Floor, Trigger;
    public float delay;
    public bool isGravityY;

    // Vector Stuff
    public GameObject vectorPrefab;
    public float vectorDuration = 2.0f;
    public Transform vectorParent;

    public Vector3 prefabOffset;
    public bool usePrefabDefaultScale = true;
    public Vector3 customPrefabScale = Vector3.one;

    private bool hasInstantiated = false;

    [Header("Audio Manager")]
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }


    void Start()
    {
        isGravityY = true;

        if (usePrefabDefaultScale && vectorPrefab != null)
        {
            customPrefabScale = vectorPrefab.transform.localScale;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isGravityY = !isGravityY;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isGravityY == false && !hasInstantiated) 
        {
            hasInstantiated = true; 

            StartCoroutine(BreakDelay());

            // Instantiate downward arrow beside the platform
            GameObject vectorRepresentation = Instantiate(
                vectorPrefab,
                vectorParent.position + prefabOffset,
                Quaternion.identity,
                vectorParent
            );

            // Scale adjustment
            if (usePrefabDefaultScale && vectorPrefab != null)
            {
                vectorRepresentation.transform.localScale = vectorPrefab.transform.localScale;
            }
            else
            {
                vectorRepresentation.transform.localScale = customPrefabScale;
            }

            // Destroy arrows after a set duration
            Destroy(vectorRepresentation, vectorDuration);
        }
    }

    // Coroutine for delayed actions
    private IEnumerator BreakDelay()
    {
        yield return new WaitForSeconds(delay);
        audioManager.PlaySFX(audioManager.breakable, 0.4f);
        Floor.SetActive(false);
        Trigger.SetActive(false);
        hasInstantiated = false; 
    }
}
