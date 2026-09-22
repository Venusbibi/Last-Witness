using System.Collections;
using UnityEngine;
using TMPro;

// สำคัญ: ชื่อคลาสตรงนี้ต้องตรงกับชื่อไฟล์เป๊ะๆ (ในภาพของคุณมี n สองตัว)
public class Dialoguebottonn : MonoBehaviour
{
    [Header("ตั้งค่าข้อความ")]
    public TextMeshProUGUI nameTextComponent; 
    public TextMeshProUGUI textComponent;     
    
    [Header("ใส่ปุ่มตัวเลือกทั้ง 4 ปุ่มที่นี่")]
    public GameObject[] choiceButtons; 

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
        if(textComponent != null) textComponent.text = string.Empty;
        if(nameTextComponent != null) nameTextComponent.text = string.Empty;
            
        foreach(GameObject btn in choiceButtons)
        {
            if(btn != null) btn.SetActive(false);
        }

        if(lines.Length > 0)
        {
            StartDialogue();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent != null && lines.Length > 0)
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
        if(textComponent == null) yield break;

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
            if(textComponent != null) textComponent.text = string.Empty;
            
            if(nameTextComponent != null)
                nameTextComponent.text = lines[index].characterName; 
                
            StartCoroutine(TypeLine());
        }
        else
        {
            foreach(GameObject btn in choiceButtons)
            {
                if(btn != null) btn.SetActive(true);
            }
        }
    }
}