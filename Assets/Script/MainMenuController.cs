using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "Main Menu";

    // สร้างช่องสำหรับลาก Panel Options มาใส่
    public GameObject optionsPanel; 

    [Header("ระบบเสียงกดปุ่ม")]
    public AudioSource audioSource;     // ลาก Audio Source มาใส่
    public AudioClip clickSound;        // ลากไฟล์เสียงคลิกมาใส่

    // ฟังก์ชันเล่นเสียงกลาง
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Cut Scenes"); 
    }

    // ฟังก์ชันสำหรับเปิดหน้า Options
    public void OpenOptions()
    {
        if(optionsPanel != null)
        {
            optionsPanel.SetActive(true); // สั่งให้ Panel แสดงขึ้นมา
        }
    }

    // ฟังก์ชันสำหรับปิดหน้า Options (ใช้กับปุ่มกากบาท)
    public void CloseOptions()
    {
        if(optionsPanel != null)
        {
            optionsPanel.SetActive(false); // สั่งให้ Panel ซ่อนกลับไป
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game"); 
        Application.Quit();     
    }
}