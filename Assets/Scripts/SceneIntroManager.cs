using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SceneIntroManager : MonoBehaviour
{
    [Header("UI Ayarları")]
    public CanvasGroup blackScreenGroup; // Siyah ekran ve yazıyı içeren Canvas Group
    public TextMeshProUGUI introText;    // Hikaye metni
    
    [Header("Hikaye Metinleri")]
    [TextArea(2, 4)]
    public string[] storyLines; // Peş peşe çıkacak metinler

    [Header("Zaman Ayarları")]
    public float waitBeforeFade = 3f; // Siyah ekran ve yazı ne kadar kalsın?
    public float fadeDuration = 2f;   // Yavaşça aydınlanma süresi

    [Header("Karakter Kontrolü (Opsiyonel)")]
    public MonoBehaviour playerController; // Intro sırasında oyuncu hareket etmesin diye

    void Start()
    {
        if (blackScreenGroup != null && introText != null)
        {
            // Başlangıçta ekran tamamen siyah
            blackScreenGroup.alpha = 1f;
            blackScreenGroup.gameObject.SetActive(true);
            introText.text = ""; // İlk başta boş
            
            // Oyuncuyu durdur
            if (playerController != null) playerController.enabled = false;

            StartCoroutine(IntroRoutine());
        }
    }

    private IEnumerator IntroRoutine()
    {
        // 1. Ekranın siyahlığını (Image) hemen yavaşça açmaya başla
        Image bgImage = blackScreenGroup.GetComponent<Image>();
        if (bgImage == null) bgImage = blackScreenGroup.GetComponentInChildren<Image>();

        if (bgImage != null)
        {
            StartCoroutine(FadeImageAlpha(bgImage, 1f, 0f, fadeDuration));
        }

        // 2. Oyuncuya kontrolü HEMEN geri ver ki beklerken yürüyebilsin
        if (playerController != null) playerController.enabled = true;

        // 3. Karanlık açılırken yazıları altyazı gibi ekranda göstermeye devam et
        if (storyLines != null && storyLines.Length > 0)
        {
            foreach (string line in storyLines)
            {
                introText.text = line;
                yield return new WaitForSeconds(waitBeforeFade);
            }
        }
        else
        {
            introText.text = "Zihnimin en derin, en karanlık odasına giriyorum...";
            yield return new WaitForSeconds(waitBeforeFade);
        }

        // 4. İşlem bitince tüm UI'ı kapat
        introText.text = "";
        blackScreenGroup.alpha = 0f;
        blackScreenGroup.gameObject.SetActive(false);
    }

    private IEnumerator FadeImageAlpha(Image img, float startAlpha, float targetAlpha, float duration)
    {
        float time = 0;
        Color c = img.color;
        while (time < duration)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            img.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        img.color = c;
    }
}
