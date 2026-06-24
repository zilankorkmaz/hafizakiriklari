using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class NotebookUIGenerator : EditorWindow
{
    [MenuItem("Tools/Oyun İçi Defter Arayüzünü Oluştur")]
    public static void GenerateNotebookUI()
    {
        NotebookManager manager = Object.FindFirstObjectByType<NotebookManager>();
        if (manager == null)
        {
            GameObject managerGo = new GameObject("NotebookManager");
            manager = managerGo.AddComponent<NotebookManager>();
            Undo.RegisterCreatedObjectUndo(managerGo, "Create Notebook Manager");
        }

        // Canvas
        GameObject canvasGo = new GameObject("NotebookCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 900; // Pause menüsünün üstünde çıksın
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // ------------------
        // 1. KİTAP PANELİ (2 Sayfa)
        // ------------------
        GameObject bookPanel = new GameObject("BookPanel");
        bookPanel.transform.SetParent(canvasGo.transform, false);
        RectTransform openBookRt = bookPanel.AddComponent<RectTransform>();
        openBookRt.anchorMin = new Vector2(0.15f, 0.1f);
        openBookRt.anchorMax = new Vector2(0.85f, 0.9f);
        openBookRt.offsetMin = openBookRt.offsetMax = Vector2.zero;
        Image openBookImg = bookPanel.AddComponent<Image>();
        openBookImg.color = new Color(0.9f, 0.85f, 0.7f, 1f);

        Text leftText = CreatePageText(bookPanel.transform, "LeftPageText", new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), font);
        Text rightText = CreatePageText(bookPanel.transform, "RightPageText", new Vector2(0.5f, 0.5f), new Vector2(0f, 0.5f), font);

        Button btnCloseBook = CreateSmallButton(bookPanel.transform, "X", new Vector2(0.95f, 0.9f), new Vector2(1f, 1f), font);
        Button btnPrev = CreateSmallButton(bookPanel.transform, "<", new Vector2(0.05f, 0.05f), new Vector2(0.15f, 0.12f), font);
        Button btnNext = CreateSmallButton(bookPanel.transform, ">", new Vector2(0.85f, 0.05f), new Vector2(0.95f, 0.12f), font);

        // ------------------
        // 2. TEK SAYFA PANELİ (Yerdeki Kağıtlar)
        // ------------------
        GameObject singlePanel = new GameObject("SingleNotePanel");
        singlePanel.transform.SetParent(canvasGo.transform, false);
        RectTransform singleRt = singlePanel.AddComponent<RectTransform>();
        singleRt.anchorMin = new Vector2(0.3f, 0.1f);
        singleRt.anchorMax = new Vector2(0.7f, 0.9f);
        singleRt.offsetMin = singleRt.offsetMax = Vector2.zero;
        Image singleImg = singlePanel.AddComponent<Image>();
        singleImg.color = new Color(0.92f, 0.88f, 0.75f, 1f);

        Text singleText = CreatePageText(singlePanel.transform, "SingleNoteText", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), font);
        singleText.rectTransform.sizeDelta = new Vector2(650, 750); // Ortada daha büyük

        Button btnCloseNote = CreateSmallButton(singlePanel.transform, "X", new Vector2(0.9f, 0.9f), new Vector2(0.98f, 0.98f), font);

        // Objeleri Manager'a Bağla
        manager.bookPanel = bookPanel;
        // manager.leftPageText = leftText;
        // manager.rightPageText = rightText;
        manager.nextButton = btnNext;
        manager.prevButton = btnPrev;

        manager.singleNotePanel = singlePanel;
        // manager.singleNoteText = singleText;

        // Buton Eventlerini Bağla
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnCloseBook.onClick, manager.CloseNotebook);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnCloseNote.onClick, manager.CloseSingleNote);

        bookPanel.SetActive(false);
        singlePanel.SetActive(false);

        Undo.RegisterCreatedObjectUndo(canvasGo, "Create Notebook Canvas");
        EditorUtility.SetDirty(manager);

        Debug.Log("Oyun İçi Defter Arayüzü başarıyla oluşturuldu! Menüden 'Oyun İçi Defter Arayüzünü Oluştur' çalıştırdınız.");
    }

    private static Text CreatePageText(Transform parent, string name, Vector2 anchor, Vector2 pivot, Font font)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor; rt.anchorMax = anchor; rt.pivot = pivot;
        rt.sizeDelta = new Vector2(560, 500); rt.anchoredPosition = Vector2.zero;
        Text txt = go.AddComponent<Text>();
        txt.font = font; txt.fontStyle = FontStyle.BoldAndItalic; txt.fontSize = 46;
        txt.alignment = TextAnchor.MiddleCenter; txt.color = new Color(0.11f, 0.11f, 0.11f, 1f);
        return txt;
    }

    private static Button CreateSmallButton(Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax, Font font)
    {
        GameObject go = new GameObject(text + "Btn");
        go.transform.SetParent(parent, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.offsetMin = rt.offsetMax = Vector2.zero;
        Image img = go.AddComponent<Image>(); img.color = new Color(0.3f, 0.2f, 0.1f, 1f);
        Button btn = go.AddComponent<Button>(); btn.targetGraphic = img;

        GameObject txtGo = new GameObject("Text");
        txtGo.transform.SetParent(go.transform, false);
        RectTransform txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        Text txt = txtGo.AddComponent<Text>();
        txt.font = font; txt.fontSize = 28; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = text;
        return btn;
    }
}
