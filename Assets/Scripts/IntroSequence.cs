using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class IntroSequence : MonoBehaviour
{
    public GameObject introPanel;
    public TextMeshProUGUI introText;

    public float introDuration = 2f;
    public float fadeDuration = 2f;
    public float glitchDuration = 0.8f;

    private CanvasGroup canvasGroup;
    private Vector3 originalTextPos;
    private string originalText;
    void Start()
    {
        canvasGroup=introPanel.GetComponent<CanvasGroup>();
        originalTextPos = introText.rectTransform.localPosition;
        originalText=introText.text;

        StartCoroutine(PlayIntro());
    }
    IEnumerator PlayIntro()
    {
        introPanel.SetActive(true);
        canvasGroup.alpha = 1.0f;

        yield return StartCoroutine(GlitchEffect());

        yield return new WaitForSeconds(introDuration);
        float t = 0;
        while (t < fadeDuration)
        {
            t+=Time.deltaTime;
            canvasGroup.alpha = 1-(t/fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        introPanel.SetActive(false);
    }
    IEnumerator GlitchEffect()
    {
        float timer = 0f;
        while (timer < glitchDuration)
        {
            timer += 0.06f;
            float offsetX = Random.Range(-4f, 4f);
            float offsetY = Random.Range(-4f, 4f);

            introText.rectTransform.localPosition = new Vector3(offsetX, offsetY, 0f);
            if (Random.value > 0.5f)
                introText.text = originalText;
            yield return new WaitForSeconds(0.06f);
        }
        introText.rectTransform.localPosition = originalTextPos;
        introText.text = originalText;
    }
    string GlitchText(string input)
    {
        char[] chars = input.ToCharArray();
        string glitchChars = "#@$%&!?";

        int changes=Random.Range(1,2);
        for (int i = 0; i < changes; i++) {
            int index = Random.Range(0, chars.Length);
            if (chars[index] != ' ')
                chars[index] = glitchChars[Random.Range(0, glitchChars.Length)];
        }
        return new string(chars);
    }

    void Update()
    {
        
    }
}
