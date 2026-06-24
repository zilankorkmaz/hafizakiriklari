using UnityEngine;
using TMPro;

public class PowerSwitch : MonoBehaviour
{
    public static bool isPowerOn = false; // Tüm sistemler için genel güç durumu
    public static int collectedFuses = 0; // Toplanan sigorta sayısı

    [Header("Bulmaca Ayarları")]
    public int requiredFuses = 3; // Gereken toplam sigorta sayısı

    [Header("Ayarlar")]
    public bool triggersPanic = true; // Şalter açıldığında panik başlasın mı? (Sahne 3 için false yapılabilir)
    public Light[] roomLightsToEnable; // Şalter açıldığında yanacak oda ışıkları

    [Header("UI Ayarları")]
    public GameObject interactText; // Ekranda belirecek "E - Şalteri Aç" yazısı (Canvas içinde)

    [Header("Görsel/İşitsel Efektler")]
    public AudioSource switchSound; // Şalter açılma sesi
    public Light indicatorLight;    // Şalterin üzerindeki küçük ışık (Opsiyonel)

    private bool playerNearby = false;

    void Start()
    {
        isPowerOn = false; // Oyun başladığında güç kapalı
        collectedFuses = 0; // Sayacı sıfırla

        if (interactText != null)
            interactText.SetActive(false);

        if (indicatorLight != null)
        {
            indicatorLight.color = Color.red; // Güç yokken ışık kırmızı
        }
    }

    void Update()
    {
        if (playerNearby && !isPowerOn && Input.GetKeyDown(KeyCode.E))
        {
            if (collectedFuses >= requiredFuses)
            {
                TurnOnPower();
            }
            else
            {
                Debug.Log("Yeterli sigorta yok!");
            }
        }
    }

    private void TurnOnPower()
    {
        isPowerOn = true;
        Debug.Log("Sisteme güç verildi!");

        // Etkileşim yazısını gizle
        if (interactText != null) 
            interactText.SetActive(false);

        // Ses çal
        if (switchSound != null)
            switchSound.Play();

        // Işığı yeşil yap
        if (indicatorLight != null)
        {
            indicatorLight.color = Color.green;
        }

        // Oda Işıklarını Yak
        if (roomLightsToEnable != null && roomLightsToEnable.Length > 0)
        {
            foreach (Light l in roomLightsToEnable)
            {
                if (l != null) l.enabled = true;
            }
        }

        // --- PANİK DURUMU ---
        if (triggersPanic && PanicManager.Instance != null)
        {
            PanicManager.Instance.StartPanicMode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPowerOn)
        {
            playerNearby = true;
            if (interactText != null)
            {
                interactText.SetActive(true);
                string msg = (collectedFuses >= requiredFuses) 
                    ? "E - Şalteri Aç" 
                    : $"Eksik Sigorta ({collectedFuses}/{requiredFuses})";
                SetInteractionText(msg);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (interactText != null)
                interactText.SetActive(false);
        }
    }

    private void SetInteractionText(string msg)
    {
        var tmp = interactText.GetComponent<TextMeshProUGUI>();
        if (tmp != null) { tmp.text = msg; return; }

        var txt = interactText.GetComponent<UnityEngine.UI.Text>();
        if (txt != null) txt.text = msg;
    }
}
