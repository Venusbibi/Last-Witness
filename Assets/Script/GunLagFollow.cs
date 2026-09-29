using UnityEngine;

public class GunLagFollow : MonoBehaviour
{
    [Header("ตั้งค่าความหน่วงของปืน")]
    public float lagSpeed = 10.0f;
    public float maxOffset = 1.5f;

    private Vector3 initialLocalPos;
    private Camera cam;

    void Start()
    {
        // บันทึกตำแหน่งเริ่มต้นเฉพาะของปืน
        initialLocalPos = transform.localPosition;
        cam = Camera.main;
    }

    void Update()
    {
        if (Time.timeScale == 0f || cam == null) return;

        // ดึงตำแหน่งเมาส์บนจอมาแปลงให้ปืนขยับตามแบบหน่วงๆ
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = cam.nearClipPlane + 2.0f;

        Vector3 targetWorldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        Vector3 targetLocalPos = transform.parent != null ? transform.parent.InverseTransformPoint(targetWorldPos) : targetWorldPos;

        Vector3 clampedPos = Vector3.ClampMagnitude(targetLocalPos - initialLocalPos, maxOffset) + initialLocalPos;

        // ขยับเฉพาะตำแหน่ง Local ของตัวปืน ไม่เกี่ยวกับกล้อง
        transform.localPosition = Vector3.Lerp(transform.localPosition, clampedPos, lagSpeed * Time.deltaTime);
    }
}