using UnityEngine;

public class Yakit : MonoBehaviour
{
    [SerializeField] private float kapasite = 100f;
    [SerializeField] private float rolantiTuketimi = .16f;
    [SerializeField] private float gazTuketimi = .32f;
    public float Kalan { get; private set; }
    public float Oran => Kalan / kapasite;
    private SortieUcus ucus;
    private void Awake() { ucus = GetComponent<SortieUcus>(); Doldur(); }
    private void Update()
    {
        if (GorevYoneticisi.Aktif == null || !GorevYoneticisi.Aktif.Oynaniyor) return;
        Kalan = Mathf.Max(0, Kalan - (rolantiTuketimi + gazTuketimi * ucus.GazOrani) * Time.deltaTime);
    }
    public void Doldur() => Kalan = kapasite;
}
