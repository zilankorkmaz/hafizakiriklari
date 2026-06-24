using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Oyuncu görünmez bir alana girdiğinde ekranda yavaşça beliren ve 
/// kaybolan hatıra (geçmiş konuşma) metinleri oluşturur.
/// </summary>
public class MemoryTrigger : MonoBehaviour
{
    [Header("Hatıra Ayarları")]
    [TextArea(3, 5)]
    [Tooltip("Ekranda belirecek konuşma metni")]
    public string memoryText = "Y: Bizi dinlemeyecek...";
    
    [Tooltip("Metnin ekranda okunabilir kalacağı süre (saniye)")]
    public float displayDuration = 4f; 
    
    [Tooltip("Metnin belirme ve silinme hızı")]
    public float fadeSpeed = 1.5f;
    
    [Tooltip("Tetiklenme Mesafesi (Oyuncu ne kadar yaklaşınca çalışsın)")]
    public float triggerDistance = 3f;

    [Header("UI ve Ses")]
    [Tooltip("Canvas'ın içinde tam ortada duran TextMeshPro yazısı")]
    public TextMeshProUGUI uiTextElement; 
    
    [Tooltip("Yazı çıkarken çalacak korkutucu/fısıltı sesi (Opsiyonel)")]
    public AudioClip whisperSound; // Kullanıcının direkt .mp3/.wav dosyasını atabilmesi için değiştirildi

    [Tooltip("Sahnede yürüdüğün karakter (PlayerCapsule). Boş bırakırsan otomatik bulur.")]
    public Transform playerTransform;

    private bool hasTriggered = false;
    private AudioSource internalAudioSource;

    private void Start()
    {
        // Başlangıçta yazıyı görünmez yap (Sadece ilk trigger yapacaksa)
        // Not: Eğer sahnede birden fazla MemoryTrigger varsa ve aynı UI'ı kullanıyorlarsa,
        // Start fonksiyonu içinde yazıyı sıfırlamak çakışmaya yol açmaz çünkü Alpha'yı 0 yaparız.
        if (uiTextElement != null && !hasTriggered)
        {
            Color c = uiTextElement.color;
            c.a = 0f;
            uiTextElement.color = c;
        }

        // Ses çalmak için gizli bir AudioSource oluştur
        if (whisperSound != null)
        {
            internalAudioSource = gameObject.AddComponent<AudioSource>();
            internalAudioSource.playOnAwake = false;
            internalAudioSource.clip = whisperSound;
            internalAudioSource.spatialBlend = 0f; // Sesi 2D yapar (kulaklığın iki tarafından da duyulur)
        }

        // Oyuncuyu otomatik bul (Kameradan veya İsimden)
        if (playerTransform == null)
        {
            if (Camera.main != null) 
            {
                playerTransform = Camera.main.transform;
            }
            else 
            {
                GameObject p = GameObject.Find("PlayerCapsule");
                if (p != null) playerTransform = p.transform;
                else Debug.LogWarning("MemoryTrigger: Sahnede oyuncu bulunamadı!");
            }
        }
    }

    private void Update()
    {
        // Eğer oyuncu bulunamadıysa veya zaten tetiklendiyse hiçbir şey yapma
        if (playerTransform == null || hasTriggered) return;

        // Oyuncu ile bu görünmez küp arasındaki mesafeyi ölç
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // Eğer oyuncu yeterince yaklaştıysa tetikle!
        if (distance <= triggerDistance)
        {
            Debug.Log("Mesafe ile Tetiklendi: " + gameObject.name);
            hasTriggered = true;
            StartCoroutine(ShowMemoryRoutine());
        }
    }

    private IEnumerator ShowMemoryRoutine()
    {
        // Önce fısıltı sesini çal (eğer eklendiyse)
        if (internalAudioSource != null && whisperSound != null) 
        {
            internalAudioSource.Play();
        }

        // Eğer kullanıcı UI Text atamadıysa, sadece sesi çalıp objeyi silebiliriz
        if (uiTextElement == null)
        {
            Debug.LogWarning("MemoryTrigger: UI Text atanmamış, sadece ses çalındı!");
            
            // Sadece ses eklendiyse, ses bitene kadar bekle ve sonra kendini yok et
            if (whisperSound != null)
                yield return new WaitForSeconds(whisperSound.length);
            
            Destroy(gameObject);
            yield break;
        }

        // Metni ata
        uiTextElement.text = memoryText;

        // 1. Aşama: Yavaşça Belir (Fade In)
        yield return StartCoroutine(FadeTextTo(1f));

        // 2. Aşama: Ekranda Bekle
        yield return new WaitForSeconds(displayDuration);

        // 3. Aşama: Yavaşça Silin (Fade Out)
        yield return StartCoroutine(FadeTextTo(0f));
        
        // İstersen tetiklendikten sonra görünmez objeyi silebilirsin ki bir daha çarpmmasın
        Destroy(gameObject);
    }

    private IEnumerator FadeTextTo(float targetAlpha)
    {
        Color c = uiTextElement.color;
        float startAlpha = c.a;
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime * fadeSpeed;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, time);
            uiTextElement.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        uiTextElement.color = c;
    }
}
