using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 4;        // กำหนดเลือดสูงสุดของศัตรูตัวนี้จากข้างนอก Inspector ได้
    [HideInInspector]
    public int currentHealth;

    [Header("Weak Point System")]
    public Collider[] possibleWeakPoints;
    private Collider currentWeakPoint;

    [Header("Enemy Health UI Blocks (ไอคอนเลือดศัตรู)")]
    [Tooltip("ลาก GameObject รูปเลือดศัตรูมาใส่ที่นี่ตามลำดับ (เช่น HP1, HP2)")]
    public GameObject[] enemyHealthBlocks;

    [Header("2D Hit Flash Effect (กระพริบขาวเมื่อโดนยิง)")]
    public SpriteRenderer enemySpriteRenderer;
    public Sprite whiteFlashSprite;
    public float flashDuration = 0.15f;
    private Sprite originalSprite;

    [Header("Visual Effects (ระบบรูปภาพบอกจุดอ่อน 2D)")]
    public Sprite weakPointSprite;
    public float iconScale = 0.5f;

    private GameObject spawnedIconObject; 

    public GameObject bloodEffectPrefab;
    public GameObject sparkEffectPrefab;

    [Header("Audio Settings (เสียงเอฟเฟกต์)")]
    public AudioSource audioSource;         // ช่องใส่ Audio Source ของตัวบอส
    public AudioClip weakPointHitSound;     // ช่องใส่ไฟล์เสียงยิงโดนจุดอ่อน

    void Start()
    {
        // กำหนดให้เลือดปัจจุบันเริ่มต้นเท่ากับ Max Health ที่ตั้งไว้จาก Inspector
        currentHealth = maxHealth;

        if (enemySpriteRenderer != null)
        {
            originalSprite = enemySpriteRenderer.sprite;
        }

        UpdateEnemyHealthUI();
        RandomizeAndShowWeakPoint();
    }

    public void RandomizeAndShowWeakPoint()
    {
        if (possibleWeakPoints != null && possibleWeakPoints.Length > 0)
        {
            if (spawnedIconObject != null)
            {
                Destroy(spawnedIconObject);
            }

            int randomIndex = Random.Range(0, possibleWeakPoints.Length);
            currentWeakPoint = possibleWeakPoints[randomIndex];
            
            if (weakPointSprite != null && currentWeakPoint != null)
            {
                spawnedIconObject = new GameObject("WeakPoint_Icon");
                spawnedIconObject.transform.SetParent(transform);

                Vector3 spawnPos = currentWeakPoint.bounds.center;
                spawnedIconObject.transform.position = spawnPos;
                spawnedIconObject.transform.localScale = new Vector3(iconScale, iconScale, 1f);

                SpriteRenderer sr = spawnedIconObject.AddComponent<SpriteRenderer>();
                sr.sprite = weakPointSprite;
                
                sr.sortingLayerName = enemySpriteRenderer.sortingLayerName;
                sr.sortingOrder = enemySpriteRenderer.sortingOrder + 10;
            }
        }
    }

    public void ShowWeakPointVisual()
    {
        if (spawnedIconObject != null)
        {
            spawnedIconObject.SetActive(true);
        }
    }

    public void HideWeakPointVisual()
    {
        if (spawnedIconObject != null)
        {
            spawnedIconObject.SetActive(false);
        }
    }

    public bool TryHitWeakPoint(Collider hitCollider, int damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        Debug.Log("ยิงโดน Collider: " + (hitCollider != null ? hitCollider.name : "ไม่มี") + 
                  " | จุดอ่อนปัจจุบัน: " + (currentWeakPoint != null ? currentWeakPoint.name : "ไม่มี"));

        // ตรวจสอบว่า Collider ที่ยิงโดนตรงกับจุดอ่อนปัจจุบันหรือไม่
        if (hitCollider == currentWeakPoint || (currentWeakPoint != null && hitCollider.transform.IsChildOf(currentWeakPoint.transform)))
        {
            // --- โค้ดที่เพิ่มใหม่: เล่นเสียงโดนจุดอ่อน ---
            if (audioSource != null && weakPointHitSound != null)
            {
                audioSource.PlayOneShot(weakPointHitSound);
            }
            // ------------------------------------

            currentHealth -= damage;
            UpdateEnemyHealthUI(); 
            Debug.Log("ยิงโดนจุดอ่อน! เลือดศัตรูเหลือ: " + currentHealth);

            if (enemySpriteRenderer != null && whiteFlashSprite != null)
            {
                StartCoroutine(FlashWhiteRoutine());
            }
            
            if (bloodEffectPrefab != null) Instantiate(bloodEffectPrefab, hitPoint, Quaternion.identity);

            if (currentHealth > 0)
            {
                RandomizeAndShowWeakPoint();
            }
            else
            {
                if (spawnedIconObject != null) Destroy(spawnedIconObject);
                Debug.Log("ศัตรูตายแล้ว!");
            }
            return true;
        }
        else
        {
            if (sparkEffectPrefab != null) Instantiate(sparkEffectPrefab, hitPoint, Quaternion.identity);
            Debug.Log("ยิงโดนตัว แต่ไม่ใช่จุดอ่อน!");
            return false;
        }
    }
    // ฟังก์ชันเปิด/ปิดไอคอนรูปเลือดตามจำนวนเลือดที่เหลือ
    void UpdateEnemyHealthUI()
    {
        if (enemyHealthBlocks != null)
        {
            for (int i = 0; i < enemyHealthBlocks.Length; i++)
            {
                if (enemyHealthBlocks[i] != null)
                {
                    if (i < currentHealth)
                    {
                        enemyHealthBlocks[i].SetActive(true);
                    }
                    else
                    {
                        enemyHealthBlocks[i].SetActive(false);
                    }
                }
            }
        }
    }

    IEnumerator FlashWhiteRoutine()
    {
        enemySpriteRenderer.sprite = whiteFlashSprite;
        yield return new WaitForSeconds(flashDuration);
        enemySpriteRenderer.sprite = originalSprite;
    }
} 