using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ฟังก์ชันนี้จะรับชื่อฉากที่คุณพิมพ์ใส่ แล้วทำการโหลดฉากนั้น
    public void GoToScene(string sceneName)
    {
        // เปลี่ยนตัวเลข เป็นตัวแปร sceneName แบบนี้ครับ
        SceneManager.LoadScene(sceneName);
    }
}