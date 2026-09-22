using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 6;         
    private int currentAmmo;        

    [Header("Gun Settings")]
    public float weaponRange = 50f; 
    public int damage = 1; 
    public Camera fpsCamera;        

    [Header("UI Settings (เชื่อมต่อหน้าจอ)")]
    public TextMeshProUGUI ammoText; 
    [Tooltip("ลากรูปกระสุน Bullet1 ถึง Bullet6 มาใส่ที่นี่")]
    public GameObject[] bulletIcons;

    [Header("Effects (เอฟเฟกต์)")]
    public GameObject muzzleFlash;

    [Header("Cooldown Settings")]
    public float fireRate = 1f; 
    private float nextFireTime = -1f; 

    [Header("Round Rules (เงื่อนไขการยิง)")]
    private int shotsFired = 0;       // นับจำนวนครั้งที่ยิงไป
    public int maxAllowedShots = 2;   // จำกัดให้ยิงได้ไม่เกิน 2 นัด (สำหรับ Round 1)
    public GameObject gameOverPanel;  // หน้าต่าง Game Over (ถ้ามี)

    void Start()
    {
        maxAmmo = 6; // กำหนดให้สูงสุดแสดงผล 6 นัด

        if (GameData.instance != null)
        {
            currentAmmo = GameData.instance.currentAmmo;
            maxAmmo = GameData.instance.maxAmmo;
        }
        else
        {
            currentAmmo = maxAmmo; 
        }

        UpdateAmmoUI();
        
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot(); 
            nextFireTime = Time.time + fireRate; 
        }
    }

    void Shoot()
    {
        currentAmmo--;
        shotsFired++; // บันทึกว่ามีการยิงเกิดขึ้น 1 นัด

        if (GameData.instance != null)
        {
            GameData.instance.currentAmmo = currentAmmo;
        }

        UpdateAmmoUI(); 

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true); 
            muzzleFlash.GetComponent<Animator>().Play("MuzzleFlash", -1, 0f); 
            Invoke("HideMuzzleFlash", 0.20f); 
        }

        Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

        bool hitEnemy = false;
        if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, weaponRange))
        {
            Debug.Log("ยิงโดน: " + hit.transform.name);

            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null)
            {
                target.TakeDamage(damage); 
                hitEnemy = true;
            }
        }

        // เงื่อนไข: ถ้าครบโควต้า 2 นัดแล้ว (หรือยิงเกิน 2 นัด) แต่ศัตรูยังไม่ตาย / ยิงพลาด ให้แสดงหน้า Game Over
        if (shotsFired >= maxAllowedShots)
        {
            // สามารถเพิ่มเช็คว่าถ้าศัตรูยังไม่ตาย หรือจะให้จบเกมทันทีหลังยิงครบ 2 นัด
            Invoke("CheckGameOverCondition", 0.5f);
        }
    }

    void CheckGameOverCondition()
    {
        // ตรวจสอบว่าถ้าศัตรูยังเหลือเลือดอยู่ แล้วเรายิงครบโควต้า 2 นัดแล้ว ให้แสดง Game Over
        EnemyHealth enemy = FindObjectOfType<EnemyHealth>();
        if (enemy != null)
        {
            // ถ้าศัตรูยังไม่ตายหลังจากยิงครบโควต้า
            Debug.Log("ยิงเกินโควต้า 2 นัด หรือจัดการไม่สำเร็จ!");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    void HideMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "BULLET: " + currentAmmo + " / " + maxAmmo;
        }

        for (int i = 0; i < bulletIcons.Length; i++)
        {
            if (i < currentAmmo)
            {
                bulletIcons[i].SetActive(true);  
            }
            else
            {
                bulletIcons[i].SetActive(false); 
            }
        }
    }
}