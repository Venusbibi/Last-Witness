using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats (ตั้งค่าศัตรู)")]
    public float maxHealth = 3f; // เลือดสูงสุด (โดนยิงกี่นัดตาย)
    private float currentHealth;

    void Start()
    {
        // เริ่มเกมมาให้เลือดเต็ม
        currentHealth = maxHealth;
    }

    // ฟังก์ชันนี้จะเปิดรับความเสียหายเมื่อถูกผู้เล่นยิง
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("โดนยิง! เลือดศัตรูเหลือ: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("ศัตรูตายแล้ว!");
        // ลบศัตรูตัวนี้ทิ้งไปเลย (ลบ Object ที่สคริปต์นี้แปะอยู่)
        Destroy(gameObject); 
    }
}