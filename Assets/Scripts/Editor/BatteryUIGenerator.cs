using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class BatteryUIGenerator : EditorWindow
{
    [MenuItem("Tools/Pil Göstergesini Sahnede Oluştur")]
    public static void GenerateBatteryUI()
    {
        // Ana Canvas (veya varsa NotebookCanvas'ı kullan)
        GameObject canvasGo = GameObject.Find("NotebookCanvas");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("BatteryCanvas", typeof(RectTransform));
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        // Ana Pil Paneli (Sağ alt köşe)
        GameObject batteryPanel = new GameObject("BatteryPanel", typeof(RectTransform));
        batteryPanel.transform.SetParent(canvasGo.transform, false);
        RectTransform panelRt = batteryPanel.GetComponent<RectTransform>();
        panelRt.anchorMin = new Vector2(1, 0);
        panelRt.anchorMax = new Vector2(1, 0);
        panelRt.pivot = new Vector2(1, 0);
        panelRt.anchoredPosition = new Vector2(-20, 20); // Kenarlardan 20 birim içeride
        panelRt.sizeDelta = new Vector2(250, 50);

        // Arka Plan (Opsiyonel, karanlıkta belli olması için hafif siyah)
        Image bgImg = batteryPanel.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.5f);

        // "Pil:" Yazısı
        GameObject textGo = new GameObject("PilYazisi", typeof(RectTransform));
        textGo.transform.SetParent(batteryPanel.transform, false);
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0, 0);
        textRt.anchorMax = new Vector2(0.3f, 1);
        textRt.offsetMin = textRt.offsetMax = Vector2.zero;
        
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        Text uiText = textGo.AddComponent<Text>();
        uiText.text = "Pil:";
        uiText.font = font;
        uiText.fontSize = 24;
        uiText.fontStyle = FontStyle.Bold;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = Color.white;

        // Barlar için kapsayıcı
        GameObject barsPanel = new GameObject("Bars", typeof(RectTransform));
        barsPanel.transform.SetParent(batteryPanel.transform, false);
        RectTransform barsRt = barsPanel.GetComponent<RectTransform>();
        barsRt.anchorMin = new Vector2(0.3f, 0.1f);
        barsRt.anchorMax = new Vector2(0.95f, 0.9f);
        barsRt.offsetMin = barsRt.offsetMax = Vector2.zero;
        
        HorizontalLayoutGroup hlg = barsPanel.AddComponent<HorizontalLayoutGroup>();
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.spacing = 5;

        // 4 Adet Bar Oluştur
        Image[] barlar = new Image[4];
        for (int i = 0; i < 4; i++)
        {
            GameObject barGo = new GameObject("Bar_" + (i+1), typeof(RectTransform));
            barGo.transform.SetParent(barsPanel.transform, false);
            Image barImg = barGo.AddComponent<Image>();
            barImg.color = Color.green;
            barlar[i] = barImg;
        }

        // FenerKontrol objesini bul ve barları ona bağla
        FenerKontrol fener = FindFirstObjectByType<FenerKontrol>();
        if (fener != null)
        {
            fener.pilBarlari = barlar;
            EditorUtility.SetDirty(fener);
            Debug.Log("Pil göstergesi oluşturuldu ve " + fener.gameObject.name + " objesindeki FenerKontrol'e otomatik bağlandı!");
        }
        else
        {
            Debug.LogWarning("Pil göstergesi oluşturuldu ancak sahnede FenerKontrol bulunamadı. FenerKontrol'e barları elinizle bağlamanız gerekebilir.");
        }

        Undo.RegisterCreatedObjectUndo(batteryPanel, "Create Battery UI");
        Selection.activeGameObject = batteryPanel;
    }
}
