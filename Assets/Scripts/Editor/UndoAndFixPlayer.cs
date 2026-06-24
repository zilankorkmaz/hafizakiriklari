using UnityEngine;
using UnityEditor;

public class UndoAndFixPlayer : EditorWindow
{
    [MenuItem("Tools/Karakter Boyunu ve Feneri Kurtar")]
    public static void FixIt()
    {
        GameObject player = GameObject.Find("PlayerCapsule");
        if (player == null) player = GameObject.Find("FirstPersonController");

        if (player != null)
        {
            // 1. Yanlışlıkla kapanmış olabilecek TÜM MeshRenderer'ları geri aç (fener modeli vs.)
            MeshRenderer[] allRenderers = player.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var mr in allRenderers)
            {
                mr.enabled = true;
            }

            // Kendi kapsül vücudumuzu kapatmanın doğru yolu (Sadece Kapsülün mesh'ini sil)
            MeshFilter mf = player.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null && mf.sharedMesh.name.Contains("Capsule"))
            {
                MeshRenderer mr = player.GetComponent<MeshRenderer>();
                if (mr != null) mr.enabled = false;
            }

            // 2. BOYLARI DÜZELT (StarterAssets Cinemachine mantığına göre)
            Transform cameraRoot = player.transform.Find("PlayerCameraRoot");
            if (cameraRoot != null)
            {
                // Kameranın asıl bağlandığı kökü yukarı kaldır
                Vector3 pos = cameraRoot.localPosition;
                pos.y = 1.6f; 
                cameraRoot.localPosition = pos;
                EditorUtility.SetDirty(cameraRoot);
            }

            // Normal kameranın kendi offsetini sıfırla
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.transform.localPosition = Vector3.zero; // Cinemachine kullanılıyorsa bu sıfır olmalı
                cam.nearClipPlane = 0.05f;
                EditorUtility.SetDirty(cam);
            }

            // 3. Fener şarjını fulle
            FenerKontrol fener = FindFirstObjectByType<FenerKontrol>();
            if (fener != null)
            {
                fener.mevcutsarj = 100f;
                EditorUtility.SetDirty(fener);
            }

            Debug.Log("Karakterin Cinemachine boyu ayarlandı, fenerin şarjı fullendi ve kapsül gizlendi!");
        }
    }
}
