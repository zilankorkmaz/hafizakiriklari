using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    // Oyun durumları ve kalıcı sayfalar
    public static bool IsGameOver = false;
    public static bool HasChosenForget = false;
    public static List<string> PersistedPages = new List<string>();

    [Header("Hikaye Metniniz")]
    [TextArea(3, 10)]
    public string defaultWarningNote = "Eğer tekrar hafızanı kaybedersen, bil ki bu yaptığını hatırlayacağına öl daha iyi. Sana son notum da bu.";
    [TextArea(3, 10)]
    public string labFinalIntroText = "Karanlık, soğuk ve yabancı bir yer... Kendimi burada buldum. Kim olduğuma dair hiçbir fikrim yok. Bu notlar bana yardımcı olabilir mi?";

    [Header("Arayüz Bağlantıları")]
    public GameObject openBookPanel;
    public Text leftPageText;
    public Text rightPageText;
    public Button prevPageBtn;
    public Button nextPageBtn;
    public Text newGameBtnText; 

    [Header("Ayarlar Arayüzü")]
    public GameObject settingsPanel;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Menü Müziği")]
    public AudioClip menuMusic;
    private AudioSource _audioSource;

    private List<string> _activeBookContent = new List<string>();
    private int _currentPageIndex = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (newGameBtnText != null)
        {
            newGameBtnText.text = IsGameOver ? "Tekrar Başla" : "Yeni Oyun";
        }

        if (openBookPanel != null)
            openBookPanel.SetActive(false);
            
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        PrepareBookContent();
        LoadSettings();

        // Müzik çalar oluşturma
        _audioSource = gameObject.GetComponent<AudioSource>();
        if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
        
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        
        if (menuMusic != null)
        {
            _audioSource.clip = menuMusic;
            _audioSource.Play();
        }

        UpdateMusicVolume();
    }

    public static void SetMenuState(bool isGameOver, List<string> pagesFromGame, bool choseForget)
    {
        IsGameOver = isGameOver;
        HasChosenForget = choseForget;
        PersistedPages.Clear();
        if (pagesFromGame != null && pagesFromGame.Count > 0)
        {
            PersistedPages.AddRange(pagesFromGame);
        }
    }

    private void PrepareBookContent()
    {
        _activeBookContent.Clear();
        _activeBookContent.Add(defaultWarningNote);
        _activeBookContent.Add(""); // Sağ sayfayı boş bırakmak için

        if (HasChosenForget) 
        { 
            // Sadece uyarı notu kalır 
        }
        else if (PersistedPages.Count > 0) 
        {
            _activeBookContent.AddRange(PersistedPages);
        }
        else 
        {
            _activeBookContent.Add(labFinalIntroText);
        }
    }

    public void OnClickOpenBook()
    {
        _currentPageIndex = 0;
        UpdateBookDisplay();
        if (openBookPanel != null) openBookPanel.SetActive(true);
    }

    public void OnClickCloseBook()
    {
        if (openBookPanel != null) openBookPanel.SetActive(false);
    }

    public void OnClickNextPage() { ChangePage(2); }
    public void OnClickPrevPage() { ChangePage(-2); }

    private void ChangePage(int amount)
    {
        _currentPageIndex += amount;
        UpdateBookDisplay();
    }

    private void UpdateBookDisplay()
    {
        if (leftPageText != null)
            leftPageText.text = (_currentPageIndex >= 0 && _currentPageIndex < _activeBookContent.Count) ? _activeBookContent[_currentPageIndex] : "";

        int rightIndex = _currentPageIndex + 1;
        if (rightPageText != null)
            rightPageText.text = (rightIndex >= 0 && rightIndex < _activeBookContent.Count) ? _activeBookContent[rightIndex] : "";

        if (prevPageBtn != null) prevPageBtn.gameObject.SetActive(_currentPageIndex > 0);
        if (nextPageBtn != null) nextPageBtn.gameObject.SetActive(_currentPageIndex + 2 < _activeBookContent.Count);
    }

    public void OnClickStartGame()
    {
        Debug.Log("Yeni Oyun butonuna tıklandı! lab_final yükleniyor...");
        IsGameOver = false;
        
        // Seslerin kapalı kalmasını önlemek için resetle
        AudioListener.pause = false;

        // Yeni oyuna başlarken hafızadaki 'son' yazısını sıfırla ki bir sonraki seferde ana menü temiz gelsin
        PlayerPrefs.SetInt("GameEndState", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("lab_final");
    }

    public void OnClickQuitGame()
    {
        Debug.Log("Çıkış butonuna tıklandı! Uygulama kapatılıyor...");
        Application.Quit();
    }

    // AYARLAR MENÜSÜ FONKSİYONLARI
    public void OnClickOpenSettings()
    {
        LoadSettings();
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void OnClickCloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        FixSlider(masterSlider);
        if (masterSlider != null)
        {
            masterSlider.interactable = true;
            masterSlider.onValueChanged.RemoveAllListeners();
            masterSlider.value = PlayerPrefs.GetFloat("hk_vol_master", 1f);
            masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        }
        FixSlider(musicSlider);
        if (musicSlider != null)
        {
            musicSlider.interactable = true;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.value = PlayerPrefs.GetFloat("hk_vol_music", 1f);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        FixSlider(sfxSlider);
        if (sfxSlider != null)
        {
            sfxSlider.interactable = true;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.value = PlayerPrefs.GetFloat("hk_vol_sfx", 1f);
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }
    }

    private void FixSlider(Slider slider)
    {
        if (slider == null) return;
        
        // Handle yoksa slider'ı kaydırmak (sürüklemek) inanılmaz zorlaşır ve takılır.
        // Bu yüzden kodla görünmez bir "Tutma Noktası (Handle)" ekliyoruz.
        if (slider.handleRect == null)
        {
            // Kaydırma alanı (Handle Slide Area)
            GameObject handleArea = new GameObject("FakeSlideArea", typeof(RectTransform));
            handleArea.transform.SetParent(slider.transform, false);
            RectTransform areaRt = handleArea.GetComponent<RectTransform>();
            areaRt.anchorMin = Vector2.zero;
            areaRt.anchorMax = Vector2.one;
            areaRt.offsetMin = Vector2.zero;
            areaRt.offsetMax = Vector2.zero;

            // Tutma noktası (Handle)
            GameObject handleObj = new GameObject("FakeHandle", typeof(RectTransform));
            handleObj.transform.SetParent(handleArea.transform, false);
            RectTransform handleRt = handleObj.GetComponent<RectTransform>();
            
            // Yüksekliği parent'a (Slider) eşitleyip genişliği 40 piksel yapıyoruz
            handleRt.anchorMin = new Vector2(0f, 0f);
            handleRt.anchorMax = new Vector2(0f, 1f);
            handleRt.pivot = new Vector2(0.5f, 0.5f);
            handleRt.sizeDelta = new Vector2(40f, 0f); 
            handleRt.anchoredPosition = Vector2.zero;
            
            Image handleImg = handleObj.AddComponent<Image>();
            handleImg.color = new Color(1f, 1f, 1f, 0f); // Görünmez ama etkileşimli
            handleImg.raycastTarget = true;
            
            slider.handleRect = handleRt;
            slider.targetGraphic = handleImg;
        }
        else if (slider.targetGraphic == null)
        {
            Image img = slider.GetComponent<Image>();
            if (img == null)
            {
                img = slider.gameObject.AddComponent<Image>();
                img.color = new Color(1f, 1f, 1f, 0f);
            }
            img.raycastTarget = true;
            slider.targetGraphic = img;
        }
    }

    private void UpdateMusicVolume()
    {
        float master = PlayerPrefs.GetFloat("hk_vol_master", 1f);
        AudioListener.volume = master;

        if (_audioSource != null)
        {
            float music = PlayerPrefs.GetFloat("hk_vol_music", 1f);
            _audioSource.volume = music; // Master is already applied via AudioListener
        }
    }

    private void OnMasterVolumeChanged(float val) { PlayerPrefs.SetFloat("hk_vol_master", val); UpdateMusicVolume(); }
    private void OnMusicVolumeChanged(float val) { PlayerPrefs.SetFloat("hk_vol_music", val); UpdateMusicVolume(); }
    private void OnSfxVolumeChanged(float val) { PlayerPrefs.SetFloat("hk_vol_sfx", val); }
}
