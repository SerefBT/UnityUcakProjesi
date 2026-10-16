using System;
using UnityEngine;

public class GorevYoneticisi : MonoBehaviour
{
    [SerializeField] private GameObject usBolgesi;
    [SerializeField] private PuanCuzdani cuzdan;
    [SerializeField] private Saglik saglik;
    [SerializeField] private Yakit yakit;
    [SerializeField] private float sureSaniye = 150f;
    public static GorevYoneticisi Aktif { get; private set; }
    public static event Action<string> SortieBasarisiz;
    public event Action<int> GorevBitti;
    public int AnaToplam { get; private set; }
    public int YanToplam { get; private set; }
    public int AnaAlinan { get; private set; }
    public int YanAlinan { get; private set; }
    public float KalanSure { get; private set; }
    public bool DonusFazi { get; private set; }
    public bool GorevTamam { get; private set; }
    public bool Basarisiz { get; private set; }
    public bool Basladi { get; private set; }
    public string Sebep { get; private set; }
    public bool Oynaniyor => Basladi && !GorevTamam && !Basarisiz;
    private bool hasarAldi;
    private void Awake() { Aktif = this; }
    private void OnEnable()
    {
        KesifHedefi.Fotograflandi += Hedef;
        SortieUcus.Carpisti += Basarisizlik;
        if (saglik != null) saglik.CanDegisti += Can;
    }
    private void OnDisable()
    {
        KesifHedefi.Fotograflandi -= Hedef;
        SortieUcus.Carpisti -= Basarisizlik;
        if (saglik != null) saglik.CanDegisti -= Can;
        if (Aktif == this) Aktif = null;
    }
    private void Start()
    {
        foreach (var target in FindObjectsByType<KesifHedefi>(FindObjectsSortMode.None))
            if (target.YanHedef) YanToplam++; else AnaToplam++;
        usBolgesi.SetActive(false);
        KalanSure = sureSaniye;
    }
    public void Baslat() { Basladi = true; if (AnaToplam == 0) DonusuAc(); }
    private void Update()
    {
        if (!Oynaniyor || sureSaniye <= 0) return;
        KalanSure = Mathf.Max(0, KalanSure - Time.deltaTime);
        if (KalanSure <= 0) Basarisizlik("Süre doldu");
    }
    private void Can(int kalan, int maximum) { if (kalan < maximum) hasarAldi = true; }
    private void Hedef(int amount, bool optional)
    {
        if (!Oynaniyor) return;
        if (optional) YanAlinan++; else AnaAlinan++;
        if (AnaAlinan >= AnaToplam) DonusuAc();
    }
    private void DonusuAc() { DonusFazi = true; usBolgesi.SetActive(true); }
    public void UsseVarildi()
    {
        if (!Oynaniyor || !DonusFazi) return;
        GorevTamam = true;
        DonusFazi = false;
        yakit.Doldur();
        cuzdan.Donus(!hasarAldi);
        GorevBitti?.Invoke(cuzdan.Puan);
    }
    public void Basarisizlik(string neden)
    {
        if (!Oynaniyor) return;
        Basarisiz = true;
        Sebep = neden;
        SortieBasarisiz?.Invoke(neden);
    }
}
