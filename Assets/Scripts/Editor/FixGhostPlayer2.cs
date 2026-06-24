using UnityEngine;
using UnityEditor;

public class FixGhostPlayer2
{
    [MenuItem("Tools/Karakteri Yere Indir")]
    public static void Fix()
    {
        GameObject player = GameObject.FindObjectOfType<StarterAssets.FirstPersonController>()?.gameObject;
        if (player == null) return;
        
        // 1. Zemin algilayiciyi kendi bedeniyle karistirmamasi icin katmani ayarla
        player.layer = 2; // Ignore Raycast
        
        StarterAssets.FirstPersonController fpc = player.GetComponent<StarterAssets.FirstPersonController>();
        if (fpc != null)
        {
            fpc.GroundLayers = ~(1 << 2); // Ignore Raycast haric her sey zemin
        }

        // 2. Karakteri masanin oldugu konuma (yere) isinla
        // Eger ucarak havada (tavanda) dogduysa asagi insin
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false; // Isinlama icin cc gecici kapanmali

        player.transform.position = new Vector3(21f, 1.5f, -9f);

        if (cc != null) cc.enabled = true;

        EditorUtility.SetDirty(player);
        Debug.Log("ISLEM TAMAM! Karakter havadan alindi, ayaklari yere bastirildi ve sensörleri duzeltildi!");
    }
}
