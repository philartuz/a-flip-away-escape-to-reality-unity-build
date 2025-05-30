using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public Animator animatorDialogue;
    public Animator animatorPressF;
    public Dialogue dialogue;
    bool player_detection = false;
    public bool dialogue_active = false;

    public GameObject speakersObject;
    public Animator speakersAnimator;

    [Header("Audio Manager")]
    private AudioManager audioManager;

    [Header("Player Camera")]
    private Camera playerCamera;
    private Vector3 defaultCameraPosition;
    private float defaultSize;
    public float zoomedSize = 4f;
    public float zoomSpeed = 2f;
    public Transform npcTransform; // Assign the player's transform in Inspector


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        playerCamera = Camera.main; 
        defaultSize = playerCamera.orthographicSize;
        defaultCameraPosition = playerCamera.transform.position; // Store original position
    }

    private void Start()
    {
        dialogueManager.OnDialogueEnd += HandleDialogueEnd;
    }



    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
        //Object.FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);
    }

    private void Update()
    {
        speakersAnimator = speakersObject.GetComponent<Animator>();

        // inside collider and press f for first time
        if (!dialogue_active && player_detection && Input.GetKeyDown(KeyCode.F))
        {
            // Calculate new camera position to focus between Player and NPC
            Vector3 focusPoint = (npcTransform.position + speakersObject.transform.position) / 2;
            Vector3 newCameraPosition = new Vector3(focusPoint.x, focusPoint.y, playerCamera.transform.position.z);

            StartCoroutine(AdjustCamera(newCameraPosition, zoomedSize));
            TriggerDialogue();
            dialogue_active = true;
            speakersAnimator.SetBool("isPlaying", true);
            audioManager.StartDialogue();

        }

        // inside collider and press f after activating first dialogue
        else if (dialogue_active && player_detection && Input.GetKeyDown(KeyCode.F))
        {
            dialogueManager.DisplayNextSentence();
            //Object.FindFirstObjectByType<DialogueManager>().DisplayNextSentence();
        }

        if (!dialogue_active && player_detection)
        {
            speakersAnimator.SetBool("isPlaying", false);
            audioManager.StopDialogue();

        }



    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("u r within npc vicinity");
        if (other.name == "Player" || other.CompareTag("Player")) 
        {
            player_detection = true;
            animatorPressF.SetBool("isOpen", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        player_detection = false;
        dialogueManager.EndDialogue();
        //Object.FindFirstObjectByType<DialogueManager>().EndDialogue();
        animatorDialogue.SetBool("isOpen", false);
        animatorPressF.SetBool("isOpen", false);
        speakersAnimator.SetBool("isPlaying", false);
        audioManager.StopDialogue();
    }

    private IEnumerator AdjustCamera(Vector3 targetPosition, float targetSize)
    {
        float elapsed = 0f;
        float duration = 1f; // Increased duration for smoother transition
        Vector3 startPosition = playerCamera.transform.position;
        float startSize = playerCamera.orthographicSize;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // SmoothStep formula for ease-in/out

            playerCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            playerCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        playerCamera.transform.position = targetPosition;
        playerCamera.orthographicSize = targetSize;
    }

    private void HandleDialogueEnd()
    {
        StartCoroutine(AdjustCamera(defaultCameraPosition, defaultSize));
    }

}
        