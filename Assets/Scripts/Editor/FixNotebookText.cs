using UnityEngine;
using UnityEditor;

public class FixNotebookText : EditorWindow
{
    [MenuItem("Tools/Defterin İlk Sayfasını Düzelt")]
    public static void FixText()
    {
        string newText = "Deney başarısız oldu... Hafızanı kaybetmiş olabilirsin. Eğer bunu okuyorsan, bil ki kontrolden çıkan bu kaosu durdurabilecek tek kişi sensin. Sana bırakabildiğim tek şey bu günlüğüm.";
        
        // Sahnede gizli kalmış olabilecek TÜM NotebookManager'ları bul ve güncelle
        NotebookManager[] managers = FindObjectsByType<NotebookManager>(FindObjectsSortMode.None);
        foreach (var manager in managers)
        {
            manager.defaultStartingPage = newText;
            EditorUtility.SetDirty(manager);
        }

        // Defterin tasarımındaki "LeftPageText" (Sol Sayfa Yazısı) objelerini bul ve içindeki metni de zorla değiştir
        UnityEngine.UI.Text[] allTexts = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Text>();
        foreach (var txt in allTexts)
        {
            if (txt.gameObject.name == "LeftPageText" || txt.text.Contains("Gizemli laboratuvara"))
            {
                txt.text = newText;
                EditorUtility.SetDirty(txt);
            }
        }

        Debug.Log("Defter yazıları zorla güncellendi! Sahnede " + managers.Length + " adet NotebookManager bulundu ve düzeltildi.");
    }
}
