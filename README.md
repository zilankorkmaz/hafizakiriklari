# HafizaKiriklari_Temiz

## Calisma Sahnesi

Projede aktif calisma sahnesi `Assets/Scenes/MainScene.unity` dosyasidir.

Projeyi cektikten sonra lutfen bu sahneyi acip gelistirmeye devam edin.
Build icin baslangic sahnesi de `MainScene` olarak ayarlidir.

## Ses ve duraklatma menusu

Repodaki `MainScene` sahnesinde kok nesne olarak **`GameplayAudioAndMenu`** bulunur (ses clip'i Inspector'dan atanir).

1. Ek sahneye ihtiyaciniz varsa: **Tools > Hafiza > Sahneye Ses ve Menu Ekle** veya bos GameObject uzerine `GameplayAudioAndMenu` surukleyin. `MainScene` zaten iceriyorsa bu adimi atlayin.
2. Inspector'da **Ambience Clip** alanina arka plan sesinizi (loop uygun WAV/MP3) surukleyin. Alternatif: `Assets/Resources/Audio/` altina clip koyup **Resources Ambience Path** alanina ornegin `Audio/ambiance` yazin.
3. Oyunda **Tab** ile menu acilir/kapanir; genel / muzik / SFX sesleri `PlayerPrefs` ile kaydedilir. `Esc` ile not paneli kullanan sahnelerle cakismamasi icin varsayilan Tab'dir; **Pause Key** alanindan degistirebilirsiniz.
4. Kod icinden kisa efekt: `GameplayAudioAndMenu.Instance?.PlaySfx(clip);`
