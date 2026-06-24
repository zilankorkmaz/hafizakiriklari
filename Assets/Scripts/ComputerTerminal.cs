using UnityEngine;

public class ComputerTerminal : MonoBehaviour
{
    [Header("Terminal Metinleri (Turlara Göre)")]
    [Tooltip("Döngü (Tur) sayısına göre ekranda ne yazacağını belirler. 1. kutu = 1. Tur.")]
    [TextArea(4, 10)]
    public string[] loopTexts = new string[] {
        "C:\\> ONAY BEKLENIYOR...\nDurum: SİSTEM STABİL.\nSon Kullanıcı: Dr. Evans.",
        "C:\\> HATA: 404 \nBİRDEN FAZLA KALP ATIŞI TESPİT EDİLDİ.\nUyanıyorlar.",
        "C:\\> FATAL ERROR\n\nARKANA BAKMA."
    };

    [Header("UI Ayarları")]
    public GameObject interactText; // "E - Terminale Gir" yazısı

    private bool playerNearby = false;

    void Start()
    {
        if (interactText != null)
            interactText.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (ComputerUIManager.Instance != null && !ComputerUIManager.Instance.IsTerminalOpen())
            {
                string textToShow = GetTextForCurrentLoop();
                ComputerUIManager.Instance.OpenTerminal(textToShow);
            }
        }
    }

    private string GetTextForCurrentLoop()
    {
        if (loopTexts == null || loopTexts.Length == 0)
        {
            return "C:\\> SİSTEM HATASI: VERİ BULUNAMADI.";
        }

        int currentLoop = 1;
        if (LoopManager.Instance != null)
        {
            currentLoop = LoopManager.Instance.currentLoop;
        }

        int index = Mathf.Clamp(currentLoop - 1, 0, loopTexts.Length - 1);
        return loopTexts[index];
    }

    private bool IsPlayer(Collider other)
    {
        return other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.GetComponentInParent<CharacterController>() != null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            playerNearby = true;
            if (interactText != null) interactText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            playerNearby = false;
            if (interactText != null) interactText.SetActive(false);
            
            // Bilgisayarın başından uzaklaşınca (hala açıksa) terminali otomatik kapat
            if (ComputerUIManager.Instance != null)
            {
                ComputerUIManager.Instance.CloseTerminal();
            }
        }
    }
}
