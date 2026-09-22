using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameData : MonoBehaviour
{
    public static GameData instance;

    [Header("Player Persistent Stats")]
    public int currentAmmo = 6; // บังคับให้เป็น 6
    public int maxAmmo = 6;     // บังคับให้เป็น 6

    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // ถ้ามีตัวเดิมอยู่แล้ว ให้สั่งอัปเดตบังคับค่ากระสุนให้เป็น 6 เต็มด้วย (ป้องกันมันจำค่าเก่า 3 นัด)
            instance.currentAmmo = 6;
            instance.maxAmmo = 6;
            Destroy(gameObject); 
        }
    }

    public void GoToStoryScene()
    {
        PlayerShooting player = FindObjectOfType<PlayerShooting>();
        if (player != null)
        {
            // บันทึกค่ากระสุนปัจจุบันก่อนเปลี่ยนฉาก
        }

        Time.timeScale = 1f; 
        SceneManager.LoadScene(4); 
    }

    public class SceneController : MonoBehaviour
{
    public void RestartScene()
    {
        Time.timeScale = 1f; // คืนค่าเวลาให้เกมเดินปกติ
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // โหลดฉากปัจจุบันใหม่
    }

    public void GoToMainScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // หรือใส่ชื่อฉากเมนูหลักของคุณ
    }
}
}