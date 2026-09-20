using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings (ตั้งค่าการเดิน)")]
    public float moveSpeed = 5f;
    public float minX = -5f; 
    public float maxX = 5f;

    [Header("Cover Settings (จุดซ่อนตัว)")]
    [Tooltip("ลากกล่องฝั่งซ้ายมาใส่ช่องนี้")]
    public Transform leftCoverBox;  
    [Tooltip("ลากกล่องฝั่งขวามาใส่ช่องนี้")]
    public Transform rightCoverBox; 
    [Tooltip("ความเร็วในการพุ่งไปหลบหลังกล่อง")]
    public float slideSpeed = 10f;  

    // สถานะว่ากำลังหลบอยู่หรือไม่
    private bool isCovering = false;
    private float targetX;

    void Update()
    {
        // 1. กด Q พุ่งไปกล่องซ้าย, กด E พุ่งไปกล่องขวา
        if (Input.GetKeyDown(KeyCode.Q) && leftCoverBox != null)
        {
            isCovering = true;
            targetX = leftCoverBox.position.x; 
        }
        else if (Input.GetKeyDown(KeyCode.E) && rightCoverBox != null)
        {
            isCovering = true;
            targetX = rightCoverBox.position.x; 
        }

        // 2. ถ้ากดปุ่มเดิน A/D จะเป็นการสั่งให้ "ออกจากที่ซ่อน"
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0 && isCovering)
        {
            isCovering = false; 
        }

        // 3. ระบบเคลื่อนที่ (สลับระหว่างพุ่งไปหลบ กับ เดินปกติ)
        if (isCovering)
        {
            // สไลด์กล้องไปที่ตำแหน่งแกน X ของกล่องเป้าหมาย
            float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * slideSpeed);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else
        {
            // ระบบเดินซ้าย-ขวาตามปกติ
            float newX = transform.position.x + (moveInput * moveSpeed * Time.deltaTime);
            newX = Mathf.Clamp(newX, minX, maxX);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}