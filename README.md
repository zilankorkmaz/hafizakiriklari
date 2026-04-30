# HafizaKiriklari_Temiz

Unity ile geliştirilmiş hikâye odaklı keşif oyunu prototipi.

---

## 🎬 Çalışma Sahnesi

Projenin ana senaryo ve build başlangıç sahnesi:

`Assets/Scenes/lab_final.unity`

Projeyi açtıktan sonra geliştirmeye bu sahneden devam edilmesi önerilir.  
Diğer sahneler (örneğin `SampleScene`) isteğe bağlıdır ve kaldırılabilir.

Build ayarlarında yalnızca bu sahnenin bulunduğundan emin olun:

`ProjectSettings/EditorBuildSettings.asset`

---

## 🔊 Ses ve Duraklatma Menüsü

`lab_final` sahnesinde kök nesne olarak **GameplayAudioAndMenu** bulunur.

Bu bileşen:

- Arka plan ambiyans/müzik çalar  
- SFX (efekt) seslerini yönetir  
- Tab tuşu ile açılan duraklatma menüsü oluşturur  
- Ses ayarlarını (Master / Music / SFX) kaydeder  

---

## ⚙️ Kurulum

### 1. Sahneye ekleme

Başka bir sahnede kullanmak için:
Tools > Hafiza > Sahneye Ses ve Menu Ekle

veya boş bir GameObject’e `GameplayAudioAndMenu` scriptini ekleyebilirsiniz.

---

### 2. Arka plan sesi ekleme

- Inspector’daki **Ambience Clip** alanına bir ses dosyası (WAV/MP3) sürükleyin.

Alternatif olarak: 
Assets/Resources/Audio/
klasörüne ses ekleyip,  
**Resources Ambience Path** alanına örneğin: 
Audio/ambience
yazabilirsiniz.

---

## 🎮 Kullanım

- **Tab** → Menü aç/kapat  
- Ses ayarları otomatik olarak kaydedilir (PlayerPrefs)  
- Pause tuşu Inspector’dan değiştirilebilir  

---

## 💻 Kod ile kullanım

Efekt sesi çalmak için:

```csharp
GameplayAudioAndMenu.Instance?.PlaySfx(clip);