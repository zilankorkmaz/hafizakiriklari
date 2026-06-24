using UnityEngine;
using System.Collections;

public class LethalThreat : MonoBehaviour
{
    [Header("Tuzak Ayarları")]
    [Tooltip("Bu obje sadece bir kez mi çalışsın?")]
    public bool triggerOnce = true;
    
    [Header("Efektler (Opsiyonel)")]
    public AudioSource deathSound; // Çarpıldığında/Öldüğünde çıkacak ses
    public ParticleSystem hitEffect; // Kan veya kıvılcım efekti

    [Header("İksir Mekaniği")]
    public float deathTimer = 60f; // Saniye cinsinden ölüm süresi (1 dakika)
    public GameObject curePotionObject; // Odadaki pasif iksir objesi
    public TMPro.TextMeshProUGUI timerText; // Geri sayımı gösterecek ekrandaki metin

    private bool isPoisoned = false;
    private bool isCured = false;
    private bool sequenceStarted = false;

    void Update()
    {
        if (isPoisoned && !isCured)
        {
            deathTimer -= Time.deltaTime;
            
            if (timerText != null)
            {
                int seconds = Mathf.CeilToInt(deathTimer);
                timerText.text = "Zehir: " + seconds.ToString() + "s";
                timerText.color = (seconds <= 15) ? Color.red : Color.yellow;
            }

            // Süre biterse oyuncu ölür
            if (deathTimer <= 0)
            {
                isPoisoned = false;
                if (GameEndingManager.Instance != null)
                {
                    GameEndingManager.Instance.TriggerLoopEnding();
                }
            }
        }
    }

    public void CurePoison()
    {
        isCured = true;
        isPoisoned = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sequenceStarted || isCured) return; // Zaten başladıysa veya kurtulduysa bir daha tetikleme

        if (other.CompareTag("Player"))
        {
            sequenceStarted = true;
            StartCoroutine(PoisonSequence());
        }
    }

    private System.Collections.IEnumerator PoisonSequence()
    {
        // Ses ve efektleri oynat
        if (deathSound != null) deathSound.Play();
        if (hitEffect != null) hitEffect.Play();

        // Glitch efekti ve panik yazısı
        if (GameEndingManager.Instance != null)
        {
            GameEndingManager.Instance.TriggerGlitchEffect();
            GameEndingManager.Instance.ShowTemporaryText("Eyvah! Ölümcül suya bastım. 1 dakika dolmadan iksiri bulmalıyım yoksa öleceğim!", 5f);
        }

        // 5 saniye bekle (yazı ekranda okunsun)
        yield return new WaitForSeconds(5f);

        // İksiri odada görünür (aktif) hale getir
        if (curePotionObject != null)
        {
            curePotionObject.SetActive(true);
        }

        // Süreyi başlat ve ekranda göster
        if (timerText != null) timerText.gameObject.SetActive(true);
        isPoisoned = true;
    }
}
