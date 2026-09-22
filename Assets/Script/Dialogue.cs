using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI nameTextComponent; 
    public TextMeshProUGUI textComponent;     
    
    // --- โค้ดที่เพิ่มใหม่: ช่องสำหรับใส่ปุ่มที่อยากให้โผล่มาตอนคุยจบ ---
    [Header("ปุ่มที่จะให้โผล่มาตอนคุยจบ")]
    public GameObject buttonToShowAfterDialogue; 
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
            
        // ตอนเริ่มเกม ให้บังคับซ่อนปุ่มไว้ก่อนอัตโนมัติ
        if(buttonToShowAfterDialogue != null)
        {
            buttonToShowAfterDialogue.SetActive(false);
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
            // --- โค้ดที่แก้ไขใหม่: เมื่อคุยจบหน้าสุดท้าย ---
            // 1. เปิดปุ่มให้แสดงขึ้นมา
            if(buttonToShowAfterDialogue != null)
            {
                buttonToShowAfterDialogue.SetActive(true); 
            }
            // 2. ปิดหน้าต่างบทสนทนาทิ้งไป
            gameObject.SetActive(false); 
        }
    }
}