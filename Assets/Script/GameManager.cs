using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Scene Settings")]
    public string storySceneName = "StoryScene"; 

    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f; 
    }

    public void Victory()
    {
        if (winPanel != null) 
        {
            winPanel.SetActive(true);
        }
        Time.timeScale = 0f; 
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBGL
        Application.OpenURL("about:blank");
        #else
        Application.Quit();
        #endif
    }

    public void GoToStoryScene()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(3);
    }
}