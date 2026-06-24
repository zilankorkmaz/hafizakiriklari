using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameEndingManager : MonoBehaviour
{
    public static GameEndingManager Instance;

    [Header("UI Panels")]
    public CanvasGroup fadePanel; // Ekranı karartmak veya beyazlatmak için
    public Image fadePanelImage;  // Rengini değiştirmek için
    public GameObject choicePanel; // Gerçek son butonları

    [Header("Metinler")]
    public TextMeshProUGUI storyText; // Ekranda belirecek yazılar

    [Header("Hikaye İçerikleri (Finaller)")]
    [TextArea(2, 4)] public string loopEndingText1 = "Neredeyim ben?...";
    [TextArea(2, 4)] public string loopEndingText2 = "Yine en başa döndük...";

    [TextArea(3, 5)] public string betrayalEndingText = "Deney başarılı...\n\nY ve Z'yi o odada ölüme terk ettim.\nGerçekleri öğrenmemeleri için.\nBen kurban değil, onların celladıyım.";

    [TextArea(2, 4)] public string trueEndingIntroText = "Özgürsün... Peki gerçeğin ağırlığını taşıyabilecek misin?";
    [TextArea(3, 5)] public string trueEndingRememberText = "Hepimiz suçluyduk...\nAma bedeli sadece Y ve Z ödedi.\nArtık hatırlıyorum.";
    [TextArea(2, 4)] public string trueEndingForgetText = "Bazen cehalet, en büyük lütuftur...";

    [Header("Karakter Kontrolü")]
    public MonoBehaviour playerController; // Final girdiğinde oyuncu hareketini durdurmak için (FirstPersonController vs.)

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Başlangıçta panelleri gizle
        if (choicePanel != null) choicePanel.SetActive(false);
        if (fadePanel != null)
        {
            fadePanel.alpha = 0;
            fadePanel.gameObject.SetActive(false);
        }
        if (storyText != null) storyText.text = "";
    }

    // FİNAL 1: Kısır Döngü (Hafıza Kaybı - Zaman biterse veya yanlış yapılırsa)
    public void TriggerLoopEnding()
    {
        StartCoroutine(LoopEndingRoutine());
    }

    private IEnumerator LoopEndingRoutine()
    {
        // Kaybetme sonunu PlayerPrefs'e kaydet (2 = Standart Kaybetme)
        PlayerPrefs.SetInt("GameEndState", 2);
        PlayerPrefs.Save();

        DisablePlayer();
        
        // Siyah ekran
        fadePanelImage.color = Color.black;
        fadePanel.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(fadePanel, 0f, 1f, 2f)); // 2 saniyede kararır

        // Yazılar
        storyText.color = Color.white;
        storyText.text = loopEndingText1;
        yield return StartCoroutine(FadeText(storyText, 0f, 1f, 1.5f));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeText(storyText, 1f, 0f, 1.5f));

        storyText.text = loopEndingText2;
        yield return StartCoroutine(FadeText(storyText, 0f, 1f, 1.5f));
        yield return new WaitForSeconds(3f);

        // Ana Menüye dön ve toplanan sayfaları bildir
        var pages = NotebookManager.Instance != null ? NotebookManager.Instance.collectedPages : null;
        MainMenuUIManager.SetMenuState(true, pages, false);
        SceneManager.LoadScene("MainMenu"); 
    }

    // FİNAL 2: İhanet (Oyuncu gizli odadan gaz verirse)
    public void TriggerBetrayalEnding()
    {
        StartCoroutine(BetrayalEndingRoutine());
    }

    private IEnumerator BetrayalEndingRoutine()
    {
        DisablePlayer();

        // Kırmızımsı bir ekran veya yavaş kararma
        fadePanelImage.color = new Color(0.5f, 0f, 0f, 1f); // Koyu kırmızı
        fadePanel.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(fadePanel, 0f, 1f, 2.5f));

        storyText.color = Color.white;
        storyText.text = betrayalEndingText;
        yield return StartCoroutine(FadeText(storyText, 0f, 1f, 3f));
        yield return new WaitForSeconds(6f);

        // Ana Menüye dön ve toplanan sayfaları bildir
        var pages = NotebookManager.Instance != null ? NotebookManager.Instance.collectedPages : null;
        MainMenuUIManager.SetMenuState(true, pages, false);
        SceneManager.LoadScene("MainMenu"); 
    }

    // FİNAL 3: Gerçek Özgürlük (Doğru çıkış kapısı açıldığında)
    public void TriggerTrueEnding()
    {
        // Eski kod yerine yeni sisteme yönlendir!
        if (GameEndManager.Instance != null)
        {
            GameEndManager.Instance.ShowTrueEndingChoice();
        }
        else
        {
            StartCoroutine(TrueEndingRoutine());
        }
    }

    private IEnumerator TrueEndingRoutine()
    {
        DisablePlayer();

        // Parlak beyaz ışık / fade
        fadePanelImage.color = Color.white;
        fadePanel.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(fadePanel, 0f, 1f, 3f));

        // Sesleri kapatmak istersen (isteğe bağlı)
        AudioListener.pause = true;

        storyText.color = Color.black; // Arka plan beyaz olduğu için yazıyı siyah yap
        storyText.text = trueEndingIntroText;
        yield return StartCoroutine(FadeText(storyText, 0f, 1f, 2f));
        yield return new WaitForSeconds(1f);

        // Seçim ekranını aç
        choicePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- BUTON FONKSİYONLARI (Gerçek Son için) ---

    public void OnClickRemember()
    {
        PlayerPrefs.SetInt("GameEndState", 1);
        PlayerPrefs.Save();
        choicePanel.SetActive(false);
        StartCoroutine(ShowFinalStory(false));
    }

    public void OnClickForget()
    {
        PlayerPrefs.SetInt("GameEndState", 2);
        PlayerPrefs.Save();
        choicePanel.SetActive(false);
        StartCoroutine(ShowFinalStory(true));
    }

    private IEnumerator ShowFinalStory(bool choseForget)
    {
        yield return new WaitForSeconds(1f); 
        AudioListener.pause = false; // Sesleri geri aç
        
        var pages = NotebookManager.Instance != null ? NotebookManager.Instance.collectedPages : null;
        MainMenuUIManager.SetMenuState(true, pages, choseForget);
        SceneManager.LoadScene("MainMenu"); // Oyun biter, menüye döner
    }


    // --- YARDIMCI FONKSİYONLAR ---

    public void ShowTemporaryText(string message, float duration)
    {
        if (storyText == null) return;
        StartCoroutine(TempTextRoutine(message, duration));
    }

    private IEnumerator TempTextRoutine(string message, float duration)
    {
        storyText.color = Color.white;
        storyText.text = message;
        yield return StartCoroutine(FadeText(storyText, 0f, 1f, 0.5f));
        yield return new WaitForSeconds(duration);
        yield return StartCoroutine(FadeText(storyText, 1f, 0f, 0.5f));
    }

    public void TriggerGlitchEffect()
    {
        if (fadePanel == null || fadePanelImage == null) return;
        StartCoroutine(GlitchRoutine());
    }

    private IEnumerator GlitchRoutine()
    {
        fadePanel.gameObject.SetActive(true);
        
        // Kırmızı parlama
        fadePanelImage.color = new Color(1f, 0f, 0f, 0.4f);
        fadePanel.alpha = 1f;
        yield return new WaitForSeconds(0.1f);
        
        fadePanel.alpha = 0f;
        yield return new WaitForSeconds(0.1f);
        
        // Beyaz parlama
        fadePanelImage.color = new Color(1f, 1f, 1f, 0.6f);
        fadePanel.alpha = 1f;
        yield return new WaitForSeconds(0.1f);
        
        fadePanel.alpha = 0f;
        fadePanel.gameObject.SetActive(false);
    }

    public void FadeScreenPartial(float targetAlpha, float duration, Color color)
    {
        if (fadePanel == null || fadePanelImage == null) return;
        
        fadePanelImage.color = color;
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(fadePanel, fadePanel.alpha, targetAlpha, duration));
    }

    private void DisablePlayer()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float targetAlpha, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }
        cg.alpha = targetAlpha;
    }

    private IEnumerator FadeText(TextMeshProUGUI tmpText, float startAlpha, float targetAlpha, float duration)
    {
        Color originalColor = tmpText.color;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            Color c = tmpText.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            tmpText.color = c;
            yield return null;
        }
        Color finalColor = tmpText.color;
        finalColor.a = targetAlpha;
        tmpText.color = finalColor;
    }
}
