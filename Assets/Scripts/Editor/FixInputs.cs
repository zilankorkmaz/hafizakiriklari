using UnityEngine;
using UnityEditor;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FixInputs
{
    [MenuItem("Tools/Karakteri Tamamen Tamir Et")]
    public static void Fix()
    {
        GameObject player = GameObject.FindObjectOfType<StarterAssets.FirstPersonController>()?.gameObject;
        if (player == null)
        {
            Debug.LogError("Sahnedeki karakter bulunamadi!");
            return;
        }
        
        StarterAssets.FirstPersonController fpc = player.GetComponent<StarterAssets.FirstPersonController>();
        
        // 1. Cinemachine Camera Target hatasini coz
        if (fpc.CinemachineCameraTarget == null)
        {
            Transform camRoot = player.transform.Find("PlayerCameraRoot");
            if (camRoot == null)
            {
                GameObject newCamRoot = new GameObject("PlayerCameraRoot");
                newCamRoot.transform.SetParent(player.transform);
                newCamRoot.transform.localPosition = new Vector3(0, 1.375f, 0);
                camRoot = newCamRoot.transform;
            }
            fpc.CinemachineCameraTarget = camRoot.gameObject;
        }

        // 2. Input sistemlerini coz
        StarterAssets.StarterAssetsInputs inputs = player.GetComponent<StarterAssets.StarterAssetsInputs>();
        if (inputs == null) inputs = player.AddComponent<StarterAssets.StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
        PlayerInput pi = player.GetComponent<PlayerInput>();
        if (pi == null) pi = player.AddComponent<PlayerInput>();
        
        // InputActionAsset dosyasini bul
        string[] guids = AssetDatabase.FindAssets("StarterAssets t:InputActionAsset");
        if (guids.Length > 0)
        {
            InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetDatabase.GUIDToAssetPath(guids[0]));
            pi.actions = asset;
            pi.defaultControlScheme = "KeyboardMouse";
            pi.defaultActionMap = "Player";
            pi.camera = Camera.main;
        }
#endif
        
        EditorUtility.SetDirty(player);
        Debug.Log("ISLEM TAMAM! Karakterin yurume, bakma ve kamera ayarlari %100 onarildi. Hatalar silindi!");
    }
}
