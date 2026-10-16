using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class Rota02Kontrol
{
    [MenuItem("Rota02/Ucus ve Radar Testlerini Calistir")]
    public static void Calistir()
    {
        SessionState.SetBool("Rota02Simulasyon", false);
        Kontrol();
    }

    private static void Kontrol()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && !SessionState.GetBool("Rota02Simulasyon", false))
        {
            var flight = UnityEngine.Object.FindFirstObjectByType<HedefeGit>();
            if (flight == null) return;
            RadarTesti();
            foreach (int fps in new[] { 30, 144 })
            {
                var clone = UnityEngine.Object.Instantiate(flight.gameObject);
                try
                {
                    var simulated = clone.GetComponent<HedefeGit>();
                    for (int frame = 0; frame < fps * 10; frame++) simulated.Ilerle(1f / fps);
                    if (simulated.Mesafe > 8.1f) throw new Exception("Varis testi basarisiz: " + simulated.Mesafe);
                    var position = simulated.transform.position;
                    for (int frame = 0; frame < fps; frame++) simulated.Ilerle(1f / fps);
                    if (Vector3.Distance(position, simulated.transform.position) > .01f) throw new Exception("Varista durmuyor");
                    Debug.Log("ROTA02_TEST_OK: " + fps + " FPS, hedefe varis ve durus; mesafe=" + simulated.Mesafe);
                }
                finally { UnityEngine.Object.DestroyImmediate(clone); }
            }
            Onizleme();
            SessionState.SetBool("Rota02Simulasyon", true);
            SessionState.SetBool("Rota02TestBitti", true);
        }
    }

    private static void RadarTesti()
    {
        var radarObject = new GameObject("GeciciRadarTest");
        var targetObject = new GameObject("GeciciHedefTest");
        try
        {
            var radar = radarObject.AddComponent<Radar>();
            var serialized = new SerializedObject(radar);
            serialized.FindProperty("hedef").objectReferenceValue = targetObject.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            targetObject.transform.position = new Vector3(0, 0, 30);
            radar.Degerlendir();
            if (!radar.Goruyor) throw new Exception("Onde hedef gorulmuyor");
            targetObject.transform.position = new Vector3(10, 0, 30);
            radar.Degerlendir();
            if (!radar.Goruyor || radar.YanTaraf <= 0) throw new Exception("Sag isaret hatasi");
            targetObject.transform.position = new Vector3(-10, 0, 30);
            radar.Degerlendir();
            if (radar.YanTaraf >= 0) throw new Exception("Sol isaret hatasi");
            targetObject.transform.position = new Vector3(0, 0, -30);
            radar.Degerlendir();
            if (radar.Goruyor) throw new Exception("Arkada hedef goruluyor");
            targetObject.transform.position = new Vector3(0, 0, 130);
            radar.Degerlendir();
            if (radar.Goruyor) throw new Exception("Menzil hatasi");
            serialized.Update();
            serialized.FindProperty("yarimAci").floatValue = 10;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            targetObject.transform.position = new Vector3(10, 0, 30);
            radar.Degerlendir();
            if (radar.Goruyor) throw new Exception("Dar koni hatasi");
            Debug.Log("ROTA02_RADAR_OK: on/arka, sag/sol, menzil ve dar koni dogrulandi.");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(radarObject);
            UnityEngine.Object.DestroyImmediate(targetObject);
        }
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
        File.WriteAllBytes(Path.Combine(Application.dataPath, "../Rota02Preview.png"), texture.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = previous;
        target.Release();
        UnityEngine.Object.DestroyImmediate(target);
        UnityEngine.Object.DestroyImmediate(texture);
    }
}
