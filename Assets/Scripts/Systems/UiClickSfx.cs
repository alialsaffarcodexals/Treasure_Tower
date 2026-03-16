using UnityEngine;

namespace TreasureTower.Systems
{
    public static class UiClickSfx
    {
        private const string MenuClickResourcePath = "Audio/menu_click";
        private static AudioClip cachedClip;

        public static void Play()
        {
            if (cachedClip == null)
            {
                cachedClip = Resources.Load<AudioClip>(MenuClickResourcePath);
            }

            AudioOneShot.Play(cachedClip, Vector3.zero, 0.9f);
        }
    }
}
