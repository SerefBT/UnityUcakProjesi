using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public static class Rota03Kurulum
{
    private const string ScenePath = "Assets/Scenes/Rota03.unity";
    private const string InputPath = "Assets/InputSystem_Actions.inputactions";

    static Rota03Kurulum()
    {
        EditorApplication.delayCall += IlkKurulum;
    }

    private static void IlkKurulum()
    {
        var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
        var flightMap = actions == null ? null : actions.FindActionMap("Ucus");
        bool changed = false;
        if (flightMap != null)
        {
            foreach (var action in flightMap.actions)
                for (int index = 0; index < action.bindings.Count; index++)
                    if (action.bindings[index].groups != "Keyboard&Mouse")
                    {
                        action.ChangeBinding(index).WithGroup("Keyboard&Mouse");
                        changed = true;
                    }
        }
        if (changed)
        {
            File.WriteAllText(InputPath, actions.ToJson());
            AssetDatabase.ImportAsset(InputPath, ImportAssetOptions.ForceSynchronousImport);
        }
        if (!File.Exists(ScenePath) || !File.ReadAllText(ScenePath).Contains("PilotGostergeleri")) Kur();
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path == ScenePath && !SessionState.GetBool("Rota03HiyerarsiDuzeltildi", false))
        {
            var aircraft = Nesne(scene, "Nu_D36");
            HiyerarsiDuzelt(aircraft);
            EditorSceneManager.SaveScene(scene, ScenePath);
            SessionState.SetBool("Rota03HiyerarsiDuzeltildi", true);
            var renderers = aircraft.GetComponentsInChildren<Renderer>();
            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers) bounds.Encapsulate(renderer.bounds);
            Debug.Log("ROTA03_MODEL_OK: root=" + aircraft.transform.position + "; bounds=" + bounds.size + "; minY=" + bounds.min.y);
            Onizleme(Nesne(scene, "Main Camera").GetComponent<Camera>());
        }
    }

    [MenuItem("Rota03/Pilot Sahnesini Olustur")]
    public static void Kur()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
        if (actions == null) return;
        if (actions.FindActionMap("Ucus") == null)
        {
            var map = actions.AddActionMap("Ucus");
            Eksen(map, "Yunuslama", "s", "w", "scale(factor=-1)");
            Eksen(map, "Yatis", "a", "d");
            Eksen(map, "Sapma", "q", "e");
            Eksen(map, "Gaz", "downArrow", "upArrow");
            map.AddAction("Hasar", InputActionType.Button, "<Keyboard>/h", groups: "Keyboard&Mouse");
            File.WriteAllText(InputPath, actions.ToJson());
            AssetDatabase.ImportAsset(InputPath, ImportAssetOptions.ForceSynchronousImport);
            actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputPath);
        }
        bool dirty = EditorSceneManager.GetActiveScene().isDirty;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Rota02.unity", dirty ? OpenSceneMode.Additive : OpenSceneMode.Single);
        EditorSceneManager.SetActiveScene(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        var aircraft = Nesne(scene, "Nu_D36");
        aircraft.transform.SetPositionAndRotation(new Vector3(0, 20, 0), Quaternion.identity);
        foreach (var autopilot in aircraft.GetComponentsInChildren<HedefeGit>()) autopilot.enabled = false;
        HiyerarsiDuzelt(aircraft);
        var flight = aircraft.AddComponent<UcusKontrol>();
        var health = aircraft.AddComponent<Saglik>();
        aircraft.AddComponent<HasarDeneme>();
        var input = aircraft.AddComponent<PlayerInput>();
        input.actions = actions;
        input.defaultActionMap = "Ucus";
        input.notificationBehavior = PlayerNotifications.SendMessages;
        var oldHud = Nesne(scene, "RotaBilgi");
        if (oldHud != null) Object.DestroyImmediate(oldHud);
        var hud = new GameObject("PilotGostergeleri");
        EditorSceneManager.MoveGameObjectToScene(hud, scene);
        var healthText = hud.AddComponent<CanYazisi>();
        Bagla(healthText, "saglik", health);
        var info = hud.AddComponent<PilotBilgi>();
        Bagla(info, "ucus", flight);
        Bagla(info, "canYazisi", healthText);
        Bagla(info, "oyuncu", input);
        Bagla(info, "balon", Nesne(scene, "Balon").transform);
        var cameraObject = Nesne(scene, "Main Camera");
        cameraObject.GetComponent<RotaKamera>().enabled = false;
        var follow = cameraObject.AddComponent<TakipKamerasi>();
        Bagla(follow, "hedef", aircraft.transform);
        cameraObject.transform.position = aircraft.transform.TransformPoint(new Vector3(0, 3.2f, -11));
        cameraObject.transform.LookAt(aircraft.transform.position + aircraft.transform.up * 1.2f, aircraft.transform.up);
        cameraObject.GetComponent<Camera>().fieldOfView = 63.5f;
        var ground = Nesne(scene, "UcusAlani");
        ground.transform.localScale = new Vector3(160, 1, 160);
        var runwayMaterial = Malzeme("Pist", new Color(.13f, .16f, .19f));
        var markingMaterial = Malzeme("PistCizgi", new Color(.9f, .85f, .65f));
        Parca(scene, "Pist", new Vector3(0, .02f, 130), new Vector3(18, .04f, 360), runwayMaterial);
        for (int index = 0; index < 18; index++)
            Parca(scene, "PistCizgi_" + index, new Vector3(0, .05f, -30 + index * 19), new Vector3(.35f, .02f, 8), markingMaterial);
        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true),
            new EditorBuildSettingsScene("Assets/Scenes/Rota02.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Hangar01.unity", true)
        };
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = aircraft;
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.LookAt(aircraft.transform.position, Quaternion.Euler(18, 145, 0), 16);
        Debug.Log("ROTA03_HAZIR: manuel ucus, input eslemeleri, saglik olaylari ve kamera kaydedildi.");
    }

    private static void Eksen(InputActionMap map, string name, string negative, string positive, string processors = null)
    {
        var action = map.AddAction(name, InputActionType.Value, processors: processors);
        action.expectedControlType = "Axis";
        action.AddCompositeBinding("1DAxis").With("Negative", "<Keyboard>/" + negative, "Keyboard&Mouse").With("Positive", "<Keyboard>/" + positive, "Keyboard&Mouse");
    }

    private static void HiyerarsiDuzelt(GameObject aircraft)
    {
        foreach (var autopilot in aircraft.GetComponentsInChildren<HedefeGit>())
        {
            autopilot.enabled = false;
            if (autopilot.transform == aircraft.transform) continue;
            var source = PrefabUtility.GetCorrespondingObjectFromSource(autopilot.transform);
            if (source == null) continue;
            autopilot.transform.localPosition = source.localPosition;
            autopilot.transform.localRotation = source.localRotation;
        }
    }

    private static void Onizleme(Camera camera)
    {
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
        Object.DestroyImmediate(target);
        Object.DestroyImmediate(texture);
    }

    private static GameObject Nesne(UnityEngine.SceneManagement.Scene scene, string name)
    {
        foreach (var root in scene.GetRootGameObjects())
            if (root.name == name) return root;
        return null;
    }

    private static void Bagla(Object component, string field, Object value)
    {
        var serialized = new SerializedObject(component);
        serialized.FindProperty(field).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
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

    private static void Parca(UnityEngine.SceneManagement.Scene scene, string name, Vector3 position, Vector3 scale, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        EditorSceneManager.MoveGameObjectToScene(obj, scene);
    }
}
