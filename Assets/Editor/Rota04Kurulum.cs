using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class Rota04Kurulum
{
    private const string Path = "Assets/Scenes/Rota04.unity";
    static Rota04Kurulum() { EditorApplication.delayCall += Ilk; }
    private static void Ilk()
    {
        if (!File.Exists(Path) && File.Exists("Assets/UI/menu-arkaplan.png")) Kur();
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path == Path && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            var flight = Find(scene, "Nu_D36").GetComponent<SortieUcus>();
            var settings = new SerializedObject(flight);
            if (settings.FindProperty("engeller").intValue != 256)
            {
                settings.FindProperty("engeller").intValue = 256;
                settings.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(flight);
                PrefabUtility.RecordPrefabInstancePropertyModifications(flight);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene, Path);
            }
            SessionState.SetBool("Rota04ConfigReady", true);
        }
        if (scene.path == Path && !EditorApplication.isPlayingOrWillChangePlaymode && !SessionState.GetBool("Rota04GorselSon", false))
        {
            var ground = Find(scene, "UcusAlani");
            ground.GetComponent<Renderer>().sharedMaterial = Mat("SortieZemin", new Color(.35f, .39f, .23f));
            var turret = Find(scene, "Ucaksavar");
            if (turret != null) turret.transform.position = new Vector3(55, 0, -25);
            EditorSceneManager.SaveScene(scene, Path);
            AssetDatabase.SaveAssets();
            SessionState.SetBool("Rota04GorselSon", true);
        }
    }
    [MenuItem("Rota04/Ilk Sortie Sahnesini Kur")]
    public static void Kur()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/Rota03.unity", OpenSceneMode.Single);
        var aircraft = Find(scene, "Nu_D36");
        aircraft.transform.SetPositionAndRotation(new Vector3(0, .05f, 0), Quaternion.identity);
        PrefabUtility.RecordPrefabInstancePropertyModifications(aircraft.transform);
        aircraft.GetComponent<UcusKontrol>().enabled = false;
        foreach (var automatic in aircraft.GetComponentsInChildren<HedefeGit>()) automatic.enabled = false;
        var flight = aircraft.AddComponent<SortieUcus>();
        var flightSettings = new SerializedObject(flight);
        flightSettings.FindProperty("engeller").intValue = 256;
        flightSettings.ApplyModifiedPropertiesWithoutUndo();
        var fuel = aircraft.AddComponent<Yakit>();
        var sphere = aircraft.AddComponent<SphereCollider>();
        sphere.radius = 3;
        sphere.center = Vector3.up * 1.2f;
        var body = aircraft.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        var input = aircraft.GetComponent<PlayerInput>();
        var actions = Object.Instantiate(input.actions);
        actions.name = "Sortie_Actions";
        string inputPath = "Assets/Sortie_Actions.inputactions";
        File.WriteAllText(inputPath, actions.ToJson());
        AssetDatabase.ImportAsset(inputPath, ImportAssetOptions.ForceSynchronousImport);
        input.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(inputPath);
        input.defaultActionMap = "Ucus";
        input.defaultControlScheme = "Keyboard&Mouse";
        var oldHud = Find(scene, "PilotGostergeleri");
        if (oldHud != null) oldHud.SetActive(false);
        var balloon = Find(scene, "Balon");
        if (balloon != null) balloon.SetActive(false);
        var game = new GameObject("Oyun");
        var wallet = game.AddComponent<PuanCuzdani>();
        var mission = game.AddComponent<GorevYoneticisi>();
        game.AddComponent<YenidenBasla>();
        var baseObject = new GameObject("UsBolgesi");
        var baseCollider = baseObject.AddComponent<BoxCollider>();
        baseCollider.isTrigger = true;
        baseCollider.size = new Vector3(55, 220, 55);
        baseCollider.center = Vector3.up * 110;
        var baseScript = baseObject.AddComponent<UsBolgesi>();
        Bagla(baseScript, "gorev", mission);
        Bagla(mission, "usBolgesi", baseObject);
        Bagla(mission, "cuzdan", wallet);
        Bagla(mission, "saglik", aircraft.GetComponent<Saglik>());
        Bagla(mission, "yakit", fuel);
        var camera = Find(scene, "Main Camera");
        camera.GetComponent<TakipKamerasi>().enabled = false;
        camera.GetComponent<RotaKamera>().enabled = false;
        var follow = camera.AddComponent<SortieKamera>();
        Bagla(follow, "hedef", aircraft.transform);
        camera.transform.position = aircraft.transform.TransformPoint(new Vector3(0, 3.2f, -11));
        camera.transform.LookAt(aircraft.transform.position + Vector3.up * 1.2f);
        camera.GetComponent<Camera>().farClipPlane = 5000;
        camera.GetComponent<Camera>().backgroundColor = new Color(.69f, .75f, .55f);
        var hud = game.AddComponent<UcusHUD>();
        Bagla(hud, "ucus", flight); Bagla(hud, "gorev", mission); Bagla(hud, "cuzdan", wallet); Bagla(hud, "yakit", fuel); Bagla(hud, "us", baseObject.transform);
        string[] fields = { "cerceve", "hizKadrani", "yukseklikKadrani", "ibre", "gazPlaka", "gazTopuz", "pusula", "isaretler", "dugmeler", "tablo", "menu" };
        string[] textures = { "hud-cerceve", "gosterge-hiz", "gosterge-yukseklik", "gosterge-ibre", "gaz-kolu-plaka", "gaz-kolu-topuz", "pusula-serit", "hedef-isaretleri", "dugme-seti", "puan-tablosu", "menu-arkaplan" };
        for (int index = 0; index < fields.Length; index++)
        {
            string assetPath = "Assets/UI/" + textures[index] + ".png";
            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.wrapMode = textures[index] == "pusula-serit" ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
            importer.alphaIsTransparency = true;
            importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
            Bagla(hud, fields[index], AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath));
        }
        var cream = Mat("SortieKrem", new Color(.84f, .81f, .66f));
        var red = Mat("SortiePas", new Color(.58f, .22f, .16f));
        var dark = Mat("SortieKoyu", new Color(.21f, .23f, .16f));
        var water = Mat("BakirDeniz", new Color(.29f, .43f, .45f));
        var ground = Find(scene, "UcusAlani");
        ground.transform.localScale = new Vector3(400, 1, 400);
        Part("BakirDeniz", PrimitiveType.Cube, new Vector3(1350, -.3f, 900), new Vector3(2000, .1f, 4000), water, null, false);
        var first = Target("KontrolIstasyonu", new Vector3(0, 0, 400), false, cream, red);
        Target("DenizFeneri", new Vector3(-220, 0, 600), false, cream, red);
        Target("GozlemKulesi", new Vector3(250, 0, 750), false, cream, red);
        Target("YanDepo", new Vector3(160, 0, 450), true, dark, red);
        Target("YanKamp", new Vector3(-120, 0, 220), true, dark, red);
        Directory.CreateDirectory("Assets/Prefabs");
        PrefabUtility.SaveAsPrefabAsset(first, "Assets/Prefabs/Hedef.prefab");
        for (int index = 0; index < 18; index++)
        {
            float x = index % 2 == 0 ? -70 : 90;
            Part("Tarla_" + index, PrimitiveType.Cube, new Vector3(x, -.02f, index * 80), new Vector3(80, .02f, 68), index % 3 == 0 ? dark : cream, null, false);
        }
        Part("Hangar", PrimitiveType.Cube, new Vector3(-45, 5, 40), new Vector3(20, 10, 28), dark, null, true);
        Part("KontrolKulubesi", PrimitiveType.Cube, new Vector3(40, 3, 55), new Vector3(10, 6, 9), cream, null, true);
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(.69f, .75f, .55f);
        RenderSettings.fogStartDistance = 700;
        RenderSettings.fogEndDistance = 2600;
        EditorSceneManager.SaveScene(scene, Path);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Path, true), new EditorBuildSettingsScene("Assets/Scenes/Rota03.unity", true), new EditorBuildSettingsScene("Assets/Scenes/Rota02.unity", true), new EditorBuildSettingsScene("Assets/Scenes/Hangar01.unity", true) };
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = aircraft;
        if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.LookAt(aircraft.transform.position, Quaternion.Euler(18, 145, 0), 16);
        Debug.Log("ROTA04_HAZIR");
    }
    private static GameObject Target(string name, Vector3 position, bool optional, Material cream, Material red)
    {
        var root = new GameObject(name);
        root.transform.position = position;
        var collider = root.AddComponent<BoxCollider>();
        collider.isTrigger = true; collider.size = new Vector3(30, 180, 30); collider.center = Vector3.up * 90;
        var target = root.AddComponent<KesifHedefi>();
        var serialized = new SerializedObject(target);
        serialized.FindProperty("puan").intValue = optional ? 25 : 100;
        serialized.FindProperty("yanHedef").boolValue = optional;
        serialized.FindProperty("hedefAdi").stringValue = name;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        if (name == "DenizFeneri")
        {
            for (int index = 0; index < 7; index++) Part("FenerKat_" + index, PrimitiveType.Cylinder, position + Vector3.up * (2 + index * 4), new Vector3(7 - index * .4f, 2, 7 - index * .4f), index % 2 == 0 ? cream : red, root.transform, true);
            Part("FenerIsik", PrimitiveType.Sphere, position + Vector3.up * 31, new Vector3(5, 3, 5), cream, root.transform, true);
        }
        else Part("Bina", PrimitiveType.Cube, position + Vector3.up * (optional ? 3 : 6), new Vector3(optional ? 8 : 14, optional ? 6 : 12, optional ? 8 : 14), cream, root.transform, true);
        return root;
    }
    private static void Part(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, Transform parent, bool obstacle)
    {
        var obj = GameObject.CreatePrimitive(type); obj.name = name;
        obj.transform.position = position; obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        if (parent != null) obj.transform.SetParent(parent, true);
        if (obstacle) obj.layer = 8; else Object.DestroyImmediate(obj.GetComponent<Collider>());
    }
    private static Material Mat(string name, Color color)
    {
        var existing = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/" + name + ".mat");
        if (existing != null) return existing;
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color };
        AssetDatabase.CreateAsset(material, "Assets/Materials/" + name + ".mat"); return material;
    }
    private static GameObject Find(Scene scene, string name) { foreach (var root in scene.GetRootGameObjects()) if (root.name == name) return root; return null; }
    private static void Bagla(Object component, string field, Object value)
    {
        var serialized = new SerializedObject(component); serialized.FindProperty(field).objectReferenceValue = value; serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
