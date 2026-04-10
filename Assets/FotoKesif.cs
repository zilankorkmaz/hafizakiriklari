using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class FotoKesif : MonoBehaviour
{
    [Header("UI Objeleri")]
    public GameObject glitchUI;
    public GameObject karartmaUI;
    public TMPro.TextMeshProUGUI etkilesimYazisi; //E
    public TMPro.TextMeshProUGUI  altyaziText;

    [Header("Efekt ayarları")]
    public float glitchSuresi = 0.4f;
    public float incelemeSuresi = 2f;
    public float kararmaHizi = 1f;

    private bool alandaMi = false;
    private bool fotoIncelendi = false;
    private bool sinematikBasladi = false;


    void Update()
    {
        if (alandaMi && !fotoIncelendi && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(HafizaKirigiSekansi());
    }
    IEnumerator HafizaKirigiSekansi()
    {
        sinematikBasladi = true;
        if (etkilesimYazisi != null) etkilesimYazisi.gameObject.SetActive(false);

        glitchUI.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        glitchUI.SetActive(false);

        Debug.Log("Hafıza kırığı bulundu. Not okunuyor.");
        karartmaUI.SetActive(true);
        Image img=karartmaUI.GetComponent<Image>();
        float timer = 0f;
        while (timer < 1.1f)
        {
            img.color=new Color(0,0,0);
            timer += Time.deltaTime * 0.4f;
            yield return null;
        }

        altyaziText.gameObject.SetActive(true);
        altyaziText.text = "";

        altyaziText.text = "Tarih 15.03.2026...";
        yield return new WaitForSeconds(2f);
        altyaziText.text = "Notu yazan haklıymış.\nHiçbir şey hatırlayamıyorum.";
        yield return new WaitForSeconds(2f);
        altyaziText.text = "Sadece 1 haftada ne olmuş olabilir ki?";
        yield return new WaitForSeconds(2f);

        fotoIncelendi = true;
        Debug.Log("Hafıza yeniden yükleniyor. Sahne 1 bitti.");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !fotoIncelendi)
        {
            alandaMi=true;
            if (etkilesimYazisi != null)
            {
                etkilesimYazisi.text = "E ile fotografi incele";
                etkilesimYazisi.gameObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            alandaMi = false;
            if (etkilesimYazisi != null)
            {
                etkilesimYazisi.text = "";
                etkilesimYazisi.gameObject.SetActive(false);
            }
        }
    }
}
