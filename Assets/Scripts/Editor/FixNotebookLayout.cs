using UnityEngine;
using UnityEditor;

public class FixNotebookLayout
{
    [MenuItem("Tools/Defter Ekranini Duzelt")]
    public static void Fix()
    {
        NotebookManager mgr = Object.FindFirstObjectByType<NotebookManager>();
        if (mgr == null)
        {
            Debug.LogError("Sahne icinde NotebookManager bulunamadi!");
            return;
        }

        if (mgr.bookPanel == null)
        {
            Debug.LogError("NotebookManager icinde BookPanel atanmamis!");
            return;
        }
        
        RectTransform rt = mgr.bookPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            // Tam ekran yayilmasi icin ayarlari sifirla
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.anchoredPosition = Vector2.zero;
            
            Debug.Log("HARIKA! Defter boyutu ekrana sigacak sekilde duzeltildi!");
            EditorUtility.SetDirty(mgr.bookPanel);
        }
    }
}
