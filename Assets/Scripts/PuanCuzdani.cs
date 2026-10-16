using System;
using UnityEngine;

public class PuanCuzdani : MonoBehaviour
{
    [SerializeField] private int inisPuani = 30;
    [SerializeField] private int hasarsizPuani = 40;
    public int Puan { get; private set; }
    public int AnaPuan { get; private set; }
    public int YanPuan { get; private set; }
    public int DonusPuan { get; private set; }
    public int HasarsizPuan { get; private set; }
    public event Action<int> PuanDegisti;
    public event Action<int> PuanEklendi;
    private void OnEnable() => KesifHedefi.Fotograflandi += Hedef;
    private void OnDisable() => KesifHedefi.Fotograflandi -= Hedef;
    private void Hedef(int amount, bool optional)
    {
        if (optional) YanPuan += amount; else AnaPuan += amount;
        Ekle(amount);
    }
    public void Ekle(int amount) { Puan += amount; PuanDegisti?.Invoke(Puan); PuanEklendi?.Invoke(amount); }
    public void Donus(bool hasarsiz)
    {
        DonusPuan = inisPuani;
        Ekle(inisPuani);
        if (!hasarsiz) return;
        HasarsizPuan = hasarsizPuani;
        Ekle(hasarsizPuani);
    }
}
