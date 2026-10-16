using System;
using UnityEngine;

public class KesifHedefi : MonoBehaviour
{
    [SerializeField] private int puan = 100;
    [SerializeField] private bool yanHedef;
    [SerializeField] private string hedefAdi = "Keşif noktası";
    public bool YanHedef => yanHedef;
    public string HedefAdi => hedefAdi;
    public bool Alindi { get; private set; }
    public static event Action<int, bool> Fotograflandi;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<UcusKontrol>() != null) FotoCek();
    }

    public void FotoCek()
    {
        if (Alindi || GorevYoneticisi.Aktif == null || !GorevYoneticisi.Aktif.Oynaniyor) return;
        Alindi = true;
        Fotograflandi?.Invoke(puan, yanHedef);
        gameObject.SetActive(false);
    }
}
