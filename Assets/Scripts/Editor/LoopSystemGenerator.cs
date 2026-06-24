using UnityEngine;
using UnityEditor;

public class LoopSystemGenerator : EditorWindow
{
    [MenuItem("Tools/Sonsuz Döngü Sistemini Kur (P.T. Tarzı)")]
    public static void GenerateLoopSystem()
    {
        if (FindAnyObjectByType<LoopManager>() != null)
        {
            Debug.LogWarning("Sahnede zaten bir LoopManager var. Lütfen önce onu silin.");
            return;
        }

        // Ana Klasör Objesi
        GameObject loopSystem = new GameObject("LoopSystem_Level2");

        // Yönetici Obje
        GameObject managerGo = new GameObject("LoopManager");
        managerGo.transform.SetParent(loopSystem.transform);
        managerGo.AddComponent<LoopManager>();

        // Başlangıç Noktası (Işınlanılacak Hedef Nokta)
        GameObject startPoint = new GameObject("Loop_StartPoint_Baslangic");
        startPoint.transform.SetParent(loopSystem.transform);
        startPoint.transform.position = new Vector3(0, 0, 0);

        // Bitiş Noktası (Işınlayıcı Kapı/Duvar Trigger'ı)
        GameObject endPoint = new GameObject("Loop_EndPoint_KapiTrigger");
        endPoint.transform.SetParent(loopSystem.transform);
        endPoint.transform.position = new Vector3(0, 0, 20); // Örnek olarak 20 birim ileriye koyduk

        BoxCollider col = endPoint.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(4, 4, 1); // Koridoru kapatacak büyüklükte

        LoopTeleporter teleporter = endPoint.AddComponent<LoopTeleporter>();
        teleporter.teleportTarget = startPoint.transform;

        Undo.RegisterCreatedObjectUndo(loopSystem, "Create Loop System");
        Selection.activeGameObject = loopSystem;

        Debug.Log("P.T. tarzı döngü sistemi kuruldu! 'LoopSystem_Level2' objesi Hierarchy'de");
    }
}
