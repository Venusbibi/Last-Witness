using UnityEngine;

public class MouseAimController : MonoBehaviour
{
    [Header("ตั้งค่าความเร็วเมาส์")]
    public float mouseSensitivity = 30f;

    [Header("ความหน่วงของปืนและกล้อง (Inertia)")]
    public float aimSmoothSpeed = 20f;

    [Header("ระยะขอบเขตการหัน")]
    public float maxLookUpAndDown = 30f; 
    public float maxLookLeftAndRight = 10f; 

    private float targetX = 0f;
    private float targetY = 0f;
    private float currentX = 0f;
    private float currentY = 0f;
    
    private Quaternion startRotation; 

    void Start()
    {
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        targetY += mouseX;
        targetX -= mouseY; 

        targetX = Mathf.Clamp(targetX, -maxLookUpAndDown, maxLookUpAndDown);
        targetY = Mathf.Clamp(targetY, -maxLookLeftAndRight, maxLookLeftAndRight);

        currentX = Mathf.Lerp(currentX, targetX, aimSmoothSpeed * Time.deltaTime);
        currentY = Mathf.Lerp(currentY, targetY, aimSmoothSpeed * Time.deltaTime);

        transform.localRotation = startRotation * Quaternion.Euler(currentX, currentY, 0f);
    }
}