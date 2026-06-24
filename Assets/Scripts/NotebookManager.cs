using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookManager : MonoBehaviour
{
    public static NotebookManager Instance { get; private set; }

    [Header("Book UI (2 Pages)")]
    public GameObject bookPanel;
    public TMPro.TextMeshProUGUI leftPageText;
    public TMPro.TextMeshProUGUI rightPageText;
    public TMPro.TextMeshProUGUI pageNumberText; // Yeni Eklenen
    public Button nextButton;
    public Button prevButton;

    [Header("Single Note UI (1 Page)")]
    public GameObject singleNotePanel;
    public TMPro.TextMeshProUGUI singleNoteText;

    [Header("Defter İçeriği")]
    [TextArea(3, 10)]
    public string defaultStartingPage = "Deney başarısız oldu... Hafızanı kaybetmiş olabilirsin. Eğer bunu okuyorsan, bil ki kontrolden çıkan bu kaosu durdurabilecek tek kişi sensin. Sana bırakabildiğim tek şey bu günlüğüm.";
    
    public List<string> collectedPages = new List<string>();

    private List<string> activeBookContent = new List<string>();
    private int currentPageIndex = 0;
    private bool isNotebookOpen = false;
    private bool isSingleNoteOpen = false;
    private MonoBehaviour playerController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        // Obje silme (Destroy(gameObject)) islemini kaldirdik, cunku sahnedeki yanlis eklenmis 
        // NotebookManager bilesenleri silindiginde uzerindeki NoteInteraction veya Buton eventleri bozabiliyordu.
    }

    private void Start()
    {
        if (bookPanel != null) bookPanel.SetActive(false);
        if (singleNotePanel != null) singleNotePanel.SetActive(false);

        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        else 
        {
            Button btn = FindButtonComponent(bookPanel, new string[] { "NextButton", "NextPageButton", ">Btn" });
            if (btn != null) { nextButton = btn; nextButton.onClick.AddListener(NextPage); }
        }

        if (prevButton != null) prevButton.onClick.AddListener(PrevPage);
        else 
        {
            Button btn = FindButtonComponent(bookPanel, new string[] { "PrevButton", "PrevPageButton", "<Btn" });
            if (btn != null) { prevButton = btn; prevButton.onClick.AddListener(PrevPage); }
        }
    }

    private void Update()
    {
        if (isNotebookOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) CloseNotebook();
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) PrevPage();
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) NextPage();
        }
        else if (isSingleNoteOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) CloseSingleNote();
        }
    }

    public void AddPage(string text)
    {
        if (!collectedPages.Contains(text))
        {
            collectedPages.Add(text);
            Debug.Log("Yeni sayfa eklendi.");
        }
    }

    // --- TEK SAYFA / ETRAFTAN BULUNAN NOT SİSTEMİ ---
    public void ShowSingleNote(string textContent, MonoBehaviour controller, AudioClip readingSound = null)
    {
        if (singleNotePanel == null)
        {
            Transform[] allT = GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allT)
            {
                if (t.name == "SingleNotePanel" || t.name == "NotePanel")
                {
                    singleNotePanel = t.gameObject;
                    break;
                }
            }

            if (singleNotePanel == null)
            {
                Transform[] globalT = Resources.FindObjectsOfTypeAll<Transform>();
                foreach (Transform t in globalT)
                {
                    if (t.gameObject.scene.isLoaded && (t.name == "SingleNotePanel" || t.name == "NotePanel"))
                    {
                        singleNotePanel = t.gameObject;
                        break;
                    }
                }
            }
        }
        
        if (singleNotePanel == null) return;

        // Okunma / Fısıltı sesi çalma (Yeni eklendi)
        if (readingSound != null)
        {
            AudioSource aSrc = gameObject.GetComponent<AudioSource>();
            if (aSrc == null) aSrc = gameObject.AddComponent<AudioSource>();
            aSrc.PlayOneShot(readingSound);
        }

        bool textSet = false;
        if (singleNoteText != null)
        {
            singleNoteText.text = textContent;
            textSet = true;
        }
        else
        {
            Transform[] allT = singleNotePanel.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allT)
            {
                if (t.name == "SingleNoteText" || t.name == "Text")
                {
                    TMPro.TextMeshProUGUI tmp = t.GetComponent<TMPro.TextMeshProUGUI>();
                    if (tmp != null && (t.name.Contains("Text") || t.name.Contains("Content")))
                    {
                        singleNoteText = tmp;
                        singleNoteText.text = textContent;
                        textSet = true;
                        break;
                    }

                    // Legacy UI Text destegi
                    UnityEngine.UI.Text legacyText = t.GetComponent<UnityEngine.UI.Text>();
                    if (legacyText != null && (t.name.Contains("Text") || t.name.Contains("Content")))
                    {
                        legacyText.text = textContent;
                        textSet = true;
                        break;
                    }
                }
            }
        } if (!textSet) return;
        
        isSingleNoteOpen = true;
        singleNotePanel.SetActive(true);
        
        playerController = controller;
        if (playerController != null) playerController.enabled = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseSingleNote()
    {
        if (Instance != null && Instance != this) { Instance.CloseSingleNote(); return; }

        isSingleNoteOpen = false;
        if (singleNotePanel != null) singleNotePanel.SetActive(false);
        
        if (playerController != null) playerController.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenNotebook(MonoBehaviour controller = null, AudioClip readingSound = null)
    {
        Debug.Log("OpenNotebook cagirildi!");
        
        // Okunma / Fısıltı sesi çalma (Yeni eklendi)
        if (readingSound != null)
        {
            AudioSource aSrc = gameObject.GetComponent<AudioSource>();
            if (aSrc == null) aSrc = gameObject.AddComponent<AudioSource>();
            aSrc.PlayOneShot(readingSound);
        }

        if (bookPanel == null) 
        {
            Transform[] allT = GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allT)
            {
                if (t.name == "BookPanel" || t.name == "NotebookPanel" || t.name == "DefterPaneli")
                {
                    bookPanel = t.gameObject;
                    break;
                }
            }

            if (bookPanel == null)
            {
                Transform[] globalT = Resources.FindObjectsOfTypeAll<Transform>();
                foreach (Transform t in globalT)
                {
                    if (t.gameObject.scene.isLoaded && (t.name == "BookPanel" || t.name == "NotebookPanel" || t.name == "DefterPaneli"))
                    {
                        bookPanel = t.gameObject;
                        break;
                    }
                }
            }
            
            if (bookPanel == null)
            {
                Debug.LogError("NotebookManager'da Book Panel atanmamis ve bulunamadi!");
                return;
            }
        }

        activeBookContent.Clear();
        activeBookContent.Add(defaultStartingPage);
        activeBookContent.AddRange(collectedPages);
        
        isNotebookOpen = true;
        bookPanel.SetActive(true);
        Debug.Log("Defter ekranda gorunur yapildi!");
        
        currentPageIndex = 0; 
        
        playerController = controller;
        if (playerController != null) playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        UpdateDisplay();
    }

    public void CloseNotebook()
    {
        if (Instance != null && Instance != this) { Instance.CloseNotebook(); return; }

        isNotebookOpen = false;
        if (bookPanel != null) bookPanel.SetActive(false);

        if (playerController != null) playerController.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void NextPage()
    {
        if (Instance != null && Instance != this) { Instance.NextPage(); return; }

        bool hasRightPage = rightPageText != null || HasChildNamed(bookPanel, "RightPageText");
        int increment = hasRightPage ? 2 : 1;
        if (currentPageIndex + increment < activeBookContent.Count)
        {
            currentPageIndex += increment;
            UpdateDisplay();
        }
    }

    public void PrevPage()
    {
        if (Instance != null && Instance != this) { Instance.PrevPage(); return; }

        bool hasRightPage = rightPageText != null || HasChildNamed(bookPanel, "RightPageText");
        int increment = hasRightPage ? 2 : 1;
        if (currentPageIndex >= increment)
        {
            currentPageIndex -= increment;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        string leftStr = (currentPageIndex >= 0 && currentPageIndex < activeBookContent.Count) ? activeBookContent[currentPageIndex] : "";
        SetPageText(leftPageText, "LeftPageText", leftStr);

        int rightIndex = currentPageIndex + 1;
        string rightStr = (rightIndex >= 0 && rightIndex < activeBookContent.Count) ? activeBookContent[rightIndex] : "";
        SetPageText(rightPageText, "RightPageText", rightStr);

        bool hasRightPage = rightPageText != null || HasChildNamed(bookPanel, "RightPageText");
        int increment = hasRightPage ? 2 : 1;

        // --- YENİ EKLENEN KISIM: Sayfa Numarası Gösterimi ---
        if (pageNumberText != null)
        {
            // Eğer iki sayfa yan yana görünüyorsa (1-2), (3-4) gibi gösterilebilir ama tek sayfa ise (1 / Toplam) olarak gösterelim.
            if (hasRightPage)
            {
                int maxPage = Mathf.Min(rightIndex + 1, activeBookContent.Count);
                pageNumberText.text = (currentPageIndex + 1).ToString() + "-" + maxPage.ToString() + " / " + activeBookContent.Count.ToString();
            }
            else
            {
                pageNumberText.text = (currentPageIndex + 1).ToString() + " / " + activeBookContent.Count.ToString();
            }
        }
        // ---------------------------------------------------

        UpdateButtonState(prevButton, new string[] { "PrevButton", "PrevPageButton", "<Btn" }, currentPageIndex > 0);
        UpdateButtonState(nextButton, new string[] { "NextButton", "NextPageButton", ">Btn" }, currentPageIndex + increment < activeBookContent.Count);
    }

    private void SetPageText(TMPro.TextMeshProUGUI tmpField, string objName, string textContent)
    {
        if (tmpField != null)
        {
            tmpField.text = textContent;
            return;
        }
        
        if (bookPanel != null)
        {
            Transform[] allT = bookPanel.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allT)
            {
                if (t.name == objName)
                {
                    var tmp = t.GetComponent<TMPro.TextMeshProUGUI>();
                    if (tmp != null) { tmp.text = textContent; return; }
                    var txt = t.GetComponent<UnityEngine.UI.Text>();
                    if (txt != null) { txt.text = textContent; return; }
                }
            }
        }
    }

    private bool HasChildNamed(GameObject parentObj, string name)
    {
        if (parentObj == null) return false;
        Transform[] allT = parentObj.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allT)
        {
            if (t.name == name) return true;
        }
        return false;
    }

    private Button FindButtonComponent(GameObject parentObj, string[] names)
    {
        if (parentObj == null) return null;
        Transform[] allT = parentObj.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allT)
        {
            foreach (string n in names)
            {
                if (t.name == n)
                {
                    Button b = t.GetComponent<Button>();
                    if (b != null) return b;
                }
            }
        }
        return null;
    }

    private void UpdateButtonState(Button btnField, string[] searchNames, bool state)
    {
        if (btnField != null)
        {
            btnField.gameObject.SetActive(state);
            return;
        }
        Button b = FindButtonComponent(bookPanel, searchNames);
        if (b != null) b.gameObject.SetActive(state);
    }
}
