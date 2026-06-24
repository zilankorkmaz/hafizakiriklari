using UnityEngine;
using UnityEditor;
using System.Linq;

public class FixPlayerPhysicsAndCamera : EditorWindow
{
    [MenuItem("Tools/Kamera ve Çarpışma Sorunlarını Düzelt")]
    public static void FixIssues()
    {
        // 1. OYUNCUYU BUL (StarterAssets FirstPersonController)
        GameObject player = GameObject.Find("PlayerCapsule");
        if (player == null) player = GameObject.Find("FirstPersonController");
        
        if (player != null)
        {
            // Boyunu ve kalınlığını düzelt
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.height = 2.0f;
                cc.radius = 0.4f;
                cc.center = new Vector3(0, 1.0f, 0);
                EditorUtility.SetDirty(cc);
            }

            // Kendi vücudunu (kapsülü) görmesini engelle
            MeshRenderer mr = player.GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                mr.enabled = false; // Vücut görünmez olsun
                EditorUtility.SetDirty(mr);
            }

            // 2. KAMERAYI BUL VE DÜZELT
            Camera cam = player.GetComponentInChildren<Camera>();
            if (cam == null) cam = Camera.main;

            if (cam != null)
            {
                // Duvarların arkasını görmeyi engelle (Near Clip Plane küçültülür)
                cam.nearClipPlane = 0.03f;
                
                // Kamerayı tam göz hizasına (Y = 1.6) çıkar
                Vector3 localPos = cam.transform.localPosition;
                localPos.y = 1.6f;
                localPos.x = 0f; // Tam merkeze al
                cam.transform.localPosition = localPos;
                
                EditorUtility.SetDirty(cam);
            }
            
            Debug.Log("1) Karakterin boyu, kamera hizası ve duvar arkasını görme sorunu çözüldü!");
        }
        else
        {
            Debug.LogWarning("Sahnede Player (Oyuncu) bulunamadı! İsmi FirstPersonController veya PlayerCapsule olmalı.");
        }

        // 3. ÇEVREDEKİ EŞYALARIN İÇİNDEN GEÇMEYİ ENGELLE
        // Sahnede MeshRenderer'ı olan ama Collider'ı olmayan eşyalara otomatik Collider ekle
        MeshRenderer[] allMeshes = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);
        int addedColliders = 0;
        foreach (var mesh in allMeshes)
        {
            // Eğer objede veya parent'ında hiçbir collider yoksa ve bu bir eşyaysa (Desk, Table, TV, Wall vs.)
            if (mesh.GetComponentInParent<Collider>() == null)
            {
                string name = mesh.gameObject.name.ToLower();
                // Basit dekoratif küçük objeler dışındaki şeylere çarpışma ekle
                if (!name.Contains("light") && !name.Contains("decal"))
                {
                    mesh.gameObject.AddComponent<MeshCollider>();
                    addedColliders++;
                }
            }
        }

        Debug.Log("2) İçinden geçilen " + addedColliders + " adet eşyaya otomatik çarpışma (Collider) eklendi!");
    }
}
