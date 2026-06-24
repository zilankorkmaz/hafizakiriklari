using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class FotoKesif : MonoBehaviour
{
    [Header("UI Objeleri")]
    public GameObject glitchUI;
    public GameObject karartmaUI;
    public TMPro.TextMeshProUGUI etkilesimYazisi; //E
    public TMPro.TextMeshProUGUI  altyaziText;

    [Header("Hikaye Metinleri")]
    [TextArea(2, 4)]
    public string[] noteLines; // Unity Inspector'dan eklenecek not satırları

    [Header("Efekt ayarları")]
    public float glitchSuresi = 0.4f;
    public float incelemeSuresi = 2f;
    public float kararmaHizi = 1f;

    [Header("Ses Efektleri")]
    public AudioClip fisiltiSesi; // Okunurken çalacak opsiyonel ses

    private bool alandaMi = false;
    private bool fotoIncelendi = false;
    private bool sinematikBasladi = false;


    void Update()
    {
        if (alandaMi && !sinematikBasladi && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(HafizaKirigiSekansi());
    }
    IEnumerator HafizaKirigiSekansi()
    {
        sinematikBasladi = true;
        if (etkilesimYazisi != null) etkilesimYazisi.gameObject.SetActive(false);

        // Eğer oyuncu masaya çok erken gelirse, ekrandaki Tutorial yazılarını anında siler
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance.HideTutorial();
        }

        glitchUI.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        glitchUI.SetActive(false);

        // Fısıltı Sesi Çal (Varsa)
        if (fisiltiSesi != null)
        {
            AudioSource aSrc = gameObject.GetComponent<AudioSource>();
            if (aSrc == null)
            {
                aSrc = gameObject.AddComponent<AudioSource>();
                aSrc.spatialBlend = 0f; // Sesi 2D yaparak mesafeden kısılmasını engelle
            }
            aSrc.volume = 1f; // Sesi en yükseğe al
            aSrc.PlayOneShot(fisiltiSesi);
        }

        Debug.Log("Hafıza kırığı bulundu. Not okunuyor.");
        karartmaUI.SetActive(true);
        Image img = karartmaUI.GetComponent<Image>();
        float timer = 0f;
        while (timer < 1.1f)
        {
            img.color = new Color(0, 0, 0, timer / 1.1f);
            timer += Time.deltaTime * 0.4f;
            yield return null;
        }
        img.color = new Color(0, 0, 0, 1f); // Tam siyah olduğundan emin ol

        altyaziText.gameObject.SetActive(true);
        altyaziText.text = "";

        // Inspector'dan girilen not satırlarını sırayla göster
        if (noteLines != null && noteLines.Length > 0)
        {
            foreach (string line in noteLines)
            {
                altyaziText.text = line;
                yield return new WaitForSeconds(3f); // Her satır ekranda 3 saniye kalır
            }
        }
        else
        {
            // Eğer inspector'dan metin girilmemişse varsayılan eski metni göster (Hata olmaması için)
            altyaziText.text = "15 Mart...";
            yield return new WaitForSeconds(2f);
            altyaziText.text = "Hiçbir şey hatırlamıyorum.";
            yield return new WaitForSeconds(2f);
            altyaziText.text = "Bugünün tarihini bile bilmiyorum.";
            yield return new WaitForSeconds(2f);
            altyaziText.text = "Şifre bu olabilir mi?";
            yield return new WaitForSeconds(2.5f);
        }

        // Altyazıyı kapat
        altyaziText.gameObject.SetActive(false);

        // Ekranı yavaşça aydınlat (Fade Out)
        timer = 1f;
        while (timer > 0f)
        {
            img.color = new Color(0, 0, 0, timer);
            timer -= Time.deltaTime;
            yield return null;
        }
        karartmaUI.SetActive(false);

        sinematikBasladi = false;
        
        // Eğer oyuncu hala alanın içindeyse E yazısını tekrar çıkar
        if (alandaMi && etkilesimYazisi != null)
        {
            etkilesimYazisi.text = "E ile incele";
            etkilesimYazisi.gameObject.SetActive(true);
        }

        Debug.Log("Hafıza yeniden yüklendi. Sahne 1 hikayesi anlaşıldı.");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            alandaMi=true;
            if (!sinematikBasladi && etkilesimYazisi != null)
            {
                etkilesimYazisi.text = "E ile incele";
                etkilesimYazisi.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            alandaMi = false;
            if (etkilesimYazisi != null)
            {
                etkilesimYazisi.text = "";
                etkilesimYazisi.gameObject.SetActive(false);
            }
        }
    }
}
