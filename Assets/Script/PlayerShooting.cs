using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 3;  
    public int currentAmmo; 
    public GameObject[] ammoIcons; 

    [Header("Gun Settings")]
    public float weaponRange = 50f; 
    public int damage = 1; 
    public Camera fpsCamera;        
    public GameObject muzzleFlash;
    
    [Header("UI Settings")]
    public GameObject gameOverPanel;  
    public GameObject nextStagePanel; 
    public TextMeshProUGUI timerText; 
    public string countdownMessage = "MEMORIZE & AIM";

    [Header("Game Flow Timing")]
    public float countdownSpeed = 1f; 
    public float shootTimeLimit = 3f; 

    private float currentTimer;
    private bool canShoot = false; 
    
    [Header("Systems")]
    public MouseAimController aimController; 
    public PlayerHealth playerHealth; 
    public EnemyAttack enemyAttack; 
    
    public float normalSensitivity = 50f; 
    public float slowSensitivity = 3f; 

    [Header("Attack Timing Settings (ตั้งเวลาสวนกลับของบอส)")]
    [Tooltip("ระยะเวลาที่เลเซอร์แสดงผลก่อนจะหายไป")]
    public float laserShowDuration = 0.4f; 
    [Tooltip("ระยะเวลาก่อนที่ Panel สีแดงจะขึ้นหลังจากเลเซอร์หาย")]
    public float delayBeforeDamage = 0.0f; 
    
    private EnemyHealth targetEnemy;

    void Start()
    {
        currentAmmo = maxAmmo;
        if (muzzleFlash != null) muzzleFlash.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (nextStagePanel != null) nextStagePanel.SetActive(false);
        
        targetEnemy = FindObjectOfType<EnemyHealth>();
        if (playerHealth == null) playerHealth = FindObjectOfType<PlayerHealth>();
        if (enemyAttack == null) enemyAttack = FindObjectOfType<EnemyAttack>();
        
        UpdateAmmoUI();
        Time.timeScale = 1f; 

        StartCoroutine(RoundFlowRoutine());
    }

    void Update()
    {
        bool isUIVisible = (gameOverPanel != null && gameOverPanel.activeSelf) || 
                           (nextStagePanel != null && nextStagePanel.activeSelf);

        if (canShoot && !isUIVisible)
        {
            if (aimController != null) aimController.mouseSensitivity = normalSensitivity;

            currentTimer -= Time.deltaTime;
            
            if (timerText != null) timerText.text = "TIME: " + currentTimer.ToString("F1");

            if (Input.GetButtonDown("Fire1"))
            {
                if (currentAmmo > 0)
                {
                    Shoot(); 
                }
            }

            if (currentTimer <= 0)
            {
                FailRound("หมดเวลา! โดนบอสสวนกลับ!");
            }
        }
    }

    IEnumerator RoundFlowRoutine()
    {
        canShoot = false;

        if (targetEnemy != null)
        {
            targetEnemy.RandomizeAndShowWeakPoint();
            targetEnemy.ShowWeakPointVisual();
        }

        if (aimController != null) aimController.mouseSensitivity = slowSensitivity;

        if (timerText != null) timerText.text = countdownMessage + ": 3";
        yield return new WaitForSeconds(countdownSpeed);
        
        if (timerText != null) timerText.text = countdownMessage + ": 2";
        yield return new WaitForSeconds(countdownSpeed);
        
        if (timerText != null) timerText.text = countdownMessage + ": 1";
        yield return new WaitForSeconds(countdownSpeed);

        if (timerText != null) timerText.text = "SHOOT!";
        currentTimer = shootTimeLimit;
        canShoot = true;
    }

    void Shoot()
    {
        currentAmmo--; 
        UpdateAmmoUI();
        canShoot = false; 

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true); 
            muzzleFlash.GetComponent<Animator>().Play("MuzzleFlash", -1, 0f); 
            Invoke("HideMuzzleFlash", 0.20f); 
        }

        Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;
        
        if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, weaponRange))
        {
            EnemyHealth enemy = hit.transform.GetComponentInParent<EnemyHealth>();
            if (enemy == null)
            {
                enemy = hit.transform.GetComponent<EnemyHealth>();
            }
            
            if (enemy != null)
            {
                bool isWeakPointHit = enemy.TryHitWeakPoint(hit.collider, damage, hit.point, hit.normal);
                
                if (isWeakPointHit)
                {
                    if (enemy.currentHealth <= 0)
                    {
                        Debug.Log("ชนะแล้ว!");
                        if (nextStagePanel != null)
                        {
                            nextStagePanel.SetActive(true);
                            Time.timeScale = 0f;
                            UnlockCursor(); 
                        }
                    }
                    else
                    {
                        StartCoroutine(NextRoundDelayRoutine());
                    }
                    return; 
                }
            }
        }

        FailRound("ยิงพลาด! โดนบอสสวนกลับ!"); 
    }

    IEnumerator NextRoundDelayRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(RoundFlowRoutine());
    }
    
    void FailRound(string reason)
    {
        canShoot = false;
        Debug.Log(reason);
        
        // เริ่มต้นคิวการสวนกลับ (เลเซอร์มาก่อน แล้ว Panel แดง/เลือดลดตามหลัง)
        StartCoroutine(CounterAttackSequenceRoutine());
    }

    IEnumerator CounterAttackSequenceRoutine()
    {
        // 1. สั่งให้บอสยิงเลเซอร์ออกมาก่อนตามเวลาที่ตั้งไว้
        if (enemyAttack != null)
        {
            enemyAttack.PlayLaserAttackCustom(laserShowDuration);
            yield return new WaitForSeconds(laserShowDuration); 
        }

        if (delayBeforeDamage > 0)
        {
            yield return new WaitForSeconds(delayBeforeDamage);
        }

        // 2. หลังจากเลเซอร์หายไป ค่อยลดเลือดและให้ Panel สีแดงกระพริบขึ้นตามหลังรอบเดียว
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1); 
        }

        // 3. ตรวจสอบสถานะเกมโอเวอร์
        if (currentAmmo <= 0)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Time.timeScale = 0f;
                UnlockCursor();
            }
        }
        else
        {
            StartCoroutine(NextRoundDelayRoutine());
        }
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HideMuzzleFlash()
    {
        if (muzzleFlash != null) muzzleFlash.SetActive(false);
    }
    
    void UpdateAmmoUI()
    {
        if (ammoIcons != null)
        {
            for (int i = 0; i < ammoIcons.Length; i++)
            {
                if (ammoIcons[i] != null)
                {
                    if (i < currentAmmo)
                    {
                        ammoIcons[i].SetActive(true);
                    }
                    else
                    {
                        ammoIcons[i].SetActive(false);
                    }
                }
            }
        }
    }
}