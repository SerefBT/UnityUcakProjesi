using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SortieUcus : MonoBehaviour
{
    [SerializeField] private float enCokHiz = 35f;
    [SerializeField] private float kalkisHizi = 26f;
    [SerializeField] private float ivme = 12f;
    [SerializeField] private float donusHizi = 42f;
    [SerializeField] private float yunuslamaHizi = 40f;
    [SerializeField] private float inisAcisi = 18f;
    [SerializeField] private float govdeYaricapi = 2.4f;
    [SerializeField] private LayerMask engeller = 256;
    private float yunuslama, yatis, pitch, heading, bank;
    private bool kalkti;
    public float Hiz { get; private set; }
    public float GazOrani { get; private set; }
    public bool Carpti { get; private set; }
    public bool Yerde => !kalkti;
    public static event Action<string> Carpisti;

    public void OnYunuslama(InputValue value) => yunuslama = value.Get<float>();
    public void OnYatis(InputValue value) => yatis = value.Get<float>();

    private void Start()
    {
        heading = transform.eulerAngles.y;
    }

    private void Update()
    {
        var mission = GorevYoneticisi.Aktif;
        if (mission == null || !mission.Oynaniyor || Carpti) return;
        var fuel = GetComponent<Yakit>();
        if (fuel != null && fuel.Oran <= 0) { Cakil("Yakıt tükendi"); return; }
        float dt = Time.deltaTime;
        if (!kalkti) GazOrani = Mathf.Clamp01(GazOrani - yunuslama * .55f * dt);
        Hiz = Mathf.MoveTowards(Hiz, GazOrani * enCokHiz, ivme * dt);
        heading += yatis * donusHizi * dt * (kalkti ? 1 : .5f);
        if (kalkti) pitch = Mathf.Clamp(pitch + yunuslama * yunuslamaHizi * dt, -50, 50);
        bank = Mathf.Lerp(bank, kalkti ? -yatis * 28 : 0, 1 - Mathf.Exp(-5 * dt));
        transform.rotation = Quaternion.Euler(pitch, heading, bank);
        Vector3 previous = transform.position;
        Vector3 next = previous + transform.forward * Hiz * dt;
        if (!kalkti)
        {
            next.y = .05f;
            if (Hiz >= kalkisHizi)
            {
                kalkti = true;
                pitch = -8f;
                next.y = .1f;
            }
        }
        else if (next.y <= .05f)
        {
            bool safe = Hiz < kalkisHizi && Mathf.Abs(pitch) < inisAcisi && Mathf.Abs(bank) < inisAcisi * 1.5f;
            if (!safe) { Cakil("Zemine sert çarptın"); return; }
            kalkti = false;
            pitch = 0;
            next.y = .05f;
        }
        Vector3 displacement = next - previous;
        if (kalkti && next.y > 1.55f &&
            ((displacement.sqrMagnitude > 0 && Physics.SphereCast(previous + Vector3.up * 1.2f, govdeYaricapi, displacement.normalized, out _, displacement.magnitude, engeller, QueryTriggerInteraction.Ignore))
            || Physics.CheckSphere(next + Vector3.up * 1.2f, govdeYaricapi, engeller, QueryTriggerInteraction.Ignore)))
        { Cakil("Bir engele çarptın"); return; }
        transform.position = next;
    }

    public void Cakil(string neden)
    {
        if (Carpti) return;
        Carpti = true;
        Hiz = 0;
        Carpisti?.Invoke(neden);
    }
}
