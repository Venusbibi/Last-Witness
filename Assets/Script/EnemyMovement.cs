using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f; // ความเร็วในการเดินสไลด์ซ้ายขวา

    private Transform player;
    private SpriteRenderer spriteRenderer; 

    void Start()
    {
        // หากล้องหลัก (ผู้เล่น)
        if (Camera.main != null)
        {
            player = Camera.main.transform;
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player != null)
        {
            // ล็อคเป้าหมายให้เป็นตำแหน่ง X ของผู้เล่น (ซ้าย-ขวา) 
            // ส่วน Y (ความสูง) และ Z (ระยะห่างหน้าหลัง) ให้คงที่ไว้เท่าเดิม
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);

            // สั่งให้ศัตรูขยับสไลด์ซ้าย-ขวา ตามเป้าหมาย
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // --- ระบบหันซ้าย/หันขวา ---
            if (spriteRenderer != null)
            {
                // ถ้าเป้าหมาย (ผู้เล่น) อยู่ทางขวาของศัตรู
                if (targetPosition.x > transform.position.x + 0.1f) 
                {
                    spriteRenderer.flipX = false; // หันขวา
                }
                // ถ้าเป้าหมาย (ผู้เล่น) อยู่ทางซ้ายของศัตรู
                else if (targetPosition.x < transform.position.x - 0.1f) 
                {
                    spriteRenderer.flipX = true; // หันซ้าย
                }
            }
        }
    }
}