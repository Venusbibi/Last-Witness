using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats (ตั้งค่าเลือด)")]
    public int maxHealth = 6; // เลือดสูงสุด 6 ช่องตามรูป
    private int currentHealth;

    [Header("UI Settings (เชื่อมต่อหน้าจอ)")]
    [Tooltip("ลากรูปช่องเลือด HP1 ถึง HP6 มาใส่เรียงตามลำดับ")]
    public GameObject[] healthBlocks; 

    void Start()
    {
        // เริ่มเกมมาให้เลือดเต็ม 6 ช่อง
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    void Update()
    {
        // ระบบทดสอบ: กดปุ่ม T เพื่อจำลองการโดนศัตรูโจมตี 1 ดาเมจ
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(1); 
        }
    }

    // ฟังก์ชันรับดาเมจ (ศัตรูจะเรียกใช้ฟังก์ชันนี้เวลายิงโดนเรา)
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        // ป้องกันไม่ให้เลือดติดลบ
        if (currentHealth < 0) currentHealth = 0;
        
        Debug.Log("ผู้เล่นโดนโจมตี! เลือดเหลือ: " + currentHealth);
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        // วนลูปเพื่อเปิด/ปิดรูปช่องเลือดตามจำนวนเลือดที่เหลืออยู่
        for (int i = 0; i < healthBlocks.Length; i++)
        {
            if (i < currentHealth)
            {
                healthBlocks[i].SetActive(true);  // เลือดเหลือ -> โชว์ช่องสีแดง
            }
            else
            {
                healthBlocks[i].SetActive(false); // เลือดลด -> ซ่อนช่องสีแดง (หรือถ้ามีรูปช่องว่างสีดำ ก็สั่งเปลี่ยนรูปแทนการ SetActive ได้)
            }
        }
    }

    void Die()
    {
        Debug.Log("ผู้เล่นตาย! (Game Over)");
    }
}