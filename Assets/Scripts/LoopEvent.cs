using UnityEngine;

public class LoopEvent : MonoBehaviour
{
    [Header("Hangi Turlarda Görünsün?")]
    [Tooltip("Örneğin 2 ve 3 yazarsanız, bu obje sadece 2. ve 3. turda sahnede olur.")]
    public int[] activeInLoops;

    [Header("Döngü Kırıldığında Davranış")]
    [Tooltip("Döngü kırıldığında (3. tur bittiğinde) bu obje sahnede kalsın mı?")]
    public bool keepActiveWhenBroken = false;

    private void Start()
    {
        // Bir frame gecikmeli çalıştır ki LoopManager kesin olarak hazır olsun
        Invoke(nameof(CheckLoopVisibility), 0.1f);
        
        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.OnLoopChanged.AddListener(CheckLoopVisibility);
            LoopManager.Instance.OnLoopBroken.AddListener(CheckLoopVisibility);
        }
    }

    private void CheckLoopVisibility()
    {
        if (LoopManager.Instance == null) return;

        // Döngü kırıldıysa ve bu obje kalıcıysa
        if (LoopManager.Instance.isLoopBroken && keepActiveWhenBroken)
        {
            gameObject.SetActive(true);
            return;
        }
        else if (LoopManager.Instance.isLoopBroken && !keepActiveWhenBroken)
        {
            gameObject.SetActive(false);
            return;
        }

        // Normal döngü devam ederken
        bool shouldBeActive = false;
        foreach (int loop in activeInLoops)
        {
            if (loop == LoopManager.Instance.currentLoop)
            {
                shouldBeActive = true;
                break;
            }
        }

        gameObject.SetActive(shouldBeActive);
    }
}
