using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Player Health UI Blocks")]
    public GameObject[] healthBlocks;

    [Header("Game Over UI")]
    public GameObject gameOverPanel; // ช่องใส่ UI หน้าจอ Game Over

    [Header("Damage Panel Effect")]
    public GameObject playerDamagePanel;
    public float panelFlashDuration = 1.0f; 

    void Start()
    {
        currentHealth = maxHealth;
        UpdatePlayerHealthUI();

        if (playerDamagePanel != null) playerDamagePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdatePlayerHealthUI();

        if (playerDamagePanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashPlayerDamagePanelRoutine());
        }

        // ตรวจสอบความตายตรงนี้
        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        Debug.Log("ผู้เล่นตายแล้ว! เปิด Game Over Panel");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f; // หยุดเวลาเกม
        
        // ปลดล็อคเคอร์เซอร์เมาส์เพื่อให้กดปุ่มบน UI ได้
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator FlashPlayerDamagePanelRoutine()
    {
        playerDamagePanel.SetActive(true);
        yield return new WaitForSeconds(panelFlashDuration);
        playerDamagePanel.SetActive(false);
    }

    void UpdatePlayerHealthUI()
    {
        if (healthBlocks != null)
        {
            for (int i = 0; i < healthBlocks.Length; i++)
            {
                if (healthBlocks[i] != null)
                {
                    if (i < currentHealth)
                    {
                        healthBlocks[i].SetActive(true);
                    }
                    else
                    {
                        healthBlocks[i].SetActive(false);
                    }
                }
            }
        }
    }
}