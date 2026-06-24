using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PasswordDoor : MonoBehaviour
{
    [Header("Kapı Ayarları")]
    public float acilmaHizi = 2f;
    public float acilmaMiktari = 3f; // Kapı Y ekseninde ne kadar yukarı kalkacak?
    public bool yanaAcilsin = false; // Kapı yana doğru açılacaksa (X ekseni) bunu true yapın

    [Header("Sahne Geçişi")]
    public string nextSceneName = "Level_2"; // Kapı açılınca geçilecek sahnenin adı
    public float sceneTransitionDelay = 2f;  // Kapı açıldıktan kaç saniye sonra geçilsin

    private bool aciliyor = false;
    private Vector3 hedefPozisyon;

    void Start()
    {
        // Kapının açılacağı hedef konumu belirle
        if (yanaAcilsin)
        {
            hedefPozisyon = transform.position + new Vector3(acilmaMiktari, 0, 0); // X ekseninde yana
        }
        else
        {
            hedefPozisyon = transform.position + new Vector3(0, acilmaMiktari, 0); // Y ekseninde yukarı
        }
    }

    void Update()
    {
        if (aciliyor)
        {
            // Kapıyı hedefe doğru yumuşak bir şekilde hareket ettir
            transform.position = Vector3.Lerp(transform.position, hedefPozisyon, Time.deltaTime * acilmaHizi);

            // Hedefe çok yaklaştığında hareketi durdur (Performans tasarrufu)
            if (Vector3.Distance(transform.position, hedefPozisyon) < 0.05f)
            {
                transform.position = hedefPozisyon;
                aciliyor = false;
            }
        }
    }

    public void OpenDoor()
    {
        if (!aciliyor)
        {
            aciliyor = true;
            Debug.Log("Şifre doğru, kapı açılıyor!");
            
            // Panik Modunu Durdur
            if (PanicManager.Instance != null)
            {
                PanicManager.Instance.StopPanicMode();
            }

            // Yeni sahneyi yükleme sürecini başlat
            StartCoroutine(LoadNextSceneRoutine());
        }
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        yield return new WaitForSeconds(sceneTransitionDelay);

        if (nextSceneName == "TrueEnding")
        {
            if (GameEndManager.Instance != null)
                GameEndManager.Instance.ShowTrueEndingChoice();
            else
                Debug.LogWarning("GameEndManager bulunamadı! Lütfen Tools menüsünden Final Ekranını kurun.");
        }
        else if (nextSceneName == "BetrayalEnding")
        {
            if (GameEndManager.Instance != null)
                GameEndManager.Instance.ShowBetrayalEnding();
            else
                Debug.LogWarning("GameEndManager bulunamadı! Lütfen Tools menüsünden Final Ekranını kurun.");
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("Yeni sahne yükleniyor: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Geçilecek yeni sahne adı girilmemiş!");
        }
    }
}
