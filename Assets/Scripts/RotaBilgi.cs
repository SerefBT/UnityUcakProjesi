using UnityEngine;

public class RotaBilgi : MonoBehaviour
{
    [SerializeField] private HedefeGit ucus;
    [SerializeField] private Radar radar;

    private void OnGUI()
    {
        if (ucus == null || radar == null) return;
        GUI.Box(new Rect(18, 18, 330, 140), "ROTA 02 | Nu.D.36");
        GUI.Label(new Rect(32, 48, 300, 24), "Hedef: Gözlem balonu | " + ucus.Mesafe.ToString("F1") + " m");
        GUI.Label(new Rect(32, 74, 300, 24), "Radar: " + (radar.Goruyor ? "HEDEF GÖRÜŞ KONİSİNDE" : "Hedef koni dışında"));
        GUI.Label(new Rect(32, 100, 300, 24), "Dış çarpım (Y): " + radar.YanTaraf.ToString("F2"));
        GUI.Label(new Rect(32, 126, 300, 24), "Scene > Gizmos: yönler ve görüş konisi");
    }
}
