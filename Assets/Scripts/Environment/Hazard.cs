using TreasureTower.Core;
using TreasureTower.Player;
using UnityEngine;

namespace TreasureTower.Environment
{
    public sealed class Hazard : MonoBehaviour
    {
        [SerializeField] private AudioClip deathClip;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<PlayerController2D>(out var player))
            {
                return;
            }

            Systems.AudioOneShot.Play(deathClip, player.transform.position, 0.9f);
            GameManager.Instance?.HandlePlayerDeath(player);
        }
    }
}
