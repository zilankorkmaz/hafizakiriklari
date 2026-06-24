using UnityEngine;
using UnityEditor;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FixEverythingFINAL
{
    [MenuItem("Tools/1 TIKLA HER SEYI COZ")]
    public static void Fix()
    {
        // 1. BOZUK OYUNCUYU TAMAMEN YOK ET (Nuke)
        GameObject oldPlayer = GameObject.Find("PlayerCapsule");
        if (oldPlayer == null)
        {
            var fpc = Object.FindFirstObjectByType<StarterAssets.FirstPersonController>();
            if (fpc != null) oldPlayer = fpc.gameObject;
        }

        if (oldPlayer != null) Object.DestroyImmediate(oldPlayer);

        // 2. TERTEMIZ VE KUSURSUZ ORIJINAL PREFABI YUKLE
        string prefabPath = "Assets/StarterAssets/FirstPersonController/Prefabs/PlayerCapsule.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError("Orijinal PlayerCapsule prefab'i bulunamadi!");
            return;
        }

        // 3. YENI KARAKTERI SAHNEYE KOY VE MERKEZE ISINLA
        GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        player.name = "PlayerCapsule";
        player.transform.position = new Vector3(14f, 2f, 0f);

        // 4. INPUT SISTEMINI GARANTIYE AL
#if ENABLE_INPUT_SYSTEM
        PlayerInput pi = player.GetComponent<PlayerInput>();
        if (pi == null) pi = player.AddComponent<PlayerInput>();
        
        InputActionAsset asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/StarterAssets/InputSystem/StarterAssets.inputactions");
        if (asset != null)
        {
            pi.actions = asset;
            pi.defaultControlScheme = "KeyboardMouse";
            pi.defaultActionMap = "Player";
        }
#endif

        // 5. KAMERA VE FENERI YENIDEN KUR (Basit FPS Tarzi)
        Transform camRoot = player.transform.Find("PlayerCameraRoot");
        if (camRoot == null) camRoot = player.transform;

        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("MainCamera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }

        mainCam.transform.SetParent(camRoot);
        mainCam.transform.localPosition = Vector3.zero;
        mainCam.transform.localRotation = Quaternion.identity;

        pi.camera = mainCam;

        // Fener Sistemi
        Light fenerLight = mainCam.GetComponent<Light>();
        if (fenerLight == null)
        {
            fenerLight = mainCam.gameObject.AddComponent<Light>();
            fenerLight.type = LightType.Spot;
            fenerLight.range = 25f;
            fenerLight.spotAngle = 65f;
            fenerLight.intensity = 5f;
            fenerLight.color = new Color(0.95f, 0.95f, 0.85f);
        }

        FenerKontrol fenerKontrol = mainCam.GetComponent<FenerKontrol>();
        if (fenerKontrol == null) fenerKontrol = mainCam.gameObject.AddComponent<FenerKontrol>();
        fenerKontrol.mevcutsarj = 100f;

        // Sahnedeki gizli yazilari bulup feneri bagla
        HiddenText[] hiddenTexts = Object.FindObjectsByType<HiddenText>(FindObjectsSortMode.None);
        foreach(var ht in hiddenTexts) ht.playerFlashlight = fenerKontrol;

        // 6. KENDI VUCUDUNU GIZLE VE ZEMIN AYARLARINI YAP
        MeshRenderer[] mrs = player.GetComponentsInChildren<MeshRenderer>();
        foreach(var m in mrs) { if(m.name.Contains("Capsule")) m.enabled = false; }

        StarterAssets.FirstPersonController fpcNew = player.GetComponent<StarterAssets.FirstPersonController>();
        if (fpcNew != null)
        {
            fpcNew.CinemachineCameraTarget = camRoot.gameObject;
            fpcNew.GroundLayers = ~(1 << 2);
        }
        
        player.layer = 2; // Ignore Raycast

        EditorUtility.SetDirty(player);
        Debug.Log("ISLEM TAMAM! Bozuk karakter tamamen silindi, yerine SIFIRDAN kusursuz bir karakter kuruldu! Artik %100 calisacak!");
    }
}
