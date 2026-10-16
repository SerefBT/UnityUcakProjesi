using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Saglik))]
public class HasarDeneme : MonoBehaviour
{
    private Saglik saglik;

    private void Awake()
    {
        saglik = GetComponent<Saglik>();
    }

    public void OnHasar(InputValue value)
    {
        if (value.isPressed) saglik.Vur(10);
    }
}
