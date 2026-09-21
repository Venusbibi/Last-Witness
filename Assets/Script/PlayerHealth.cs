using UnityEngine;
using UnityEngine.UI; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 6;
    private int currentHealth;

    [Header("UI Blocks")]
    public GameObject[] healthBlocks; 

    [Header("Heart Icon Settings")]
    public Image heartImage;          
    public Sprite fullHeartSprite;    // รูปหัวใจปกติ
    public Sprite hurtHeartSprite;    // รูปหัวใจตอนโดนยิง
    public Sprite deadHeartSprite;    // รูปหัวใจตอนตาย

    // ตัวแปรสำหรับนับเวลาโชว์รูปโดนยิง
    private float hurtTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        
        // เริ่มเกมมา ให้เลือดเต็มและโชว์รูปปกติ
        UpdateHealthBlocks();
        if (heartImage != null)
        {
            heartImage.sprite = fullHeartSprite;
        }
    }

    void Update()
    {
        // ถ้านับเวลายังมากกว่า 0 และผู้เล่นยังไม่ตาย
        if (hurtTimer > 0 && currentHealth > 0)
        {
            hurtTimer -= Time.deltaTime; // นับเวลาถอยหลังตามเวลาจริง

            // เมื่อเวลาหมด (ครบ  วิ) ให้เปลี่ยนกลับเป็นรูปหัวใจปกติ
            if (hurtTimer <= 0 && heartImage != null)
            {
                heartImage.sprite = fullHeartSprite;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth < 0) currentHealth = 0; 

        UpdateHealthBlocks(); // อัปเดตช่องเลือดสีแดง

        if (currentHealth > 0)
        {
            // ถ้ายิงโดนแต่ยังไม่ตาย -> โชว์รูปโดนยิง และรีเซ็ตเวลาเป็น  วินาทีใหม่
            if (heartImage != null)
            {
                heartImage.sprite = hurtHeartSprite;
                hurtTimer = 1f; // ตั้งเวลานับถอยหลัง  วินาที
            }
        }
        else if (currentHealth == 0)
        {
            // ถ้าตาย -> โชว์รูปตาย และหยุดนับเวลา
            if (heartImage != null)
            {
                heartImage.sprite = deadHeartSprite;
            }
            hurtTimer = 0f; 
            Die();
        }
    }

    void UpdateHealthBlocks()
    {
        // เปิด/ปิด ช่องเลือดสีแดงตามจำนวนเลือดที่เหลือ
        for (int i = 0; i < healthBlocks.Length; i++)
        {
            if (healthBlocks[i] != null)
            {
                healthBlocks[i].SetActive(i < currentHealth);
            }
        }
    }

    void Die()
    {
        Debug.Log("ผู้เล่นตาย!");
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
    }
}