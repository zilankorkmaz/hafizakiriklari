using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oyuncu bu görünmez objeye çarptığında otomatik olarak belirtilen sahneye geçiş yapar.
/// (Koridorun sonundaki çıkış kapısı için)
/// </summary>
public class LevelTransition : MonoBehaviour
{
    [Tooltip("Geçmek istediğin sahnenin tam adı (Örn: Level_3)")]
    public string nextSceneName = "Level_3";

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        TryTransition(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryTransition(other);
    }

    private void TryTransition(Collider other)
    {
        bool isPlayer = other.CompareTag("Player") || 
                        other.GetComponent<CharacterController>() != null || 
                        other.GetComponentInParent<CharacterController>() != null ||
                        other.GetComponentInChildren<Camera>() != null;

        if (!isTransitioning && isPlayer)
        {
            isTransitioning = true;
            Debug.Log("Sahne Geçişi Başlatılıyor: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
