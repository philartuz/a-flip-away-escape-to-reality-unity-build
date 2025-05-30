using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    [Space(5)]
    [Header("Main Menu")]
    public string levelToLoad;

    [Space(5)]
    [Header("Loading Screen")]
    public GameObject LoadingScreen;
    public Image LoadingBarFill;


    #region Main Menu Things
    public void PlayGame()
    {
        StartCoroutine(LoadSceneAsync("World1_0_A")); 
    }
	public void PlayCredits()
	{
		StartCoroutine(LoadSceneAsync("Credits"));
	}
	public void ReturnMenu()
	{
		StartCoroutine(LoadSceneAsync("Main Menu"));
	}
	public void QuitGame()
    {
        Application.Quit(); 
    }

    public void LoadLevel(string levelToLoad)
    {
        StartCoroutine(LoadSceneAsync(levelToLoad));
    }
    #endregion


    #region Loading Screen

    IEnumerator LoadSceneAsync(string levelToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelToLoad);
        operation.allowSceneActivation = false; 

        LoadingScreen.SetActive(true);

        float fakeProgress = 0f; 

        // Artificial delay
        float minimumLoadTime = 2f; 
        float elapsedTime = 0f;

        while (fakeProgress < 1f || operation.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;

            fakeProgress = Mathf.MoveTowards(fakeProgress, 1f, Time.deltaTime * 0.5f);

            if (elapsedTime >= minimumLoadTime)
            {
                fakeProgress = Mathf.Clamp01(operation.progress / 0.9f);
            }

            LoadingBarFill.fillAmount = fakeProgress;

            yield return null;
        }

        operation.allowSceneActivation = true;
    }


    #endregion


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
