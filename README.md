# DavutUcakProje

Unity 6000.0.31f1 (6.0), Universal Render Pipeline.

Nu.D.36 esintili çift kanatlı uçakla hedefe yönelme, radar görüş konisi ve vektör gösterimi.

- `Assets/Scenes/Hangar01.unity`: ilk haftanın sabit uçak sahnesi.
- `Assets/Scenes/Rota02.unity`: ikinci haftanın uçuş ve radar sahnesi. Play ile otomatik uçuş başlar.
- `Assets/Scenes/Rota03.unity`: üçüncü haftanın manuel pilot sahnesi; varsayılan açılış sahnesi.
- `Assets/Scenes/Rota04.unity`: İlk Sortie; güncel açılış sahnesi. 3 ana/2 yan hedef, üsse dönüş, puan, süre, yakıt ve dönem HUD'u.
- `Docs/ROTA04_KULLANIM.md`: WASD kontrolü ve görev kuralları. `Belgeler/ASSET-LISANS.md`: görsel üretim kayıtları.
- `Docs/ROTA03_KULLANIM.md`: pilot kontrolleri, gaz davranışı ve sağlık olaylarının açıklaması.
- Scene penceresindeki Gizmos açıkken ileri yön, hedef doğrultusu ve radar konisi görünür.
- Uçaktaki HedefeGit ile hız, dönüş hızı ve varış mesafesi; Ucaksavar üzerindeki Radar ile açı ve menzil değiştirilebilir.
- Radar'ın Goruyor ve Yan Taraf alanları Inspector'da canlı görünür; edit modunda da hesaplanır.
- `BlenderSource/`: kaynak Blender, FBX üretim betiği ve GLB dosyası.
- `Docs/MDA_Ornek.md`: örnek tasarım analizi.

Föy Unity 6.3 LTS ister; mevcut kurulu proje sürümü korunmuştur.
GitHub deposu: https://github.com/SerefBT/UnityUcakProjesi

Föydeki normalize deneyi hakkında: verilen kodda hareket `transform.forward` ile yapıldığı için
yalnızca `fark.normalized` yerine `fark` yazmak uçağın hızını değiştirmez; LookRotation yönü kullanır.
Hız farkını göstermek için `yon * hiz * deltaTime` hareketinde normalize edilmiş ve edilmemiş
vektörler karşılaştırılmalıdır. Normal uçuş burun yönünde ve sabit 14 m/s hızdadır.

Kanat yatışı rota dönüşünden ayrı hesaplanır ve her karede mutlak olarak uygulanır;
bu sayede föyde uyarılan birikerek takla atma sorunu oluşmaz.

Git ignore kuralları için kaynak: https://github.com/github/gitignore/blob/main/Unity.gitignore
