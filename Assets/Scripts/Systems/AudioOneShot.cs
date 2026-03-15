using UnityEngine;

namespace TreasureTower.Systems
{
    public static class AudioOneShot
    {
        public static void Play(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null)
            {
                return;
            }

            var settingsVolume = AudioSettingsManager.Instance != null ? AudioSettingsManager.Instance.SfxVolume : 1f;
            var scaledVolume = volume * settingsVolume;
            if (scaledVolume <= 0.001f)
            {
                return;
            }

            AudioSource.PlayClipAtPoint(clip, position, scaledVolume);
        }
    }
}
