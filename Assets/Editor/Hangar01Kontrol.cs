using System.IO;
using UnityEditor;
using UnityEngine;

public static class Hangar01Kontrol
{
    static Hangar01Kontrol()
    {
        EditorApplication.update += OynatmaKontrol;
    }

    private static float baslangicZamani;
    private static Quaternion baslangicDonusu;
    private static bool olcumBasladi;

    private static void OynatmaKontrol()
    {
        if (!SessionState.GetBool("Hangar01OynatmaTesti", false) || !EditorApplication.isPlaying) return;
        var propeller = Object.FindFirstObjectByType<PervaneDondur>();
        if (propeller == null) return;
        if (!olcumBasladi)
        {
            baslangicZamani = Time.realtimeSinceStartup;
            baslangicDonusu = propeller.transform.localRotation;
            olcumBasladi = true;
        }
        if (Time.realtimeSinceStartup - baslangicZamani < .31f) return;
        Debug.Log("Hangar01 Play testi: pervane aci degisimi=" + Quaternion.Angle(baslangicDonusu, propeller.transform.localRotation));
        SessionState.SetBool("Hangar01OynatmaTesti", false);
        EditorApplication.isPlaying = false;
    }

    [MenuItem("Hangar01/Sahneyi ve Pervaneyi Kontrol Et")]
    public static void Kontrol()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        olcumBasladi = false;
        var aircraft = GameObject.Find("Nu_D36");
        var camera = Camera.main;
        if (aircraft == null || camera == null) return;
        foreach (var materialGuid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" }))
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(materialGuid));
            switch (material.name)
            {
                case "Govde": material.color = new Color(.65f, .035f, .025f); break;
                case "Kanat": material.color = new Color(.91f, .81f, .58f); break;
                case "Metal": material.color = new Color(.24f, .28f, .32f); break;
                case "Lastik": material.color = new Color(.025f, .025f, .025f); break;
                case "Kokpit": material.color = new Color(.11f, .055f, .025f); break;
            }
            EditorUtility.SetDirty(material);
        }
        AssetDatabase.SaveAssets();
        var renderers = aircraft.GetComponentsInChildren<Renderer>();
        var bounds = renderers[0].bounds;
        foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
        Debug.Log("Hangar01 olculer: " + bounds.size + "; minY=" + bounds.min.y);
        var propeller = aircraft.GetComponentInChildren<PervaneDondur>();
        if (propeller == null) throw new System.Exception("Pervane betigi eksik");
        Debug.Log("Hangar01 pervane: " + propeller.transform.position + "; eksen=" + propeller.transform.forward);
        var target = new RenderTexture(1280, 720, 24);
        var previous = RenderTexture.active;
        camera.targetTexture = target;
        camera.Render();
        RenderTexture.active = target;
        var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
        image.Apply();
        File.WriteAllBytes(Path.Combine(Application.dataPath, "../Hangar01Preview.png"), image.EncodeToPNG());
        camera.targetTexture = null;
        RenderTexture.active = previous;
        Object.DestroyImmediate(image);
        target.Release();
        Object.DestroyImmediate(target);
        SessionState.SetBool("Hangar01KontrolRenkli", true);
        Debug.Log("Hangar01 kamera onizlemesi kaydedildi.");
        SessionState.SetBool("Hangar01OynatmaTesti", true);
        EditorApplication.isPlaying = true;
    }
}
