using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class Hangar01Kurulum
{
    private const string ScenePath = "Assets/Scenes/Hangar01.unity";

    static Hangar01Kurulum()
    {
        EditorApplication.delayCall += IlkKurulum;
    }

    private static void IlkKurulum()
    {
        if (!File.Exists(ScenePath)) Kur();
    }

    [MenuItem("Hangar01/Sahneyi Olustur")]
    public static void Kur()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var model = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Nu_D36.fbx");
        if (model == null) return;
        if (!Application.isBatchMode && EditorSceneManager.GetActiveScene().isDirty) return;
        Directory.CreateDirectory("Assets/Materials");
        Directory.CreateDirectory("Assets/Scenes");
        var importer = (ModelImporter)AssetImporter.GetAtPath("Assets/Models/Nu_D36.fbx");
        importer.globalScale = 1;
        importer.useFileScale = true;
        foreach (var asset in AssetDatabase.LoadAllAssetsAtPath("Assets/Models/Nu_D36.fbx"))
        {
            if (!(asset is Material original)) continue;
            string path = "Assets/Materials/" + original.name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                material = new Material(shader) { name = original.name, color = original.color };
                AssetDatabase.CreateAsset(material, path);
            }
            importer.AddRemap(new AssetImporter.SourceAssetIdentifier(original), material);
        }
        importer.SaveAndReimport();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var aircraft = (GameObject)PrefabUtility.InstantiatePrefab(model);
        aircraft.name = "Nu_D36";
        aircraft.transform.position = Vector3.zero;
        foreach (var child in aircraft.GetComponentsInChildren<Transform>())
        {
            if (child.name != "Pervane") continue;
            child.gameObject.AddComponent<PervaneDondur>();
        }
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Zemin_50m";
        ground.transform.localScale = new Vector3(5, 1, 5);
        var groundMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
        groundMaterial.color = new Color(.23f, .29f, .25f);
        AssetDatabase.CreateAsset(groundMaterial, "Assets/Materials/Zemin.mat");
        ground.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "Olcek_Kupu_1m";
        cube.transform.position = new Vector3(6, .5f, 0);
        var lightObject = new GameObject("Directional Light", typeof(Light));
        var light = lightObject.GetComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 2;
        light.shadows = LightShadows.Soft;
        lightObject.transform.rotation = Quaternion.Euler(45, -35, 0);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(.55f, .6f, .7f);
        var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(11, 7, 12);
        cameraObject.transform.LookAt(new Vector3(0, 1, 0));
        var camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.48f, .68f, .83f);
        camera.fieldOfView = 48;
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = aircraft;
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.LookAt(new Vector3(0, 1, 0), Quaternion.Euler(23, 140, 0), 12);
        Debug.Log("Hangar01 hazir: model, malzemeler, pervane betigi, zemin ve kamera kaydedildi.");
    }
}
