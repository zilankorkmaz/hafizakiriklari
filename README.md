# 🧠 Hafıza Kırıkları (Memory Fragments)

*"15 Mart... Hiçbir şey hatırlamıyorum. Bugünün tarihini bile bilmiyorum. Şifre bu olabilir mi?"*

**Hafıza Kırıkları**, karanlık ve gizemli bir laboratuvarda geçen, hikâye odaklı birinci şahıs bir keşif ve gerilim oyunudur. Geçmişine dair hiçbir şey hatırlamayan bir karakterin, etrafa saçılmış notları ve eski fotoğrafları inceleyerek kim olduğunu ve bu laboratuvarda neler yaşandığını çözme çabasını konu alır.

---

## 📖 Hikâye

Karanlık bir odada uyanıyorsunuz. Neden burada olduğunuzu veya kim olduğunuzu bilmiyorsunuz. Etrafınızdaki terk edilmiş laboratuvar, yarım kalmış deneyler ve korkutucu fısıltılar barındırıyor. Tek rehberiniz, size ait olup olmadığını bile bilmediğiniz eski bir günlük ve duvardaki fotoğraflar. 

*"Madem baş araştırmacıyım, neden onlara emir veriyormuşum gibi giyinmişim? Neden ellerim kan içinde?"* 
Geçmişin sırlarını çözün ve buradan kaçmanın bir yolunu bulun.

---

## ✨ Özellikler

- **Gelişmiş Etkileşim Sistemi:** Çevredeki notları okuyun, fotoğrafları inceleyin ve gizli şifreleri ortaya çıkarın.
- **Karanlık ve Atmosferik URP Grafikleri:** Unity Universal Render Pipeline (URP) ile tasarlanmış karanlık, gerilim dolu ortamlar.
- **Dinamik Fener Mekaniği:** Karanlık köşeleri aydınlatın, ancak pilinize dikkat edin! Pil azaldığında feneriniz titremeye başlar.
- **Ses ve Psikolojik Gerilim:** Arka plan ambiyansları ve "fısıltı" efektleriyle desteklenen ürkütücü atmosfer.

---

## 🎮 Kontroller

| Tuş | Eylem |
| :--- | :--- |
| **W, A, S, D** | Hareket Etme |
| **Mouse (Fare)** | Etrafa Bakma |
| **E** | Etkileşime Geçme (Not/Fotoğraf okuma, Kapı açma) |
| **F** | El Fenerini Aç / Kapat |
| **Tab** | Duraklatma Menüsü (Pause) ve Ses Ayarları |

---

## 🛠️ Teknik Altyapı ve Kurulum

Bu proje **Unity 3D** oyun motoru ile geliştirilmiştir.

### 🎬 Çalışma Sahnesi
Projenin ana senaryo ve başlangıç sahnesi:
`Assets/Scenes/lab_final.unity`

Projeyi Unity'de açtıktan sonra geliştirmeye bu sahneden devam edilmesi önerilir. Build ayarlarında (`ProjectSettings/EditorBuildSettings.asset`) yalnızca bu sahnenin bulunduğundan emin olun.

### 🔊 Ses ve Duraklatma Menüsü Sistemi
`lab_final` sahnesinde kök nesne olarak **GameplayAudioAndMenu** bulunur. Bu sistem şunları sağlar:
- Arka plan ambiyans ve müziklerini çalar.
- Tüm SFX (efekt) seslerini tek bir merkezden yönetir.
- **Tab** tuşu ile açılan duraklatma menüsünü yönetir.
- Oyuncunun Master, Music ve SFX ses ayarlarını `PlayerPrefs` kullanarak kaydeder.

**Kendi sahnenize eklemek için:**
Üst menüden `Tools > Hafiza > Sahneye Ses ve Menu Ekle` yolunu izleyebilir veya boş bir nesneye `GameplayAudioAndMenu` scriptini atayabilirsiniz.

**Kod içerisinden ses çalmak için örnek kullanım:**
```csharp
GameplayAudioAndMenu.Instance?.PlaySfx(clip);
```

---

## 🤝 Katkıda Bulunma

Bu proje kişisel bir geliştirme prototipidir. Herhangi bir hata bulursanız veya öneriniz varsa, lütfen **Issues** (Sorunlar) sekmesinden bildirin.