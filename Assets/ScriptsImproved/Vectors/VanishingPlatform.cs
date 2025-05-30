using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VanishingPlatform : MonoBehaviour
{

    [Header("Momentum Settings")]
    public GameObject Floor;
    public GameObject Trigger;
    public float baseDelay = 5.0f; 
    public float minDelay = 0.1f;
    public float momentumScaling = 0.12f;
    private float momentum;
    private float maxVelocity;
    private float momentumPushable;
    private float maxVelocityPushable;

    [Space(10)]
    [Header("Vector Initialization")]
    public GameObject vectorPrefab;
    public float vectorDuration = 5.0f;
    public Transform vectorParent;
    private Transform downArrow, upArrow;
    private Transform downArrowBody, upArrowBody;

    [Space(10)]
    [Header("RigidBody References")]
    public GameObject player;
    public Rigidbody rbPlayer;
    public GameObject pushable;
    public Rigidbody rbPush;



    [Space(10)]
    [Header("Prefab Customization")]
    public Vector3 prefabOffset;
    public bool usePrefabDefaultScale = true;


    [Header("Audio Manager")]
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rbPlayer = player.GetComponent<Rigidbody>();


    }

    private void Update()
    {
        maxVelocity = 0;  
        momentum = rbPlayer.mass * rbPlayer.linearVelocity.y;

        float currentVelocity = Mathf.Abs(rbPlayer.linearVelocity.y);

        if (currentVelocity > maxVelocity)
        {
            maxVelocity = currentVelocity;
        }

        momentum = rbPlayer.mass * maxVelocity;
        Debug.Log("Max Recorded Velocity: " + maxVelocity);

        // FOR PUSHABLE
        pushable = FindLatestPushable(player.transform);

        // If we found a pushable, update its Rigidbody reference
        if (pushable != null)
        {
            rbPush = pushable.GetComponent<Rigidbody>();
            Debug.Log("PUSHABLE VELOCITY " + rbPush.linearVelocity.y);
        }

        if (rbPush != null) {

            maxVelocityPushable = 0;
            momentumPushable = rbPush.mass * rbPush.linearVelocity.y;

            float currentVelocityPushable = Mathf.Abs(rbPush.linearVelocity.y);

            if (currentVelocityPushable > maxVelocityPushable)
            {
                maxVelocityPushable = currentVelocityPushable;
            }

            momentumPushable = rbPush.mass * maxVelocityPushable;
            Debug.Log("Max Recorded Velocity Pushable: " + maxVelocityPushable);
        }

        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Rigidbody RB = other.GetComponent<Rigidbody>();
            //RB.mass = 1.0f;
            float adjustedDelay = baseDelay;

            // float momentum = RB.mass * RB.linearVelocity.y;
            float scaledMomentum = momentum * momentumScaling;

            #region Momentum Debugging
            //Debug.Log("Current Mass:" + RB.mass);
            //Debug.Log("Recorded Velocity" + rbPlayer.linearVelocity.y);
            //Debug.Log("Current Momentum:" + momentum);
            //Debug.Log("New Momentum:" + scaledMomentum);
            #endregion

            // Adjust delay inversely proportional to momentum
            adjustedDelay = Mathf.Max(minDelay, baseDelay - scaledMomentum);
            Debug.Log("New Delay:" + adjustedDelay);
            StartCoroutine(BreakDelay(adjustedDelay));


            GameObject vectorRepresentation = Instantiate(
                vectorPrefab,
                vectorParent.position + prefabOffset,
                Quaternion.identity,
                vectorParent
            );


            downArrow = vectorRepresentation.transform.GetChild(0);
            upArrow = vectorRepresentation.transform.GetChild(1);

            downArrowBody = downArrow.transform.GetChild(0);
            upArrowBody = upArrow.transform.GetChild(0);


            if (downArrowBody != null && upArrow != null)
            {
                float baseScale = 0.6f;

                // scaling for arrow
                float targetDownScaleY = baseScale + (momentum * 0.1f);
                float targetUpScaleY = baseScale + (momentum * 0.05f);

                // positioning for arrow
                float targetDownPositionY = baseScale - (momentum * 0.1f);
                float targetUpPositionY = baseScale - (momentum * 0.2f);

                if (targetDownPositionY > 0) {
                    targetUpPositionY = -1;
                    targetDownScaleY = 1; 
                }
                if (targetUpPositionY > 0)
                {
                    targetUpPositionY = -1;
                }

                Debug.Log("LOOK HERE targetDownPositionY:" + targetDownPositionY);
                Debug.Log("LOOK HERE targetUpPositionY  :" + targetUpPositionY);

                // Animate the down arrow
                StartCoroutine(AnimateArrow(
                    downArrowBody,
                    downArrowBody.localScale,
                    new Vector3(downArrowBody.localScale.x, targetDownScaleY, downArrowBody.localScale.z),
                    downArrow,
                    downArrow.localPosition,
                    new Vector3(downArrow.localPosition.x, targetDownPositionY, downArrow.localPosition.z),
                    0.7f
                ));

                // Animate the up arrow
                StartCoroutine(AnimateArrow(
                    upArrowBody,
                    upArrowBody.localScale,
                    new Vector3(upArrowBody.localScale.x, targetUpScaleY, upArrowBody.localScale.z),
                    upArrow,
                    upArrow.localPosition,
                    new Vector3(upArrow.localPosition.x, targetUpPositionY, upArrow.localPosition.z),
                    0.7f
                ));
            }

            maxVelocity = 0;
            Destroy(vectorRepresentation, vectorDuration);

        }

        if (other.CompareTag("Pushable"))
        {
            float adjustedDelay = baseDelay;

            float scaledMomentum = momentumPushable * momentumScaling;

            #region Momentum Debugging
            //Debug.Log("Current Mass:" + RB.mass);
            //Debug.Log("Recorded Velocity" + rbPlayer.linearVelocity.y);
            //Debug.Log("Current Momentum:" + momentum);
            //Debug.Log("New Momentum:" + scaledMomentum);
            #endregion

            // Adjust delay inversely proportional to momentum
            adjustedDelay = Mathf.Max(minDelay, baseDelay - scaledMomentum);
            Debug.Log("New Delay:" + adjustedDelay);
            StartCoroutine(BreakDelay(adjustedDelay));


            GameObject vectorRepresentation = Instantiate(
                vectorPrefab,
                vectorParent.position + prefabOffset,
                Quaternion.identity,
                vectorParent
            );


            downArrow = vectorRepresentation.transform.GetChild(0);
            upArrow = vectorRepresentation.transform.GetChild(1);

            downArrowBody = downArrow.transform.GetChild(0);
            upArrowBody = upArrow.transform.GetChild(0);


            if (downArrowBody != null && upArrow != null)
            {
                float baseScale = 0.6f;

                // scaling for arrow
                float targetDownScaleY = baseScale + (momentumPushable * 0.1f);
                float targetUpScaleY = baseScale + (momentumPushable * 0.05f);

                // positioning for arrow
                float targetDownPositionY = baseScale - (momentumPushable * 0.1f);
                float targetUpPositionY = baseScale - (momentumPushable * 0.2f);


                // Animate the down arrow
                StartCoroutine(AnimateArrow(
                    downArrowBody,
                    downArrowBody.localScale,
                    new Vector3(downArrowBody.localScale.x, targetDownScaleY, downArrowBody.localScale.z),
                    downArrow,
                    downArrow.localPosition,
                    new Vector3(downArrow.localPosition.x, targetDownPositionY, downArrow.localPosition.z),
                    0.7f
                ));

                // Animate the up arrow
                StartCoroutine(AnimateArrow(
                    upArrowBody,
                    upArrowBody.localScale,
                    new Vector3(upArrowBody.localScale.x, targetUpScaleY, upArrowBody.localScale.z),
                    upArrow,
                    upArrow.localPosition,
                    new Vector3(upArrow.localPosition.x, targetUpPositionY, upArrow.localPosition.z),
                    0.7f
                ));
            }

            maxVelocityPushable = 0;
            Destroy(vectorRepresentation, vectorDuration);

        }
    }


   private IEnumerator AnimateArrow(Transform arrow, Vector3 initialScale, Vector3 targetScale, Transform arrowMain, Vector3 initialPosition, Vector3 targetPosition, float duration)
    {
        float elapsed = 0f;
        //Debug.Log("Animation Started for: " + arrow.name);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            arrow.localScale = Vector3.Lerp(initialScale, targetScale, t);
            arrowMain.localPosition = Vector3.Lerp(initialPosition, targetPosition, t);

            //Debug.Log($"Animating {arrow.name}: t={t}, scale={arrow.localScale}, position={arrowMain.localPosition}");

            yield return null;
        }

        // Ensure final values are set
        arrow.localScale = targetScale;
        arrowMain.localPosition = targetPosition;
        //Debug.Log("Animation Completed for: " + arrow.name);
    }



    private IEnumerator BreakDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioManager.PlaySFX(audioManager.breakable, 0.2f);
        Floor.SetActive(false);

        yield return new WaitForSeconds (vectorDuration);
        Trigger.SetActive(false);
    }

    private GameObject FindLatestPushable(Transform parent)
    {
        GameObject latestPushable = null;

        foreach (Transform child in parent)
        {
            if (child.CompareTag("Pushable"))
            {
                latestPushable = child.gameObject; // Always update to the latest found
            }

            // Recursively check deeper children
            GameObject nestedPushable = FindLatestPushable(child);
            if (nestedPushable != null)
            {
                latestPushable = nestedPushable;
            }
        }

        return latestPushable;
    }
 
}
