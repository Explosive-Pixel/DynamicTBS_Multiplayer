using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ReplaceFontsInScenesAndPrefabs : EditorWindow
{
    public TMP_FontAsset newTMPFont;
    public Font newLegacyFont;

    private static readonly string scenesFolder = "Assets/Scenes";
    private static readonly string prefabsFolder = "Assets/Prefabs";

    [MenuItem("Tools/Replace Fonts (Scenes + Prefabs)")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceFontsInScenesAndPrefabs>("Replace Fonts (Scenes + Prefabs)");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace Fonts in Scenes and Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox($"This will only modify assets inside:\n• {scenesFolder}\n• {prefabsFolder}\n\nMake sure you have backups or version control before proceeding.", MessageType.Info);

        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newTMPFont, typeof(TMP_FontAsset), false);
        newLegacyFont = (Font)EditorGUILayout.ObjectField("New Legacy Font", newLegacyFont, typeof(Font), false);

        if (GUILayout.Button("Replace Fonts"))
        {
            if (EditorUtility.DisplayDialog("Replace Fonts",
                $"This will search through all prefabs in '{prefabsFolder}' and all scenes in '{scenesFolder}' and permanently change font references.\n\nAre you sure you want to continue?",
                "Yes, Replace Fonts", "Cancel"))
            {
                ReplaceFontsInScopedFolders();
            }
        }
    }

    void ReplaceFontsInScopedFolders()
    {
        int count = 0;

        try
        {
            AssetDatabase.StartAssetEditing();

            // --- 1. Replace in Prefabs ---
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabsFolder });
            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    count += ReplaceFontsInGameObject(prefab);
                    EditorUtility.SetDirty(prefab);
                }
            }

            // --- 2. Replace in Scenes ---
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { scenesFolder });
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

                int sceneCount = 0;
                foreach (var root in scene.GetRootGameObjects())
                {
                    sceneCount += ReplaceFontsInGameObject(root);
                }

                if (sceneCount > 0)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                    count += sceneCount;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }

        Debug.Log($"[Replace Fonts] Replaced fonts on {count} text components inside '{prefabsFolder}' and '{scenesFolder}'.");
    }

    int ReplaceFontsInGameObject(GameObject go)
    {
        int changed = 0;

        // Legacy UI Text
        foreach (var text in go.GetComponentsInChildren<Text>(true))
        {
            if (newLegacyFont != null && text.font != newLegacyFont)
            {
                Undo.RecordObject(text, "Replace Legacy Font");
                text.font = newLegacyFont;
                EditorUtility.SetDirty(text);
                changed++;
            }
        }

        // TMP UI Text
        foreach (var tmp in go.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (newTMPFont != null && tmp.font != newTMPFont)
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = newTMPFont;
                EditorUtility.SetDirty(tmp);
                changed++;
            }
        }

        // TMP 3D Text
        foreach (var tmp in go.GetComponentsInChildren<TextMeshPro>(true))
        {
            if (newTMPFont != null && tmp.font != newTMPFont)
            {
                Undo.RecordObject(tmp, "Replace TMP Font");
                tmp.font = newTMPFont;
                EditorUtility.SetDirty(tmp);
                changed++;
            }
        }

        return changed;
    }
}
