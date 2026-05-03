using UnityEngine;
using TMPro;

public class KeypadInteract : MonoBehaviour
{
    [Header("UI Referansları")]
    public GameObject interactText; // Ekranda belirecek "E ile Şifre Gir" yazısı (Canvas içinde)
    public KeypadUI keypadUI;       // Şifre ekranı yöneticisi

    private bool playerNearby = false;

    void Start()
    {
        if (interactText != null)
            interactText.SetActive(false);
    }

    void Update()
    {
        // Oyuncu yakındaysa ve E'ye basarsa
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!PowerSwitch.isPowerOn)
            {
                // Elektrik yokken basılırsa uyarıyı belirginleştir (Opsiyonel olarak ses çalabilir)
                Debug.Log("Elektrik yok, şifre paneli çalışmıyor!");
                return;
            }

            if (!keypadUI.IsKeypadOpen())
            {
                // Etkileşim yazısını gizle ve şifre panelini aç
                if (interactText != null) interactText.SetActive(false);
                keypadUI.OpenKeypad();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            // Eğer keypad o an açık değilse etkileşim yazısını göster
            if (!keypadUI.IsKeypadOpen() && interactText != null)
            {
                interactText.SetActive(true);
                TextMeshProUGUI tmpText = interactText.GetComponent<TextMeshProUGUI>();
                if (tmpText != null)
                {
                    if (PowerSwitch.isPowerOn)
                    {
                        tmpText.text = "E - Sifreyi Gir";
                        tmpText.color = Color.white;
                    }
                    else
                    {
                        tmpText.text = "GUC YOK - Salteri Bul";
                        tmpText.color = Color.red;
                    }
                }
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
            
            // Oyuncu uzaklaştığında keypad'i zorla kapat
            if (keypadUI.IsKeypadOpen())
            {
                keypadUI.CloseKeypad();
            }
        }
    }
}
