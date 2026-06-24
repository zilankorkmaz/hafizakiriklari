using UnityEngine;

public class GameEndTrigger : MonoBehaviour
{
    public string endingType = "BetrayalEnding"; // "TrueEnding" veya "BetrayalEnding"
    public AudioSource whisperSound; // Fısıltı sesi için referans
    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("Oyun sonu tetiklendi: " + endingType);
            
            // Fısıltı sesini çal
            if (whisperSound != null)
            {
                whisperSound.Play();
            }

            if (GameEndManager.Instance != null)
            {
                if (endingType == "BetrayalEnding")
                {
                    GameEndManager.Instance.ShowBetrayalEnding();
                }
                else if (endingType == "TrueEnding")
                {
                    GameEndManager.Instance.ShowTrueEndingChoice();
                }
            }
            else
            {
                Debug.LogWarning("GameEndManager bulunamadı!");
            }
        }
    }
}
