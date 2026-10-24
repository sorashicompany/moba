using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public static class AshenCrownBuild
{
    public static void BuildAndroid()
    {
        const string sceneDir = "Assets/Scenes";
        const string scenePath = sceneDir + "/AshenCrown.unity";
        Directory.CreateDirectory(sceneDir);
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, scenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
        Directory.CreateDirectory("builds/Android");
        var options = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = "builds/Android/AshenCrown.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        var report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new System.Exception("Android build failed: " + report.summary.result);
    }
}
