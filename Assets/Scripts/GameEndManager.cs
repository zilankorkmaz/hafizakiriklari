using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    [Header("UI Referansları")]
    public GameObject blackPanel;
    public Text narrativeText;
    public GameObject choicePanel; 

    [Header("Cinematic Audio")]
    public AudioClip heartbeatSound;
    public AudioClip slamSound;
    public AudioClip glitchSes;
    private AudioSource cinematicSource;

    [Header("Oyun Sonu Hikaye Metinleri")]
    [TextArea(3, 5)] public string betrayalText = "Sonunda kapıyı açtın ve elimize düştün.";
    [TextArea(3, 5)] public string trueEndingIntroText = "Gerçekleri olduğu gibi kabullenmeye hazır mısın, yoksa bu tatlı yalanda yaşamaya devam mı edeceksin?";
    [TextArea(3, 5)] public string trueEndingRememberText = "Her şeyi hatırlıyorum... Acıyı, hatalarımı ve kim olduğumu. Artık özgürüm.";
    [TextArea(3, 5)] public string trueEndingForgetText = "Gözlerimi kapatıyorum... Cehalet bazen en büyük lütuftur.";

    private bool isEndingTriggered = false;

    private void Awake()
    {
        Instance = this;
        if(blackPanel != null) blackPanel.SetActive(false);
        if(choicePanel != null) choicePanel.SetActive(false);
        if(narrativeText != null) narrativeText.text = "";

        cinematicSource = gameObject.AddComponent<AudioSource>();
        cinematicSource.playOnAwake = false;
    }

    private void Start()
    {
        // 1. Butonların tetiklenmeme sorununu çöz (Runtime Binding)
        if (choicePanel != null)
        {
            Button[] buttons = choicePanel.GetComponentsInChildren<Button>(true);
            if (buttons.Length >= 2)
            {
                buttons[0].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(OnRememberSelected);
                
                buttons[1].onClick.RemoveAllListeners();
                buttons[1].onClick.AddListener(OnForgetSelected);
            }
        }

        // 2. Yazıların ekrandan kaybolması (taşıp gizlenmesi) sorununu çöz
        if (narrativeText != null)
        {
            narrativeText.horizontalOverflow = HorizontalWrapMode.Wrap;
            narrativeText.verticalOverflow = VerticalWrapMode.Overflow;
        }
    }

    private void Update()
    {
        // Seçim ekranı açık olduğu sürece farenin kilitlenmesini engelle
        if (choicePanel != null && choicePanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // YANLIŞ KAPI - ÖLÜM (Suya basınca veya kapı arkasında tetiklenir)
    public void ShowBetrayalEnding()
    {
        if (isEndingTriggered) return;
        isEndingTriggered = true;
        StartCoroutine(BetrayalRoutine());
    }

    private IEnumerator BetrayalRoutine()
    {
        DisablePlayer(); // Oyuncuyu dondur
        
        blackPanel.SetActive(true);
        choicePanel.SetActive(false);
        narrativeText.color = Color.red;
        narrativeText.text = betrayalText;
        
        // PlayerPrefs'e kaydet (3 = Yanlış kapı ölümü)
        PlayerPrefs.SetInt("GameEndState", 3);
        PlayerPrefs.Save();

        yield return new WaitForSeconds(5f); // 5 saniye yazıyı okusun

        var pages = NotebookManager.Instance != null ? NotebookManager.Instance.collectedPages : null;
        MainMenuUIManager.SetMenuState(true, pages, false);

        SceneManager.LoadScene("MainMenu");
    }

    // DOĞRU KAPI - SEÇİM EKRANI
    public void ShowTrueEndingChoice()
    {
        if (isEndingTriggered) return;
        isEndingTriggered = true;

        DisablePlayer(); // Oyuncuyu dondur

        blackPanel.SetActive(true);
        narrativeText.color = Color.white;
        narrativeText.text = trueEndingIntroText;
        choicePanel.SetActive(true);
        choicePanel.transform.SetAsLastSibling();
        
        // Fareyi görünür yap (Butonlara tıklayabilmesi için)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // BUTON: HATIRLA (İyi Son)
    public void OnRememberSelected()
    {
        choicePanel.SetActive(false);
        StartCoroutine(RememberRoutine());
    }

    private IEnumerator RememberRoutine()
    {
        narrativeText.color = Color.green;
        narrativeText.text = trueEndingRememberText;
        
        // PlayerPrefs'e kaydet (1 = Hatırladı / Kazandı)
        PlayerPrefs.SetInt("GameEndState", 1);
        PlayerPrefs.Save();

        // Yazıyı okumak için sarsıntı ve efektler başlamadan önce biraz zaman tanıyalım
        yield return new WaitForSeconds(2f);

        // 1. Kalp atışı sesi
        if (heartbeatSound != null) {
            cinematicSource.clip = heartbeatSound;
            cinematicSource.loop = true;
            cinematicSource.Play();
        }

        // 2. Kırmızı parlama efekti (Ekran siyahken kırmızı parlayıp geri siyaha döner)
        Image bgImage = blackPanel.GetComponent<Image>();
        if (bgImage != null)
        {
            bgImage.color = Color.red;
            StartCoroutine(FadeImageColor(bgImage, Color.red, Color.black, 2f));
        }

        // 3. Kamera sarsıntısı
        yield return StartCoroutine(CameraShake(3f, 0.2f));

        if (cinematicSource.isPlaying) cinematicSource.Stop();

        // 4. Sert kapı kapanma / Şok sesi
        if (slamSound != null) {
            cinematicSource.PlayOneShot(slamSound);
        }

        // Metnin ekranda kalma süresini uzatıyoruz (eski süre 1.5 saniyeydi)
        yield return new WaitForSeconds(4f);

        var pages = NotebookManager.Instance != null ? NotebookManager.Instance.collectedPages : null;
        MainMenuUIManager.SetMenuState(true, pages, false);

        SceneManager.LoadScene("MainMenu");
    }

    // BUTON: UNUT (Kötü Son)
    public void OnForgetSelected()
    {
        choicePanel.SetActive(false);
        StartCoroutine(ForgetRoutine());
    }

    private IEnumerator ForgetRoutine()
    {
        narrativeText.color = new Color(0.7f, 0.7f, 0.7f); // Gri
        narrativeText.text = trueEndingForgetText;
        
        // PlayerPrefs'e kaydet (2 = Unuttu / Kaybetti)
        PlayerPrefs.SetInt("GameEndState", 2);
        PlayerPrefs.Save();

        if (glitchSes != null) {
            cinematicSource.PlayOneShot(glitchSes);
        }

        // Text Glitch (Yazı bozulma) efekti
        float elapsed = 0f;
        string originalText = narrativeText.text;
        string glitchChars = "!@#$%^&*()_+-=[]{}|;':,./<>?";
        
        while (elapsed < 3.5f)
        {
            elapsed += Time.deltaTime;
            if (Random.value > 0.85f) 
            {
                char[] charArray = originalText.ToCharArray();
                for (int i = 0; i < 6; i++)
                {
                    int randIdx = Random.Range(0, charArray.Length);
                    charArray[randIdx] = glitchChars[Random.Range(0, glitchChars.Length)];
                }
                narrativeText.text = new string(charArray);
                narrativeText.color = new Color(Random.value, Random.value, Random.value);
            }
            else
            {
                narrativeText.text = originalText;
                narrativeText.color = new Color(0.7f, 0.7f, 0.7f);
            }
            yield return new WaitForSeconds(0.05f);
        }
        
        narrativeText.text = originalText;
        narrativeText.color = new Color(0.7f, 0.7f, 0.7f);

        // Fısıltı kapanışı
        narrativeText.color = new Color(0.5f, 0f, 0f); // Koyu kırmızı
        narrativeText.text = "Emin misin?";
        yield return new WaitForSeconds(0.15f); // Saliselik göster
        narrativeText.text = "";

        yield return new WaitForSeconds(1.5f);

        var pages = NotebookManager.Instance != null ? NotebookManager.Instance.collectedPages : null;
        MainMenuUIManager.SetMenuState(true, pages, true);

        SceneManager.LoadScene("MainMenu");
    }

    private void DisablePlayer()
    {
        // Oyuncuyu bul ve hareket scriptlerini kapat
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (var s in scripts)
            {
                string sName = s.GetType().Name.ToLower();
                if (sName.Contains("move") || sName.Contains("walk") || sName.Contains("controller") || sName.Contains("fps"))
                {
                    s.enabled = false;
                }
            }

            // Fiziksel olarak da düşmesini/hareket etmesini durdur
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;
        }
    }

    // --- SİNEMATİK EFEKTLER ---
    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) yield break;
        
        Vector3 originalPos = mainCam.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCam.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.localPosition = originalPos;
    }

    private IEnumerator FadeImageColor(Image img, Color from, Color to, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            img.color = Color.Lerp(from, to, time / duration);
            yield return null;
        }
        img.color = to;
    }
}
