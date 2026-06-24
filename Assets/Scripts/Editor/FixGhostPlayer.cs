using UnityEngine;
using UnityEditor;

public class FixGhostPlayer
{
    [MenuItem("Tools/Karakteri Bedeniyle Birlestir")]
    public static void Fix()
    {
        GameObject player = GameObject.FindObjectOfType<StarterAssets.FirstPersonController>()?.gameObject;
        if (player == null)
        {
            Debug.LogError("PlayerCapsule bulunamadi!");
            return;
        }
        
        // 1. Kamera pozisyonunu sifirla (Ruh gibi disaridan gorme sorunu)
        Camera cam = player.GetComponentInChildren<Camera>();
        if (cam != null)
        {
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localRotation = Quaternion.identity;
        }

        Transform camRoot = player.transform.Find("PlayerCameraRoot");
        if (camRoot != null)
        {
            camRoot.localPosition = new Vector3(0, 1.375f, 0);
        }

        // 2. Kendi vucudunu gizle (iceriden kapsulu gormesin)
        MeshRenderer[] mrs = player.GetComponentsInChildren<MeshRenderer>();
        foreach (var mr in mrs)
        {
            // Sadece karakterin kendi ana mesh'ini kapat, elindeki fener falan varsa elleme
            if (mr.gameObject.name == "Capsule" || mr.gameObject.name == "PlayerCapsule")
                mr.enabled = false;
        }

        // 3. Ziplayamama sorunu (Yere degme algilayicisi)
        StarterAssets.FirstPersonController fpc = player.GetComponent<StarterAssets.FirstPersonController>();
        if (fpc != null)
        {
            fpc.GroundedRadius = 0.5f;
            fpc.GroundedOffset = -0.14f;
            fpc.GroundLayers = ~0; // Her seyi zemin kabul et (ziplamayi garantiye alir)
        }

        // 4. Character Controller ayarini standart insan boyutuna getir
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.center = new Vector3(0, 1f, 0);
            cc.radius = 0.5f;
            cc.height = 2f;
        }

        EditorUtility.SetDirty(player);
        Debug.Log("ISLEM TAMAM! Ruh bedene dondu, kamera karakterin gozune oturtuldu ve ziplama sistemi %100 acildi!");
    }
}
