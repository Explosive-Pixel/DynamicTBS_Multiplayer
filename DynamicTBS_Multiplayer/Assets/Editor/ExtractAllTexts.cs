using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;
using System.Linq;

public class ExtractAllTexts : EditorWindow
{
    private static readonly string scenesFolder = "Assets/Scenes";
    private static readonly string prefabsFolder = "Assets/Prefabs";

    [MenuItem("Tools/Extract All Texts (Scenes + Prefabs)")]
    public static void ShowWindow()
    {
        GetWindow<ExtractAllTexts>("Extract All Texts");
    }

    void OnGUI()
    {
        GUILayout.Label("Extract All Texts", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"This scans all prefabs in '{prefabsFolder}' and all scenes in '{scenesFolder}' for text components and exports them to a CSV file.", MessageType.Info);

        if (GUILayout.Button("Extract Texts"))
        {
            string outputPath = EditorUtility.SaveFilePanel("Save Text Report", "", "ExtractedTexts.csv", "csv");
            if (!string.IsNullOrEmpty(outputPath))
            {
                ExtractTexts(outputPath);
            }
        }
    }

    void ExtractTexts(string outputPath)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Source Type,Source Path,GameObject Hierarchy,Text Type,Text");

        int count = 0;

        // --- 1. Scan Prefabs ---
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsFolder });
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                foreach (var entry in ExtractTextsFromGameObject(prefab))
                {
                    sb.AppendLine($"Prefab,\"{path}\",\"{entry.hierarchy}\",{entry.textType},\"{Sanitize(entry.text)}\"");
                    count++;
                }
            }
        }

        // --- 2. Scan Scenes ---
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { scenesFolder });
        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var entry in ExtractTextsFromGameObject(root))
                {
                    sb.AppendLine($"Scene,\"{path}\",\"{entry.hierarchy}\",{entry.textType},\"{Sanitize(entry.text)}\"");
                    count++;
                }
            }
        }

        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"[Extract Texts] Extracted {count} text components to:\n{outputPath}");
    }

    struct TextEntry
    {
        public string hierarchy;
        public string text;
        public string textType;
    }

    static TextEntry[] ExtractTextsFromGameObject(GameObject go)
    {
        var list = new System.Collections.Generic.List<TextEntry>();

        // Legacy UI Text
        foreach (var text in go.GetComponentsInChildren<Text>(true))
        {
            list.Add(new TextEntry
            {
                hierarchy = GetFullHierarchyPath(text.gameObject),
                text = text.text,
                textType = "Legacy UI Text"
            });
        }

        // TMP UI Text
        foreach (var tmp in go.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            list.Add(new TextEntry
            {
                hierarchy = GetFullHierarchyPath(tmp.gameObject),
                text = tmp.text,
                textType = "TextMeshProUGUI"
            });
        }

        // TMP 3D Text
        foreach (var tmp in go.GetComponentsInChildren<TextMeshPro>(true))
        {
            list.Add(new TextEntry
            {
                hierarchy = GetFullHierarchyPath(tmp.gameObject),
                text = tmp.text,
                textType = "TextMeshPro (3D)"
            });
        }

        return list.ToArray();
    }

    static string GetFullHierarchyPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return path;
    }

    static string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("\"", "\"\"").Replace("\n", "\\n").Replace("\r", "");
    }
}
