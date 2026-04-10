using Unity.VisualScripting;
using UnityEngine;

public class FenerKontrol : MonoBehaviour
{
    private Light fenerIsigi;

    [Header("Fener Ayarları")]
    public float  mevcutsarj= 100f;
    public float harcamahizi = 5f;
    void Start()
    {
        fenerIsigi = GetComponent<Light>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && mevcutsarj>0)
            fenerIsigi.enabled = !fenerIsigi.enabled;
        if (fenerIsigi.enabled)
        {
            mevcutsarj -= harcamahizi * Time.deltaTime;
            if (mevcutsarj <= 0)
            {
                mevcutsarj = 0;
                fenerIsigi.enabled = false;
                Debug.Log("Fenerin şarjı bitti!");
            }
        }
    }
}
