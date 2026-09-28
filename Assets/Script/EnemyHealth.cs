using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int currentHealth = 2;

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

    void Start()
    {
        currentHealth = 2;

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

                Vector3 spawnPos = currentWeakPoint.bounds.center + new Vector3(0, 0.8f, 0);
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
        if (hitCollider == currentWeakPoint || (currentWeakPoint != null && hitCollider.transform.IsChildOf(currentWeakPoint.transform)))
        {
            currentHealth -= damage;
            UpdateEnemyHealthUI(); // อัปเดต UI เลือดทันทีที่เลือดลด
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