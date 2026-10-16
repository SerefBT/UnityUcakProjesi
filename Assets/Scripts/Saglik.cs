using System;
using UnityEngine;

public class Saglik : MonoBehaviour
{
    [SerializeField, Min(1)] private int enCokCan = 100;

    public int Can { get; private set; }
    public int EnCokCan => enCokCan;
    public event Action<int, int> CanDegisti;
    public event Action Oldu;

    private void Awake()
    {
        Can = enCokCan;
    }

    public void Vur(int miktar)
    {
        if (Can <= 0 || miktar <= 0) return;
        Can = Mathf.Max(0, Can - miktar);
        CanDegisti?.Invoke(Can, enCokCan);
        if (Can == 0) Oldu?.Invoke();
    }
}
