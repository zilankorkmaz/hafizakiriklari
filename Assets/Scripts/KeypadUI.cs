using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

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

    [Header("Özel Olaylar")]
    public UnityEvent onPasswordCorrect; // Şifre doğru girildiğinde tetiklenecek olaylar (Final vs.)

    private bool isOpen = false;
    private bool isProcessing = false;
    private float openTime = 0f;

    // Sahnede aynı anda birden fazla kapı varsa, tuşların doğru kapıya basmasını sağlar
    public static KeypadUI activeKeypad;

    void Start()
    {
        if (!isOpen)
        {
            keypadPanel.SetActive(false);
        }
        UpdateDisplay();
    }

    void Update()
    {
        // Panel açıkken Escape veya E ile kapat (hemen kapanmaması için gecikme eklendi)
        if (isOpen && Time.time > openTime + 0.1f)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                CloseKeypad();
            }
        }
    }

    public void OpenKeypad()
    {
        isOpen = true;
        activeKeypad = this; // Aktif kapı bu oldu
        keypadPanel.SetActive(true);
        currentInput = "";
        UpdateDisplay();

        // Mouse'u görünür yap ve oyuncu hareketini kitle
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
            playerController.enabled = false;
        else
        {
            var fps = FindFirstObjectByType<StarterAssets.FirstPersonController>();
            if (fps != null) fps.enabled = false;
        }

        openTime = Time.time;
    }

    public void CloseKeypad()
    {
        isOpen = false;
        if (activeKeypad == this) activeKeypad = null;
        
        keypadPanel.SetActive(false);

        // Mouse'u gizle ve oyuncu hareketini geri ver
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
            playerController.enabled = true;
        else
        {
            var fps = FindFirstObjectByType<StarterAssets.FirstPersonController>();
            if (fps != null) fps.enabled = true;
        }
    }

    public bool IsKeypadOpen()
    {
        return isOpen;
    }

    // Butonlara tıklanınca çağrılacak (0-9 rakamları için)
    public void AddDigit(string digit)
    {
        // Eğer bu kapı açık değilse ama tuşlara basıldıysa, aktif olan kapıya yönlendir
        if (!isOpen && activeKeypad != null && activeKeypad != this)
        {
            activeKeypad.AddDigit(digit);
            return;
        }

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
        if (!isOpen && activeKeypad != null && activeKeypad != this)
        {
            activeKeypad.ClearInput();
            return;
        }

        if (isProcessing) return;
        currentInput = "";
        UpdateDisplay();
    }

    // Enter (Onayla) butonu için
    public void SubmitPassword()
    {
        if (!isOpen && activeKeypad != null && activeKeypad != this)
        {
            activeKeypad.SubmitPassword();
            return;
        }

        if (isProcessing) return;

        Debug.Log($"Şifre Kontrolü Yapılıyor! Girilen: '{currentInput}', Olması Gereken: '{correctPassword}'");

        if (currentInput == correctPassword)
        {
            StartCoroutine(ShowResult("BASARILI", Color.green));
            if (targetDoor != null)
            {
                targetDoor.OpenDoor();
            }
            
            // Eğer inspector'da bir olay bağlandıysa onu çalıştır (örneğin Finali tetikle)
            if (onPasswordCorrect != null)
            {
                onPasswordCorrect.Invoke();
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
