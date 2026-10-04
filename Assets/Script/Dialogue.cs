using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // เพิ่มคำสั่งสำหรับการจัดการ Scene

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI nameTextComponent; 
    public TextMeshProUGUI textComponent;     
    
    // --- สร้างตัวเลือก Dropdown สำหรับเหตุการณ์ตอนคุยจบ ---
    public enum EndActionType
    {
        ShowCanvas,
        ChangeScene
    }

    [Header("เหตุการณ์เมื่อคุยจบ")]
    public EndActionType actionAfterDialogue;

    [Tooltip("ถ้าเลือก Show Canvas: ลาก Canvas หรือปุ่มมาใส่ที่นี่")]
    public GameObject objectToShow;

    [Tooltip("ถ้าเลือก Change Scene: พิมพ์ชื่อ Scene ที่ต้องการเปลี่ยน (อย่าลืมแอด Scene ลง Build Settings)")]
    public string sceneToLoad;
    // ---------------------------------------------------

    [System.Serializable]
    public class DialogueLine 
    {
        public string characterName; 
        [TextArea(3, 10)]
        public string sentence;      
    }

    public DialogueLine[] lines;
    public float textSpeed;
    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        if(nameTextComponent != null) 
            nameTextComponent.text = string.Empty;
            
        // ตอนเริ่มเกม ให้บังคับซ่อน UI ไว้ก่อนอัตโนมัติ (เฉพาะกรณีที่เลือกแบบ Show Canvas)
        if(actionAfterDialogue == EndActionType.ShowCanvas && objectToShow != null)
        {
            objectToShow.SetActive(false);
        }

        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index].sentence)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index].sentence;
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        if(nameTextComponent != null)
            nameTextComponent.text = lines[index].characterName; 
            
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].sentence.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            if(nameTextComponent != null)
                nameTextComponent.text = lines[index].characterName; 
                
            StartCoroutine(TypeLine());
        }
        else
        {
            // --- ทำงานตามคำสั่งที่เลือกไว้ใน Inspector เมื่อคุยจบหน้าสุดท้าย ---
            if (actionAfterDialogue == EndActionType.ShowCanvas)
            {
                if (objectToShow != null)
                {
                    objectToShow.SetActive(true);
                }
            }
           else if (actionAfterDialogue == EndActionType.ChangeScene)
            {
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    Debug.Log("กำลังพยายามเปลี่ยนไปที่ซีน: " + sceneToLoad); // เช็คว่าบรรทัดนี้โผล่ใน Console ไหม
                    SceneManager.LoadScene(sceneToLoad);
                }
                else
                {
                    Debug.LogWarning("ยังไม่ได้ใส่ชื่อ Scene ใน Inspector!");
                }
            }

            // ปิดหน้าต่างบทสนทนาทิ้งไป
            gameObject.SetActive(false); 
        }
    }
}