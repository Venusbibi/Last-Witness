using UnityEngine;

public class LaggyReticleUI : MonoBehaviour
{
    [Header("ตั้งค่าความหน่วงของเป้า")]
    [Tooltip("ยิ่งค่าน้อยมากๆ เป้าจะยิ่งหนืดและเลื้อยช้าสุดๆ (ลองใส่ค่า เช่น 0.5, 1 หรือ 2 ดูครับ)")]
    public float lagSpeed = 1.0f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f || rectTransform == null || canvasRect == null) return;

        Vector2 targetLocalPos;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                Input.mousePosition, 
                null, 
                out targetLocalPos);
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                Input.mousePosition, 
                canvas.worldCamera != null ? canvas.worldCamera : Camera.main, 
                out targetLocalPos);
        }

        // สูตรแบบถ่วงน้ำหนักพิเศษให้ช้าและหนืดเป็นพิเศษ
        float smoothFactor = lagSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetLocalPos, smoothFactor);
    }
}