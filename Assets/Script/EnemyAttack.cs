using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 1; 
    public float timeBetweenAttacks = 4f; 
    public float chargeTime = 1.5f;       

    [Header("UI & Visuals")]
    public GameObject warningIndicator; 
    public LineRenderer laserLine;
    
    [Tooltip("สีของเลเซอร์ตอนกำลังเล็ง")]
    public Color aimColor = new Color(1f, 0f, 0f, 0.3f); // แดงใสๆ
    [Tooltip("สีของเลเซอร์ตอนยิงจริง")]
    public Color shootColor = new Color(1f, 0f, 0f, 1f); // แดงเข้มทึบ
    [Tooltip("ระยะเวลาที่เลเซอร์จะโชว์ค้างตอนยิงจริง")]
    public float laserDuration = 0.2f;

    private float timer;
    private bool isCharging = false;
    private Transform playerCamera;
    private PlayerHealth playerHealth;
    private PlayerMovement2D playerMovement;

    void Start()
    {
        timer = timeBetweenAttacks;
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerMovement = FindObjectOfType<PlayerMovement2D>();
        
        if (Camera.main != null) playerCamera = Camera.main.transform;

        if (warningIndicator != null) warningIndicator.SetActive(false);
        if (laserLine != null) laserLine.enabled = false;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // ช่วงชาร์จ: แสดงลูกศรเตือน + เปิดเลเซอร์เล็งเป้าค้างไว้
        if (timer <= chargeTime && timer > 0f)
        {
            if (!isCharging)
            {
                isCharging = true;
                if (warningIndicator != null) warningIndicator.SetActive(true);
            }
            // อัปเดตตำแหน่งเลเซอร์เล็งตลอดเวลา (ให้เส้นหยุดแค่ตรงที่มันชน)
            DrawLaser(aimColor); 
        }

        // ช่วงยิงจริง
        if (timer <= 0f)
        {
            ExecuteAttack();
            
            timer = timeBetweenAttacks;
            isCharging = false;
            if (warningIndicator != null) warningIndicator.SetActive(false);
        }
    }

    // ฟังก์ชันวาดเส้นเลเซอร์ (ใช้ได้ทั้งตอนเล็งและตอนยิง)
    void DrawLaser(Color color)
    {
        if (laserLine == null || playerCamera == null) return;

        laserLine.enabled = true;
        laserLine.startColor = color;
        laserLine.endColor = color;

        Vector3 startPos = transform.position;
        // ชี้ไปทางผู้เล่น (ปรับความสูงเป้าหมายลงมานิดหน่อย)
        Vector3 direction = (playerCamera.position - new Vector3(0, 0.5f, 0)) - startPos;
        
        laserLine.SetPosition(0, startPos);

        RaycastHit hit;
        // ศัตรูยิง Raycast ออกไปเช็คว่าชนอะไรก่อน
        if (Physics.Raycast(startPos, direction.normalized, out hit, 100f))
        {
            // ถ้าชน ให้เลเซอร์หยุดที่จุดนั้น (เช่น ถ้าผู้เล่นแอบ เลเซอร์จะหยุดที่กล่อง)
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            // ถ้าไม่ชนอะไรเลย ก็ให้เส้นยืดไปไกลๆ
            laserLine.SetPosition(1, startPos + direction.normalized * 100f);
        }
    }

    void ExecuteAttack()
    {
        // ตอนยิงจริง ให้วาดเลเซอร์สีเข้มทับ
        DrawLaser(shootColor);
        StartCoroutine(ShootLaserEffect());

        // คำนวณดาเมจ
        if (playerMovement != null && playerMovement.isCovering)
        {
            Debug.Log("รอดตัว! ศัตรูยิงโดนกล่องกำบัง");
        }
        else
        {
            if (playerHealth != null)
            {
                Debug.Log("โดนยิงเต็มๆ!");
                playerHealth.TakeDamage(damage);
            }
        }
    }
    
    // หน่วงเวลาปิดเลเซอร์ตอนยิงเสร็จ
    IEnumerator ShootLaserEffect()
    {
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }
}