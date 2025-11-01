using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ExtractLocalizationTexts : EditorWindow
{
    private static readonly string scenesFolder = "Assets/Scenes";
    private static readonly string prefabsFolder = "Assets/Prefabs";

    [MenuItem("Tools/Localization/Extract Translatable Texts")]
    public static void ShowWindow()
    {
        GetWindow<ExtractLocalizationTexts>("Extract Translatable Texts");
    }

    void OnGUI()
    {
        GUILayout.Label("Extract Translatable Texts", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"Scans all prefabs in '{prefabsFolder}' and scenes in '{scenesFolder}' for text content that likely needs localization. " +
                                $"Numbers and empty fields are ignored. Prefabs are only included once.", MessageType.Info);

        if (GUILayout.Button("Extract Texts for Localization"))
        {
            string outputPath = EditorUtility.SaveFilePanel("Save Localization CSV", "", "LocalizationTexts.csv", "csv");
            if (!string.IsNullOrEmpty(outputPath))
            {
                ExtractTexts(outputPath);
            }
        }
    }

    void ExtractTexts(string outputPath)
    {
        var uniqueTexts = new HashSet<string>();
        var prefabPathsProcessed = new HashSet<string>();
        var resultEntries = new List<(string key, string english)>();
        int skippedNumeric = 0;

        // --- Helper local function to process GameObjects ---
        void ProcessGameObject(GameObject go, string sourcePath)
        {
            foreach (var entry in ExtractTextEntries(go))
            {
                if (IsNumeric(entry.text)) { skippedNumeric++; continue; }

                if (uniqueTexts.Add(entry.text))
                {
                    string key = GenerateLocalizationKey(entry.text, sourcePath, entry.hierarchy);
                    resultEntries.Add((key, entry.text));
                }
            }
        }

        // --- 1. Prefabs ---
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsFolder });
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (prefabPathsProcessed.Contains(path)) continue;

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                ProcessGameObject(prefab, path);
                prefabPathsProcessed.Add(path);
            }
        }

        // --- 2. Scenes ---
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { scenesFolder });
        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

            foreach (var root in scene.GetRootGameObjects())
            {
                ProcessGameObject(root, path);
            }
        }

        // --- Write CSV ---
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Key,English,German");
        foreach (var entry in resultEntries)
        {
            sb.AppendLine($"{Sanitize(entry.key)},\"{Sanitize(entry.english)}\",");
        }

        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"[Localization Extractor] Extracted {resultEntries.Count} texts to '{outputPath}'. Skipped {skippedNumeric} numeric-only texts.");
    }

    // ------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------
    struct TextEntry
    {
        public string hierarchy;
        public string text;
    }

    static IEnumerable<TextEntry> ExtractTextEntries(GameObject go)
    {
        var list = new List<TextEntry>();

        foreach (var t in go.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (!string.IsNullOrEmpty(t.text))
                list.Add(new TextEntry { hierarchy = GetFullHierarchyPath(t.gameObject), text = t.text });
        }

        foreach (var t in go.GetComponentsInChildren<TextMeshPro>(true))
        {
            if (!string.IsNullOrEmpty(t.text))
                list.Add(new TextEntry { hierarchy = GetFullHierarchyPath(t.gameObject), text = t.text });
        }

        foreach (var t in go.GetComponentsInChildren<Text>(true))
        {
            if (!string.IsNullOrEmpty(t.text))
                list.Add(new TextEntry { hierarchy = GetFullHierarchyPath(t.gameObject), text = t.text });
        }

        return list;
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

    static string GenerateLocalizationKey(string text, string sourcePath, string hierarchy)
    {
        string baseName = Path.GetFileNameWithoutExtension(sourcePath);
        string cleanText = Regex.Replace(text, @"[^A-Za-z0-9]+", "_").Trim('_');
        cleanText = cleanText.Length > 30 ? cleanText.Substring(0, 30) : cleanText;

        return $"{baseName}_{cleanText}".ToLowerInvariant();
    }

    static bool IsNumeric(string input)
    {
        // Remove whitespace
        input = input.Trim();
        // Match if it's all digits or digits with decimal separators
        return Regex.IsMatch(input, @"^[0-9.,\s]+$");
    }

    static string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("\"", "\"\"").Replace("\n", "\\n").Replace("\r", "");
    }
}
