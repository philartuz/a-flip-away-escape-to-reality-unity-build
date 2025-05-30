using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public string scenename;

    [Header("Audio Manager")]
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }
    

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name.Equals("BasePlayerTrigger"))
		{
            if (SceneManager.GetActiveScene().name == scenename)
            {
                audioManager.PlaySFX(audioManager.death, 0.3f);
                StartCoroutine(LoadSceneWithDelay(scenename, 0.8f));

            }

            else {
                audioManager.PlaySFX(audioManager.goal, 0.5f);
                StartCoroutine(LoadSceneWithDelay(scenename, 2.0f));
            }
			
		}
	}

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}