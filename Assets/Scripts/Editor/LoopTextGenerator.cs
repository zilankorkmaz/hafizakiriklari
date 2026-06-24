using UnityEngine;
using UnityEditor;

public class LoopTextGenerator : EditorWindow
{
    [MenuItem("Tools/Döngü Duvar Yazılarını Oluştur")]
    public static void GenerateTexts()
    {
        // 2. Tur Yazısı
        GameObject text2 = new GameObject("LoopText_Tur2_CikisYok");
        text2.transform.position = new Vector3(0, 1.5f, 5f);
        TextMesh tm2 = text2.AddComponent<TextMesh>();
        tm2.text = "BURADAN CIKIS YOK";
        tm2.color = Color.red;
        tm2.characterSize = 0.05f;
        tm2.fontSize = 100;
        tm2.alignment = TextAlignment.Center;
        tm2.anchor = TextAnchor.MiddleCenter;
        
        LoopEvent event2 = text2.AddComponent<LoopEvent>();
        event2.activeInLoops = new int[] { 2 };

        // 3. Tur Yazısı
        GameObject text3 = new GameObject("LoopText_Tur3_Uyutuyorlar");
        text3.transform.position = new Vector3(0, 1.5f, 5f); // Aynı yerde dursunlar
        TextMesh tm3 = text3.AddComponent<TextMesh>();
        tm3.text = "BİZİ UYUTUYORLAR";
        tm3.color = new Color(0.6f, 0f, 0f); // Daha koyu kırmızı
        tm3.characterSize = 0.07f;
        tm3.fontSize = 100;
        tm3.alignment = TextAlignment.Center;
        tm3.anchor = TextAnchor.MiddleCenter;

        LoopEvent event3 = text3.AddComponent<LoopEvent>();
        event3.activeInLoops = new int[] { 3 };

        Undo.RegisterCreatedObjectUndo(text2, "Create Text 2");
        Undo.RegisterCreatedObjectUndo(text3, "Create Text 3");
        
        Selection.activeGameObject = text2;
        
        Debug.Log("Duvar yazıları oluşturuldu! Lütfen Hierarchy'den 'LoopText_Tur2...' ve 'Tur3...' objelerini seçip istediğiniz duvara yerleştirin.");
    }
}
