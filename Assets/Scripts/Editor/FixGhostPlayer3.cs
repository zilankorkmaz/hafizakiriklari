using UnityEngine;
using UnityEditor;

public class FixGhostPlayer3
{
    [MenuItem("Tools/Karakteri Ayaga Kaldir")]
    public static void Fix()
    {
        GameObject player = GameObject.FindObjectOfType<StarterAssets.FirstPersonController>()?.gameObject;
        if (player == null) return;
        
        // Karakterin rotasyonunu sifirla (yanlislikla yana yikilmis)
        player.transform.rotation = Quaternion.identity;
        
        // Kameranin rotasyonunu tamamen duzelt
        Transform camRoot = player.transform.Find("PlayerCameraRoot");
        if (camRoot != null)
        {
            camRoot.localRotation = Quaternion.identity;
            
            Camera cam = camRoot.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.transform.localRotation = Quaternion.identity;
            }
        }

        // Karakterin fiziksel kutusu havada kaldiysa biraz yukari alalim ki yere tam otursun
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = new Vector3(21f, 1.5f, -9f);
        if (cc != null) cc.enabled = true;

        EditorUtility.SetDirty(player);
        Debug.Log("ISLEM TAMAM! Karakter yerden kaldirildi, tamamen dik duruma getirildi!");
    }
}
