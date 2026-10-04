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
    public float weaponRange = 100f; 
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
    public PlayerHealth playerHealth; 
    public EnemyAttack enemyAttack; 
    
    [Header("Attack Timing Settings")]
    public float laserShowDuration = 0.4f; 
    public float delayBeforeDamage = 0.0f; 
    
    private EnemyHealth targetEnemy;

    // -----------------------------------------------------
    // รวบรวมระบบเสียงทั้งหมดไว้ตรงนี้
    [Header("All Audio Settings")]
    public AudioSource gunAudioSource;      // ลำโพงสำหรับเสียงปืน
    public AudioClip gunshotSound;          // ไฟล์เสียงปืน
    
    public AudioSource sfxAudioSource;      // ลำโพงสำหรับเสียงเอฟเฟกต์อื่นๆ
    public AudioClip tickSound;             // ไฟล์เสียงนาฬิกาเดิน (Tick-Tock) หรือเสียงบี๊บ
    public AudioClip failWarningSound;      // ไฟล์เสียงเตือนตอนพลาด
    public AudioClip laserAttackSound;      // ไฟล์เสียงบอสยิงเลเซอร์
    public AudioClip playerHurtSound;       // ไฟล์เสียงตอนตัวละครโดนโจมตี

    private int lastTickSecond = -1;        // ตัวแปรเช็คเวลาเพื่อเล่นเสียง Tick-Tock
    // -----------------------------------------------------

    void Start()
    {
        currentAmmo = maxAmmo;
        if (muzzleFlash != null) muzzleFlash.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (nextStagePanel != null) nextStagePanel.SetActive(false);
        
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        foreach (var enemy in enemies)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                targetEnemy = enemy;
                break;
            }
        }

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
            currentTimer -= Time.deltaTime;
            
            if (timerText != null) timerText.text = "TIME: " + currentTimer.ToString("F1");

            // ระบบเสียง Tick-Tock ช่วงนับถอยหลังใกล้หมดเวลา
            int currentSecond = Mathf.CeilToInt(currentTimer);
            if (currentSecond != lastTickSecond && currentSecond > 0 && currentSecond <= 3)
            {
                PlaySFX(tickSound);
                lastTickSecond = currentSecond;
            }

            if (Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0))
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
        lastTickSecond = -1; 

        if (targetEnemy != null)
        {
            targetEnemy.RandomizeAndShowWeakPoint();
            targetEnemy.ShowWeakPointVisual();
        }

        // เล่นเสียง Tick-Tock ช่วงเตรียมตัว 3.. 2.. 1..
        if (timerText != null) timerText.text = countdownMessage + ": 3";
        PlaySFX(tickSound);
        yield return new WaitForSeconds(countdownSpeed);
        
        if (timerText != null) timerText.text = countdownMessage + ": 2";
        PlaySFX(tickSound);
        yield return new WaitForSeconds(countdownSpeed);
        
        if (timerText != null) timerText.text = countdownMessage + ": 1";
        PlaySFX(tickSound);
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

        // เล่นเสียงปืน
        if (gunAudioSource != null && gunshotSound != null)
        {
            gunAudioSource.PlayOneShot(gunshotSound);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true); 
            muzzleFlash.GetComponent<Animator>().Play("MuzzleFlash", -1, 0f); 
            Invoke("HideMuzzleFlash", 0.20f); 
        }

        Ray ray;
        LaggyReticleUI reticle = FindObjectOfType<LaggyReticleUI>();
        
        if (reticle != null && fpsCamera != null)
        {
            ray = fpsCamera.ScreenPointToRay(reticle.transform.position);
        }
        else
        {
            ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        }

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, weaponRange))
        {
            EnemyHealth enemy = hit.transform.GetComponentInParent<EnemyHealth>();
            if (enemy == null) enemy = hit.transform.GetComponent<EnemyHealth>();
            
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
        PlaySFX(failWarningSound); // เล่นเสียงเตือนว่าพลาด
        StartCoroutine(CounterAttackSequenceRoutine());
    }

    IEnumerator CounterAttackSequenceRoutine()
    {
        if (enemyAttack != null)
        {
            PlaySFX(laserAttackSound); // เล่นเสียงเลเซอร์
            enemyAttack.PlayLaserAttackCustom(laserShowDuration);
            yield return new WaitForSeconds(laserShowDuration); 
        }

        if (delayBeforeDamage > 0)
        {
            yield return new WaitForSeconds(delayBeforeDamage);
        }

        if (playerHealth != null)
        {
            PlaySFX(playerHurtSound); // เล่นเสียงเสียเลือด
            playerHealth.TakeDamage(1); 
        }

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

    // ฟังก์ชันตัวช่วยสำหรับเล่นเสียง SFX ง่ายๆ
    void PlaySFX(AudioClip clip)
    {
        if (sfxAudioSource != null && clip != null)
        {
            sfxAudioSource.PlayOneShot(clip);
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
                    ammoIcons[i].SetActive(i < currentAmmo);
                }
            }
        }
    }
}