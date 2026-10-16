using System.Collections.Generic;
using UnityEngine;

public class UcusHUD : MonoBehaviour
{
    [SerializeField] private Texture2D cerceve, hizKadrani, yukseklikKadrani, ibre, gazPlaka, gazTopuz, pusula, isaretler, dugmeler, tablo, menu;
    [SerializeField] private SortieUcus ucus;
    [SerializeField] private GorevYoneticisi gorev;
    [SerializeField] private PuanCuzdani cuzdan;
    [SerializeField] private Yakit yakit;
    [SerializeField] private Transform us;
    private KesifHedefi[] hedefler;
    private GUIStyle text, title, small, center;
    private class Kazanc { public int amount; public float age; }
    private readonly List<Kazanc> kazanclar = new List<Kazanc>();
    private readonly List<Rect> etiketler = new List<Rect>();
    private void Start() => hedefler = FindObjectsByType<KesifHedefi>(FindObjectsSortMode.None);
    private void OnEnable() { if (cuzdan != null) cuzdan.PuanEklendi += Kazan; }
    private void OnDisable() { if (cuzdan != null) cuzdan.PuanEklendi -= Kazan; }
    private void Kazan(int amount) => kazanclar.Add(new Kazanc { amount = amount });
    private void Update()
    {
        for (int index = kazanclar.Count - 1; index >= 0; index--)
        { kazanclar[index].age += Time.deltaTime; if (kazanclar[index].age >= 2) kazanclar.RemoveAt(index); }
    }
    private void OnGUI()
    {
        if (gorev == null) return;
        if (text == null)
        {
            text = new GUIStyle(GUI.skin.label) { fontSize = 25, fontStyle = FontStyle.Bold };
            text.normal.textColor = new Color(.94f, .90f, .78f);
            title = new GUIStyle(text) { fontSize = 42 };
            small = new GUIStyle(text) { fontSize = 21 };
            center = new GUIStyle(text) { alignment = TextAnchor.MiddleCenter };
        }
        Matrix4x4 original = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / 1920f, Screen.height / 1080f, 1));
        if (!gorev.Basladi) Menu();
        else if (gorev.GorevTamam) Sonuc();
        else Oyun();
        GUI.matrix = original;
    }
    private void Menu()
    {
        Doku(new Rect(0, 0, 1920, 1080), menu);
        Kutu(new Rect(90, 180, 650, 640));
        GUI.Label(new Rect(130, 230, 580, 65), "İLK SORTIE", title);
        GUI.Label(new Rect(130, 320, 560, 160), "Çakıl Burnu • Keşif uçuşu\n3 ana hedefi fotoğrafla, üsse dön.\nYan hedefler ek puan kazandırır.", text);
        GUI.Label(new Rect(130, 495, 560, 170), "YERDE  W/S: gazı artır / azalt\nHAVADA  W/S: yüksel / alçal\nA/D: sola / sağa dön\n26 m/s'de otomatik kalkış", text);
        if (Buton(new Rect(130, 710, 550, 72), "SORTIE'YE BAŞLA")) gorev.Baslat();
    }
    private void Oyun()
    {
        etiketler.Clear();
        if (hedefler != null)
            foreach (var target in hedefler)
                if (target != null && !target.Alindi) Isaret(target.transform.position + Vector3.up * 10, target.HedefAdi, target.YanHedef);
        if (gorev.DonusFazi) Isaret(us.position + Vector3.up * 5, "ÜSSE DÖN", false);
        Doku(new Rect(0, 0, 1920, 1080), cerceve);
        Kadran(new Rect(28, 720, 282, 282), hizKadrani, ucus.Hiz * 3.6f / 360);
        Kadran(new Rect(1650, 735, 225, 225), yukseklikKadrani, Mathf.Repeat(ucus.transform.position.y, 1000) / 1000, 0, 360);
        GUI.Label(new Rect(50, 991, 250, 35), ucus.Hiz.ToString("F0") + " m/s", center);
        GUI.Label(new Rect(1625, 1000, 235, 35), Mathf.Max(0, ucus.transform.position.y).ToString("F0") + " m", center);
        Doku(new Rect(338, 744, 90, 285), gazPlaka);
        float leverY = Mathf.Lerp(956, 764, ucus.GazOrani);
        Doku(new Rect(335, leverY, 62, 62), gazTopuz);
        Color old = GUI.color;
        GUI.color = new Color(.7f, .77f, .4f);
        GUI.DrawTexture(new Rect(403, 1000 - 218 * ucus.Hiz / 35, 10, 218 * ucus.Hiz / 35), Texture2D.whiteTexture);
        GUI.color = new Color(.75f, .2f, .15f);
        GUI.DrawTexture(new Rect(397, 1000 - 218 * 26 / 35, 23, 4), Texture2D.whiteTexture);
        GUI.color = old;
        if (pusula != null) GUI.DrawTextureWithTexCoords(new Rect(520, 28, 880, 54), pusula, new Rect(ucus.transform.eulerAngles.y / 360, 0, 1, 1));
        GUI.Label(new Rect(935, 6, 50, 40), "▼", center);
        GUI.Label(new Rect(850, 86, 220, 40), ucus.transform.eulerAngles.y.ToString("F0") + "°", center);
        Kutu(new Rect(28, 28, 280, 218));
        GUI.Label(new Rect(45, 38, 250, 40), "SORTIE", text);
        GUI.Label(new Rect(45, 85, 250, 35), "Hedef       " + gorev.AnaAlinan + " / " + gorev.AnaToplam, small);
        GUI.Label(new Rect(45, 123, 250, 35), "Yan hedef  " + gorev.YanAlinan + " / " + gorev.YanToplam, small);
        GUI.Label(new Rect(45, 161, 250, 35), "Puan         " + cuzdan.Puan, small);
        if (gorev.KalanSure <= 10) GUI.color = new Color(1, .3f, .2f);
        GUI.Label(new Rect(45, 199, 250, 35), "Süre          " + Mathf.FloorToInt(gorev.KalanSure / 60) + ":" + Mathf.FloorToInt(gorev.KalanSure % 60).ToString("00"), small);
        GUI.color = old;
        Kutu(new Rect(1580, 28, 310, 100));
        GUI.Label(new Rect(1600, 39, 280, 35), "YAKIT  %" + (yakit.Oran * 100).ToString("F0"), text);
        GUI.color = yakit.Oran < .2f ? new Color(1, .3f, .2f) : new Color(.7f, .76f, .4f);
        GUI.DrawTexture(new Rect(1600, 88, 260 * yakit.Oran, 12), Texture2D.whiteTexture);
        GUI.color = old;
        Kutu(new Rect(450, 922, 1050, 98));
        GUI.Label(new Rect(470, 932, 1020, 38), ucus.Yerde ? "GAZ VER (W)  •  Kalkış: 26 m/s" : gorev.DonusFazi ? "ANA HEDEFLER TAMAM • ÜSSÜN ÜZERİNDEN GEÇ" : "KEŞİF • İşaretli hedeflerin üzerinden geç", text);
        GUI.Label(new Rect(470, 973, 1000, 35), "W/S: " + (ucus.Yerde ? "gaz" : "yüksel / alçal") + "    A/D: dönüş    •    GAZ %" + (ucus.GazOrani * 100).ToString("F0"), small);
        for (int index = 0; index < kazanclar.Count; index++)
        {
            var gain = kazanclar[index];
            GUI.color = new Color(1, .88f, .5f, 1 - gain.age / 2);
            GUI.Label(new Rect(850, 420 - 100 * (1 - Mathf.Exp(-2 * gain.age)) + index * 45, 260, 60), "+" + gain.amount, title);
        }
        GUI.color = old;
        if (gorev.Basarisiz)
        {
            Kutu(new Rect(580, 350, 760, 240));
            GUI.Label(new Rect(620, 385, 680, 70), gorev.Sebep == "Süre doldu" ? "SÜRE DOLDU" : "ÇAKILDIN", title);
            GUI.Label(new Rect(620, 475, 680, 70), gorev.Sebep + " • Yeni sortie başlıyor…", text);
        }
    }
    private void Sonuc()
    {
        Kutu(new Rect(0, 0, 1920, 1080));
        Doku(new Rect(570, 80, 780, 920), tablo);
        Color previous = text.normal.textColor;
        text.normal.textColor = new Color(.15f, .14f, .11f);
        title.normal.textColor = text.normal.textColor;
        GUI.Label(new Rect(685, 210, 600, 70), "SORTIE TAMAM", title);
        string[] rows = { "Ana hedefler", "Yan hedefler", "Üsse dönüş", "Hasarsız dönüş", "Yakıt", "TOPLAM" };
        string[] values = { cuzdan.AnaPuan.ToString(), cuzdan.YanPuan.ToString(), cuzdan.DonusPuan.ToString(), cuzdan.HasarsizPuan.ToString(), "Dolduruldu", cuzdan.Puan.ToString() };
        for (int index = 0; index < rows.Length; index++)
        { GUI.Label(new Rect(685, 310 + index * 72, 360, 50), rows[index], text); GUI.Label(new Rect(1100, 310 + index * 72, 200, 50), values[index], text); }
        text.normal.textColor = previous;
        title.normal.textColor = previous;
        if (Buton(new Rect(720, 845, 480, 75), "YENİ SORTIE")) YenidenBasla.Yenile();
    }
    private void Isaret(Vector3 world, string name, bool optional)
    {
        Vector3 screen = Camera.main.WorldToViewportPoint(world);
        Vector2 point = new Vector2(screen.x * 1920, (1 - screen.y) * 1080);
        bool inside = screen.z > 0 && point.x > 85 && point.x < 1835 && point.y > 140 && point.y < 720;
        if (inside)
        {
            Atlas(new Rect(point.x - 38, point.y - 38, 76, 76), optional ? new Rect(.5891f, .5506f, .2438f, .3066f) : new Rect(.1333f, .51565f, .3211f, .3996f));
            Rect label = new Rect(point.x - 150, point.y + 40, 300, 65);
            for (int attempt = 0; attempt < 6; attempt++)
            {
                bool overlaps = false;
                foreach (Rect previous in etiketler) if (previous.Overlaps(label)) { overlaps = true; break; }
                if (!overlaps) break;
                label.y += 65;
            }
            etiketler.Add(label);
            string caption = name + "\n" + Vector3.Distance(ucus.transform.position, world).ToString("F0") + " m";
            Color previousColor = center.normal.textColor;
            center.normal.textColor = new Color(.12f, .13f, .09f);
            GUI.Label(new Rect(label.x + 2, label.y + 2, label.width, label.height), caption, center);
            center.normal.textColor = optional ? new Color(.85f, .82f, .64f) : previousColor;
            GUI.Label(label, caption, center);
            center.normal.textColor = previousColor;
        }
        else
        {
            Vector2 direction = point - new Vector2(960, 460);
            if (screen.z < 0) direction = -direction;
            direction = direction.normalized;
            float edge = Mathf.Min(820 / Mathf.Max(.001f, Mathf.Abs(direction.x)), 300 / Mathf.Max(.001f, Mathf.Abs(direction.y)));
            point = new Vector2(960, 460) + direction * edge;
            Matrix4x4 old = GUI.matrix;
            Dondur(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90, point);
            Atlas(new Rect(point.x - 30, point.y - 30, 60, 60), new Rect(.1581f, .116f, .261f, .3297f));
            GUI.matrix = old;
        }
    }
    private void Atlas(Rect rect, Rect uv) { if (isaretler != null) GUI.DrawTextureWithTexCoords(rect, isaretler, uv); }
    private void Kadran(Rect rect, Texture2D face, float ratio, float startAngle = -140, float endAngle = 140)
    {
        Doku(rect, face);
        Matrix4x4 old = GUI.matrix;
        Dondur(Mathf.Lerp(startAngle, endAngle, Mathf.Clamp01(ratio)), rect.center);
        float size = rect.width * .63f;
        Doku(new Rect(rect.center.x - size * .4992f, rect.center.y - size * .5494f, size, size), ibre);
        GUI.matrix = old;
    }
    private void Doku(Rect rect, Texture2D texture) { if (texture != null) GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill); }
    private void Dondur(float angle, Vector2 pivot)
    {
        Matrix4x4 scale = GUI.matrix;
        GUI.matrix = Matrix4x4.identity;
        GUIUtility.RotateAroundPivot(angle, pivot);
        GUI.matrix = scale * GUI.matrix;
    }
    private void Kutu(Rect rect)
    {
        Color old = GUI.color;
        GUI.color = new Color(.12f, .13f, .09f, .88f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = old;
    }
    private bool Buton(Rect rect, string label)
    {
        int state = rect.Contains(Event.current.mousePosition) ? (Event.current.type == EventType.MouseDown ? 2 : 1) : 0;
        if (dugmeler != null) GUI.DrawTextureWithTexCoords(rect, dugmeler, new Rect(0, 1 - (state + 1) / 4f, 1, .25f));
        Color old = center.normal.textColor;
        center.normal.textColor = new Color(.15f, .14f, .11f);
        GUI.Label(rect, label, center);
        center.normal.textColor = old;
        return GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }
}
