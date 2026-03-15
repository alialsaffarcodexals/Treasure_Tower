using TreasureTower.Core;
using TreasureTower.Enemies;
using TreasureTower.Player;
using UnityEngine;

namespace TreasureTower.Systems
{
    public sealed class Projectile2D : MonoBehaviour
    {
        public enum ProjectileOwner
        {
            Player,
            Enemy
        }

        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private ProjectileOwner owner = ProjectileOwner.Player;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private AudioClip hitClip;

        private Vector2 direction = Vector2.right;

        private void Update()
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(Vector2 newDirection, float newSpeed, ProjectileOwner newOwner, LayerMask newGroundLayers, AudioClip newHitClip)
        {
            direction = newDirection.sqrMagnitude > 0.001f ? newDirection.normalized : Vector2.right;
            speed = newSpeed;
            owner = newOwner;
            groundLayers = newGroundLayers;
            hitClip = newHitClip;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & groundLayers.value) != 0)
            {
                Destroy(gameObject);
                return;
            }

            if (owner == ProjectileOwner.Player && other.TryGetComponent<MiniBossController>(out var boss))
            {
                boss.TakeHit(1);
                AudioOneShot.Play(hitClip, transform.position, 0.82f);
                Destroy(gameObject);
                return;
            }

            if (owner == ProjectileOwner.Player && other.TryGetComponent<SimplePatrolEnemy>(out var enemy))
            {
                enemy.TakeProjectileHit();
                AudioOneShot.Play(hitClip, transform.position, 0.82f);
                Destroy(gameObject);
                return;
            }

            if (owner == ProjectileOwner.Enemy && other.TryGetComponent<PlayerController2D>(out var player))
            {
                AudioOneShot.Play(hitClip, transform.position, 0.82f);
                GameManager.Instance?.HandlePlayerDeath(player);
                Destroy(gameObject);
            }
        }
    }
}
