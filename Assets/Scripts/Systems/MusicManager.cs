using System;
using UnityEngine;

namespace TreasureTower.Systems
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        public event Action<float> VolumeChanged;

        private AudioSource audioSource;
        private float currentVolume = 0.45f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInstance()
        {
            if (Instance != null)
            {
                return;
            }

            var managerObject = new GameObject(nameof(MusicManager));
            managerObject.AddComponent<MusicManager>();
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
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            ApplyVolume();

            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.MusicVolumeChanged += OnMusicVolumeChanged;
            }
        }

        public void Play(AudioClip clip, float volume = 0.45f)
        {
            currentVolume = volume;

            if (clip == null)
            {
                audioSource.Stop();
                audioSource.clip = null;
                return;
            }

            if (audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            else if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            ApplyVolume();
        }

        private void OnDestroy()
        {
            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.MusicVolumeChanged -= OnMusicVolumeChanged;
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            ApplyVolume();
            VolumeChanged?.Invoke(value);
        }

        private void ApplyVolume()
        {
            if (audioSource == null)
            {
                return;
            }

            var settingsVolume = AudioSettingsManager.Instance != null ? AudioSettingsManager.Instance.MusicVolume : 0.75f;
            audioSource.volume = currentVolume * settingsVolume;
        }
    }
}
