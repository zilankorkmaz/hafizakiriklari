#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GameplayAudioMenuEditor
{
    const string MenuPath = "Tools/Hafiza/Sahneye Ses ve Menu Ekle";

    [MenuItem(MenuPath)]
    static void AddToActiveScene()
    {
        if (UnityEngine.Object.FindFirstObjectByType<GameplayAudioAndMenu>() != null)
        {
            EditorUtility.DisplayDialog("Hafiza", "Sahnede zaten GameplayAudioAndMenu var.", "Tamam");
            return;
        }

        var go = new GameObject("GameplayAudioAndMenu");
        go.AddComponent<GameplayAudioAndMenu>();
        Undo.RegisterCreatedObjectUndo(go, "Add GameplayAudioAndMenu");
        Selection.activeGameObject = go;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
#endif
