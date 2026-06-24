using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CopyNotebookUI
{
    [MenuItem("Tools/1 TIKLA DEFTERI GETIR")]
    public static void CopyNotebook()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        EditorSceneManager.SaveOpenScenes();

        // 1. Level_3 teki bozuk/eksik NotebookManager/NotebookPanel'leri temizle
        NotebookManager oldMgr = Object.FindFirstObjectByType<NotebookManager>();
        if (oldMgr != null) Object.DestroyImmediate(oldMgr.gameObject);

        // Eger bos panel kalmissa onu da sil
        Canvas activeCanvas = Object.FindFirstObjectByType<Canvas>();
        if (activeCanvas != null)
        {
            Transform oldPanel = activeCanvas.transform.Find("NotebookPanel");
            if (oldPanel != null) Object.DestroyImmediate(oldPanel.gameObject);
        }

        // 2. lab_final sahnesini gecici olarak arka planda ac
        Scene labScene = EditorSceneManager.OpenScene("Assets/Scenes/lab_final.unity", OpenSceneMode.Additive);

        GameObject sourceNotebookPanel = null;
        GameObject sourceNotebookManager = null;

        // Root objelerde tum hiyerarsiyi tara
        foreach (GameObject go in labScene.GetRootGameObjects())
        {
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.name == "NotebookPanel") sourceNotebookPanel = t.gameObject;
                if (t.name == "NotebookManager") sourceNotebookManager = t.gameObject;
            }
        }

        if (sourceNotebookPanel == null)
        {
            Debug.LogError("lab_final sahnesinde NotebookPanel bulunamadi! Sahne adini kontrol edin.");
            EditorSceneManager.CloseScene(labScene, true);
            return;
        }

        Scene activeScene = SceneManager.GetSceneByPath(currentScenePath);

        // Manager'i klonla
        GameObject newManager = null;
        if (sourceNotebookManager != null)
        {
            newManager = GameObject.Instantiate(sourceNotebookManager);
            newManager.name = "NotebookManager";
            SceneManager.MoveGameObjectToScene(newManager, activeScene);
        }
        else
        {
            newManager = new GameObject("NotebookManager");
            newManager.AddComponent<NotebookManager>();
            SceneManager.MoveGameObjectToScene(newManager, activeScene);
        }

        // Panel'i klonla
        GameObject newPanel = GameObject.Instantiate(sourceNotebookPanel);
        newPanel.name = "NotebookPanel";
        SceneManager.MoveGameObjectToScene(newPanel, activeScene);

        // lab_final'i geri kapat
        EditorSceneManager.CloseScene(labScene, true);

        // Panel'i aktif sahnenin Canvas'ina bagla
        if (activeCanvas != null)
        {
            newPanel.transform.SetParent(activeCanvas.transform, false);
        }

        // Kod atamalarini otomatik yap
        NotebookManager mgr = newManager.GetComponent<NotebookManager>();
        mgr.bookPanel = newPanel;
        
        // Sadece PageText'i bul (Tek sayfa duzeni icin)
        TMPro.TextMeshProUGUI pageText = null;
        foreach (var t in newPanel.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
        {
            if (t.name == "PageText") 
            {
                pageText = t;
                break;
            }
        }

        mgr.leftPageText = pageText;
        mgr.rightPageText = null; // Sag taraf bilerek bos

        // Ok butonlarini otomatik bagla
        UnityEngine.UI.Button[] buttons = newPanel.GetComponentsInChildren<UnityEngine.UI.Button>(true);
        foreach (var b in buttons)
        {
            if (b.name.ToLower().Contains("next")) mgr.nextButton = b;
            if (b.name.ToLower().Contains("prev")) mgr.prevButton = b;
        }

        // Gorunmez mikroskobik kutu sorununu coz (Tam Ekrana Yansit)
        RectTransform rt = newPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;
        }
        
        newPanel.SetActive(false);
        EditorUtility.SetDirty(mgr);

        Debug.Log("ISLEM TAMAM! NotebookPanel 1. sahneden koparildi, Canvas'a kusursuzca yerlestirildi ve tüm atamalar otomatik yapildi!");
    }
}
