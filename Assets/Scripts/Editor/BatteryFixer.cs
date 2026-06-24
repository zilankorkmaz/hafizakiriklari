using UnityEngine;
using UnityEditor;

public class BatteryFixer : EditorWindow
{
    [MenuItem("Tools/Pilleri Otomatik Düzelt")]
    public static void FixBatteries()
    {
        // 1. Sahnede üstünde "PilTopla" scripti olan her şeyi bul
        PilTopla[] piller = FindObjectsByType<PilTopla>(FindObjectsSortMode.None);
        
        if (piller.Length == 0)
        {
            Debug.LogWarning("Sahnede hiç pil bulunamadı! Lütfen pillere 'PilTopla' scriptini eklediğinizden emin olun.");
            return;
        }

        foreach (PilTopla pil in piller)
        {
            // Eğer objede Collider yoksa Sphere Collider ekle
            Collider col = pil.GetComponent<Collider>();
            if (col == null)
            {
                col = pil.gameObject.AddComponent<SphereCollider>();
            }

            // Collider'ı kesinlikle Trigger yap ve etki alanını kocaman yap ki kolay toplansın
            col.isTrigger = true;
            
            if (col is SphereCollider sphere)
            {
                sphere.radius = 2f; // Oyuncu yanından geçse bile toplasın (Büyük alan)
            }
            else if (col is BoxCollider box)
            {
                box.size = new Vector3(3, 3, 3);
            }

            EditorUtility.SetDirty(pil.gameObject);
        }

        // 2. Oyuncunun etiketini kesinlikle "Player" yap
        GameObject player = GameObject.Find("PlayerCapsule");
        if (player == null) player = GameObject.Find("FirstPersonController");

        if (player != null)
        {
            player.tag = "Player";
            EditorUtility.SetDirty(player);
        }

        Debug.Log("İŞLEM BAŞARILI: " + piller.Length + " adet pilin toplanma alanı devasa büyütüldü ve Oyuncu etiketi düzeltildi. Artık yanından geçseniz bile toplanacak!");
    }
}
