using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PanicManager : MonoBehaviour
{
    public static PanicManager Instance;

    [Header("Panik Ayarları")]
    public float panicTime = 45f; // Şifreyi bulmak için verilecek süre
    private bool isPanicActive = false;
    private float currentTime;

    [Header("UI Ayarları")]
    public TextMeshProUGUI timerText; // Ekranda geri sayımı gösterecek metin

    [Header("Efektler")]
    [Tooltip("Panik anında yanıp sönecek odadaki lambalar")]
    public Light[] roomLights;
    public AudioSource alarmSesi;

    private float[] originalIntensities;
    private Color[] originalColors;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentTime = panicTime;
        
        // Işıkların orijinal şiddetlerini ve renklerini kaydet
        if (roomLights != null && roomLights.Length > 0)
        {
            originalIntensities = new float[roomLights.Length];
            originalColors = new Color[roomLights.Length];
            
            for (int i = 0; i < roomLights.Length; i++)
            {
                if (roomLights[i] != null)
                {
                    originalIntensities[i] = roomLights[i].intensity;
                    originalColors[i] = roomLights[i].color;
                }
            }
        }

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false); // Başlangıçta gizli
        }
    }

    void Update()
    {
        if (isPanicActive)
        {
            // Süreyi azalt
            currentTime -= Time.deltaTime;

            // Ekranda süreyi güncelle (00:00 formatında)
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60);
                int seconds = Mathf.FloorToInt(currentTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                // Süre azaldıkça kırmızı yazsın veya yanıp sönsün (Opsiyonel)
                if (currentTime < 10f && seconds % 2 == 0)
                    timerText.color = Color.red;
                else
                    timerText.color = Color.white;
            }

            // Işıkları çıldırt ve kırmızı yap
            FlickerLights();

            // Süre biterse oyunu kaybet
            if (currentTime <= 0)
            {
                currentTime = 0;
                GameOver();
            }
        }
    }

    public void StartPanicMode()
    {
        if (isPanicActive) return;

        isPanicActive = true;
        Debug.Log("PANİK BAŞLADI! Kalan Süre: " + panicTime);

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        if (alarmSesi != null && !alarmSesi.isPlaying)
        {
            alarmSesi.Play();
        }
    }

    public void StopPanicMode()
    {
        isPanicActive = false;
        Debug.Log("PANİK DURDURULDU! Hayatta kaldın.");

        if (alarmSesi != null && alarmSesi.isPlaying)
        {
            alarmSesi.Stop();
        }

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        // Işıkları normale döndür
        RestoreLights();
    }

    private void FlickerLights()
    {
        if (roomLights == null || roomLights.Length == 0) return;

        for (int i = 0; i < roomLights.Length; i++)
        {
            if (roomLights[i] != null)
            {
                // Rengi kırmızı yap
                roomLights[i].color = Color.red;

                // Rastgele kapatıp açma veya şiddetini değiştirme
                if (Random.value > 0.8f) // %20 ihtimalle tamamen kapanır
                {
                    roomLights[i].enabled = false;
                }
                else
                {
                    roomLights[i].enabled = true;
                    roomLights[i].intensity = Random.Range(0f, originalIntensities[i] * 1.5f);
                }
            }
        }
    }

    private void RestoreLights()
    {
        if (roomLights == null || roomLights.Length == 0) return;

        for (int i = 0; i < roomLights.Length; i++)
        {
            if (roomLights[i] != null)
            {
                roomLights[i].enabled = true;
                roomLights[i].intensity = originalIntensities[i];
                roomLights[i].color = originalColors[i]; // Rengi geri ver
            }
        }
    }

    private void GameOver()
    {
        isPanicActive = false;
        Debug.Log("SÜRE BİTTİ! SAHNE BAŞA SARIYOR...");
        
        // Eğer sahnede GameEndingManager varsa (Final sahnesiyse) kısır döngü sonunu tetikle
        if (GameEndingManager.Instance != null)
        {
            GameEndingManager.Instance.TriggerLoopEnding();
        }
        else
        {
            // Yoksa (Sahne 2 gibi) normal şekilde sahneyi yeniden yükle
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
