using UnityEngine;
using UnityEditor;

public class WaterFixer : EditorWindow
{
    [MenuItem("Tools/Sulari Görünür Yap (Y=0.05)")]
    public static void FixWater()
    {
        int count = 0;

        ParticleSystem[] particles = FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None);
        foreach (var p in particles)
        {
            Vector3 pos = p.transform.position;
            // Eğer Y değeri çok düşükse (zemine yakınsa) yukarı kaldır
            if (pos.y < 0.1f) 
            {
                pos.y = 0.05f;
                p.transform.position = pos;
                EditorUtility.SetDirty(p.gameObject);
                count++;
            }
        }

        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("ElectricPuddle") || obj.name.Contains("Particle System"))
            {
                Vector3 pos = obj.transform.position;
                if (pos.y < 0.1f)
                {
                    pos.y = 0.05f;
                    obj.transform.position = pos;
                    EditorUtility.SetDirty(obj);
                    count++;
                }
            }
        }

        Debug.Log("İŞLEM TAMAM: Toplam " + count + " adet su/partikül zeminden 0.05 birim yukarı taşındı. Artık görünecekler!");
    }
}
