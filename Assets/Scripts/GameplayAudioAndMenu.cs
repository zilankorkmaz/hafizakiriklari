using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using StarterAssets;

/// <summary>
/// Arka plan muzigi/ambians, SFX kaynagi, Tab ile duraklatma menusu,
/// Master/Muzik/SFX sliderlari (PlayerPrefs) ve "Nasil oynanir" metni.
/// Sahneye bos bir GameObject ekleyip bu bileseni surukleyin; veya
/// Tools / Hafiza / Sahneye Ses ve Menu Ekle.
/// </summary>
[DisallowMultipleComponent]
public class GameplayAudioAndMenu : MonoBehaviour
{
    public static GameplayAudioAndMenu Instance { get; private set; }

    public float SfxVolume => _volMaster * _volSfx; // Dışarıdan SFX ses seviyesini okumak için eklendi

    const string PrefsMaster = "hk_vol_master";
    const string PrefsMusic = "hk_vol_music";
    const string PrefsSfx = "hk_vol_sfx";

    [Header("Ses")]
    [Tooltip("Dongu olarak calan arka plan (ambians veya muzik). Bos birakilirsa Resources yolu denenir.")]
    [SerializeField] AudioClip ambienceClip;

    [Tooltip("Ornek: Audio/ambience — Resources klasorune WAV koyarsaniz otomatik yuklenir.")]
    [SerializeField] string resourcesAmbiencePath = "";

    [SerializeField, Range(0f, 1f)] float ambienceBaseVolume = 0.4f;

    [Header("Duraklatma")]
    [SerializeField] KeyCode pauseKey = KeyCode.Tab;

    [TextArea(4, 12)]
    [SerializeField] string howToPlayText =
        "WASD: Hareket\nFare: Etrafa bakma\nE: Etkilesim (not vb.)\nEsc: Acik not panelini kapatma (sahne ayarina gore)\n\nDuraklatma ve ses: Tab";

    AudioSource _music;
    AudioSource _sfx;
    Canvas _canvas;
    GameObject _pauseRoot;
    GameObject _helpBlock;
    Slider _masterSlider;
    Slider _musicSlider;
    Slider _sfxSlider;

    bool _paused;
    FirstPersonController _fp;
    StarterAssetsInputs _inputs;
    Font _uiFont;

