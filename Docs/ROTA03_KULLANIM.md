# Pilot Koltuğu — Rota03

Ana sahne: `Assets/Scenes/Rota03.unity`. Play'e basıp Game penceresine tıklayarak uçuşa geçilir.
Hangar01 ve Rota02 sahneleri korunur; Rota03 içinde eski HedefeGit ve RotaKamera kapalı tutulur.

| Girdi | Eylem |
| --- | --- |
| W / S | Burnu kaldır / indir |
| A / D | Sola / sağa yatış |
| Q / E | Dünya Y ekseninde sola / sağa sapma |
| Yukarı / aşağı ok | Gazı bir basışta %2 artır / azalt |
| H | Deneme hasarı: 10 can azalt |

Gaz ve kamera, kullanıcı tercihiyle föydeki davranışı korur: gaz tuş olayında değişir;
tuşu basılı tutmak sürekli artırmaz. Kamera uzaklığı yerel (0, 3.2, -11) olarak sabittir;
gazla yalnızca görüş açısı 55–72 derece arasında değişir. Kamera LateUpdate'te çalışır.

InputSystem_Actions varlığındaki Ucus haritasında dört Axis ve bir Hasar Button bulunur.
Tuşlar uçuş kodunda bulunmaz. W ile yükselme tercihi, Yunuslama eylemindeki negatif ölçek
işlemcisiyle uygulanır; uçuş denklemi föydeki işaretini korur. PlayerInput Send Messages kullanır.
Yumuşatma başlangıç değeri föydeki 6'dır; kişisel deney sonucu olduğu iddia edilmez.

Saglik canı ve CanDegisti/Oldu olaylarını tutar. CanYazisi OnEnable'da abone olur,
OnDisable'da ayrılır; Console'a yazar ve göstergenin okuyacağı değeri günceller.
PilotBilgi hız, gaz, irtifa ve canı gösterir; tuş etiketlerini eylem varlığından okur.
Can sıfırlandığında Oldu bir kez yayınlanır. Föyde olmayan çarpışma, patlama,
otomatik yeniden başlatma veya sağlıkla uçuşu durdurma davranışı eklenmemiştir.

Uçuş Transform tabanlıdır; Rigidbody fiziği içermez. Pervane dönmeye devam eder.
Sahne kayıtlı uçak, gözlem balonu, uçaksavar, zemin ve pist içerir; Edit modunda da görünür.

DeltaTime, yumuşatma ve Update/LateUpdate kişisel deneyleri teslim ödevi olarak
işaretlenmemiştir. Bonuslar ve GitHub işlemleri bu haftanın uygulama kapsamına eklenmemiştir.
