using UnityEngine;
using UnityEngine.UI;

public class ComputerUIManager : MonoBehaviour
{
    public static ComputerUIManager Instance { get; private set; }

    [Header("UI Referansları")]
    public GameObject computerPanel;
    public Text screenText;
    public TMPro.TextMeshProUGUI screenTextTMP;

    private bool isComputerOpen = false;
    private float openTime = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    private void Start()
    {
        if (computerPanel != null)
        {
            computerPanel.SetActive(false);
        }
    }

    public void OpenTerminal(string textToDisplay)
    {
        if (computerPanel == null)
        {
            Transform[] allT = GetComponentsInChildren<Transform>(true);
            foreach(var t in allT)
            {
                if (t.name.Contains("Panel") || t.name.Contains("DOS_Screen"))
                {
                    computerPanel = t.gameObject;
                    break;
                }
            }
        }

        if (computerPanel != null)
        {
            if (screenText == null && screenTextTMP == null)
            {
                var texts = computerPanel.GetComponentsInChildren<Text>(true);
                if (texts.Length > 0) screenText = texts[0];
                
                var tmps = computerPanel.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
                if (tmps.Length > 0) screenTextTMP = tmps[0];
            }
        }

        if (computerPanel == null || (screenText == null && screenTextTMP == null)) 
        {
            Debug.LogError("ComputerUIManager: Arayüz elemanları bulunamadı!");
            return;
        }

        if (screenText != null) screenText.text = textToDisplay;
        if (screenTextTMP != null) screenTextTMP.text = textToDisplay;

        computerPanel.SetActive(true);
        isComputerOpen = true;
        openTime = Time.time;

        // Farenin kilidini aç
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Karakterin hareketini kısıtla
        var fps = FindFirstObjectByType<StarterAssets.FirstPersonController>();
        if (fps != null) fps.enabled = false;
    }

    public void CloseTerminal()
    {
        if (computerPanel != null)
        {
            computerPanel.SetActive(false);
        }
        isComputerOpen = false;

        // Farenin kilidini kapat
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Karakterin hareketini geri ver
        var fps = FindFirstObjectByType<StarterAssets.FirstPersonController>();
        if (fps != null) fps.enabled = true;
    }

    private void Update()
    {
        // Bilgisayar açıkken ESC tuşuna veya tekrar E'ye basılırsa kapat (açıldığı karede kapanmaması için küçük bir gecikme ekliyoruz)
        if (isComputerOpen && Time.time > openTime + 0.1f)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                CloseTerminal();
            }
        }
    }

    public bool IsTerminalOpen()
    {
        return isComputerOpen;
    }
}
