using UnityEngine;
using UnityEngine.UI; 

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 2; 
    public float currentHealth;

    [Header("UI Settings (เชื่อมต่อหน้าจอ)")]
    [Tooltip("ลาก UI ช่องเลือดบนหัวศัตรูมาใส่ตามลำดับ (ช่อง 1, ช่อง 2, ...)")]
    public GameObject[] healthBlocks; // เปลี่ยนจาก Image เป็น GameObject Array

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("ศัตรูโดนยิง! เลือดเหลือ: " + currentHealth);

        UpdateHealthUI(); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        // วนลูปเพื่อเช็คช่องเลือดแต่ละช่อง
        for (int i = 0; i < healthBlocks.Length; i++)
        {
            if (healthBlocks[i] != null)
            {
                // ถ้าเลขช่อง (i) น้อยกว่าเลือดที่เหลืออยู่ (currentHealth) -> เปิดให้เห็นช่องเลือด (SetActive = true)
                // ถ้าเลขช่อง (i) มากกว่าหรือเท่ากับเลือดที่เหลือ -> ปิดช่องเลือด (SetActive = false)
                healthBlocks[i].SetActive(i < currentHealth);
            }
        }
    }

    void Die()
    {
        Debug.Log("ศัตรูตายแล้ว!");
        
        // เรียกฟังก์ชันชัยชนะ (Victory) จาก GameManager ที่เราทำไว้
        if (GameManager.instance != null) 
        {
            GameManager.instance.Victory(); 
        }
        
        Destroy(gameObject); 
    }
}