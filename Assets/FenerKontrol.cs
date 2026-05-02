using Unity.VisualScripting;
using UnityEngine;

public class FenerKontrol : MonoBehaviour
{
    private Light fenerIsigi;

    [Header("Fener Ayarları")]
    public float mevcutsarj = 100f;
    public float harcamahizi = 5f;

    [Header("Titreme (Flicker) Ayarları")]
    [Tooltip("Pil bu değerin altına düştüğünde fener titremeye başlar.")]
    public float sarjUyariSiniri = 20f;
    public float titremeMinHiz = 0.05f;
    public float titremeMaxHiz = 0.25f;

    [Header("Ses Ayarları (Gerilim)")]
    [Tooltip("Pil azaldığında veya bittiğinde çalacak nefes/kalp atışı sesi.")]
    public AudioSource gerilimSesi;

    private float titremeZamani = 0f;
    private bool fenerAcikMi = false;
    private float orijinalYogunluk;

    void Start()
    {
        fenerIsigi = GetComponent<Light>();
        if (fenerIsigi != null)
        {
            orijinalYogunluk = fenerIsigi.intensity;
            fenerAcikMi = fenerIsigi.enabled;
        }
    }

    void Update()
    {
        // F tuşu ile feneri açıp kapatma
        if (Input.GetKeyDown(KeyCode.F) && mevcutsarj > 0)
        {
            fenerAcikMi = !fenerAcikMi;
            if (fenerIsigi != null)
            {
                fenerIsigi.enabled = fenerAcikMi;
                if (fenerAcikMi) fenerIsigi.intensity = orijinalYogunluk;
            }
        }

        if (fenerAcikMi && fenerIsigi != null)
        {
            mevcutsarj -= harcamahizi * Time.deltaTime;

            if (mevcutsarj <= 0)
            {
                mevcutsarj = 0;
                fenerAcikMi = false;
                fenerIsigi.enabled = false;
                Debug.Log("Fenerin şarjı bitti!");
            }
            else if (mevcutsarj <= sarjUyariSiniri)
            {
                // Titreme (Flicker) Efekti
                titremeZamani -= Time.deltaTime;
                if (titremeZamani <= 0)
                {
                    // Işığın şiddetini rastgele düşür
                    fenerIsigi.intensity = Random.Range(orijinalYogunluk * 0.2f, orijinalYogunluk);
                    
                    // Kısa anlık kapanmalar yarat (%20 ihtimalle)
                    fenerIsigi.enabled = Random.value > 0.2f;

                    titremeZamani = Random.Range(titremeMinHiz, titremeMaxHiz);
                }
            }
            else
            {
                // Şarj yeterliyse ışık normal yanmaya devam etsin
                fenerIsigi.enabled = true;
                fenerIsigi.intensity = orijinalYogunluk;
            }
        }
        else if (!fenerAcikMi && fenerIsigi != null)
        {
            fenerIsigi.enabled = false;
        }

        // --- GERİLİM SESİ KONTROLÜ ---
        if (gerilimSesi != null)
        {
            // Tab menüsündeki ses ayarını (Genel Ses * SFX Sesi) gerilim sesine uygula
            if (GameplayAudioAndMenu.Instance != null)
            {
                gerilimSesi.volume = GameplayAudioAndMenu.Instance.SfxVolume;
            }

            // Şarj sınırın altındaysa (veya bittiyse) gerilim sesini çal (Döngüde değilse başlat)
            if (mevcutsarj <= sarjUyariSiniri)
            {
                if (!gerilimSesi.isPlaying) 
                {
                    gerilimSesi.Play();
                }
            }
            else
            {
                // Şarj yeterliyse (örneğin pil bulunduysa) sesi durdur
                if (gerilimSesi.isPlaying) 
                {
                    gerilimSesi.Stop();
                }
            }
        }
    }
}
