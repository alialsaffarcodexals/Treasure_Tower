using UnityEngine;

namespace TreasureTower.Systems
{
    public sealed class MusicSceneTrack : MonoBehaviour
    {
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private float volume = 0.45f;

        private void OnEnable()
        {
            PlayNow();
        }

        private void Start()
        {
            PlayNow();
        }

        public void PlayNow()
        {
            MusicManager.Instance?.Play(musicClip, volume);
        }
    }
}
