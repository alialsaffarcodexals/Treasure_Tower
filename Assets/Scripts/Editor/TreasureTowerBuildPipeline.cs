using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace TreasureTower.Editor
{
    public static class TreasureTowerBuildPipeline
    {
        private const string WindowsBuildPath = "Builds/Windows/TreasureTower.exe";

        public static void BuildWindowsPlayer()
        {
            var enabledScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();

            Directory.CreateDirectory(Path.GetDirectoryName(WindowsBuildPath) ?? "Builds/Windows");

            var options = new BuildPlayerOptions
            {
                scenes = enabledScenes,
                locationPathName = WindowsBuildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new Exception($"Windows build failed: {report.summary.result}");
            }
        }
    }
}
