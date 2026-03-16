using System;
using TreasureTower.Core;
using TreasureTower.Player;
using TreasureTower.Systems;
using UnityEngine;

namespace TreasureTower.Enemies
{
    public sealed class MiniBossController : MonoBehaviour
    {
        [SerializeField] private string bossName = "Mini Boss";
        [SerializeField] private int maxHealth = 6;
        [SerializeField] private float shootCooldown = 2.4f;
        [SerializeField] private float projectileSpeed = 8f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private AudioClip hitClip;
        [SerializeField] private AudioClip deathClip;
        [SerializeField] private AudioClip projectileHitClip;
        [SerializeField] private LayerMask projectileGroundLayers;
        [SerializeField] private GameObject minionTemplate;
        [SerializeField] private int smallWaveCount = 0;
        [SerializeField] private float smallWaveInterval = 0f;
        [SerializeField] private int largeWaveCount = 0;
        [SerializeField] private float largeWaveInterval = 0f;
        [SerializeField] private int rewardCoins = 3;
        [SerializeField] private bool rewardGem = true;
        [SerializeField] private Sprite coinSprite;
        [SerializeField] private Sprite gemSprite;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip gemClip;

        public event Action<MiniBossController> HealthChanged;

        public string BossName => bossName;
        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDefeated { get; private set; }

        private PlayerController2D player;
        private float nextShotTime;
        private float nextSmallWaveTime;
        private float nextLargeWaveTime;

        private void Awake()
        {
            CurrentHealth = Mathf.Max(1, maxHealth);
            spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
            nextSmallWaveTime = smallWaveCount > 0 && smallWaveInterval > 0f ? Time.time : float.PositiveInfinity;
            nextLargeWaveTime = largeWaveCount > 0 && largeWaveInterval > 0f ? Time.time + largeWaveInterval : float.PositiveInfinity;
        }

        private void Update()
        {
            if (IsDefeated)
            {
                return;
            }

            if (player == null)
            {
                player = FindFirstObjectByType<PlayerController2D>();
            }

            if (player == null)
            {
                return;
            }

            if (Time.time >= nextShotTime)
            {
                ShootAtPlayer();
                nextShotTime = Time.time + shootCooldown;
            }

            if (minionTemplate != null && smallWaveCount > 0 && smallWaveInterval > 0f && Time.time >= nextSmallWaveTime)
            {
                SpawnMinionWave(smallWaveCount);
                nextSmallWaveTime = Time.time + smallWaveInterval;
            }

            if (minionTemplate != null && largeWaveCount > 0 && largeWaveInterval > 0f && Time.time >= nextLargeWaveTime)
            {
                SpawnMinionWave(largeWaveCount);
                nextLargeWaveTime = Time.time + largeWaveInterval;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsDefeated || !collision.collider.TryGetComponent<PlayerController2D>(out var hitPlayer))
            {
                return;
            }

            GameManager.Instance?.HandlePlayerDeath(hitPlayer);
        }

        public void TakeHit(int damage)
        {
            if (IsDefeated)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - Mathf.Max(1, damage));
            AudioOneShot.Play(hitClip, transform.position, 0.85f);
            HealthChanged?.Invoke(this);

            if (CurrentHealth <= 0)
            {
                Defeat();
            }
        }

        private void ShootAtPlayer()
        {
            var projectileObject = new GameObject($"{bossName}_Projectile");
            projectileObject.transform.position = transform.position + new Vector3(player.transform.position.x >= transform.position.x ? 0.6f : -0.6f, 0.15f, 0f);

            var renderer = projectileObject.AddComponent<SpriteRenderer>();
            renderer.sprite = projectileSprite;
            renderer.sortingOrder = 7;

            var collider = projectileObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.16f;

            projectileObject.AddComponent<Rigidbody2D>().gravityScale = 0f;
            var projectile = projectileObject.AddComponent<Projectile2D>();
            var direction = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
            projectile.Initialize(direction, projectileSpeed, Projectile2D.ProjectileOwner.Enemy, projectileGroundLayers, projectileHitClip);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0f;
            }

            AudioOneShot.Play(shootClip, transform.position, 0.85f);
        }

        private void SpawnMinionWave(int count)
        {
            for (var index = 0; index < count; index++)
            {
                var xOffset = UnityEngine.Random.Range(-2.4f, 2.4f);
                var minion = Instantiate(minionTemplate, transform.position + new Vector3(xOffset, -0.32f, 0f), Quaternion.identity);
                if (minion.TryGetComponent<SimplePatrolEnemy>(out var patrolEnemy))
                {
                    patrolEnemy.EnablePlayerChase(player != null ? player.transform : null);
                }

                minion.SetActive(true);
            }
        }

        private void Defeat()
        {
            IsDefeated = true;
            AudioOneShot.Play(deathClip, transform.position, 0.9f);
            HealthChanged?.Invoke(this);
            SpawnRewards();
            Destroy(gameObject);
        }

        private void SpawnRewards()
        {
            for (var index = 0; index < rewardCoins; index++)
            {
                var xOffset = -0.8f + (index * 0.4f);
                SpawnCollectible($"RewardCoin_{index}", coinSprite, transform.position + new Vector3(xOffset, 0.35f, 0f), Collectible.CollectibleKind.Coin, coinClip);
            }

            if (rewardGem)
            {
                SpawnCollectible("RewardGem", gemSprite, transform.position + new Vector3(0f, 0.9f, 0f), Collectible.CollectibleKind.Gem, gemClip);
            }
        }

        private void SpawnCollectible(string name, Sprite sprite, Vector3 position, Collectible.CollectibleKind kind, AudioClip pickupClip)
        {
            var collectibleObject = new GameObject(name);
            collectibleObject.transform.position = position;
            var renderer = collectibleObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 6;

            var collider = collectibleObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.24f;

            var collectible = collectibleObject.AddComponent<Collectible>();
            collectible.Configure(kind, 1, pickupClip, kind == Collectible.CollectibleKind.Coin ? 0.82f : 0.92f);
        }
    }
}
