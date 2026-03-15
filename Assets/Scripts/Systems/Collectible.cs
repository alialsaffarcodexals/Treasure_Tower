using TreasureTower.Core;
using UnityEngine;

namespace TreasureTower.Systems
{
    public sealed class Collectible : MonoBehaviour
    {
        public enum CollectibleKind
        {
            Coin,
            Gem
        }

        [SerializeField] private CollectibleKind kind = CollectibleKind.Coin;
        [SerializeField] private int amount = 1;
        [SerializeField] private AudioClip pickupClip;
        [SerializeField] private float pickupVolume = 0.9f;
        [SerializeField] private GameObject pickupEffect;

        private bool collected;

        public void Configure(CollectibleKind newKind, int newAmount, AudioClip newPickupClip, float newPickupVolume = 0.9f)
        {
            kind = newKind;
            amount = Mathf.Max(1, newAmount);
            pickupClip = newPickupClip;
            pickupVolume = newPickupVolume;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (collected || !other.TryGetComponent<TreasureTower.Player.PlayerController2D>(out _))
            {
                return;
            }

            collected = true;

            if (kind == CollectibleKind.Coin)
            {
                GameManager.Instance?.RegisterCoin(amount);
            }
            else
            {
                GameManager.Instance?.RegisterGem(amount);
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            AudioOneShot.Play(pickupClip, transform.position, pickupVolume);

            Destroy(gameObject);
        }
    }
}
