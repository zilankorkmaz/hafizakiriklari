using UnityEngine;

public class PilTopla : MonoBehaviour
{
    public float sarjmik = 70f;
    
    // Yerde kendi etrafında dönmesi için
    void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Oyuncu mu çarptı diye kontrol et (Etiket veya CharacterController üzerinden)
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null || other.GetComponentInParent<CharacterController>() != null)
        {
            // Sahnede yanlışlıkla birden fazla FenerKontrol kalmış olabilir, Hepsini bulup fulle!
            FenerKontrol[] fenerler = Object.FindObjectsByType<FenerKontrol>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            if (fenerler.Length > 0)
            {
                foreach(var fener in fenerler)
                {
                    fener.SarjDoldur(sarjmik);
                    Debug.Log("PİL TOPLANDI! Yeni şarj: " + fener.mevcutsarj + " (Objesi: " + fener.gameObject.name + ")");
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("PİL TOPLANAMADI: Sahnedeki oyuncuda veya kamerada FenerKontrol scripti bulunamadı!");
            }
        }
        else
        {
            Debug.Log("Pile dokunan nesne oyuncu olarak algılanmadı: " + other.gameObject.name);
        }
    }
}
