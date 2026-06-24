using UnityEngine;

public class LoopTeleporter : MonoBehaviour
{
    [Header("Işınlanma Ayarları")]
    public Transform teleportTarget; // Koridorun başındaki ışınlanma noktası

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (LoopManager.Instance != null)
            {
                // Eğer döngü zaten kırıldıysa (örneğin limit aşıldıysa) ışınlama! Oyuncu kapıdan çıksın.
                if (LoopManager.Instance.isLoopBroken)
                {
                    return; 
                }

                // Döngü devam ediyorsa turu artır
                LoopManager.Instance.IncrementLoop();

                // Tur artırıldıktan sonra döngü sınırına ulaşıldıysa (kırıldıysa) ışınlama!
                if (LoopManager.Instance.isLoopBroken)
                {
                    return;
                }
            }

            if (teleportTarget == null) return;

            // --- KUSURSUZ IŞINLANMA MATEMATİĞİ ---
            // Oyuncunun kapıdan ne kadar sağda/solda geçtiğini hesapla ki tam aynı konumda başa dönsün
            Vector3 offset = other.transform.position - transform.position;
            
            // Y eksenindeki offseti sıfırla ki havada uçmasın veya yere gömülmesin
            offset.y = 0;

            // Oyuncuyu hedef noktaya (koridorun başına) aynı hiza ile yerleştir
            other.transform.position = teleportTarget.position + offset;
            
            // Unity'nin fizik/kamera gecikmelerini önlemek için
            Physics.SyncTransforms();
        }
    }
}
