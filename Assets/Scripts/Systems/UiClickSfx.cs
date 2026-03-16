using UnityEngine;

namespace TreasureTower.Systems
{
    public static class UiClickSfx
    {
        private const string MenuClickResourcePath = "Audio/menu_click";
        private static AudioClip cachedClip;
        private static AudioSource audioSource;

        public static void Play()
        {
            if (cachedClip == null)
            {
                cachedClip = Resources.Load<AudioClip>(MenuClickResourcePath);
            }

            if (cachedClip == null)
            {
                return;
            }

            EnsureAudioSource();
            var settingsVolume = AudioSettingsManager.Instance != null ? AudioSettingsManager.Instance.UiSfxVolume : 1f;
            var scaledVolume = 0.9f * settingsVolume;
            if (scaledVolume <= 0.001f)
            {
                return;
            }

            audioSource.PlayOneShot(cachedClip, scaledVolume);
        }

        private static void EnsureAudioSource()
        {
            if (audioSource != null)
            {
                return;
            }

            var audioObject = new GameObject("UiClickSfxPlayer");
            Object.DontDestroyOnLoad(audioObject);
            audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
        }
    }
}
