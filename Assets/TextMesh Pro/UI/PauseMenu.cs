using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public CanvasGroup mainCanvas;
    public CanvasGroup bossCanvas;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        mainCanvas.alpha = 1f;
        mainCanvas.interactable = true;
        mainCanvas.blocksRaycasts = true;
        bossCanvas.alpha = 1f;
        bossCanvas.interactable = true;
        bossCanvas.blocksRaycasts = true;
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        mainCanvas.alpha = 0f;
        mainCanvas.interactable = false;
        mainCanvas.blocksRaycasts = false;
        bossCanvas.alpha = 0f;
        bossCanvas.interactable = false;
        bossCanvas.blocksRaycasts = false;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
