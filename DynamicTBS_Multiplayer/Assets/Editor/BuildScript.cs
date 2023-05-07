using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    public enum BuildType
    {
        Client,
        Server
    }

    static string[] scenePaths = { "Assets/Scenes/Current/00_ServerScene.unity", "Assets/Scenes/Current/01_MainMenuScene.unity", "Assets/Scenes/Current/02_OnlineMenuScene.unity", "Assets/Scenes/Current/03_GameScene.unity", "Assets/Scenes/Current/04_TutorialScene.unity", "Assets/Scenes/Current/05_LoreScene.unity", "Assets/Scenes/Current/06_CreditsScene.unity" };

    public static string[] ConfigureScenes(BuildType buildType)
    {
        /* EditorBuildSettings.scenes = new EditorBuildSettingsScene[scenePaths.Length];

         EditorBuildSettings.scenes[0] = new EditorBuildSettingsScene(scenePaths[0], buildType == BuildType.Server);

         for (int i = 1; i < scenePaths.Length; i++)
         {
             EditorBuildSettings.scenes[i] = new EditorBuildSettingsScene(scenePaths[i], buildType == BuildType.Client);
         } */


        var scenes = new List<string>();
        if (buildType == BuildType.Server)
        {
            scenes.Add(scenePaths[0]);
        } else
        {
            for (int i = 1; i < scenePaths.Length; i++)
            {
                scenes.Add(scenePaths[i]);
            }
        }

        return scenes.ToArray();
    }

    [MenuItem("Build/Server Build")]
    public static void PerformServerBuild()
    {
        PerformBuild(BuildType.Server);
    }

    [MenuItem("Build/Client Build")]
    public static void PerformClientBuild()
    {
        PerformBuild(BuildType.Client);
    }

    private static void PerformBuild(BuildType buildType)
    {
        Debug.Log("Performing build: " + buildType);
        string buildPath = "Builds/" + buildType.ToString() +"/Skyrats.exe";

        // Get the build settings
       /* BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        buildPlayerOptions.scenes = ConfigureScenes(buildType);

        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        // Build the player
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);*/

        BuildPipeline.BuildPlayer(ConfigureScenes(buildType), buildPath, BuildTarget.StandaloneWindows64, BuildOptions.None);

        // Check if the build succeeded
       /* if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded!");
        }
        else
        {
            Debug.LogError("Build failed!");
        }*/
    }
}
