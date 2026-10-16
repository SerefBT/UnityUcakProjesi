using UnityEngine;
using UnityEngine.SceneManagement;
public class YenidenBasla : MonoBehaviour
{
    [SerializeField] private float bekleme = 2.5f;
    private float sayac = -1;
    private void OnEnable() => GorevYoneticisi.SortieBasarisiz += Basarisizlik;
    private void OnDisable() => GorevYoneticisi.SortieBasarisiz -= Basarisizlik;
    private void Basarisizlik(string reason) { if (sayac < 0) sayac = bekleme; }
    private void Update() { if (sayac < 0) return; sayac -= Time.deltaTime; if (sayac <= 0) Yenile(); }
    public static void Yenile() => SceneManager.LoadScene(SceneManager.GetActiveScene().path);
}
