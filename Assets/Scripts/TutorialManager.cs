using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI Referansı")]
    public TextMeshProUGUI tutorialText;
    
    [Header("Mesajlar")]
    [TextArea]
    public string firstMessage = "Karanlık... F tuşuna basarak fenerini aç/kapat.";
    [TextArea]
    public string secondMessage = "Tab tuşu ile ayarları aç.";
    [TextArea]
    public string thirdMessage = "İlk ipucu için masaya dikkatli bak!";

    [Header("Zaman Ayarları")]
    public float displayTime = 4f; // Ekranda kalma süresi
    public float fadeTime = 1f;    // Yavaşça belirme/kaybolma süresi

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (tutorialText != null)
        {
            // Başlangıçta yazıyı görünmez yap (Alpha = 0)
            Color c = tutorialText.color;
            c.a = 0f;
            tutorialText.color = c;

            StartCoroutine(ShowTutorialRoutine());
        }
    }

    private IEnumerator ShowTutorialRoutine()
    {
        yield return new WaitForSeconds(8f); // Oyun başladıktan 1 saniye sonra başla (Sahne oturması için)

        // Birinci yazıyı göster
        tutorialText.text = firstMessage;
        yield return StartCoroutine(FadeText(1f)); // Yavaşça belir (Fade In)
        yield return new WaitForSeconds(displayTime); // Ekranda beklet
        yield return StartCoroutine(FadeText(0f)); // Yavaşça kaybol (Fade Out)

        yield return new WaitForSeconds(0.5f); // İki yazı arası yarım saniye es

        // İkinci yazıyı göster
        tutorialText.text = secondMessage;
        yield return StartCoroutine(FadeText(1f)); // Yavaşça belir
        yield return new WaitForSeconds(displayTime); // Ekranda beklet
        yield return StartCoroutine(FadeText(0f)); // Yavaşça kaybol

        yield return new WaitForSeconds(0.5f); // İki yazı arası yarım saniye es

        // Üçüncü yazıyı göster
        tutorialText.text = thirdMessage;
        yield return StartCoroutine(FadeText(1f)); // Yavaşça belir
        yield return new WaitForSeconds(displayTime); // Ekranda beklet
        yield return StartCoroutine(FadeText(0f)); // Yavaşça kaybol
        
        // İşimiz bitince scripti devre dışı bırak ki boşuna çalışmasın
        this.enabled = false;
    }

    private IEnumerator FadeText(float targetAlpha)
    {
        float startAlpha = tutorialText.color.a;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeTime);
            
            Color c = tutorialText.color;
            c.a = newAlpha;
            tutorialText.color = c;
            
            yield return null;
        }

        // Tam hedef değere oturt
        Color finalColor = tutorialText.color;
        finalColor.a = targetAlpha;
        tutorialText.color = finalColor;
    }

    // Dışarıdan (örneğin hafıza sahnesine girildiğinde) tutorial'ı zorla kapatmak için
    public void HideTutorial()
    {
        StopAllCoroutines();
        if (tutorialText != null)
        {
            tutorialText.gameObject.SetActive(false);
        }
        this.enabled = false;
    }
}
