using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // เพิ่ม namespace สำหรับ UI

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    // เพิ่มตัวแปรสำหรับเก็บปุ่มช้อยส์ทั้งหมด
    public GameObject[] choiceButtons;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        
        // ซ่อนปุ่มทั้งหมดไว้ตอนเริ่มต้น
        HideButtons();
        
        StartDialogue();
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        // พิมพ์ตัวอักษรทีละตัว
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // เมื่อพิมพ์ข้อความจบแล้ว ให้แสดงปุ่มช้อยส์ขึ้นมา
        ShowButtons();
    }

    void HideButtons()
    {
        foreach (GameObject btn in choiceButtons)
        {
            if (btn != null)
                btn.SetActive(false); // ปิดการแสดงผลปุ่ม
        }
    }

    void ShowButtons()
    {
        foreach (GameObject btn in choiceButtons)
        {
            if (btn != null)
                btn.SetActive(true); // เปิดการแสดงผลปุ่มเมื่อพิมพ์เสร็จ
        }
    }
}