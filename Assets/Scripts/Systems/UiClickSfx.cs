using UnityEngine;

namespace TreasureTower.Systems
{
    public static class UiClickSfx
    {
        private const string MenuClickResourcePath = "Audio/menu_click";
        private const float PreviewCooldown = 0.08f;

        private static AudioClip cachedClip;
        private static AudioSource audioSource;
        private static float lastPreviewTime = float.NegativeInfinity;

        public static void Play()
        {
            PlayInternal(false);
        }

        public static void PlayPreview()
        {
            PlayInternal(true);
        }

        private static void PlayInternal(bool preview)
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

            if (preview)
            {
                var now = Time.unscaledTime;
                if (now - lastPreviewTime < PreviewCooldown)
                {
                    return;
                }

                lastPreviewTime = now;
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
