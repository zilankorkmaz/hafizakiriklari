using UnityEngine;
using UnityEditor;

public class FixEverything
{
    [MenuItem("Tools/Son Duzeltme (Merkeze Isinla)")]
    public static void Fix()
    {
        GameObject player = GameObject.FindObjectOfType<StarterAssets.FirstPersonController>()?.gameObject;
        if (player == null) return;
        
        // 1. Karakteri duvarin/masanin icine sikismayacagi Odanin TAM MERKEZINE isinla
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = new Vector3(14f, 2f, 0f); // Odanin genis bir noktasi
        if (cc != null) cc.enabled = true;

        // 2. Kameranin onunu kapatan siyah cismi (kendi vucudunu) kameradan tamamen sil
        player.layer = 2; // Ignore Raycast
        Transform[] allChildren = player.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allChildren) t.gameObject.layer = 2; // Tum parcalari layer 2 yap
        
        Transform camRoot = player.transform.Find("PlayerCameraRoot");
        if (camRoot != null)
        {
            Camera cam = camRoot.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                // Kamera Layer 2'yi (kendini) gormesin
                cam.cullingMask &= ~(1 << 2);
            }
        }

        // Fazladan olusmus PlayerCameraRoot varsa sil
        if (camRoot != null)
        {
            Camera cam = camRoot.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                Transform extraRoot = cam.transform.Find("PlayerCameraRoot");
                if (extraRoot != null) Object.DestroyImmediate(extraRoot.gameObject);
            }
        }

        EditorUtility.SetDirty(player);
        Debug.Log("ISLEM TAMAM! Odanin merkezine isinlandiniz, kamerayi kapatan siyahlik silindi!");
    }
}
