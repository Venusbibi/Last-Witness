using UnityEngine;
using System.Collections;

public class DelayedButton : MonoBehaviour
{
    // ลากปุ่ม "ไปต่อ" มาใส่ในช่องนี้ผ่าน Inspector
    public GameObject nextButton; 
    // ระยะเวลากี่วินาทีก่อนปุ่มจะแสดงขึ้นมา (ตั้งไว้ 5 วินาที)
    public float delayTime = 5f; 

    void Start()
    {
        // เริ่มต้นให้ปุ่มซ่อนอยู่
        if (nextButton != null)
        {
            nextButton.SetActive(false);
            // เริ่มนับเวลาถอยหลัง
            StartCoroutine(ShowButtonRoutine());
        }
    }

    IEnumerator ShowButtonRoutine()
    {
        // รอเวลาตามที่กำหนด (5 วินาที)
        yield return new WaitForSeconds(delayTime);

        // แสดงปุ่มขึ้นมา
        if (nextButton != null)
        {
            nextButton.SetActive(true);
        }
    }
}