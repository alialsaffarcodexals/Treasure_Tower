using System;
using UnityEditor;

namespace TreasureTower.Editor
{
    [InitializeOnLoad]
    public static class TreasureTowerBatchBootstrap
    {
        private const string AutoBuildVariable = "TREASURE_TOWER_AUTOBUILD";

        static TreasureTowerBatchBootstrap()
        {
            var shouldRun = string.Equals(
                global::System.Environment.GetEnvironmentVariable(AutoBuildVariable),
                "1",
                StringComparison.Ordinal);

            if (!shouldRun)
            {
                return;
            }

            EditorApplication.delayCall += Run;
        }

        private static void Run()
        {
            EditorApplication.delayCall -= Run;

            try
            {
                TreasureTowerProjectSetup.BuildInitialGame();
            }
            finally
            {
                EditorApplication.Exit(0);
            }
        }
    }
}
