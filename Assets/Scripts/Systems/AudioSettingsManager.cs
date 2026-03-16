using System;
using UnityEngine;

namespace TreasureTower.Systems
{
    public sealed class AudioSettingsManager : MonoBehaviour
    {
        private const string MusicVolumeKey = "TreasureTower.MusicVolume";
        private const string SfxVolumeKey = "TreasureTower.SfxVolume";
        private const string UiSfxVolumeKey = "TreasureTower.UiSfxVolume";

        public static AudioSettingsManager Instance { get; private set; }

        public event Action<float> MusicVolumeChanged;
        public event Action<float> SfxVolumeChanged;
        public event Action<float> UiSfxVolumeChanged;

        public float MusicVolume { get; private set; } = 0.75f;
        public float SfxVolume { get; private set; } = 0.85f;
        public float UiSfxVolume { get; private set; } = 0.9f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInstance()
        {
            if (Instance != null)
            {
                return;
            }

            var managerObject = new GameObject(nameof(AudioSettingsManager));
            managerObject.AddComponent<AudioSettingsManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.85f);
            UiSfxVolume = PlayerPrefs.GetFloat(UiSfxVolumeKey, 0.9f);
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.Save();
            MusicVolumeChanged?.Invoke(MusicVolume);
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            PlayerPrefs.Save();
            SfxVolumeChanged?.Invoke(SfxVolume);
        }

        public void SetUiSfxVolume(float value)
        {
            UiSfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(UiSfxVolumeKey, UiSfxVolume);
            PlayerPrefs.Save();
            UiSfxVolumeChanged?.Invoke(UiSfxVolume);
        }
    }
}
