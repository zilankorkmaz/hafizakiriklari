using Unity.VisualScripting;
using UnityEngine;

public class FenerKontrol : MonoBehaviour
{
    private Light fenerIsigi;

    [Header("Fener Ayarları")]
    public float mevcutsarj = 100f;
    public float harcamahizi = 2f;

    [Header("Titreme (Flicker) Ayarları")]
    [Tooltip("Pil bu değerin altına düştüğünde fener titremeye başlar.")]
    public float sarjUyariSiniri = 20f;
    public float titremeMinHiz = 0.05f;
    public float titremeMaxHiz = 0.25f;

    [Header("Ses Ayarları (Gerilim)")]
    [Tooltip("Pil azaldığında veya bittiğinde çalacak nefes/kalp atışı sesi.")]
    public AudioSource gerilimSesi;

    [Header("UI Ayarları")]
    [Tooltip("Pil seviyesini gösterecek 4 adet yeşil bar resmi")]
    public UnityEngine.UI.Image[] pilBarlari;

    private float titremeZamani = 0f;
    private bool fenerAcikMi = false;
    private float orijinalYogunluk;

    // --- DIŞARIDAN OKUNABİLMESİ İÇİN ---
    public bool IsFenerAcik => fenerAcikMi && fenerIsigi != null && fenerIsigi.enabled;
    public Light FenerLight => fenerIsigi;

    public void SarjDoldur(float miktar)
    {
        mevcutsarj = Mathf.Clamp(mevcutsarj + miktar, 0f, 100f);
        
        // Şarj dolduğunda fener kapalıysa otomatik geri yansın
        if (!fenerAcikMi && mevcutsarj > 0)
        {
            fenerAcikMi = true;
            if (fenerIsigi != null)
            {
                fenerIsigi.enabled = true;
                fenerIsigi.intensity = orijinalYogunluk;
            }
        }
    }

    void Start()
    {
        // Eğer pilBarlari dizisi boşsa veya içindeki elemanlar null ise referansları yenile
        bool needsRefresh = false;
        if (pilBarlari == null || pilBarlari.Length == 0) 
        {
            needsRefresh = true;
        }
        else 
        {
            foreach (var p in pilBarlari)
            {
                if (p == null) { needsRefresh = true; break; }
            }
        }

        if (needsRefresh)
        {
            GameObject barsPanel = GameObject.Find("Bars");
            
            // Eğer aktif değilse Canvas'ların altından bul
            if (barsPanel == null)
            {
                Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (Canvas c in canvases)
                {
                    Transform[] allT = c.GetComponentsInChildren<Transform>(true);
                    foreach (Transform t in allT)
                    {
                        if (t.name == "Bars")
                        {
                            barsPanel = t.gameObject;
                            break;
                        }
                    }
                    if (barsPanel != null) break;
                }
            }

            if (barsPanel != null)
            {
                // Kendi üzerindeki Image hariç (varsa), sadece altındaki barları al (isim sıralı yapmak en garantisi)
                var images = barsPanel.GetComponentsInChildren<UnityEngine.UI.Image>(true);
                System.Collections.Generic.List<UnityEngine.UI.Image> validBars = new System.Collections.Generic.List<UnityEngine.UI.Image>();
                foreach (var img in images)
                {
                    if (img.gameObject != barsPanel) validBars.Add(img);
                }
                
                // İsimlerine göre (Bar_1, Bar_2) sıraya diz ki tersten sönmesin
                validBars.Sort((a, b) => a.gameObject.name.CompareTo(b.gameObject.name));
                pilBarlari = validBars.ToArray();
            }
        }

        fenerIsigi = GetComponent<Light>();
        if (fenerIsigi != null)
        {
            orijinalYogunluk = fenerIsigi.intensity;
            fenerAcikMi = fenerIsigi.enabled;
        }
        else
        {
            Debug.LogWarning("FenerKontrol scripti Light componenti olmayan bir objede (" + gameObject.name + ") bulundu. UI çakışmasını önlemek için script devredışı bırakılıyor.");
            this.enabled = false;
            return;
        }
    }

    void Update()
    {
        if (fenerIsigi == null) return;

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

        GuncellePilUI();
    }

    private void GuncellePilUI()
    {
        if (pilBarlari == null || pilBarlari.Length == 0) return;

        // mevcutsarj 0 ile 100 arasında
        int acikBarSayisi = 0;
        if (mevcutsarj > 75) acikBarSayisi = 4;
        else if (mevcutsarj > 50) acikBarSayisi = 3;
        else if (mevcutsarj > 25) acikBarSayisi = 2;
        else if (mevcutsarj > 0) acikBarSayisi = 1;
        else acikBarSayisi = 0;

        for (int i = 0; i < pilBarlari.Length; i++)
        {
            if (pilBarlari[i] != null)
            {
                pilBarlari[i].enabled = (i < acikBarSayisi);
                
                // Tek bar kaldıysa kırmızı, yoksa yeşil yap
                if (acikBarSayisi == 1)
                {
                    pilBarlari[i].color = Color.red;
                }
                else
                {
                    pilBarlari[i].color = Color.green;
                }
            }
        }
    }
}
