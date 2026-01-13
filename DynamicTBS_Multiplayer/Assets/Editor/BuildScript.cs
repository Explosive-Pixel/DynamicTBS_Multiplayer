using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

public static class BuildScript
{
    public enum Platform
    {
        Windows,
        Mac,
        WebGL
    }

    static readonly string[] scenePaths = { "Assets/Scenes/01_MainMenuScene.unity", "Assets/Scenes/02_OnlineMenuScene.unity", "Assets/Scenes/03_OfflineMenuScene.unity", "Assets/Scenes/04_GameScene.unity", "Assets/Scenes/05_TutorialScene.unity", "Assets/Scenes/06_HallOfFame.unity", "Assets/Scenes/07_CreditsScene.unity" };

    [MenuItem("Build/Windows Build")]
    public static void PerformBuildWindows()
    {
        PerformBuild(Platform.Windows);
    }

    [MenuItem("Build/Windows Build (Development)")]
    public static void PerformDevelopmentBuildWindows()
    {
        PerformBuild(Platform.Windows, true);
    }

    [MenuItem("Build/Mac iOS Build")]
    public static void PerformBuildMac()
    {
        PerformBuild(Platform.Mac);
    }

    [MenuItem("Build/Mac iOS Build (Development)")]
    public static void PerformDevelopmentBuildMac()
    {
        PerformBuild(Platform.Mac, true);
    }

    [MenuItem("Build/WebGL Build")]
    public static void PerformBuildWebGL()
    {
        PerformBuild(Platform.WebGL);
    }

    [MenuItem("Build/WebGL Build (Development)")]
    public static void PerformDevelopmentBuildWebGL()
    {
        PerformBuild(Platform.WebGL, true);
    }

    private static void PerformBuild(Platform platform, bool developmentBuild = false)
    {
        Debug.Log("Performing build for platform: " + platform);
        string buildPath = "Builds/" + platform.ToString() + "/" + PlayerSettings.productName + "-" + PlayerSettings.bundleVersion;

        BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        if (platform == Platform.Windows)
        {
            buildTarget = BuildTarget.StandaloneWindows64;
            buildPath += ".exe";
        }
        else if (platform == Platform.Mac)
        {
            buildTarget = BuildTarget.StandaloneOSX;
            buildPath += ".app";
        }
        else if (platform == Platform.WebGL)
        {
            buildTarget = BuildTarget.WebGL;
        }

        // TODO: Set Build Settings
        SetPlayerSettings();

        BuildReport report = BuildPipeline.BuildPlayer(scenePaths, buildPath, buildTarget, developmentBuild ? BuildOptions.Development : BuildOptions.None);

        // Check if the build succeeded
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded for platform " + platform + "!");
        }
        else
        {
            Debug.LogError("Build for platform " + platform + " failed.");
        }
    }

    private static void SetPlayerSettings()
    {
        PlayerSettings.resizableWindow = false;
        PlayerSettings.allowFullscreenSwitch = true;
        PlayerSettings.runInBackground = true;
        PlayerSettings.resetResolutionOnWindowResize = true;
    }


    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        CopyFile("readme.txt", pathToBuiltProject);
        //CopyFile("steam_appid.txt", pathToBuiltProject);
    }

    private static void CopyFile(string filename, string pathToBuiltProject)
    {
        string readmePath = Path.Combine(Application.dataPath, filename);
        string buildFolderPath = Path.GetDirectoryName(pathToBuiltProject);

        if (File.Exists(readmePath))
        {
            string buildReadmePath = Path.Combine(buildFolderPath, filename);
            File.Copy(readmePath, buildReadmePath, true);

            Debug.Log("File '" + filename + "' copied to build folder.");
        }
        else
        {
            Debug.LogError("File '" + filename + "' not found.");
        }
    }
}
