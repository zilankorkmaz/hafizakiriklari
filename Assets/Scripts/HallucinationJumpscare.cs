using UnityEngine;
using UnityEngine.UI;

public class HallucinationJumpscare : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Bu objeyi çizecek olan ana kamera (Main Camera)")]
    public Camera mainCamera; 
    
    [Header("Slender Man Efektleri")]
    [Tooltip("Adamın üstüne ekleyeceğiniz çınlama veya parazit sesi")]
    public AudioSource staticSound;
    [Tooltip("Maksimum etkiye ulaşması için kaç saniye bakmalı?")]
    public float timeToMaxEffect = 2.5f;

    private bool hasAppeared = false;
    private bool wasSeen = false;
    private Renderer objRenderer;
    private float lookTimer = 0f;

    void Start()
    {
        objRenderer = GetComponentInChildren<Renderer>();
        if (objRenderer != null) objRenderer.enabled = false;
    }

    void Update()
    {
        if (!PowerSwitch.isPowerOn) return;

        // Şalter açıldı, adam belirdi
        if (!hasAppeared)
        {
            if (objRenderer != null) objRenderer.enabled = true;
            hasAppeared = true;
            
            if (staticSound != null)
            {
                staticSound.volume = 0f;
                staticSound.loop = true;
                staticSound.Play();
            }
        }

        if (hasAppeared && mainCamera != null && objRenderer != null)
        {
            bool isLookingAtMe = IsVisibleFrom(objRenderer, mainCamera);

            if (isLookingAtMe)
            {
                wasSeen = true;
                lookTimer += Time.deltaTime;
                
                // Ne kadar uzun süre bakarsa etki o kadar artar (0'dan 1'e kadar)
                float effectIntensity = Mathf.Clamp01(lookTimer / timeToMaxEffect);

                // Sesi giderek yükselt ve yavaşlat (ürpertici etki)
                if (staticSound != null)
                {
                    staticSound.volume = effectIntensity;
                    staticSound.pitch = 1f - (effectIntensity * 0.4f);
                }

                // Ekranı GameEndingManager üzerinden yavaşça kırmızı-siyaha boya
                if (GameEndingManager.Instance != null && GameEndingManager.Instance.fadePanelImage != null)
                {
                    GameEndingManager.Instance.fadePanel.gameObject.SetActive(true);
                    GameEndingManager.Instance.fadePanel.alpha = effectIntensity * 0.85f; // %85'e kadar kararır
                    GameEndingManager.Instance.fadePanelImage.color = Color.Lerp(Color.black, new Color(0.5f, 0f, 0f), effectIntensity);
                }
            }
            else if (wasSeen && !isLookingAtMe)
            {
                // Oyuncu kafasını çevirdi! Etkileri sıfırla ve yok ol
                if (staticSound != null) staticSound.Stop();
                
                if (GameEndingManager.Instance != null && GameEndingManager.Instance.fadePanel != null)
                {
                    GameEndingManager.Instance.fadePanel.alpha = 0f;
                    GameEndingManager.Instance.fadePanel.gameObject.SetActive(false);
                }

                Debug.Log("Oyuncu kafasını çevirdi, Slender-Halüsinasyon yok oldu!");
                Destroy(gameObject);
            }
        }
    }

    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
