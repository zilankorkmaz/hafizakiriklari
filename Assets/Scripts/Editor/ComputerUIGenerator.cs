using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class ComputerUIGenerator : EditorWindow
{
    [MenuItem("Tools/DOS Bilgisayar Arayüzünü Oluştur")]
    public static void GenerateComputerUI()
    {
        if (FindAnyObjectByType<ComputerUIManager>() != null)
        {
            Debug.LogWarning("Sahnede zaten bir ComputerCanvas/ComputerUIManager var.");
            return;
        }

        // Ana Canvas
        GameObject canvasGo = new GameObject("ComputerCanvas", typeof(RectTransform));
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10; // Defterin veya diğer şeylerin üstünde çıksın
        
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGo.AddComponent<GraphicRaycaster>();
        ComputerUIManager uiManager = canvasGo.AddComponent<ComputerUIManager>();

        // DOS Ekranı Paneli
        GameObject screenPanel = new GameObject("DOS_ScreenPanel", typeof(RectTransform));
        screenPanel.transform.SetParent(canvasGo.transform, false);
        RectTransform screenRt = screenPanel.GetComponent<RectTransform>();
        screenRt.anchorMin = new Vector2(0.1f, 0.1f);
        screenRt.anchorMax = new Vector2(0.9f, 0.9f);
        screenRt.offsetMin = Vector2.zero;
        screenRt.offsetMax = Vector2.zero;
        
        Image screenBg = screenPanel.AddComponent<Image>();
        screenBg.color = new Color(0.02f, 0.05f, 0.02f, 0.95f); // Koyu siyah-yeşil arka plan

        // Kenarlık Çizgisi (İsteğe bağlı şık bir çerçeve)
        Outline outline = screenPanel.AddComponent<Outline>();
        outline.effectColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
        outline.effectDistance = new Vector2(2, 2);

        // Terminal Yazısı
        GameObject textGo = new GameObject("TerminalText", typeof(RectTransform));
        textGo.transform.SetParent(screenPanel.transform, false);
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0.05f, 0.05f);
        textRt.anchorMax = new Vector2(0.95f, 0.95f);
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Text terminalText = textGo.AddComponent<Text>();
        terminalText.font = font;
        terminalText.color = new Color(0.2f, 1f, 0.2f); // Parlak yeşil hacker yazısı
        terminalText.fontSize = 32;
        terminalText.alignment = TextAnchor.UpperLeft;
        terminalText.text = "C:\\> Yükleniyor...";
        
        // Yöneticide bağlantıları kur
        uiManager.computerPanel = screenPanel;
        uiManager.screenText = terminalText;
        
        // Panele başlangıçta kapalı olduğunu belirtmek için false yap
        screenPanel.SetActive(false);

        Undo.RegisterCreatedObjectUndo(canvasGo, "Create Computer UI");
        Selection.activeGameObject = canvasGo;

        Debug.Log("DOS tarzı Hacker/Terminal arayüzü başarıyla oluşturuldu! Hierarchy'de 'ComputerCanvas' olarak görebilirsiniz.");
    }
}
