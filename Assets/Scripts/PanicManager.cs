using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PanicManager : MonoBehaviour
{
    public static PanicManager Instance;

    [Header("Panik Ayarları")]
    public float panicTime = 45f; // Şifreyi bulmak için verilecek süre
    private bool isPanicActive = false;
    private float currentTime;

    [Header("Efektler")]
    [Tooltip("Panik anında yanıp sönecek odadaki lambalar")]
    public Light[] roomLights;
    public AudioSource alarmSesi;

    private float[] originalIntensities;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentTime = panicTime;
        
        // Işıkların orijinal şiddetlerini kaydet
        if (roomLights != null && roomLights.Length > 0)
        {
            originalIntensities = new float[roomLights.Length];
            for (int i = 0; i < roomLights.Length; i++)
            {
                if (roomLights[i] != null)
                    originalIntensities[i] = roomLights[i].intensity;
            }
        }
    }

    void Update()
    {
        if (isPanicActive)
        {
            // Süreyi azalt
            currentTime -= Time.deltaTime;

            // Işıkları çıldırt (Titreme)
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
            }
        }
    }

    private void GameOver()
    {
        isPanicActive = false;
        Debug.Log("SÜRE BİTTİ! SAHNE BAŞA SARIYOR...");
        
        // Ekrana tam siyah bir UI koyup jumpscare da eklenebilir, şimdilik direkt sahneyi yeniden yüklüyoruz.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
