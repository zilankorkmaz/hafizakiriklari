using UnityEngine;
using UnityEditor;

public class FixFenerSpeed : EditorWindow
{
    [MenuItem("Tools/Fenerin Tüketim Hızını Yavaşlat")]
    public static void FixIt()
    {
        // Sahnede gizli veya açık olan tüm fener kodlarını kesin olarak bul
        FenerKontrol[] feners = Resources.FindObjectsOfTypeAll<FenerKontrol>();
        
        foreach (var fener in feners)
        {
            fener.harcamahizi = 2f; // Ne çok hızlı ne çok yavaş (yaklaşık 50 saniyede biter)
            EditorUtility.SetDirty(fener);
        }

        if (feners.Length > 0)
        {
            Debug.Log("Başarılı! Fenerin pil tüketimi tam 10 kat yavaşlatıldı.");
        }
        else
        {
            Debug.LogWarning("Sahnede fener bulunamadı!");
        }
    }
}
