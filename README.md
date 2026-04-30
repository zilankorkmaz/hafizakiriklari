# HafizaKiriklari_Temiz

## Calisma Sahnesi

Ana senaryo ve build baslangic sahnesi: **`Assets/Scenes/lab_final.unity`**.

Projeyi cektikten sonra gelistirmeye bu sahneden devam edin. `MainScene`, `SampleScene` vb. yardimci veya eski denemeler olabilir; resmi akis `lab_final` uzerinedir.

`ProjectSettings/EditorBuildSettings.asset` icinde ilk (ve su an tek) build sahnesi `lab_final` olarak ayarlidir. Build Profiles icinde de ayni sahneyi kullandiginizdan emin olun.

## Ses ve duraklatma menusu

`lab_final` sahnesinde kok nesne olarak **`GameplayAudioAndMenu`** bulunur (ses clip'i Inspector'dan atanir).

1. Baska sahneye eklemeniz gerekirse: **Tools > Hafiza > Sahneye Ses ve Menu Ekle** veya bos GameObject uzerine `GameplayAudioAndMenu` surukleyin.
2. Inspector'da **Ambience Clip** alanina arka plan sesinizi (loop uygun WAV/MP3) surukleyin. Alternatif: `Assets/Resources/Audio/` altina clip koyup **Resources Ambience Path** alanina ornegin `Audio/ambiance` yazin.
3. Oyunda **Tab** ile menu acilir/kapanir; genel / muzik / SFX sesleri `PlayerPrefs` ile kaydedilir. **Pause Key** Inspector'dan degistirilebilir.
4. Kod icinden kisa efekt: `GameplayAudioAndMenu.Instance?.PlaySfx(clip);`
