using UnityEngine;

public class NoteInteraction : MonoBehaviour
{
    public GameObject interactText;
    public GameObject notePanel;
    public MonoBehaviour playerController;

    private bool playerNearby = false;
    private bool noteOpen=false;
    void Start()
    {
        interactText.SetActive(false);
        notePanel.SetActive(false);
    }

    void Update()
    {
        if(playerNearby && !noteOpen && Input.GetKeyDown(KeyCode.E))
        {
            noteOpen = true;
            notePanel.SetActive(true);
            interactText.SetActive(false);

            if(playerController!=null)
                playerController.enabled = false;

            Debug.Log("Not açıldı");
        }
        if (noteOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            noteOpen = false;
            notePanel.SetActive(false);
            if (playerController != null)
                playerController.enabled = true;

            if (playerNearby)
                interactText.SetActive(true);

            Debug.Log("Not kapandı");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNearby = true;
            if(!noteOpen)
                interactText.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNearby = false;
            interactText.SetActive(false);
        }
    }
}
