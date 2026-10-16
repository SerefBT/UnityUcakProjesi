using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class Rota02Kurulum
{
    private const string ScenePath = "Assets/Scenes/Rota02.unity";

    static Rota02Kurulum()
    {
        EditorApplication.delayCall += IlkKurulum;
    }

    private static void IlkKurulum()
    {
        if (!File.Exists(ScenePath)) Kur();
    }

    [MenuItem("Rota02/Sahneyi Olustur")]
    public static void Kur()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (EditorSceneManager.GetActiveScene().isDirty) return;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Hangar01.unity");
        EditorSceneManager.SaveScene(scene, ScenePath);
        var aircraft = GameObject.Find("Nu_D36");
        aircraft.transform.position = new Vector3(0, 20, 0);
        var ground = GameObject.Find("Zemin_50m");
        ground.name = "UcusAlani";
        ground.transform.localScale = new Vector3(30, 1, 30);
        Object.DestroyImmediate(GameObject.Find("Olcek_Kupu_1m"));
        var balloonMaterial = Malzeme("BalonAltin", new Color(.95f, .52f, .08f));
        var metal = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Metal.mat");
        var balloon = Parca("Balon", PrimitiveType.Sphere, new Vector3(40, 30, 60), Vector3.one * 6, balloonMaterial);
        Parca("Sepet", PrimitiveType.Cube, new Vector3(40, 25.5f, 60), new Vector3(1.6f, 1, 1.6f), metal, balloon.transform);
        for (int side = -1; side <= 1; side += 2)
        {
            Parca("BalonHalati", PrimitiveType.Cylinder, new Vector3(40 + side * .65f, 26.9f, 60), new Vector3(.045f, 1.1f, .045f), metal, balloon.transform);
        }
        var turret = new GameObject("Ucaksavar");
        turret.transform.position = Vector3.zero;
        turret.transform.rotation = Quaternion.Euler(-25, 20, 0);
        var baseObject = Parca("UcaksavarTabani", PrimitiveType.Cylinder, new Vector3(0, .5f, 0), new Vector3(3, .5f, 3), metal);
        baseObject.transform.SetParent(turret.transform, true);
        var barrel = Parca("Namlu", PrimitiveType.Cube, Vector3.zero, new Vector3(.35f, .35f, 5), metal, turret.transform);
        barrel.transform.localPosition = new Vector3(0, 1, 2);
        barrel.transform.localRotation = Quaternion.identity;
        var flight = aircraft.AddComponent<HedefeGit>();
        Bagla(flight, "hedef", balloon.transform);
        var radar = turret.AddComponent<Radar>();
        Bagla(radar, "hedef", aircraft.transform);
        var camera = Camera.main;
        camera.transform.position = aircraft.transform.position + new Vector3(18, 10, -28);
        camera.transform.LookAt(aircraft.transform.position + Vector3.forward * 16);
        camera.fieldOfView = 65;
        camera.farClipPlane = 600;
        var follow = camera.gameObject.AddComponent<RotaKamera>();
        Bagla(follow, "ucak", aircraft.transform);
        var hud = new GameObject("RotaBilgi").AddComponent<RotaBilgi>();
        Bagla(hud, "ucus", flight);
        Bagla(hud, "radar", radar);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true), new EditorBuildSettingsScene("Assets/Scenes/Hangar01.unity", true) };
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = aircraft;
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.drawGizmos = true;
            SceneView.lastActiveSceneView.LookAt(new Vector3(15, 18, 25), Quaternion.Euler(27, -25, 0), 70);
        }
        Debug.Log("ROTA02_HAZIR: sahne kaydedildi; hedef ve radar baglantilari tamam.");
    }

    private static GameObject Parca(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, Transform parent = null)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        if (parent != null) obj.transform.SetParent(parent, true);
        return obj;
    }

    private static Material Malzeme(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void Bagla(Object component, string field, Object value)
    {
        var serialized = new SerializedObject(component);
        serialized.FindProperty(field).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
