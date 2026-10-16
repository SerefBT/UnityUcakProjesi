using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class Rota04Kontrol
{
    private static int stage;
    private static float startTime;
    private static KesifHedefi[] targets;
    private static int targetIndex;
    [MenuItem("Rota04/Gorev Dongusunu Dogrula")]
    public static void Baslat()
    {
        stage = 0;
        targetIndex = 0;
        SessionState.SetBool("Rota04Started5", false);
        SessionState.SetBool("Rota04Checked5", false);
        EditorApplication.update -= Run;
        EditorApplication.update += Run;
    }
    [InitializeOnLoadMethod]
    private static void Devam()
    {
        if (SessionState.GetBool("Rota04Started5", false) && !SessionState.GetBool("Rota04Checked5", false))
            EditorApplication.update += Run;
    }
    private static void Run()
    {
        if (SessionState.GetBool("Rota04Checked5", false)) return;
        var flight = UnityEngine.Object.FindFirstObjectByType<SortieUcus>();
        if (flight == null) return;
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (!SessionState.GetBool("Rota04ConfigReady", false)) return;
            if (SessionState.GetBool("Rota04Started5", false)) return;
            SessionState.SetBool("Rota04Started5", true);
            var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
            var view = EditorWindow.GetWindow(type);
            var property = type.GetProperty("drawGizmos", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property != null && property.CanWrite) property.SetValue(view, false);
            var field = type.GetField("m_Gizmos", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null) field.SetValue(view, false);
            view.Focus();
            EditorApplication.isPlaying = true;
            return;
        }
        if (!EditorApplication.isPlaying || Time.time < .15f) return;
        try
        {
            var mission = GorevYoneticisi.Aktif;
            var wallet = UnityEngine.Object.FindFirstObjectByType<PuanCuzdani>();
            if (stage == 0)
            {
                Application.runInBackground = true;
                Check(mission.AnaToplam == 3 && mission.YanToplam == 2, "3 ana 2 yan hedef");
                targets = UnityEngine.Object.FindObjectsByType<KesifHedefi>(FindObjectsSortMode.None);
                mission.Baslat();
                Set(flight, "yunuslama", -1f);
                ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(Application.dataPath, "../Rota04HUD.png"));
                Next(); return;
            }
            float elapsed = Time.realtimeSinceStartup - startTime;
            if (stage == 1)
            {
                if (elapsed < 3f) return;
                Check(!flight.Yerde && flight.Hiz >= 26, "W ile otomatik kalkis");
                Check(flight.GetComponent<Yakit>().Oran < 1, "Yakit tuketimi");
                Set(flight, "yunuslama", 0f);
                flight.enabled = false;
                Move(flight, targets[0].transform.position + Vector3.up * 35);
                Next(); return;
            }
            if (stage == 2)
            {
                if (elapsed < .3f) return;
                Check(targets[targetIndex].Alindi, "Fizik tetikleyicisi " + targetIndex);
                targets[targetIndex].FotoCek();
                targetIndex++;
                if (targetIndex < targets.Length) { Move(flight, targets[targetIndex].transform.position + Vector3.up * 35); startTime = Time.realtimeSinceStartup; return; }
                Check(wallet.Puan == 350 && mission.DonusFazi, "350 puan ve donus fazi");
                Move(flight, new Vector3(0, 30, 0));
                Next(); return;
            }
            if (stage == 3)
            {
                if (elapsed < .3f) return;
                Check(mission.GorevTamam && wallet.Puan == 420, "Usten gecis ve 420 toplam puan");
                Check(Mathf.Approximately(flight.GetComponent<Yakit>().Oran, 1), "Uste yakit dolumu");
                ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(Application.dataPath, "../Rota04Sonuc.png"));
                Next(); return;
            }
            if (stage == 4)
            {
                if (elapsed < .5f) return;
                YenidenBasla.Yenile(); Next(); return;
            }
            if (stage == 5)
            {
                if (elapsed < .6f) return;
                Check(!mission.Basladi && wallet.Puan == 0, "Yeni sortie sifirlama");
                mission.Baslat();
                typeof(GorevYoneticisi).GetField("<KalanSure>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(mission, .1f);
                Next(); return;
            }
            if (stage == 6)
            {
                if (elapsed < .5f) return;
                Check(mission.Basarisiz && mission.Sebep == "Süre doldu", "Sure bitisi");
                Next(); return;
            }
            if (stage == 7)
            {
                if (elapsed < 3f) return;
                Check(!mission.Basladi && wallet.Puan == 0, "Sure bitince otomatik yeniden baslama");
                mission.Baslat();
                Set(flight, "kalkti", true);
                Move(flight, new Vector3(-45, 5, 40));
                Next(); return;
            }
            if (stage == 8)
            {
                if (elapsed < .3f) return;
                Check(flight.Carpti && mission.Basarisiz, "Engel taramasi ve cakilma pos=" + flight.transform.position + ", enabled=" + flight.enabled + ", overlap=" + Physics.CheckSphere(flight.transform.position + Vector3.up * 1.2f, 2.4f, 256, QueryTriggerInteraction.Ignore));
                Debug.Log("ROTA04_TEST_OK: kalkis, yakit, fizik checkpoint, puan, donus, sure, restart ve carpma.");
                SessionState.SetBool("Rota04Checked5", true);
                EditorApplication.isPlaying = false;
            }
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            SessionState.SetBool("Rota04Checked5", true);
            EditorApplication.isPlaying = false;
        }
    }
    private static void Move(SortieUcus flight, Vector3 position) { flight.transform.position = position; flight.GetComponent<Rigidbody>().position = position; Physics.SyncTransforms(); }
    private static void Set(object target, string field, object value) => target.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, value);
    private static void Next() { stage++; startTime = Time.realtimeSinceStartup; }
    private static void Check(bool condition, string name) { if (!condition) throw new Exception("ROTA04_TEST_FAIL: " + name); Debug.Log("ROTA04_CHECK_OK: " + name); }
}
