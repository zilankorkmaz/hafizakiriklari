using UnityEngine;
using UnityEngine.Events;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance { get; private set; }

    [Header("Döngü Ayarları")]
    public int currentLoop = 1;
    public int maxLoops = 4; // 4. turdan sonra biter
    public bool isLoopBroken = false;

    [Header("Ses Ayarları")]
    public AudioClip normalSound;
    public AudioClip panicAlarm;
    private AudioSource audioSource;

    [Header("Olaylar")]
    public UnityEvent OnLoopChanged;
    public UnityEvent OnLoopBroken; // Döngü bittiğinde tetiklenir

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Kendi üzerinde AudioSource yoksa ekle
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.loop = true;
        audioSource.spatialBlend = 0f; // 2D Ses (Her yerden duyulur)

        // Başlangıç sesini çal
        if (normalSound != null)
        {
            audioSource.clip = normalSound;
            audioSource.Play();
        }
    }

    public void IncrementLoop()
    {
        if (isLoopBroken) return;

        currentLoop++;
        Debug.Log("Döngü Artırıldı: " + currentLoop + ". tura girildi.");

        // 3. ve 4. Döngüde Alarm Sesine Geçiş
        if (currentLoop == 3 && panicAlarm != null && audioSource != null)
        {
            audioSource.clip = panicAlarm;
            audioSource.Play();
        }

        if (currentLoop > maxLoops)
        {
            BreakLoop();
        }
        else
        {
            OnLoopChanged?.Invoke();
        }
    }

    private void BreakLoop()
    {
        isLoopBroken = true;
        Debug.Log("Döngü KIRILDI! Oyuncu koridordan çıkabilir.");
        OnLoopBroken?.Invoke();
    }
}
