using UnityEngine;

public class CanYazisi : MonoBehaviour
{
    [SerializeField] private Saglik saglik;

    public int GosterilenCan { get; private set; }
    public int GosterilenEnCokCan { get; private set; }

    private void OnEnable()
    {
        if (saglik == null) return;
        saglik.CanDegisti += CanDegisti;
        GosterilenCan = saglik.Can;
        GosterilenEnCokCan = saglik.EnCokCan;
    }

    private void Start()
    {
        if (saglik == null) return;
        GosterilenCan = saglik.Can;
        GosterilenEnCokCan = saglik.EnCokCan;
    }

    private void OnDisable()
    {
        if (saglik != null) saglik.CanDegisti -= CanDegisti;
    }

    private void CanDegisti(int kalan, int enCok)
    {
        GosterilenCan = kalan;
        GosterilenEnCokCan = enCok;
        Debug.Log("Can: " + kalan + "/" + enCok);
    }
}
