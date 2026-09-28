using UnityEngine;

public class MouseAimController : MonoBehaviour
{
    [Header("ตั้งค่าความเร็วเมาส์")]
    public float mouseSensitivity = 150f;

    [Header("ระยะขอบเขตการหันเป้า (กรอบการเล็ง)")]
    public float maxLookUpAndDown = 15f; 
    public float maxLookLeftAndRight = 20f; 

    private float currentX = 0f;
    private float currentY = 0f;
    
    private Quaternion startRotation; 

    void Start()
    {
        LockCursor();
        startRotation = transform.localRotation;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        currentY += mouseX;
        currentX -= mouseY; 

        currentX = Mathf.Clamp(currentX, -maxLookUpAndDown, maxLookUpAndDown);
        currentY = Mathf.Clamp(currentY, -maxLookLeftAndRight, maxLookLeftAndRight);

        transform.localRotation = startRotation * Quaternion.Euler(currentX, currentY, 0f);
    }

    void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}