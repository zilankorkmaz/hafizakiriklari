using UnityEngine;
using TMPro;

public class HiddenText : MonoBehaviour
{
    [Header("Gereksinimler")]
    public FenerKontrol playerFlashlight; // Oyuncunun fener scripti

    [Header("Görünürlük Ayarları")]
    public float revealDistance = 15f; // Ne kadar yakından görünür
    [Range(0f, 90f)]
    public float revealAngle = 45f; // Fener tam üstüne mi bakmalı? (Açı)
    public float fadeSpeed = 3f; // Silinme/Belirme hızı

    private TextMeshProUGUI uiText;
    private TextMeshPro textMesh; // 3D Dünya objesi olarak eklenirse diye

    void Start()
    {
        uiText = GetComponent<TextMeshProUGUI>();
        textMesh = GetComponent<TextMeshPro>();

        // Başlangıçta görünmez yap
        SetAlpha(0f);
    }

    void Update()
    {
        if (playerFlashlight == null) 
        {
            Debug.LogWarning("HiddenText: Player Flashlight atanmamış!");
            return;
        }

        bool shouldBeVisible = false;

        if (playerFlashlight.IsFenerAcik)
        {
            Light fenerLight = playerFlashlight.FenerLight;
            if (fenerLight != null)
            {
                // Fener ile yazı arasındaki mesafe
                float distance = Vector3.Distance(transform.position, fenerLight.transform.position);
                
                if (distance <= revealDistance)
                {
                    // Fenerin baktığı yön ile yazıya olan yön arasındaki açı
                    Vector3 dirToText = (transform.position - fenerLight.transform.position).normalized;
                    float angle = Vector3.Angle(fenerLight.transform.forward, dirToText);

                    // Fener tam yazıya tutuluyorsa
                    if (angle <= revealAngle)
                    {
                        shouldBeVisible = true;
                    }
                }
            }
        }

        // Yumuşak geçiş (Fade in / Fade out)
        float currentAlpha = GetAlpha();
        float targetAlpha = shouldBeVisible ? 1f : 0f;

        if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
        {
            SetAlpha(Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime));
        }
    }

    private float GetAlpha()
    {
        if (uiText != null) return uiText.color.a;
        if (textMesh != null) return textMesh.color.a;
        return 0f;
    }

    private void SetAlpha(float alpha)
    {
        if (uiText != null)
        {
            Color c = uiText.color;
            c.a = alpha;
            uiText.color = c;
        }
        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a = alpha;
            textMesh.color = c;
        }
    }
}
