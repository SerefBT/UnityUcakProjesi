using UnityEngine;
using UnityEngine.InputSystem;

public class PilotBilgi : MonoBehaviour
{
    [SerializeField] private UcusKontrol ucus;
    [SerializeField] private CanYazisi canYazisi;
    [SerializeField] private PlayerInput oyuncu;
    [SerializeField] private Transform balon;
    private GUIStyle baslik;
    private GUIStyle metin;

    private void OnGUI()
    {
        if (ucus == null || canYazisi == null || oyuncu == null) return;
        if (baslik == null)
        {
            baslik = new GUIStyle(GUI.skin.label) { fontSize = 19, fontStyle = FontStyle.Bold };
            baslik.normal.textColor = new Color(1f, .8f, .4f);
            metin = new GUIStyle(GUI.skin.label) { fontSize = 15 };
        }
        GUI.Box(new Rect(18, 18, 310, 212), GUIContent.none);
        GUI.Label(new Rect(34, 28, 290, 30), "PİLOT KOLTUĞU | Nu.D.36", baslik);
        GUI.Label(new Rect(34, 66, 285, 25), "HIZ  " + ucus.Hiz.ToString("F0") + " m/s       GAZ  %" + (ucus.GazOrani * 100).ToString("F0"), metin);
        GUI.Label(new Rect(34, 96, 285, 25), "İRTİFA  " + ucus.transform.position.y.ToString("F0") + " m", metin);
        GUI.Label(new Rect(34, 126, 285, 25), "CAN  " + canYazisi.GosterilenCan + " / " + canYazisi.GosterilenEnCokCan, metin);
        Color onceki = GUI.color;
        GUI.color = canYazisi.GosterilenCan > 30 ? new Color(.3f, .85f, .55f) : new Color(1, .3f, .2f);
        GUI.DrawTexture(new Rect(34, 159, 274f * canYazisi.GosterilenCan / Mathf.Max(1, canYazisi.GosterilenEnCokCan), 8), Texture2D.whiteTexture);
        GUI.color = onceki;
        if (balon != null)
            GUI.Label(new Rect(34, 185, 285, 25), "BALON  " + Vector3.Distance(ucus.transform.position, balon.position).ToString("F0") + " m", metin);
        GUI.Box(new Rect(18, Screen.height - 124, 510, 106), GUIContent.none);
        GUI.Label(new Rect(34, Screen.height - 116, 480, 25), "Yunuslama: " + Tuslar("Yunuslama") + "    Yatış: " + Tuslar("Yatis"), metin);
        GUI.Label(new Rect(34, Screen.height - 86, 480, 25), "Sapma: " + Tuslar("Sapma") + "    Gaz: " + Tuslar("Gaz"), metin);
        GUI.Label(new Rect(34, Screen.height - 56, 480, 25), "Hasar denemesi: " + Tuslar("Hasar") + " (-10 can)", metin);
        if (canYazisi.GosterilenCan == 0)
            GUI.Label(new Rect(Screen.width / 2f - 170, 32, 340, 35), "Can tükendi — hasar olayı tamamlandı", metin);
    }

    private string Tuslar(string eylem)
    {
        var action = oyuncu.actions.FindAction("Ucus/" + eylem);
        return action == null ? "—" : action.GetBindingDisplayString();
    }
}
