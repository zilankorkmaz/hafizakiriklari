using UnityEngine;
using UnityEditor;

public class FixBlueMaterial : EditorWindow
{
    [MenuItem("Tools/Mavi Zemini Düzelt (Beyaz Yap)")]
    public static void FixIt()
    {
        // Geçici bir obje oluşturup onun üzerinden varsayılan materyale ulaşıyoruz
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Material defaultMat = temp.GetComponent<Renderer>().sharedMaterial;
        
        // Rengi eski orijinal beyaz rengine geri döndürüyoruz
        defaultMat.color = Color.white;
        
        // Geçici objeyi siliyoruz
        DestroyImmediate(temp);
        
        Debug.Log("Varsayılan materyal başarıyla eski rengine (Beyaz) döndürüldü!");
    }
}
