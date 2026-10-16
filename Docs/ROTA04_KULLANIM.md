# İlk Sortie

Sahne: Assets/Scenes/Rota04.unity. Play ile menü açılır. SORTIE'YE BAŞLA düğmesi görevi başlatır.

Yerde W gazı artırır, S azaltır. 26 m/s'de otomatik kalkış olur. Azami hız 35 m/s'dir.
Havada W yükseltir, S alçaltır; gaz kalkıştaki ayarında kalır. A/D dönüş ve görsel yatış yapar.
H tuşu önceki haftadan gelen 10 canlık hasar denemesidir; hasarsız dönüş bonusunu etkiler.

3 ana hedefin her biri 100, iki isteğe bağlı yan hedefin her biri 25 puandır.
Tetikleyici sütunları 30 x 180 x 30 metredir. 180 metreden yukarı uçmak hedef toplamayı engeller.
Hedeflerin üstünden geçmek fotoğraf sayılır. Her hedef yalnızca bir kez puan verir.
Ana hedefler tamamlandığında üs açılır. Üs bölgesinden geçmek yeterlidir, iniş gerekmez.
Üsse dönüş 30, hiç hasar almadan dönüş 40 puandır. En yüksek toplam 420'dir.

Süre 150 saniyedir. Hedef rotası yaklaşık 2 kilometredir; yüksek hızda teorik düz hat turu
yaklaşık 60 saniye, dönüş ve kalkış eklenince daha uzundur. Kişisel oynanış ölçümü olduğu iddia edilmez.
Yakıt 100 birimdir. Tüketim saniyede .16 + .32 * gaz oranıdır; tam gazda yaklaşık 208 saniye yeter.
Bu yüzden normal 150 saniyelik görevde yakıt gereksiz ceza yaratmaz; süre sınırı artırılırsa dolaşmayı sınırlar.
Yakıt üste doldurulur. Yakıt, süre, hız ve ödüller ilgili bileşenlerin Inspector alanlarından ayarlanabilir.

Sert zemin teması ve engel taraması görevi kaybettirir. Süre bitimi veya yakıt tükenmesi de kayıp sayılır.
Kayıptan 2.5 saniye sonra yeni görev yüklenir. Başarı ekranındaki YENİ SORTIE görevi sıfırlar.
Puan görevler arasında birikmez; ikinci uçak ve uçak açma sistemi kullanıcı tercihiyle yoktur.

Arayüzdeki bütün değerler canlıdır. İbre ve gaz topuzu ayrı PNG'dir.
Hedef atlası piksel alfa sınırlarından ölçülmüştür; pusula Repeat, diğer dokular Clamp kullanır.
Dokular sıkıştırılmamış Sprite olarak, mipmap kapalı içe aktarılır.
Üretim kaynakları ve promptlar Belgeler/ASSET-LISANS.md içindedir.
Önceki sahneler ve önceki haftanın girdi varlığı korunur. Rota04 ayrı Sortie_Actions kullanır.
