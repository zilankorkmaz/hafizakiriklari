using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuGenerator : EditorWindow
{
    [MenuItem("Tools/Ana Menü Arayüzünü Sahnede Oluştur")]
    public static void GenerateUI()
    {
        // 1. Canvas oluştur
        GameObject canvasGo = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        // 2. EventSystem
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // 3. Arka Plan
        GameObject bgGo = new GameObject("Background");
        bgGo.transform.SetParent(canvasGo.transform, false);
        StretchFull(bgGo.AddComponent<RectTransform>());
        Image bgImg = bgGo.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.1f, 1f); 

        // 4. Başlık
        GameObject titleGo = new GameObject("TitleText");
        titleGo.transform.SetParent(canvasGo.transform, false);
        RectTransform titleRt = titleGo.AddComponent<RectTransform>();
        titleRt.anchorMin = new Vector2(0.2f, 0.6f);
        titleRt.anchorMax = new Vector2(0.8f, 0.8f); // Daha geniş alan
        titleRt.offsetMin = titleRt.offsetMax = Vector2.zero;
        Text titleTxt = titleGo.AddComponent<Text>();
        titleTxt.font = font; titleTxt.fontSize = 80; titleTxt.fontStyle = FontStyle.Bold;
        titleTxt.alignment = TextAnchor.MiddleCenter; titleTxt.color = Color.white;
        titleTxt.text = "Hafıza Kırıkları";
        Shadow shadow = titleGo.AddComponent<Shadow>(); shadow.effectDistance = new Vector2(4, -4);

        // 5. Butonlar Paneli (Yan Yana)
        GameObject btnPanel = new GameObject("ButtonsPanel");
        btnPanel.transform.SetParent(canvasGo.transform, false);
        RectTransform bpRt = btnPanel.AddComponent<RectTransform>();
        bpRt.anchorMin = new Vector2(0.25f, 0.35f);
        bpRt.anchorMax = new Vector2(0.75f, 0.5f); // Orta hizanın altı
        bpRt.offsetMin = bpRt.offsetMax = Vector2.zero;
        HorizontalLayoutGroup hlg = btnPanel.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 30; hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true; hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true; hlg.childForceExpandHeight = false;

        Button btnStart = CreateButton(btnPanel.transform, "Yeni Oyun", font);
        Button btnSettings = CreateButton(btnPanel.transform, "Ayarlar", font);
        Button btnQuit = CreateButton(btnPanel.transform, "Çıkış", font);

        // 6. Kitap Kapağı Butonu (Sağ Üstte Küçük)
        GameObject coverGo = new GameObject("BookCoverBtn");
        coverGo.transform.SetParent(canvasGo.transform, false);
        RectTransform coverRt = coverGo.AddComponent<RectTransform>();
        coverRt.anchorMin = new Vector2(0.85f, 0.65f); // Sağ üst köşe
        coverRt.anchorMax = new Vector2(0.95f, 0.85f);
        coverRt.offsetMin = coverRt.offsetMax = Vector2.zero;
        Image coverImg = coverGo.AddComponent<Image>();
        coverImg.color = new Color(0.3f, 0.2f, 0.1f, 1f);
        Button coverBtn = coverGo.AddComponent<Button>();
        coverBtn.targetGraphic = coverImg;

        // 7. Açık Kitap Paneli
        GameObject openBookPanel = new GameObject("OpenBookPanel");
        openBookPanel.transform.SetParent(canvasGo.transform, false);
        RectTransform openBookRt = openBookPanel.AddComponent<RectTransform>();
        openBookRt.anchorMin = new Vector2(0.15f, 0.1f);
        openBookRt.anchorMax = new Vector2(0.85f, 0.9f);
        openBookRt.offsetMin = openBookRt.offsetMax = Vector2.zero;
        Image openBookImg = openBookPanel.AddComponent<Image>();
        openBookImg.color = new Color(0.9f, 0.85f, 0.7f, 1f);

        // Sol Sayfa
        Text leftText = CreatePageText(openBookPanel.transform, "LeftPageText", new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), font);
        // Sağ Sayfa
        Text rightText = CreatePageText(openBookPanel.transform, "RightPageText", new Vector2(0.5f, 0.5f), new Vector2(0f, 0.5f), font);

        // Kitap İçi Butonlar
        Button btnClose = CreateSmallButton(openBookPanel.transform, "X", new Vector2(0.95f, 0.9f), new Vector2(1f, 1f), font);
        Button btnPrev = CreateSmallButton(openBookPanel.transform, "<", new Vector2(0.05f, 0.05f), new Vector2(0.15f, 0.12f), font);
        Button btnNext = CreateSmallButton(openBookPanel.transform, ">", new Vector2(0.85f, 0.05f), new Vector2(0.95f, 0.12f), font);

        // 8. MainMenuUIManager Bağlantıları
        GameObject managerGo = new GameObject("MenuManager");
        MainMenuUIManager manager = managerGo.AddComponent<MainMenuUIManager>();
        manager.openBookPanel = openBookPanel;
        manager.leftPageText = leftText;
        manager.rightPageText = rightText;
        manager.prevPageBtn = btnPrev;
        manager.nextPageBtn = btnNext;
        manager.newGameBtnText = btnStart.GetComponentInChildren<Text>();

        // Eventleri Bağla
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnStart.onClick, manager.OnClickStartGame);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnQuit.onClick, manager.OnClickQuitGame);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(coverBtn.onClick, manager.OnClickOpenBook);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnClose.onClick, manager.OnClickCloseBook);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnPrev.onClick, manager.OnClickPrevPage);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btnNext.onClick, manager.OnClickNextPage);

        openBookPanel.SetActive(false);

        Undo.RegisterCreatedObjectUndo(canvasGo, "Create Main Menu Canvas");
        Undo.RegisterCreatedObjectUndo(managerGo, "Create Menu Manager");
        Selection.activeGameObject = managerGo;
        
        Debug.Log("Ana Menü arayüzü kalıcı olarak sahneye eklendi! Artık Play modunda olmasanız bile Inspector'dan her şeyi düzenleyebilirsiniz.");
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    private static Button CreateButton(Transform parent, string text, Font font)
    {
        GameObject go = new GameObject(text + "Btn");
        go.transform.SetParent(parent, false);
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.minHeight = 80; le.preferredHeight = 80;
        Image img = go.AddComponent<Image>(); img.color = new Color(0.4f, 0.25f, 0.15f, 0.9f);
        Button btn = go.AddComponent<Button>(); btn.targetGraphic = img;

        GameObject txtGo = new GameObject("Text");
        txtGo.transform.SetParent(go.transform, false);
        StretchFull(txtGo.AddComponent<RectTransform>());
        Text txt = txtGo.AddComponent<Text>();
        txt.font = font; txt.fontSize = 36; txt.alignment = TextAnchor.MiddleCenter;
        txt.color = new Color(0.95f, 0.9f, 0.8f); txt.text = text;
        return btn;
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
        StretchFull(txtGo.AddComponent<RectTransform>());
        Text txt = txtGo.AddComponent<Text>();
        txt.font = font; txt.fontSize = 28; txt.alignment = TextAnchor.MiddleCenter; txt.color = Color.white; txt.text = text;
        return btn;
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

    [MenuItem("Tools/Ayarlar Panelini Ekle")]
    public static void GenerateSettingsPanel()
    {
        MainMenuUIManager manager = Object.FindFirstObjectByType<MainMenuUIManager>();
        if (manager == null)
        {
            Debug.LogError("Sahnede MenuManager bulunamadı! Önce 'Ana Menü Arayüzünü Sahnede Oluştur' seçeneğini kullanın.");
            return;
        }

        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        // Ayarlar Paneli Arka Plan
        GameObject settingsGo = new GameObject("SettingsPanel");
        settingsGo.transform.SetParent(canvas.transform, false);
        RectTransform sRt = settingsGo.AddComponent<RectTransform>();
        sRt.anchorMin = new Vector2(0.3f, 0.2f);
        sRt.anchorMax = new Vector2(0.7f, 0.8f);
        sRt.offsetMin = sRt.offsetMax = Vector2.zero;
        Image sImg = settingsGo.AddComponent<Image>();
        sImg.color = new Color(0.15f, 0.1f, 0.05f, 0.95f); // Koyu kahverengi arka plan

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // Başlık
        GameObject titleGo = new GameObject("TitleText");
        titleGo.transform.SetParent(settingsGo.transform, false);
        RectTransform tRt = titleGo.AddComponent<RectTransform>();
        tRt.anchorMin = new Vector2(0f, 0.8f); tRt.anchorMax = new Vector2(1f, 1f);
        tRt.offsetMin = tRt.offsetMax = Vector2.zero;
        Text titleText = titleGo.AddComponent<Text>();
        titleText.font = font; titleText.fontSize = 50; titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter; titleText.color = Color.white;
        titleText.text = "SES AYARLARI";

        // Slider'lar (Master, Music, SFX)
        Slider masterSlider = CreateSlider(settingsGo.transform, "Ana Ses", new Vector2(0.1f, 0.6f), new Vector2(0.9f, 0.7f), font);
        Slider musicSlider = CreateSlider(settingsGo.transform, "Müzik Sesi", new Vector2(0.1f, 0.4f), new Vector2(0.9f, 0.5f), font);
        Slider sfxSlider = CreateSlider(settingsGo.transform, "Efekt Sesi", new Vector2(0.1f, 0.2f), new Vector2(0.9f, 0.3f), font);

        // Kapat Butonu
        Button closeBtn = CreateSmallButton(settingsGo.transform, "X", new Vector2(0.9f, 0.9f), new Vector2(1f, 1f), font);

        manager.settingsPanel = settingsGo;
        manager.masterSlider = masterSlider;
        manager.musicSlider = musicSlider;
        manager.sfxSlider = sfxSlider;

        // Ayarlar Butonunu Bul ve Event'i Bağla
        Button settingsBtn = null;
        var btns = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (var b in btns)
        {
            if (b.name == "AyarlarBtn") { settingsBtn = b; break; }
        }

        if (settingsBtn != null)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(settingsBtn.onClick, manager.OnClickOpenSettings);
        }
        
        UnityEditor.Events.UnityEventTools.AddPersistentListener(closeBtn.onClick, manager.OnClickCloseSettings);

        settingsGo.SetActive(false);
        Undo.RegisterCreatedObjectUndo(settingsGo, "Create Settings Panel");
        Debug.Log("Ayarlar Paneli Başarıyla Eklendi! Ana Menü'deki 'Ayarlar' butonuna basarak açabilirsiniz.");
    }

    private static Slider CreateSlider(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Font font)
    {
        GameObject container = new GameObject(label + "Container");
        container.transform.SetParent(parent, false);
        RectTransform rt = container.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.offsetMin = rt.offsetMax = Vector2.zero;

        // Etiket
        GameObject txtGo = new GameObject("Label");
        txtGo.transform.SetParent(container.transform, false);
        RectTransform txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = new Vector2(0f, 0.5f); txtRt.anchorMax = new Vector2(0.35f, 1f);
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        Text txt = txtGo.AddComponent<Text>();
        txt.font = font; txt.fontSize = 32; txt.alignment = TextAnchor.MiddleLeft; txt.color = Color.white; txt.text = label;

        // Standart Unity Slider oluşturma
        GameObject sliderGo = new GameObject("Slider");
        sliderGo.transform.SetParent(container.transform, false);
        RectTransform sliderRt = sliderGo.AddComponent<RectTransform>();
        sliderRt.anchorMin = new Vector2(0.4f, 0.5f); sliderRt.anchorMax = new Vector2(1f, 1f);
        sliderRt.offsetMin = new Vector2(0, -10); sliderRt.offsetMax = new Vector2(0, 10);
        
        GameObject bgGo = new GameObject("Background");
        bgGo.transform.SetParent(sliderGo.transform, false);
        StretchFull(bgGo.AddComponent<RectTransform>());
        Image bgImg = bgGo.AddComponent<Image>(); bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        GameObject fillAreaGo = new GameObject("Fill Area");
        fillAreaGo.transform.SetParent(sliderGo.transform, false);
        StretchFull(fillAreaGo.AddComponent<RectTransform>());

        GameObject fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(fillAreaGo.transform, false);
        StretchFull(fillGo.AddComponent<RectTransform>());
        Image fillImg = fillGo.AddComponent<Image>(); fillImg.color = new Color(0.8f, 0.6f, 0.2f, 1f);

        Slider slider = sliderGo.AddComponent<Slider>();
        slider.fillRect = fillGo.GetComponent<RectTransform>();
        slider.minValue = 0f; slider.maxValue = 1f; slider.value = 1f;
        
        return slider;
    }
}