    float _volMaster = 1f;
    float _volMusic = 1f;
    float _volSfx = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _uiFont = GetUIFont();
        EnsureEventSystem();
        BuildAudio();
        BuildUi();
    }

    void Start()
    {
        LoadPrefs();
        ApplyVolumes();

        var clip = ambienceClip;
        if (clip == null && !string.IsNullOrWhiteSpace(resourcesAmbiencePath))
            clip = Resources.Load<AudioClip>(resourcesAmbiencePath.Trim());

        if (clip != null && _music != null)
        {
            _music.clip = clip;
            _music.Play();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            TogglePause();
    }

    public void PlaySfx(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null || _sfx == null)
            return;
        var v = Mathf.Clamp01(_volMaster * _volSfx * volumeScale);
        _sfx.PlayOneShot(clip, v);
    }

    void TogglePause()
    {
        if (_paused)
            Resume();
        else
            Pause();
    }

    void Pause()
    {
        _paused = true;
        Time.timeScale = 0f;
        if (_pauseRoot != null)
            _pauseRoot.SetActive(true);

        FindPlayerControls();
        if (_fp != null)
            _fp.enabled = false;
        if (_inputs != null)
            _inputs.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Resume()
    {
        _paused = false;
        Time.timeScale = 1f;
        if (_pauseRoot != null)
            _pauseRoot.SetActive(false);

        if (_fp != null)
            _fp.enabled = true;
        if (_inputs != null)
        {
            _inputs.enabled = true;
            _inputs.cursorLocked = true;
        }

        if (_fp != null || _inputs != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void FindPlayerControls()
    {
        if (_fp != null && _inputs != null)
            return;
        _fp = FindFirstObjectByType<FirstPersonController>();
        if (_fp != null)
            _inputs = _fp.GetComponent<StarterAssetsInputs>();
    }

    void LoadPrefs()
    {
        _volMaster = PlayerPrefs.GetFloat(PrefsMaster, 1f);
        _volMusic = PlayerPrefs.GetFloat(PrefsMusic, 1f);
        _volSfx = PlayerPrefs.GetFloat(PrefsSfx, 1f);
        if (_masterSlider != null)
            _masterSlider.SetValueWithoutNotify(_volMaster);
        if (_musicSlider != null)
            _musicSlider.SetValueWithoutNotify(_volMusic);
        if (_sfxSlider != null)
            _sfxSlider.SetValueWithoutNotify(_volSfx);
    }

    void SavePrefs()
    {
        PlayerPrefs.SetFloat(PrefsMaster, _volMaster);
        PlayerPrefs.SetFloat(PrefsMusic, _volMusic);
        PlayerPrefs.SetFloat(PrefsSfx, _volSfx);
        PlayerPrefs.Save();
    }

    void ApplyVolumes()
    {
        if (_music != null)
            _music.volume = Mathf.Clamp01(_volMaster * _volMusic * ambienceBaseVolume);
    }

    void BuildAudio()
    {
        var musicGo = new GameObject("AmbienceAudioSource");
        musicGo.transform.SetParent(transform, false);
        _music = musicGo.AddComponent<AudioSource>();
        _music.loop = true;
        _music.playOnAwake = false;
        _music.spatialBlend = 0f;
        _music.ignoreListenerPause = true;

        var sfxGo = new GameObject("SfxAudioSource");
        sfxGo.transform.SetParent(transform, false);
        _sfx = sfxGo.AddComponent<AudioSource>();
        _sfx.loop = false;
        _sfx.playOnAwake = false;
        _sfx.spatialBlend = 0f;
    }

    void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
            return;
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    void BuildUi()
    {
        var canvasGo = new GameObject("GameplayMenuCanvas", typeof(RectTransform));
        canvasGo.transform.SetParent(transform, false);
        _canvas = canvasGo.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 500;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        _pauseRoot = new GameObject("PauseOverlay", typeof(RectTransform));
        _pauseRoot.transform.SetParent(canvasGo.transform, false);
        var overlayRt = _pauseRoot.GetComponent<RectTransform>();
        StretchFull(overlayRt);
        var dim = _pauseRoot.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.82f);
        dim.raycastTarget = true;
        _pauseRoot.SetActive(false);

        var center = new GameObject("MenuPanel", typeof(RectTransform));
        center.transform.SetParent(_pauseRoot.transform, false);
        var centerRt = center.GetComponent<RectTransform>();
        centerRt.anchorMin = new Vector2(0.5f, 0.5f);
        centerRt.anchorMax = new Vector2(0.5f, 0.5f);
        centerRt.pivot = new Vector2(0.5f, 0.5f);
        centerRt.sizeDelta = new Vector2(520f, 560f);
        centerRt.anchoredPosition = Vector2.zero;
        var panelBg = center.AddComponent<Image>();
        panelBg.color = new Color(0.12f, 0.12f, 0.14f, 0.98f);
        var vlg = center.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(24, 24, 20, 20);
        vlg.spacing = 12f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        AddTitle(center.transform, "Duraklatma — Ses");
        AddHint(center.transform, pauseKey == KeyCode.Tab
            ? "Tab: ac / kapat (Esc not paneli ile cakisabilir; pause tusu Inspector'dan degistirilebilir)."
            : pauseKey + " ile acilir / kapanir.");

        _masterSlider = AddLabeledSlider(center.transform, "Genel ses", v =>
        {
            _volMaster = v;
            ApplyVolumes();
            SavePrefs();
        });
        _musicSlider = AddLabeledSlider(center.transform, "Muzik / ambians", v =>
        {
            _volMusic = v;
            ApplyVolumes();
            SavePrefs();
        });
        _sfxSlider = AddLabeledSlider(center.transform, "Efektler (SFX)", v =>
        {
            _volSfx = v;
            SavePrefs();
        });

        CreateButton(center.transform, "Nasil oynanir?", () =>
        {
            if (_helpBlock != null)
                _helpBlock.SetActive(!_helpBlock.activeSelf);
        });

        _helpBlock = new GameObject("HowToPlayBlock", typeof(RectTransform));
        _helpBlock.transform.SetParent(center.transform, false);
        var helpLe = _helpBlock.AddComponent<LayoutElement>();
        helpLe.minHeight = 160f;
        helpLe.preferredHeight = 200f;
        helpLe.flexibleHeight = 0f;
        var helpBg = _helpBlock.AddComponent<Image>();
        helpBg.color = new Color(0.08f, 0.08f, 0.1f, 1f);
        var helpTextGo = new GameObject("Text", typeof(RectTransform));
        helpTextGo.transform.SetParent(_helpBlock.transform, false);
        var helpRt = helpTextGo.GetComponent<RectTransform>();
        StretchFull(helpRt, 12f, 12f, 12f, 12f);
        var helpTxt = helpTextGo.AddComponent<Text>();
        helpTxt.font = _uiFont;
        helpTxt.fontSize = 28;
        helpTxt.color = new Color(0.92f, 0.92f, 0.92f);
        helpTxt.alignment = TextAnchor.UpperLeft;
        helpTxt.horizontalOverflow = HorizontalWrapMode.Wrap;
        helpTxt.verticalOverflow = VerticalWrapMode.Truncate;
        helpTxt.text = howToPlayText;
        _helpBlock.SetActive(false);

        CreateButton(center.transform, "Devam", Resume);

        LayoutRebuilder.ForceRebuildLayoutImmediate(centerRt);
    }

    void AddTitle(Transform parent, string text)
    {
        var go = new GameObject("Title", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var le = go.AddComponent<LayoutElement>();
        le.minHeight = 44f;
        le.preferredHeight = 44f;
        var t = go.AddComponent<Text>();
        t.font = _uiFont;
        t.fontSize = 30;
        t.fontStyle = FontStyle.Bold;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.white;
        t.text = text;
    }

    void AddHint(Transform parent, string text)
    {
        var go = new GameObject("Hint", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var le = go.AddComponent<LayoutElement>();
        le.minHeight = 36f;
        le.preferredHeight = 48f;
        var t = go.AddComponent<Text>();
        t.font = _uiFont;
        t.fontSize = 24;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = new Color(0.75f, 0.78f, 0.85f);
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.text = text;
    }

    Slider AddLabeledSlider(Transform parent, string label, Action<float> onChanged)
    {
        var row = new GameObject("Row_" + label, typeof(RectTransform));
        row.transform.SetParent(parent, false);
        var rowLe = row.AddComponent<LayoutElement>();
        rowLe.minHeight = 72f;
        rowLe.preferredHeight = 72f;
        var v = row.AddComponent<VerticalLayoutGroup>();
        v.spacing = 6f;
        v.childAlignment = TextAnchor.UpperLeft;
        v.childControlWidth = true;
        v.childControlHeight = true;
        v.childForceExpandHeight = false;
        v.childForceExpandWidth = true;

        var labGo = new GameObject("Label", typeof(RectTransform));
        labGo.transform.SetParent(row.transform, false);
        var labLe = labGo.AddComponent<LayoutElement>();
        labLe.minHeight = 22f;
        var lab = labGo.AddComponent<Text>();
        lab.font = _uiFont;
        lab.fontSize = 26;
        lab.alignment = TextAnchor.MiddleLeft;
        lab.color = new Color(0.9f, 0.9f, 0.9f);
        lab.text = label;

        var sliderGo = new GameObject("Slider", typeof(RectTransform));
        sliderGo.transform.SetParent(row.transform, false);
        var sLe = sliderGo.AddComponent<LayoutElement>();
        sLe.minHeight = 32f;
        sLe.preferredHeight = 32f;
        var sliderRt = sliderGo.GetComponent<RectTransform>();
        sliderRt.anchorMin = Vector2.zero;
        sliderRt.anchorMax = Vector2.one;
        sliderRt.sizeDelta = Vector2.zero;
        sliderRt.anchoredPosition = Vector2.zero;

        var bg = new GameObject("Background", typeof(RectTransform));
        bg.transform.SetParent(sliderGo.transform, false);
        var bgRt = bg.GetComponent<RectTransform>();
        StretchFull(bgRt);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.22f, 0.22f, 0.26f, 1f);
        bgImg.raycastTarget = true;

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderGo.transform, false);
        var fillAreaRt = fillArea.GetComponent<RectTransform>();
        StretchFull(fillAreaRt, 8f, 8f, 8f, 8f);

        var fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(0f, 1f);
        fillRt.pivot = new Vector2(0f, 0.5f);
        fillRt.sizeDelta = Vector2.zero;
        fillRt.anchoredPosition = Vector2.zero;
        var fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.35f, 0.55f, 0.85f, 1f);
        fillImg.raycastTarget = false;

        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderGo.transform, false);
        var handleAreaRt = handleArea.GetComponent<RectTransform>();
        StretchFull(handleAreaRt, 8f, 8f, 8f, 8f);

        var handleKnob = new GameObject("Handle", typeof(RectTransform));
        handleKnob.transform.SetParent(handleArea.transform, false);
        var handleRt = handleKnob.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(24f, 0f);
        var handleImg = handleKnob.AddComponent<Image>();
        handleImg.color = Color.white;

        var slider = sliderGo.AddComponent<Slider>();
        slider.fillRect = fillRt;
        slider.targetGraphic = handleImg;
        slider.handleRect = handleRt;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.onValueChanged.AddListener(v => onChanged(v));

        return slider;
    }

    GameObject CreateButton(Transform parent, string caption, Action onClick)
    {
        var go = new GameObject("Btn_" + caption, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var le = go.AddComponent<LayoutElement>();
        le.minHeight = 48f;
        le.preferredHeight = 48f;
        var img = go.AddComponent<Image>();
        img.color = new Color(0.28f, 0.45f, 0.72f, 1f);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.35f, 0.55f, 0.88f);
        colors.pressedColor = new Color(0.2f, 0.32f, 0.55f);
        btn.colors = colors;
        btn.onClick.AddListener(() => onClick?.Invoke());

        var textGo = new GameObject("Text", typeof(RectTransform));
        textGo.transform.SetParent(go.transform, false);
        var textRt = textGo.GetComponent<RectTransform>();
        StretchFull(textRt);
        var txt = textGo.AddComponent<Text>();
        txt.font = _uiFont;
        txt.fontSize = 28;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        txt.text = caption;

        return go;
    }

    static void StretchFull(RectTransform rt, float l = 0f, float r = 0f, float t = 0f, float b = 0f)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(l, b);
        rt.offsetMax = new Vector2(-r, -t);
    }

    static Font GetUIFont()
    {
        var f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (f == null)
            f = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return f;
    }
}
