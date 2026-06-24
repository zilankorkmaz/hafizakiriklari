using UnityEngine;

public class Level2EventDirector : MonoBehaviour
{
    private void Start()
    {
        // Sahnede LoopManager ve diğer objelerin tam olarak yüklenmesi için azıcık bekliyoruz.
        Invoke(nameof(SetupEvents), 0.1f);
    }

    private void SetupEvents()
    {
        // Seslerin kapalı kalma ihtimaline karşı her ihtimale karşı aç:
        AudioListener.pause = false;

        // Sahnede çalışan AudioListener var mı kontrol et, yoksa Main Camera'ya ekle
        AudioListener listener = FindAnyObjectByType<AudioListener>();
        if (listener == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null) mainCam.gameObject.AddComponent<AudioListener>();
            else gameObject.AddComponent<AudioListener>();
        }

        // Sahnede kaydedilmiş eski '3' değerini ezip 4'e zorluyoruz
        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.maxLoops = 4;
        }


        // --- 2. KORİDOR IŞIKLARI ---
        Light[] allLights = FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int lightCounter = 0;
        foreach (Light l in allLights)
        {
            // Orijinal ışıkları bul
            if (l.type == LightType.Point && l.gameObject.name != "RedAlarmLight")
            {
                lightCounter++;
                if (l.gameObject.GetComponent<LoopEvent>() == null)
                {
                    // Mevcut ışık sadece Loop 1 ve 2'de aktif olsun (Loop 2'de yarısını kapatarak loş yapalım)
                    LoopEvent lightEvt = l.gameObject.AddComponent<LoopEvent>();
                    if (lightCounter % 2 == 0)
                        lightEvt.activeInLoops = new int[] { 1 };
                    else
                        lightEvt.activeInLoops = new int[] { 1, 2 };
                    lightEvt.keepActiveWhenBroken = false;

                    // --- 3. KIRMIZI IŞIKLAR ---
                    // Kötü duran tek bir "Directional" ışık yerine, mevcut koridor ışıklarının her birini Loop 3 ve 4 için KIRMIZI kopyalara dönüştürüyoruz
                    GameObject redCopy = Instantiate(l.gameObject, l.transform.parent);
                    redCopy.name = "RedAlarmLight";
                    Light redLight = redCopy.GetComponent<Light>();
                    redLight.color = Color.red;
                    
                    // Kopyalanan LoopEvent'i güncelle
                    LoopEvent redEvt = redCopy.GetComponent<LoopEvent>();
                    redEvt.activeInLoops = new int[] { 3, 4 };
                    redEvt.keepActiveWhenBroken = true;
                }
            }
        }

        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.OnLoopBroken.AddListener(OpenFinalDoor);
            LoopManager.Instance.OnLoopChanged?.Invoke();
        }
    }

    private void OpenFinalDoor()
    {
        // Döngü kırıldığında oyuncunun Level 3'e geçebilmesi için kapının fiziksel engelini kaldır
        GameObject finalDoor = GameObject.Find("Future_Door_Final");
        
        // Eğer tam isimle bulunamadıysa, isminde "Future_Door_Final" geçen bir objeyi ara
        if (finalDoor == null)
        {
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (t.name.Contains("Future_Door_Final") && t.gameObject.scene.isLoaded)
                {
                    finalDoor = t.gameObject;
                    break;
                }
            }
        }

        if (finalDoor != null)
        {
            // Kapının kanatlarını (alt objelerini) kapatarak geçişi aç
            foreach (Transform child in finalDoor.transform)
            {
                child.gameObject.SetActive(false);
            }
            
            // Ana objedeki engelleyici fiziksel collider'ları kapat (Trigger olanı açık bırakıyoruz ki sahne yüklensin)
            Collider[] cols = finalDoor.GetComponents<Collider>();
            foreach(var c in cols)
            {
                if (!c.isTrigger) c.enabled = false;
            }

            // GARANTİ GEÇİŞ: Kapının kendisine LevelTransition ve dev bir Trigger ekleyelim. 
            // Oyuncu içinden geçerken kesinlikle sahne değişsin.
            LevelTransition lt = finalDoor.GetComponent<LevelTransition>();
            if (lt == null)
            {
                lt = finalDoor.AddComponent<LevelTransition>();
            }
            lt.nextSceneName = "Level_3";

            BoxCollider triggerBox = finalDoor.AddComponent<BoxCollider>();
            triggerBox.isTrigger = true;
            triggerBox.size = new Vector3(5f, 5f, 5f); // Geniş bir alan
        }
        else
        {
            Debug.LogWarning("Level2EventDirector: Future_Door_Final kapısı sahnede bulunamadı! Sahne geçişi zorla yapılacak.");
        }

        // FİZİK MOTORU HATALARINA KARŞI KESİN ÇÖZÜM:
        // Kapı açıldıktan 1.5 saniye sonra hala geçiş yapılmadıysa zorla Level_3'e at
        // Kapı bulunsun veya bulunmasın, döngü kırıldıktan sonra bu coroutine çalışmalı!
        StartCoroutine(ForceLoadLevel3());
    }

    private System.Collections.IEnumerator ForceLoadLevel3()
    {
        yield return new WaitForSeconds(1.5f);
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Level_2")
        {
            Debug.Log("Fizik motoru kapı geçişini algılayamadı, zorla Level_3 yükleniyor!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level_3");
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Level_2 sahnesi her yüklendiğinde otomatik olarak çalışmasını sağla
        if (scene.name == "Level_2")
        {
            new GameObject("Level2EventDirector").AddComponent<Level2EventDirector>();
        }
    }
}
