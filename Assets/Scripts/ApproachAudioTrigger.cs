using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class ApproachAudioTrigger : MonoBehaviour
{
    [Tooltip("Çalınacak ses (Örn: heavyknock.mp3)")]
    public AudioClip knockSound;
    
    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D ses efekti (kapının oradan gelir)
        
        if (knockSound != null)
        {
            audioSource.clip = knockSound;
        }
        
        // Çarpışma kutusunun içinden geçilebilir (Trigger) olduğundan emin ol
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Eğer ses henüz çalınmadıysa ve tetikleyiciye giren objenin Tag'i "Player" ise
        if (!hasPlayed && other.CompareTag("Player"))
        {
            if (audioSource.clip != null)
            {
                audioSource.Play();
                hasPlayed = true; // Sadece 1 kere çalması için
                Debug.Log("Kapıya yaklaşıldı, vurma sesi çalıyor.");
            }
        }
    }
}
