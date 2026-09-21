using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject winPanel;

    [Header("Scene Settings")]
    public string storySceneName = "StoryScene"; // ใส่ชื่อ Scene สำหรับเข้าสู่เนื้อเรื่องและเหตุการณ์

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void Victory()
    {
        if (winPanel != null) winPanel.SetActive(true);
        Time.timeScale = 0f; // หยุดเวลาตอนชนะ
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); 
    }

    // ฟังก์ชันเปลี่ยนซีนเข้าสู่เนื้อเรื่อง (ตาม Game Loop)
    public void GoToStoryScene()
    {
        Time.timeScale = 1f; // คืนค่าเวลาก่อนเปลี่ยนฉาก
        SceneManager.LoadScene(3);
    }
}