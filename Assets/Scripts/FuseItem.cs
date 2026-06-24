using UnityEngine;
using UnityEngine.UI;

public class FuseItem : MonoBehaviour
{
    [Header("UI Ayarları")]
    public GameObject interactText; 

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
            PowerSwitch.collectedFuses++;
            Debug.Log($"Sigorta toplandı! (Toplam: {PowerSwitch.collectedFuses})");

            if (interactText != null) interactText.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (interactText != null)
            {
                interactText.SetActive(true);
                SetInteractionText("E - Sigortayı Al");
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

    private void SetInteractionText(string msg)
    {
        var tmp = interactText.GetComponent<TMPro.TextMeshProUGUI>();
        if (tmp != null) { tmp.text = msg; return; }

        var txt = interactText.GetComponent<Text>();
        if (txt != null) txt.text = msg;
    }
}
