using UnityEngine;
using UnityEngine.UI;

public class MainMenuEndState : MonoBehaviour
{
    [Header("UI Metinleri")]
    public Text mainTitleText;   // Ana başlık ("Hafıza Kırıkları" veya "Oyunu başarıyla tamamladın...")
    public Text subtitleText;    // Alt başlık ("Kaybettin!" yazdıracağımız yer)

    private void Start()
    {
        // Fareyi menüde tıklanabilir yapmak için
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (mainTitleText == null) return;

        // PlayerPrefs'ten oyunun nasıl bittiği bilgisini al (Varsayılan 0'dır, yani hiç bitmedi)
        int endState = PlayerPrefs.GetInt("GameEndState", 0);

        switch (endState)
        {
            case 1: // HATIRLADI (KAZANDI)
                mainTitleText.text = "Oyunu başarıyla tamamladın\nve hafızanı geri kazandın!";
                mainTitleText.color = Color.green;
                if (subtitleText != null) subtitleText.text = ""; 
                break;

            case 2: // UNUTTU (KAYBETTİ)
                mainTitleText.text = "Hafıza Kırıkları";
                mainTitleText.color = Color.white;
                if (subtitleText != null)
                {
                    subtitleText.text = "Kaybettin!";
                    subtitleText.color = Color.red;
                }
                break;

            case 3: // YANLIŞ KAPI (KAYBETTİ)
                mainTitleText.text = "Hafıza Kırıkları";
                mainTitleText.color = Color.white;
                if (subtitleText != null)
                {
                    subtitleText.text = "KAYBETTİN!\nEkip arkadaşların ihanetini öğrendi.";
                    subtitleText.color = Color.red;
                }
                break;
                
            default: // NORMAL AÇILIŞ (Oyun hiç bitirilmemiş)
                if (subtitleText != null)
                {
                    subtitleText.text = ""; // Oyuna ilk girişte alt başlığı temizle
                }
                break;
        }
    }

    // Oyun yeniden başladığında PlayerPrefs'i temizlemek (sıfırlamak) isteyebilirsiniz
    // Bu fonksiyonu "Play" butonuna basıldığında çağırabilirsiniz
    public void ResetGameEndState()
    {
        PlayerPrefs.SetInt("GameEndState", 0);
        PlayerPrefs.Save();
    }
}
