using UnityEngine;

namespace TreasureTower.Systems
{
    public enum DifficultyMode
    {
        Hard = 0,
        Easy = 1
    }

    public static class DifficultySettings
    {
        private const string DifficultyKey = "TreasureTower.Difficulty";

        public static DifficultyMode CurrentMode
        {
            get
            {
                var storedValue = PlayerPrefs.GetInt(DifficultyKey, (int)DifficultyMode.Hard);
                return storedValue == (int)DifficultyMode.Easy ? DifficultyMode.Easy : DifficultyMode.Hard;
            }
        }

        public static int LivesPerStage => CurrentMode == DifficultyMode.Easy ? 6 : 3;

        public static void SetMode(DifficultyMode mode)
        {
            PlayerPrefs.SetInt(DifficultyKey, (int)mode);
            PlayerPrefs.Save();
        }
    }
}
