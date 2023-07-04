using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

public static class BuildScript
{
    public enum BuildType
    {
        Client,
        Server
    }

    public enum Platform
    {
        Windows,
        Mac,
        Linux
    }

    static string[] scenePaths = { "Assets/Scenes/00_ServerScene.unity", "Assets/Scenes/01_MainMenuScene.unity", "Assets/Scenes/02_OnlineMenuScene.unity", "Assets/Scenes/03_GameScene.unity", "Assets/Scenes/04_TutorialScene.unity", "Assets/Scenes/05_HallOfFame.unity", "Assets/Scenes/06_CreditsScene.unity" };

    [MenuItem("Build/Server Build (Windows)")]
    public static void PerformServerBuildWindows()
    {
        PerformBuild(BuildType.Server, Platform.Windows);
    }

    [MenuItem("Build/Client Build (Windows)")]
    public static void PerformClientBuildWindows()
    {
        PerformBuild(BuildType.Client, Platform.Windows);
    }

    [MenuItem("Build/All Builds (Windows)")]
    public static void PerformAllBuildsWindows()
    {
        PerformServerBuildWindows();
        PerformClientBuildWindows();
    }

    [MenuItem("Build/Server Build (Mac iOS)")]
    public static void PerformServerBuildMac()
    {
        PerformBuild(BuildType.Server, Platform.Mac);
    }

    [MenuItem("Build/Client Build (Mac iOS)")]
    public static void PerformClientBuildMac()
    {
        PerformBuild(BuildType.Client, Platform.Mac);
    }

    [MenuItem("Build/All Builds (Mac iOS)")]
    public static void PerformAllBuildsMac()
    {
        PerformServerBuildMac();
        PerformClientBuildMac();
    }

    [MenuItem("Build/Server Build (Linux)")]
    public static void PerformServerBuildLinux()
    {
        PerformBuild(BuildType.Server, Platform.Linux);
    }

    [MenuItem("Build/Client Build (Linux)")]
    public static void PerformClientBuildLinux()
    {
        PerformBuild(BuildType.Client, Platform.Linux);
    }

    [MenuItem("Build/All Builds (Linux)")]
    public static void PerformAllBuildsLinux()
    {
        PerformServerBuildLinux();
        PerformClientBuildLinux();
    }

    private static void PerformBuild(BuildType buildType, Platform platform)
    {
        Debug.Log("Performing build: " + buildType);
        string buildPath = "Builds/" + platform.ToString() + "/" + buildType.ToString() +"/" + PlayerSettings.productName + "-" + PlayerSettings.bundleVersion;

        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        if (platform == Platform.Windows)
        {
            buildTarget = BuildTarget.StandaloneWindows64;
            buildPath += ".exe";
        } else if (platform == Platform.Mac)
        {
            buildTarget = BuildTarget.StandaloneOSX;
            buildPath += ".app";
        } else if (platform == Platform.Linux)
        {
            buildTarget = BuildTarget.StandaloneLinux64;
            buildPath += ".x86_64";
        }

        SetPlayerSettings(buildType);

        BuildReport report = BuildPipeline.BuildPlayer(ConfigureScenes(buildType), buildPath, buildTarget, BuildOptions.None);

        // Check if the build succeeded
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log(buildType + " Build succeeded for platform " + platform + "!");
        }
        else
        {
            Debug.LogError(buildType + " Build for platform " + platform + " failed.");
        }
    }

    private static void SetPlayerSettings(BuildType buildType)
    {
        PlayerSettings.resizableWindow = true;
        PlayerSettings.allowFullscreenSwitch = buildType == BuildType.Client;
    }

    private static string[] ConfigureScenes(BuildType buildType)
    {
        var scenes = new List<string>();
        if (buildType == BuildType.Server)
        {
            scenes.Add(scenePaths[0]);
        }
        else
        {
            for (int i = 1; i < scenePaths.Length; i++)
            {
                scenes.Add(scenePaths[i]);
            }
        }

        return scenes.ToArray();
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // Pfade zur Readme-Datei und zum Build-Ordner
        string readmePath = Path.Combine(Application.dataPath, "readme.txt");
        string buildFolderPath = Path.GetDirectoryName(pathToBuiltProject);

        // Überprüfe, ob die Readme-Datei existiert
        if (File.Exists(readmePath))
        {
            // Kopiere die Readme-Datei in den Build-Ordner
            string buildReadmePath = Path.Combine(buildFolderPath, "readme.txt");
            File.Copy(readmePath, buildReadmePath, true);

            Debug.Log("Readme copied to build folder.");
        }
        else
        {
            Debug.LogError("Readme file not found.");
        }
    }
}
