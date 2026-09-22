using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

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
    private int shotsFired = 0;       
    public int maxAllowedShots;       
    
    [Header("UI Panels (ลากหน้าต่าง UI มาใส่)")]
    public GameObject gameOverPanel;  // หน้าต่างแพ้ (You Died)
    public GameObject nextStagePanel; // หน้าต่างชนะเมื่อศัตรูตาย

    void Start()
    {
        maxAmmo = 6; 
        
        // กำหนดโควต้าตามฉาก: Round 1 ให้ 2 นัด, Round 2 ให้ 4 นัด
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Round1") 
        { 
            maxAllowedShots = 2; 
        }
        else if (currentScene == "Round2") 
        { 
            maxAllowedShots = 4; 
        }
        else 
        { 
            maxAllowedShots = 6; 
        }
        
        shotsFired = 0; 

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
        
        if (muzzleFlash != null) muzzleFlash.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (nextStagePanel != null) nextStagePanel.SetActive(false);
        
        Time.timeScale = 1f; 
    }

    void Update()
    {
        // ถ้าหน้าต่างผลลัพธ์ (ชนะ/แพ้) เปิดอยู่ จะไม่ให้ยิงต่อ
        bool isUIVisible = (gameOverPanel != null && gameOverPanel.activeSelf) || 
                           (nextStagePanel != null && nextStagePanel.activeSelf);
        
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && currentAmmo > 0 && !isUIVisible)
        {
            Shoot(); 
            nextFireTime = Time.time + fireRate; 
        }
    }

    void Shoot()
    {
        currentAmmo--;
        shotsFired++; 

        if (GameData.instance != null) GameData.instance.currentAmmo = currentAmmo;

        UpdateAmmoUI(); 

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true); 
            muzzleFlash.GetComponent<Animator>().Play("MuzzleFlash", -1, 0f); 
            Invoke("HideMuzzleFlash", 0.20f); 
        }

        Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;
        
        bool enemyKilled = false;

        if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, weaponRange))
        {
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null)
            {
                target.TakeDamage(damage); 
                
                // เช็คว่ายิงนัดนี้แล้วศัตรูตายเลยหรือไม่
                if (target.currentHealth <= 0)
                {
                    enemyKilled = true;
                }
            }
        }

        // ถ้าศัตรูตาย ให้เด้ง UI ชนะขึ้นมาทันที
        if (enemyKilled)
        {
            Debug.Log("ศัตรูตายแล้ว! แสดง UI ทันที");
            if (nextStagePanel != null)
            {
                nextStagePanel.SetActive(true);
                Time.timeScale = 0f; // หยุดเกมทันที
            }
        }
        // ถ้าศัตรูยังไม่ตาย แต่ยิงจนครบโควต้าแล้ว ให้เด้ง Game Over
        else if (shotsFired >= maxAllowedShots)
        {
            Invoke("ShowGameOver", 0.3f); 
        }
    }

    void ShowGameOver()
    {
        Debug.Log("ยิงครบโควต้า " + maxAllowedShots + " นัดแล้วแต่จัดการไม่สำเร็จ!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
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