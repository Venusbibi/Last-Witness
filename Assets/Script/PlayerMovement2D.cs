using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Movement Boundaries")]
    [Tooltip("จุดซ้ายสุดที่เดินได้")]
    public float minX = -5f; 
    [Tooltip("จุดขวาสุดที่เดินได้")]
    public float maxX = 5f;

    [Header("Gun Sway Settings (ตั้งค่าปืนส่าย)")]
    [Tooltip("ลากโมเดลปืนมาใส่ช่องนี้")]
    public Transform gunTransform; 
    [Tooltip("องศาความเอียงของปืนเวลาเดิน")]
    public float swayAmount = 2f; 
    [Tooltip("ความนุ่มนวลในการเอียง")]
    public float swaySmooth = 8f; 

    private Quaternion initialGunRotation;

    void Start()
    {
        // บันทึกค่าการหมุนดั้งเดิมของปืนเอาไว้ตอนเริ่มเกม
        if (gunTransform != null)
        {
            initialGunRotation = gunTransform.localRotation;
        }
    }

    void Update()
    {
        // 1. รับค่าการกดปุ่ม A, D หรือ ลูกศรซ้าย-ขวา (A = -1, D = 1)
        float moveInput = Input.GetAxisRaw("Horizontal");

        // 2. คำนวณตำแหน่งใหม่บนแกน X
        Vector3 newPosition = transform.position + new Vector3(moveInput, 0f, 0f) * moveSpeed * Time.deltaTime;

        // 3. ล็อกระยะไม่ให้เดินเลยขอบเขตที่กำหนด
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        // 4. อัปเดตตำแหน่งของตัวละคร/กล้อง
        transform.position = newPosition;

        // 5. ระบบทำให้ปืนเอียงตามการเคลื่อนที่ (Weapon Sway)
        if (gunTransform != null)
        {
            // คำนวณทิศทางการเอียง (เดินขวา ปืนเอนซ้าย / เดินซ้าย ปืนเอนขวา)
            float targetSwayZ = -moveInput * swayAmount;

            // สร้างองศาการหมุนใหม่
            Quaternion targetRotation = initialGunRotation * Quaternion.Euler(0f, 0f, targetSwayZ);

            // ค่อยๆ หมุนปืนไปยังองศาใหม่แบบนุ่มนวล
            gunTransform.localRotation = Quaternion.Lerp(gunTransform.localRotation, targetRotation, Time.deltaTime * swaySmooth);
        }
    }
}