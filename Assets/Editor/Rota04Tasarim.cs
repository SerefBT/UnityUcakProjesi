using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class Rota04Tasarim
{
    static Rota04Tasarim() { EditorApplication.delayCall += Uygula; }

    [MenuItem("Rota04/Tasarim Son Rotuslarini Uygula")]
    public static void Uygula()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/Rota04.unity" || GameObject.Find("TasarimSonRotuslar") != null) return;
        var group = new GameObject("TasarimSonRotuslar");
        var brass = Material("SortiePirinc", new Color(.56f, .43f, .26f));
        var dark = Material("SortiePencere", new Color(.12f, .19f, .19f));
        var cream = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/SortieKrem.mat");
        foreach (var target in Object.FindObjectsByType<KesifHedefi>(FindObjectsSortMode.None))
        {
            string label = target.name;
            switch (target.name)
            {
                case "KontrolIstasyonu": label = "Kontrol istasyonu"; break;
                case "DenizFeneri": label = "Deniz feneri"; break;
                case "GozlemKulesi": label = "Gözlem kulesi"; break;
                case "YanDepo": label = "İkmal deposu"; break;
                case "YanKamp": label = "Kıyı kampı"; break;
            }
            var settings = new SerializedObject(target);
            settings.FindProperty("hedefAdi").stringValue = label;
            settings.ApplyModifiedPropertiesWithoutUndo();
            if (target.name == "DenizFeneri") continue;
            var building = target.transform.Find("Bina");
            if (building == null) continue;
            Vector3 size = building.localScale;
            float top = building.localPosition.y + size.y / 2;
            Part("Cati", target.transform, new Vector3(0, top + .4f, 0), new Vector3(size.x + 1, .8f, size.z + 1), brass);
            Part("PencereSeridi", target.transform, new Vector3(0, top - 2, -size.z / 2 - .03f), new Vector3(size.x * .75f, 1.2f, .08f), dark);
            Part("Kapi", target.transform, new Vector3(0, 1.5f, -size.z / 2 - .06f), new Vector3(1.8f, 3, .1f), dark);
            if (target.name == "GozlemKulesi")
            {
                Part("GozlemKabini", target.transform, new Vector3(0, top + 3, 0), new Vector3(7, 5, 7), cream);
                Part("KabinCami", target.transform, new Vector3(0, top + 3.5f, -3.55f), new Vector3(6, 1.7f, .1f), dark);
                Part("KabinCatisi", target.transform, new Vector3(0, top + 5.8f, 0), new Vector3(9, .6f, 9), brass);
            }
        }
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root.name != "Hangar" && root.name != "KontrolKulubesi") continue;
            Vector3 size = root.transform.localScale;
            Part(root.name + "Cati", group.transform, root.transform.position + Vector3.up * (size.y / 2 + .4f), new Vector3(size.x + 1, .8f, size.z + 1), brass);
            if (root.name == "Hangar")
                Part("HangarKapisi", group.transform, root.transform.position + new Vector3(size.x / 2 + .06f, -1, 0), new Vector3(.1f, 7, 18), dark);
        }
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("ROTA04_TASARIM_OK: okunakli etiketler, cati, pencere, gozlem kabini ve hangar detaylari kaydedildi.");
    }

    private static Material Material(string name, Color color)
    {
        string path = "Assets/Materials/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject Part(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }
}
