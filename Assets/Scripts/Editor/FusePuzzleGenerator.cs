using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FusePuzzleGenerator : EditorWindow
{
    [MenuItem("Tools/Sigorta Bulmacasını Sahnede Oluştur")]
    public static void GeneratePuzzle()
    {
        // Ana Kök Obje
        GameObject puzzleRoot = new GameObject("SigortaBulmacasi");

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // UI Canvas'ı
        GameObject canvasGo = new GameObject("SigortaUI", typeof(RectTransform));
        canvasGo.transform.SetParent(puzzleRoot.transform, false);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // Etkileşim Yazısı (Ortada)
        GameObject txtGo = new GameObject("InteractText", typeof(RectTransform));
        txtGo.transform.SetParent(canvasGo.transform, false);
        RectTransform txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = new Vector2(0.3f, 0.4f);
        txtRt.anchorMax = new Vector2(0.7f, 0.6f);
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        Text uiText = txtGo.AddComponent<Text>();
        uiText.font = font;
        uiText.fontSize = 40;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = Color.white;
        
        // Outline for readability
        Outline outline = txtGo.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);
        
        txtGo.SetActive(false);

        // 1. Şalter (Power Switch)
        GameObject switchGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        switchGo.name = "Ana_Salter";
        switchGo.transform.SetParent(puzzleRoot.transform);
        switchGo.transform.position = new Vector3(0, 1.5f, 0);
        switchGo.transform.localScale = new Vector3(0.5f, 0.8f, 0.2f);
        switchGo.GetComponent<Renderer>().material.color = Color.gray;
        
        BoxCollider switchCol = switchGo.GetComponent<BoxCollider>();
        switchCol.isTrigger = true;
        switchCol.size = new Vector3(3f, 3f, 3f); // Geniş etkileşim alanı

        PowerSwitch pSwitch = switchGo.AddComponent<PowerSwitch>();
        pSwitch.interactText = txtGo;
        pSwitch.requiredFuses = 3;

        // 2. Sigortalar
        for (int i = 1; i <= 3; i++)
        {
            GameObject fuseGo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fuseGo.name = "Kayıp_Sigorta_" + i;
            fuseGo.transform.SetParent(puzzleRoot.transform);
            fuseGo.transform.position = new Vector3(i * 2, 1, 0);
            fuseGo.transform.localScale = new Vector3(0.2f, 0.4f, 0.2f);
            fuseGo.GetComponent<Renderer>().material.color = Color.cyan;

            Collider fuseCol = fuseGo.GetComponent<Collider>();
            DestroyImmediate(fuseCol);
            
            BoxCollider newCol = fuseGo.AddComponent<BoxCollider>();
            newCol.isTrigger = true;
            newCol.size = new Vector3(8f, 8f, 8f); // Kutu boyutunu artırarak etkileşim alanını büyütüyoruz, objenin kendisi büyümüyor.

            FuseItem fuseItem = fuseGo.AddComponent<FuseItem>();
            fuseItem.interactText = txtGo;
        }

        Undo.RegisterCreatedObjectUndo(puzzleRoot, "Create Fuse Puzzle");
        Selection.activeGameObject = puzzleRoot;

        Debug.Log("Sigorta Bulmacası sahneye eklendi! 'SigortaBulmacasi' objesinin içinden şalter ve sigortaları bulup yerleştirebilirsiniz.");
    }
}
