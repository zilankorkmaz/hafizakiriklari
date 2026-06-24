using UnityEngine;
using TMPro;

public class CurePotion : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject interactText; // Ekranda belirecek "E - İksiri İç" yazısı

    [Header("Efektler")]
    public AudioSource drinkSound; // İksir içme sesi

    private bool playerNearby = false;
    private bool isConsumed = false;

    void Start()
    {
        if (interactText != null)
            interactText.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && !isConsumed && Input.GetKeyDown(KeyCode.E))
        {
            ConsumePotion();
        }
    }

    private void ConsumePotion()
    {
        isConsumed = true;
        
        // Sesi çal
        if (drinkSound != null) drinkSound.Play();

        if (interactText != null) interactText.SetActive(false);

        // LethalThreat (Zehir) sayacını durdur
        LethalThreat threat = FindObjectOfType<LethalThreat>();
        if (threat != null)
        {
            threat.CurePoison();
        }

        // Objeyi kaybet
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Kurtulma yazısını göster
        if (GameEndingManager.Instance != null)
        {
            GameEndingManager.Instance.ShowTemporaryText("Kurtuldum... Zehir vücudumdan atılıyor.", 4f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isConsumed)
        {
            playerNearby = true;
            if (interactText != null)
            {
                interactText.SetActive(true);
                TextMeshProUGUI tmpText = interactText.GetComponent<TextMeshProUGUI>();
                if (tmpText != null) tmpText.text = "E - Iksiri Ic";
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
