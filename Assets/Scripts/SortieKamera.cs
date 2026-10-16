using UnityEngine;
public class SortieKamera : MonoBehaviour
{
    [SerializeField] private Transform hedef;
    private Vector3 velocity;
    private void LateUpdate()
    {
        if (hedef == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, hedef.TransformPoint(new Vector3(0, 3.2f, -11)), ref velocity, .18f);
        transform.LookAt(hedef.position + hedef.up * 1.2f, hedef.up);
        var flight = hedef.GetComponent<SortieUcus>();
        var camera = GetComponent<Camera>();
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, Mathf.Lerp(55, 72, flight.GazOrani), 1 - Mathf.Exp(-3 * Time.deltaTime));
    }
}
