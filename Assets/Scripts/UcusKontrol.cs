using UnityEngine;
using UnityEngine.InputSystem;

public class UcusKontrol : MonoBehaviour
{
    [SerializeField] private float enAzHiz = 8f;
    [SerializeField] private float enCokHiz = 42f;
    [SerializeField] private float yunuslamaHizi = 55f;
    [SerializeField] private float yatisHizi = 90f;
    [SerializeField] private float sapmaHizi = 30f;
    [SerializeField] private float yumusatma = 6f;

    private float yunuslama, yatis, sapma, gaz = 0.5f;
    private float hYunuslama, hYatis, hSapma;

    public float Hiz { get; private set; }
    public float GazOrani => gaz;

    public void OnYunuslama(InputValue value) => yunuslama = value.Get<float>();
    public void OnYatis(InputValue value) => yatis = value.Get<float>();
    public void OnSapma(InputValue value) => sapma = value.Get<float>();
    public void OnGaz(InputValue value) => gaz = Mathf.Clamp01(gaz + value.Get<float>() * 0.02f);

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        float katsayi = 1f - Mathf.Exp(-yumusatma * deltaTime);
        hYunuslama = Mathf.Lerp(hYunuslama, yunuslama, katsayi);
        hYatis = Mathf.Lerp(hYatis, yatis, katsayi);
        hSapma = Mathf.Lerp(hSapma, sapma, katsayi);

        transform.Rotate(Vector3.right, hYunuslama * yunuslamaHizi * deltaTime, Space.Self);
        transform.Rotate(Vector3.forward, -hYatis * yatisHizi * deltaTime, Space.Self);
        transform.Rotate(Vector3.up, hSapma * sapmaHizi * deltaTime, Space.World);

        Hiz = Mathf.Lerp(enAzHiz, enCokHiz, gaz);
        transform.position += transform.forward * Hiz * deltaTime;
    }

    private void OnDisable()
    {
        yunuslama = 0f;
        yatis = 0f;
        sapma = 0f;
    }
}
