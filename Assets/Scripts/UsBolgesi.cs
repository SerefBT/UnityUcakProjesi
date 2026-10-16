using UnityEngine;
public class UsBolgesi : MonoBehaviour
{
    [SerializeField] private GorevYoneticisi gorev;
    private void OnTriggerEnter(Collider other) { if (other.GetComponentInParent<UcusKontrol>() != null) gorev.UsseVarildi(); }
    private void OnTriggerStay(Collider other) { if (other.GetComponentInParent<UcusKontrol>() != null) gorev.UsseVarildi(); }
}
