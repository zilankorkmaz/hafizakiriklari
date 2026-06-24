using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class GameEndUIGenerator : EditorWindow
{
    [MenuItem("Tools/Final Ekranı Arayüzünü Oluştur")]
    public static void GenerateEndUI()
    {
        if (FindAnyObjectByType<GameEndManager>() != null)
        {
            Debug.LogWarning("Sahnede zaten GameEndManager var.");
            return;
        }

        // Ana Canvas
        GameObject canvasGo = new GameObject("GameEndCanvas", typeof(RectTransform));
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // En üstte çıksın
        
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasGo.AddComponent<GraphicRaycaster>();
        GameEndManager uiManager = canvasGo.AddComponent<GameEndManager>();

        // Siyah Arka Plan
        GameObject blackPanel = new GameObject("BlackPanel", typeof(RectTransform));
        blackPanel.transform.SetParent(canvasGo.transform, false);
        RectTransform blackRt = blackPanel.GetComponent<RectTransform>();
        blackRt.anchorMin = Vector2.zero;
        blackRt.anchorMax = Vector2.one;
        blackRt.offsetMin = Vector2.zero;
        blackRt.offsetMax = Vector2.zero;
        Image bgImage = blackPanel.AddComponent<Image>();
        bgImage.color = Color.black;

        // Hikaye Metni
        GameObject textGo = new GameObject("NarrativeText", typeof(RectTransform));
        textGo.transform.SetParent(blackPanel.transform, false);
        RectTransform textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0.1f, 0.4f);
        textRt.anchorMax = new Vector2(0.9f, 0.8f);
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;
        
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        Text narText = textGo.AddComponent<Text>();
        narText.font = font;
        narText.color = Color.white;
        narText.fontSize = 50;
        narText.alignment = TextAnchor.MiddleCenter;
        narText.text = "Gerçekleri kabullenmeye hazır mısın?";

        // Seçim Paneli (Butonlar)
        GameObject choicePanel = new GameObject("ChoicePanel", typeof(RectTransform));
        choicePanel.transform.SetParent(blackPanel.transform, false);
        RectTransform choiceRt = choicePanel.GetComponent<RectTransform>();
        choiceRt.anchorMin = new Vector2(0.2f, 0.2f);
        choiceRt.anchorMax = new Vector2(0.8f, 0.4f);
        choiceRt.offsetMin = Vector2.zero;
        choiceRt.offsetMax = Vector2.zero;

        // Buton 1: Hatırla
        GameObject btn1Go = new GameObject("Btn_Hatirla", typeof(RectTransform));
        btn1Go.transform.SetParent(choicePanel.transform, false);
        RectTransform btn1Rt = btn1Go.GetComponent<RectTransform>();
        btn1Rt.anchorMin = new Vector2(0.1f, 0.1f);
        btn1Rt.anchorMax = new Vector2(0.45f, 0.9f);
        btn1Rt.offsetMin = Vector2.zero;
        btn1Rt.offsetMax = Vector2.zero;
        Image btn1Img = btn1Go.AddComponent<Image>();
        btn1Img.color = new Color(0.1f, 0.4f, 0.1f); // Koyu yeşil
        Button btn1 = btn1Go.AddComponent<Button>();
        btn1.onClick.AddListener(uiManager.OnRememberSelected);

        GameObject btn1TextGo = new GameObject("Text", typeof(RectTransform));
        btn1TextGo.transform.SetParent(btn1Go.transform, false);
        RectTransform btn1TextRt = btn1TextGo.GetComponent<RectTransform>();
        btn1TextRt.anchorMin = Vector2.zero; btn1TextRt.anchorMax = Vector2.one;
        btn1TextRt.offsetMin = Vector2.zero; btn1TextRt.offsetMax = Vector2.zero;
        Text btn1Text = btn1TextGo.AddComponent<Text>();
        btn1Text.font = font; btn1Text.color = Color.white; btn1Text.fontSize = 40;
        btn1Text.alignment = TextAnchor.MiddleCenter; btn1Text.text = "HATIRLAMAYI SEÇ";

        // Buton 2: Unut
        GameObject btn2Go = new GameObject("Btn_Unut", typeof(RectTransform));
        btn2Go.transform.SetParent(choicePanel.transform, false);
        RectTransform btn2Rt = btn2Go.GetComponent<RectTransform>();
        btn2Rt.anchorMin = new Vector2(0.55f, 0.1f);
        btn2Rt.anchorMax = new Vector2(0.9f, 0.9f);
        btn2Rt.offsetMin = Vector2.zero;
        btn2Rt.offsetMax = Vector2.zero;
        Image btn2Img = btn2Go.AddComponent<Image>();
        btn2Img.color = new Color(0.4f, 0.1f, 0.1f); // Koyu kırmızı
        Button btn2 = btn2Go.AddComponent<Button>();
        btn2.onClick.AddListener(uiManager.OnForgetSelected);

        GameObject btn2TextGo = new GameObject("Text", typeof(RectTransform));
        btn2TextGo.transform.SetParent(btn2Go.transform, false);
        RectTransform btn2TextRt = btn2TextGo.GetComponent<RectTransform>();
        btn2TextRt.anchorMin = Vector2.zero; btn2TextRt.anchorMax = Vector2.one;
        btn2TextRt.offsetMin = Vector2.zero; btn2TextRt.offsetMax = Vector2.zero;
        Text btn2Text = btn2TextGo.AddComponent<Text>();
        btn2Text.font = font; btn2Text.color = Color.white; btn2Text.fontSize = 40;
        btn2Text.alignment = TextAnchor.MiddleCenter; btn2Text.text = "UNUTMAYI SEÇ";

        // Yöneticide bağla
        uiManager.blackPanel = blackPanel;
        uiManager.narrativeText = narText;
        uiManager.choicePanel = choicePanel;

        // Başlangıçta paneli gizle (Awake yapacak gerçi)
        blackPanel.SetActive(false);

        Undo.RegisterCreatedObjectUndo(canvasGo, "Create End UI");
        Selection.activeGameObject = canvasGo;

        Debug.Log("Final Ekranı UI oluşturuldu! GameEndCanvas sahnede yerini aldı.");
    }
}
