using UnityEngine;

public class PilTopla : MonoBehaviour
{
    public float sarjmik = 70f;
    void Update()
    {
        transform.Rotate(0, 100 * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FenerKontrol fener = Object.FindFirstObjectByType<FenerKontrol>();
            if (fener != null)
            {
                fener.mevcutsarj = Mathf.Clamp(fener.mevcutsarj + sarjmik, 0f, 100f);
                Debug.Log("Pil toplandı. Yeni şarj: " + fener.mevcutsarj);
                Destroy(gameObject);
            }
        }

    }
}

