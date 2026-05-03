using UnityEngine;
using TMPro;

public class PowerSwitch : MonoBehaviour
{
    public static bool isPowerOn = false; // Tüm sistemler için genel güç durumu

    [Header("UI Ayarları")]
    public GameObject interactText; // Ekranda belirecek "E - Şalteri Aç" yazısı (Canvas içinde)

    [Header("Görsel/İşitsel Efektler")]
    public AudioSource switchSound; // Şalter açılma sesi
    public Light indicatorLight;    // Şalterin üzerindeki küçük ışık (Opsiyonel)

    private bool playerNearby = false;

    void Start()
    {
        isPowerOn = false; // Oyun başladığında güç kapalı

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
            TurnOnPower();
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

        // --- YENİ EKLENEN: PANİK HEMEN BAŞLAR ---
        if (PanicManager.Instance != null)
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
                TextMeshProUGUI tmpText = interactText.GetComponent<TextMeshProUGUI>();
                if (tmpText != null) tmpText.text = "E - Salteri Ac";
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
}
