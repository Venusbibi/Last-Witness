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
        // ใช้ GetButtonDown เพื่อให้ยิงทีละนัด
        if (Input.GetButtonDown("Fire1"))
        {
            if (currentAmmo > 0)
            {
                Shoot();
            }
            else
            {
                Debug.Log("กระสุนหมด!");
            }
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
            Debug.Log("ปัง! ยิงโดน: " + hit.transform.name);
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