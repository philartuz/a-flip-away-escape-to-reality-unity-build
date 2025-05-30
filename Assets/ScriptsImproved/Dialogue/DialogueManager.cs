using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // for text ui
using TMPro;
using System; // for text mesh pro

public class DialogueManager : MonoBehaviour
{
    public TextMeshPro nameTMP;
    public TextMeshPro dialogueTMP;
    public Animator animatorDialogue;
    public Animator animatorPressF;
    public DialogueTrigger dialogueTrigger;

    // add back if UI 
    // public Text nameText;
    // public Text dialogueText; 

    private Queue<string> sentences;

    public event Action OnDialogueEnd;




    void Start()
    {
        sentences = new Queue<string>();

    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Start Conversation with:" + dialogue.name);

        animatorDialogue.SetBool("isOpen", true);
        animatorPressF.SetBool("isOpen", false);

        //nameText.text = dialogue.name; 
        nameTMP .text = dialogue.name;

        sentences.Clear();

        // queue each sentence made
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        Debug.Log(sentences.Count);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        Debug.Log(sentence);

        //dialogueText.text = sentence;
        //dialogueTMP.text = sentence; 

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
            
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueTMP.text = "";
        float delayTime = 0.025f;

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueTMP.text += letter;
            yield return new WaitForSeconds(delayTime);
        }
    }

    public void EndDialogue()
    {
        Debug.Log("End of Conversation");
        animatorDialogue.SetBool("isOpen", false);
        animatorPressF.SetBool("isOpen", true);
        dialogueTrigger.dialogue_active = false;

        OnDialogueEnd?.Invoke();
    }

   
}
