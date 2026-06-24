using UnityEngine;
using UnityEngine.UI;

public class NoteInteraction : MonoBehaviour
{
    public enum NoteType { NotebookItself, ScatteredPage }
    
    [Header("Note Settings")]
    public NoteType typeOfNote = NoteType.ScatteredPage;
    [TextArea(3, 10)]
    public string pageContent = "Not içeriği buraya...";
    
    [Header("Audio (Optional)")]
    [Tooltip("Bu not okunurken çalacak ses (Fısıltı vb.)")]
    public AudioClip readingSound;

    [Header("UI & Interaction")]
    public GameObject interactText;
    public MonoBehaviour playerController;
    public bool destroyAfterReading = false;

    private bool playerNearby = false;
    private int collidersInTrigger = 0;

    void Start()
    {
        if (interactText != null)
            interactText.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            var notebook = NotebookManager.Instance;
            if (notebook == null) notebook = FindFirstObjectByType<NotebookManager>(FindObjectsInactive.Include);

            if (notebook != null)
            {
                // Ensure the entire parent hierarchy (e.g. Canvas) is active
                Transform current = notebook.transform;
                while (current != null)
                {
                    if (!current.gameObject.activeSelf) current.gameObject.SetActive(true);
                    current = current.parent;
                }
            }

            if (typeOfNote == NoteType.NotebookItself)
            {
                // Defterin kendisine tıklandıysa, defteri aç.
                if (notebook != null)
                    notebook.OpenNotebook(playerController, readingSound);
            }
            else if (typeOfNote == NoteType.ScatteredPage)
            {
                if (notebook != null)
                {
                    // Deftere ekle
                    notebook.AddPage(pageContent);
                    // Ekranda tek sayfa olarak göster (Sesi de yolla)
                    notebook.ShowSingleNote(pageContent, playerController, readingSound);
                }

                if (destroyAfterReading)
                {
                    if (interactText != null) interactText.SetActive(false);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collidersInTrigger++;
            playerNearby = true;
            if (interactText != null)
            {
                interactText.SetActive(true);
                SetInteractionText();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collidersInTrigger--;
            if (collidersInTrigger <= 0)
            {
                collidersInTrigger = 0;
                playerNearby = false;
                if (interactText != null)
                    interactText.SetActive(false);
            }
        }
    }

    private void SetInteractionText()
    {
        string msg = (typeOfNote == NoteType.ScatteredPage) ? "E - Sayfayı Topla" : "E - Defteri Aç";

        // Try TextMeshPro first
        var tmp = interactText.GetComponent<TMPro.TextMeshProUGUI>();
        if (tmp != null) 
        {
            tmp.text = msg;
            return;
        }

        // Try standard UI Text
        var txt = interactText.GetComponent<Text>();
        if (txt != null) 
        {
            txt.text = msg;
        }
    }
}
