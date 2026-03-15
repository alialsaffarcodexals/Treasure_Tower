using TreasureTower.Player;
using UnityEngine;

namespace TreasureTower.Environment
{
    public sealed class CheckpointFlag : MonoBehaviour
    {
        [SerializeField] private Transform respawnPointOverride;
        [SerializeField] private SpriteRenderer markerRenderer;
        [SerializeField] private Color activatedColor = new(1f, 0.93f, 0.35f);

        private bool activated;
        private Color initialColor;

        private void Awake()
        {
            if (markerRenderer != null)
            {
                initialColor = markerRenderer.color;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (activated || !other.TryGetComponent<PlayerController2D>(out var player))
            {
                return;
            }

            activated = true;
            player.SetRespawnPoint(respawnPointOverride != null ? respawnPointOverride.position : transform.position);

            if (markerRenderer != null)
            {
                markerRenderer.color = activatedColor;
            }
        }
    }
}
