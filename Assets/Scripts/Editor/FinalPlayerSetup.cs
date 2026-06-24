using UnityEngine;
using UnityEditor;
using System.Linq;

public class FinalPlayerSetup : EditorWindow
{
    [MenuItem("Tools/1 TIKLA OYUNCUYU KUSURSUZ YAP")]
    public static void PerfectSetup()
    {
        // 1. Oyuncuyu bul
        GameObject player = GameObject.Find("PlayerCapsule");
        if (player == null)
        {
            Debug.LogError("PlayerCapsule bulunamadı! Lütfen sahneye NestedParent_Unpack koyduğunuzdan emin olun.");
            return;
        }

        // 2. Kendi vücudunu gizle
        MeshRenderer mr = player.GetComponentInChildren<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        // 3. Boyu yükselt (Cinemachine kökü)
        Transform camRoot = player.transform.Find("PlayerCameraRoot");
        if (camRoot != null)
        {
            Vector3 pos = camRoot.localPosition;
            pos.y = 1.6f; // Tam göz hizası
            camRoot.localPosition = pos;
            EditorUtility.SetDirty(camRoot);
        }

        // 4. Kamerayı bul ve Çift Ses (AudioListener) sorununu çöz
        Camera mainCam = player.GetComponentInChildren<Camera>();
        if (mainCam != null)
        {
            mainCam.nearClipPlane = 0.05f; // Duvar arkasını görmeyi engelle
            
            // Sahnedeki diğer AudioListener'ları bul ve sil
            AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            foreach (var l in listeners)
            {
                if (l.gameObject != mainCam.gameObject)
                {
                    DestroyImmediate(l);
                }
            }
            
            // 5. Feneri Kameraya Tak (Eğer yoksa)
            Light fenerLight = mainCam.GetComponent<Light>();
            if (fenerLight == null)
            {
                fenerLight = mainCam.gameObject.AddComponent<Light>();
                fenerLight.type = LightType.Spot;
                fenerLight.range = 25f;
                fenerLight.spotAngle = 65f;
                fenerLight.intensity = 5f;
                fenerLight.color = new Color(0.95f, 0.95f, 0.85f); // Hafif sarımsı fener ışığı
            }

            FenerKontrol fenerKontrol = mainCam.GetComponent<FenerKontrol>();
            if (fenerKontrol == null) fenerKontrol = mainCam.gameObject.AddComponent<FenerKontrol>();
            fenerKontrol.mevcutsarj = 100f;
            
            EditorUtility.SetDirty(mainCam);
        }

        // 6. Mobil joystick butonlarını sil
        GameObject mobileUI = GameObject.Find("UI_Canvas_StarterAssetsInputs_Joysticks");
        if (mobileUI != null) DestroyImmediate(mobileUI);

        Debug.Log("İŞLEM BAŞARILI: Boy uzatıldı, Fener kameraya takıldı, Vücut gizlendi, Ses hatası çözüldü ve Joystickler silindi!");
    }
}
