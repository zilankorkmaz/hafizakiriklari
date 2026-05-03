using UnityEngine;
using TMPro;
using System.Collections;

public class KeypadUI : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject keypadPanel;
    public TextMeshProUGUI passwordDisplayText;

    [Header("Şifre Ayarları")]
    public string correctPassword = "1503"; // Geçerli şifremiz
    private string currentInput = "";
    private int maxInputLength = 4;

    [Header("Bağlantılar")]
    public PasswordDoor targetDoor;       // Açılacak kapı
    public MonoBehaviour playerController; // Oyuncu kontrolcüsü (FirstPersonController vb.) - Hareketini durdurmak için

    private bool isOpen = false;
    private bool isProcessing = false;

    void Start()
    {
        keypadPanel.SetActive(false);
        UpdateDisplay();
    }

    void Update()
    {
        // Panel açıkken Escape'e basılırsa kapat
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseKeypad();
        }
    }

    public void OpenKeypad()
    {
        isOpen = true;
        keypadPanel.SetActive(true);
        currentInput = "";
        UpdateDisplay();

        // Mouse'u görünür yap ve oyuncu hareketini kitle
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
            playerController.enabled = false;
    }

    public void CloseKeypad()
    {
        isOpen = false;
        keypadPanel.SetActive(false);

        // Mouse'u gizle ve oyuncu hareketini geri ver
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
            playerController.enabled = true;
    }

    public bool IsKeypadOpen()
    {
        return isOpen;
    }

    // Butonlara tıklanınca çağrılacak (0-9 rakamları için)
    public void AddDigit(string digit)
    {
        if (isProcessing) return;

        if (currentInput.Length < maxInputLength)
        {
            currentInput += digit;
            UpdateDisplay();
        }
    }

    // Clear (Temizle) butonu için
    public void ClearInput()
    {
        if (isProcessing) return;
        currentInput = "";
        UpdateDisplay();
    }

    // Enter (Onayla) butonu için
    public void SubmitPassword()
    {
        if (isProcessing) return;

        if (currentInput == correctPassword)
        {
            StartCoroutine(ShowResult("BASARILI", Color.green));
            if (targetDoor != null)
            {
                targetDoor.OpenDoor();
            }
        }
        else
        {
            StartCoroutine(ShowResult("HATA", Color.red));
        }
    }

    private void UpdateDisplay()
    {
        if (passwordDisplayText != null)
        {
            string displayString = "";
            for (int i = 0; i < maxInputLength; i++)
            {
                if (i < currentInput.Length)
                {
                    displayString += currentInput[i] + " ";
                }
                else
                {
                    displayString += "_ ";
                }
            }

            passwordDisplayText.text = displayString.Trim();
            passwordDisplayText.color = Color.white; // Rengi normale döndür
        }
    }

    private IEnumerator ShowResult(string message, Color color)
    {
        isProcessing = true;
        if (passwordDisplayText != null)
        {
            passwordDisplayText.text = message;
            passwordDisplayText.color = color;
        }

        yield return new WaitForSeconds(1.5f);

        if (message == "BASARILI")
        {
            CloseKeypad(); // Başarılıysa paneli komple kapat
        }
        else
        {
            currentInput = "";
            UpdateDisplay();
            isProcessing = false;
        }
    }
}
