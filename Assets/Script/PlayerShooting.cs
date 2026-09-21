using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 3;         
    private int currentAmmo;        

    [Header("Gun Settings")]
    public float weaponRange = 50f; 
    public float damage = 1f;       
    public Camera fpsCamera;        

    [Header("UI Settings (เชื่อมต่อหน้าจอ)")]
    public TextMeshProUGUI ammoText; 
    [Tooltip("ลากรูปกระสุน Bullet1, 2, 3 มาใส่ที่นี่")]
    public GameObject[] bulletIcons;

    [Header("Effects (เอฟเฟกต์)")]
    public GameObject muzzleFlash;

    [Header("Cooldown Settings")]
    public float fireRate = 20f; 
    private float nextFireTime = -1f; // ตัวแปรซ่อนไว้จำเวลาที่อนุญาตให้ยิงนัดถัดไป

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        
        // ซ่อนเอฟเฟกต์ไฟปากกระบอกปืนไว้ก่อนตอนเริ่มเกม
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }

   void Update()
    {
        // เปลี่ยนเป็น GetButtonDown (บังคับคลิกทีละนัด)
        // และเพิ่มเงื่อนไข currentAmmo > 0 (กระสุนต้องมีมากกว่า 0 ถึงจะยิงได้)
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot(); 
            nextFireTime = Time.time + fireRate; 
        }
    }

    void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI(); 

        // แสดงเอฟเฟกต์และเล่นอนิเมชัน
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true); 
            // สั่งเล่นอนิเมชันโดยระบุชื่อ MuzzleFlash พร้อมใส่เครื่องหมาย ""
            muzzleFlash.GetComponent<Animator>().Play("MuzzleFlash", -1, 0f); 
            
            // สั่งให้เรียกฟังก์ชันปิดโมเดลหลังจาก 0.15 วินาที
            Invoke("HideMuzzleFlash", 0.20f); 
        }

        Vector3 rayOrigin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;

       if (Physics.Raycast(rayOrigin, fpsCamera.transform.forward, out hit, weaponRange))
{
    Debug.Log("ยิงโดน: " + hit.transform.name);

    // เช็คว่าสิ่งที่เรายิงโดน มีสคริปต์ EnemyHealth แปะอยู่ไหม
    EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
    
    // ถ้ามีสคริปต์นี้ (แปลว่าเป็นศัตรู) ให้สั่งลดเลือด 1 ดาเมจ
    if (target != null)
    {
        target.TakeDamage(1);
    }
}
    }

    // ฟังก์ชันสำหรับปิดเอฟเฟกต์
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