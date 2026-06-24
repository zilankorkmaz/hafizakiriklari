using UnityEngine;

public class TrueEndingTrigger : MonoBehaviour
{
    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            if (GameEndManager.Instance != null)
            {
                GameEndManager.Instance.ShowTrueEndingChoice();
            }
        }
    }
}

public class BetrayalEndingTrigger : MonoBehaviour
{
    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        // Ölümcül suya veya kapı arkasına değdiğinde tetiklenir
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;
            if (GameEndManager.Instance != null)
            {
                GameEndManager.Instance.ShowBetrayalEnding();
            }
        }
    }
}
