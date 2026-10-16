using UnityEngine;

public class RotaKamera : MonoBehaviour
{
    [SerializeField] private Transform ucak;
    [SerializeField] private Vector3 uzaklik = new Vector3(18f, 10f, -28f);

    private void LateUpdate()
    {
        if (ucak == null) return;
        transform.position = Vector3.Lerp(transform.position, ucak.position + uzaklik, 1f - Mathf.Exp(-4f * Time.deltaTime));
        transform.LookAt(ucak.position + ucak.forward * 16f);
    }
}
