using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public PlayerMovement playerScript;

    void Start() {
        pauseMenuUI.SetActive(false);
        GameObject player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isGamePaused)
            {
                Resume();
            }
            else {
                Pause(); 
            }
        }
        
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        isGamePaused = false;
        playerScript.enabled = true;
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        playerScript.enabled = false;

    }

    public void LoadMainMenu() {
        StartCoroutine(LoadSceneWithDelay("Main Menu", 0.5f));
        Time.timeScale = 1f; // prob rn is that charac moves after HAHAH
        //SceneManager.LoadScene("Main Menu");
        
    }

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

    }
}
