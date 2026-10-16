using UnityEngine;

[ExecuteAlways]
public class Radar : MonoBehaviour
{
    [SerializeField] private Transform hedef;
    [SerializeField, Range(1f, 89f)] private float yarimAci = 35f;
    [SerializeField] private float menzil = 120f;
    [SerializeField] private bool goruyor;
    [SerializeField] private float yanTaraf;
    public bool Goruyor => goruyor;
    public float YanTaraf => yanTaraf;

    private void Update()
    {
        Degerlendir();
    }

    public void Degerlendir()
    {
        goruyor = false;
        yanTaraf = 0f;
        if (hedef == null) return;
        Vector3 fark = hedef.position - transform.position;
        if (fark.sqrMagnitude < .0001f) return;
        Vector3 yon = fark.normalized;
        goruyor = Vector3.Dot(transform.forward, yon) > Mathf.Cos(yarimAci * Mathf.Deg2Rad)
            && fark.sqrMagnitude < menzil * menzil;
        yanTaraf = Vector3.Cross(transform.forward, yon).y;
    }

    private void OnDrawGizmos()
    {
        Degerlendir();
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 30f);
        Vector3 onceki = Vector3.zero;
        for (int adim = 0; adim <= 48; adim++)
        {
            float aci = adim * 360f / 48;
            Vector3 kenar = transform.position + transform.rotation * Quaternion.AngleAxis(aci, Vector3.forward)
                * Quaternion.AngleAxis(yarimAci, Vector3.up) * Vector3.forward * menzil;
            if (adim > 0) Gizmos.DrawLine(onceki, kenar);
            if (adim % 12 == 0) Gizmos.DrawLine(transform.position, kenar);
            onceki = kenar;
        }
        if (hedef == null) return;
        Gizmos.color = goruyor ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, hedef.position);
        Gizmos.DrawWireSphere(hedef.position, 4f);
    }
}
