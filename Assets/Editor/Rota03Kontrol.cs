using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class Rota03Kontrol
{
    private static Keyboard keyboard;
    private static Mouse mouse;
    private static int adim;
    private static float zaman;
    private static float gorus;

    [MenuItem("Rota03/Pilot Kontrollerini Dogrula")]
    public static void Baslat()
    {
        SessionState.SetBool("Rota03TestBitti3", false);
        SessionState.SetBool("Rota03TestBasladi3", false);
        adim = 0;
        EditorApplication.update -= Kontrol;
        EditorApplication.update += Kontrol;
    }

    [InitializeOnLoadMethod]
    private static void OynatmaDevam()
    {
        if (SessionState.GetBool("Rota03TestBasladi3", false) && !SessionState.GetBool("Rota03TestBitti3", false))
            EditorApplication.update += Kontrol;
    }

    private static void Kontrol()
    {
        if (SessionState.GetBool("Rota03TestBitti3", false)) return;
        var flight = UnityEngine.Object.FindFirstObjectByType<UcusKontrol>();
        if (flight == null) return;
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (SessionState.GetBool("Rota03TestBasladi3", false)) return;
            SessionState.SetBool("Rota03TestBasladi3", true);
            var gameView = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
            EditorWindow.GetWindow(gameView).Focus();
            EditorApplication.isPlaying = true;
            return;
        }
        if (!EditorApplication.isPlaying || Time.time < .2f) return;
        try
        {
            var player = flight.GetComponent<PlayerInput>();
            if (adim == 0)
            {
                Application.runInBackground = true;
                keyboard = InputSystem.AddDevice<Keyboard>();
                mouse = InputSystem.AddDevice<Mouse>();
                player.neverAutoSwitchControlSchemes = true;
                player.SwitchCurrentControlScheme("Keyboard&Mouse", keyboard, mouse);
                Dogrula(player.currentActionMap.name == "Ucus", "PlayerInput haritasi");
                Dogrula(!flight.GetComponentInChildren<HedefeGit>().enabled, "Otopilot kapali");
                Tus(Key.W);
                Sonraki();
                return;
            }
            if (Time.unscaledTime - zaman < (adim == 14 ? 1f : .35f)) return;
            switch (adim)
            {
                case 1:
                    Dogrula(flight.transform.forward.y > 0, "W yukselis");
                    Tus();
                    break;
                case 2: Tus(Key.S); break;
                case 3:
                    Tus(Key.A);
                    break;
                case 4:
                    Tus(Key.D);
                    break;
                case 5:
                    Tus(Key.Q);
                    break;
                case 6:
                    Tus(Key.E);
                    break;
                case 7:
                    Tus(Key.UpArrow);
                    break;
                case 8:
                    Dogrula(Mathf.Abs(flight.GazOrani - .52f) < .001f, "Gaz artis olayi");
                    break;
                case 9:
                    Dogrula(Mathf.Abs(flight.GazOrani - .52f) < .001f, "Gaz foydeki gibi basili tutarken sabit");
                    Tus(Key.DownArrow);
                    break;
                case 10:
                    Dogrula(Mathf.Abs(flight.GazOrani - .5f) < .001f, "Gaz azalis olayi");
                    Tus(Key.H);
                    break;
                case 11:
                    var health = flight.GetComponent<Saglik>();
                    var healthText = UnityEngine.Object.FindFirstObjectByType<CanYazisi>();
                    Dogrula(health.Can == 90 && healthText.GosterilenCan == 90, "H hasar ve can dinleyicisi");
                    healthText.enabled = false;
                    health.Vur(10);
                    Dogrula(healthText.GosterilenCan == 90, "OnDisable abonelik iptali");
                    healthText.enabled = true;
                    Dogrula(healthText.GosterilenCan == 80, "OnEnable can senkronizasyonu");
                    int olum = 0;
                    Action onDeath = () => olum++;
                    health.Oldu += onDeath;
                    health.Vur(1000);
                    health.Vur(10);
                    health.Oldu -= onDeath;
                    Dogrula(health.Can == 0 && olum == 1, "Oldu olayi bir kez");
                    Tus();
                    break;
                case 12:
                    gorus = Camera.main.fieldOfView;
                    typeof(UcusKontrol).GetField("gaz", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(flight, 1f);
                    break;
                case 13: break;
                case 14:
                    Dogrula(Camera.main.fieldOfView > gorus + 3, "Gazla gorus acisi artisi");
                    Dogrula(Mathf.Abs(flight.Hiz - 42) < .01f, "Maksimum hiz");
                    Onizleme();
                    Debug.Log("ROTA03_TEST_OK: klavye eslemeleri, W yukselis, gaz, saglik olaylari, abonelik ve kamera dogrulandi.");
                    Bitir();
                    return;
            }
            Sonraki();
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            Bitir();
        }
    }

    private static void Tus(params Key[] keys)
    {
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(keys));
        InputSystem.Update();
        if (keys.Length == 0) return;
        string action = null;
        float expected = 0;
        switch (keys[0])
        {
            case Key.W: action = "Yunuslama"; expected = -1; break;
            case Key.S: action = "Yunuslama"; expected = 1; break;
            case Key.A: action = "Yatis"; expected = -1; break;
            case Key.D: action = "Yatis"; expected = 1; break;
            case Key.Q: action = "Sapma"; expected = -1; break;
            case Key.E: action = "Sapma"; expected = 1; break;
            case Key.UpArrow: action = "Gaz"; expected = 1; break;
            case Key.DownArrow: action = "Gaz"; expected = -1; break;
            case Key.H: action = "Hasar"; expected = 1; break;
        }
        var player = UnityEngine.Object.FindFirstObjectByType<PlayerInput>();
        Dogrula(Mathf.Abs(player.actions["Ucus/" + action].ReadValue<float>() - expected) < .001f, "Esleme " + keys[0]);
    }

    private static void Sonraki()
    {
        adim++;
        zaman = Time.unscaledTime;
    }

    private static void Dogrula(bool condition, string name)
    {
        if (!condition) throw new Exception("ROTA03_TEST_FAIL: " + name);
        Debug.Log("ROTA03_CHECK_OK: " + name);
    }

    private static void Bitir()
    {
        if (keyboard != null) InputSystem.RemoveDevice(keyboard);
        if (mouse != null) InputSystem.RemoveDevice(mouse);
        SessionState.SetBool("Rota03TestBitti3", true);
        EditorApplication.isPlaying = false;
    }

    private static void Onizleme()
    {
        var camera = Camera.main;
        var target = new RenderTexture(1280, 720, 24);
        var previous = RenderTexture.active;
        camera.targetTexture = target;
        camera.Render();
        RenderTexture.active = target;
        var texture = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        texture.Apply();
        File.WriteAllBytes(Path.Combine(Application.dataPath, "../Rota03Preview.png"), texture.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = previous;
        target.Release();
        UnityEngine.Object.DestroyImmediate(target);
        UnityEngine.Object.DestroyImmediate(texture);
    }
}
