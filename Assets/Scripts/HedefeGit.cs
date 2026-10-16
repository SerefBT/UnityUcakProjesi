using UnityEngine;

public class HedefeGit : MonoBehaviour
{
    [SerializeField] private Transform hedef;
    [SerializeField] private float hiz = 14f;
    [SerializeField] private float donusHizi = 60f;
    [SerializeField] private float varisMesafesi = 8f;
    [SerializeField] private float maksimumYatis = 45f;
    [SerializeField] private float yatisHizi = 90f;
    private Quaternion rotaDonusu;
    private float yatis;
    public Transform Hedef => hedef;
    public float Mesafe => hedef == null ? 0f : Vector3.Distance(transform.position, hedef.position);

    private void OnEnable()
    {
        rotaDonusu = Quaternion.LookRotation(transform.forward, Vector3.up);
        yatis = 0f;
    }

    private void Update()
    {
        Ilerle(Time.deltaTime);
    }

    public void Ilerle(float deltaTime)
    {
        if (hedef == null) return;
        Vector3 fark = hedef.position - transform.position;
        float mesafe = fark.magnitude;
        if (mesafe < .001f) return;
        Vector3 yon = fark.normalized;
        bool vardi = mesafe <= varisMesafesi;
        float yan = Vector3.Cross(rotaDonusu * Vector3.forward, yon).y;
        if (!vardi)
        {
            rotaDonusu = Quaternion.RotateTowards(rotaDonusu, Quaternion.LookRotation(yon), donusHizi * deltaTime);
        }
        yatis = Mathf.MoveTowards(yatis, vardi ? 0f : -yan * maksimumYatis, yatisHizi * deltaTime);
        transform.rotation = rotaDonusu * Quaternion.AngleAxis(yatis, Vector3.forward);
        if (!vardi)
        {
            float adim = Mathf.Min(hiz * deltaTime, mesafe - varisMesafesi);
            transform.position += transform.forward * adim;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * 12f);
        if (hedef == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, hedef.position);
        Gizmos.DrawWireSphere(hedef.position, varisMesafesi);
    }
}
