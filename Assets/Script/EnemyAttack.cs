using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [Header("Visuals")]
    public Color shootColor = Color.red;

    public void PlayLaserAttackCustom(float duration)
    {
        StartCoroutine(ShowDynamicLaserRoutine(duration));
    }

    IEnumerator ShowDynamicLaserRoutine(float duration)
    {
        // ค้นหาตำแหน่งผู้เล่นในฉาก ( Player )
        GameObject playerObj = GameObject.FindWithTag("Player");
        Vector3 targetPos;

        if (playerObj != null)
        {
            // ให้เลเซอร์พุ่งไปที่ตัวผู้เล่น (ระดับอก/ลำตัว) จะได้ไม่บังกลางจอ
            targetPos = playerObj.transform.position + new Vector3(0, 1.0f, 0);
        }
        else if (Camera.main != null)
        {
            // เผื่อหา Player ไม่เจอ ให้พุ่งไปที่หน้ากล้องแต่มองต่ำลงมา
            targetPos = Camera.main.transform.position - new Vector3(0, 0.5f, 0);
        }
        else
        {
            yield break;
        }

        // สร้างเส้นเลเซอร์ชั่วคราว
        GameObject tempLaserObj = new GameObject("Active_Dynamic_Laser");
        LineRenderer lr = tempLaserObj.AddComponent<LineRenderer>();

        lr.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = 0.2f;
        lr.endWidth = 0.2f;
        lr.startColor = shootColor;
        lr.endColor = shootColor;

        lr.positionCount = 2;

        // จุดที่ 0: ตำแหน่งเริ่มต้นที่ตัวบอส (ปรับความสูงได้ตรง Vector3(0, 0.2f, 0))
        Vector3 bossPos = transform.position + new Vector3(0, 0.5f, 0);
        lr.SetPosition(0, bossPos);

        // จุดที่ 1: พุ่งไปที่ตัวผู้เล่นในฉาก (ไม่พุ่งเข้าหน้ากล้องตรงกลาง)
        lr.SetPosition(1, targetPos);

        // รอเวลาแสดงผลตามที่ตั้งไว้
        yield return new WaitForSeconds(duration);

        // ลบเลเซอร์ทิ้งเมื่อหมดเวลา
        Destroy(tempLaserObj);
    }
}