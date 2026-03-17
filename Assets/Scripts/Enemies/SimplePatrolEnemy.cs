using TreasureTower.Core;
using TreasureTower.Player;
using TreasureTower.Systems;
using UnityEngine;

namespace TreasureTower.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class SimplePatrolEnemy : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float patrolDistance = 2.5f;
        [SerializeField] private float stompVelocityThreshold = -0.1f;
        [SerializeField] private float stompClearance = 0.05f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioClip defeatClip;
        [SerializeField] private AudioClip playerHitClip;
        [SerializeField] private Sprite dropCoinSprite;
        [SerializeField] private AudioClip coinPickupClip;

        private Rigidbody2D body;
        private Collider2D enemyCollider;
        private Vector3 startPosition;
        private bool defeated;
        private float normalizedTravel;
        private bool movingRight = true;
        private bool chasePlayer;
        private Transform chaseTarget;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            enemyCollider = GetComponent<Collider2D>();
            startPosition = transform.position;
            spriteRenderer ??= GetComponentInChildren<SpriteRenderer>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }

        private void FixedUpdate()
        {
            if (defeated)
            {
                return;
            }

            if (chasePlayer && chaseTarget != null)
            {
                var direction = Mathf.Sign(chaseTarget.position.x - transform.position.x);
                if (Mathf.Abs(chaseTarget.position.x - transform.position.x) < 0.05f)
                {
                    direction = 0f;
                }

                movingRight = direction >= 0f;
                var chasePosition = new Vector2(transform.position.x + (direction * speed * Time.fixedDeltaTime), startPosition.y);
                body.MovePosition(chasePosition);

                if (spriteRenderer != null && direction != 0f)
                {
                    spriteRenderer.flipX = movingRight;
                }

                return;
            }

            var span = Mathf.Max(0.25f, patrolDistance * 2f);
            normalizedTravel += (speed / span) * Time.fixedDeltaTime;
            if (normalizedTravel > 1f)
            {
                normalizedTravel -= 1f;
            }

            var pingPong = Mathf.PingPong(normalizedTravel * 2f, 1f);
            var xOffset = Mathf.Lerp(-patrolDistance, patrolDistance, pingPong);
            var targetPosition = new Vector2(startPosition.x + xOffset, startPosition.y);
            movingRight = xOffset >= 0f;
            body.MovePosition(targetPosition);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = movingRight;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandlePlayerCollision(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            HandlePlayerCollision(collision);
        }

        private void HandlePlayerCollision(Collision2D collision)
        {
            if (defeated || !collision.collider.TryGetComponent<PlayerController2D>(out var player))
            {
                return;
            }

            if (CanBeStompedBy(player, collision))
            {
                Defeat(player);
                return;
            }

            AudioOneShot.Play(playerHitClip, player.transform.position, 0.85f);
            GameManager.Instance?.HandlePlayerDeath(player);
        }

        private bool CanBeStompedBy(PlayerController2D player, Collision2D collision)
        {
            if (player.Velocity.y > stompVelocityThreshold)
            {
                return false;
            }

            var playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider == null || enemyCollider == null)
            {
                return false;
            }

            var playerBottom = playerCollider.bounds.min.y;
            var playerCenter = playerCollider.bounds.center.y;
            var playerCenterX = playerCollider.bounds.center.x;
            var enemyCenter = enemyCollider.bounds.center.y;
            var enemyTop = enemyCollider.bounds.max.y;
            var enemyMinX = enemyCollider.bounds.min.x;
            var enemyMaxX = enemyCollider.bounds.max.x;
            var horizontalAlignment = playerCenterX >= enemyMinX - stompClearance && playerCenterX <= enemyMaxX + stompClearance;
            if (!horizontalAlignment)
            {
                return false;
            }

            var playerFeetNearTop = playerBottom >= enemyTop - stompClearance && playerBottom <= enemyTop + 0.35f;
            var playerAboveEnemy = playerCenter >= enemyCenter;
            var contactFromAbove = false;

            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y <= -0.25f)
                {
                    contactFromAbove = true;
                    break;
                }
            }

            return playerAboveEnemy && playerFeetNearTop && contactFromAbove;
        }

        private void Defeat(PlayerController2D player)
        {
            defeated = true;
            player.Bounce();
            AudioOneShot.Play(defeatClip, transform.position, 0.85f);
            SpawnCoinDrop();
            Destroy(gameObject);
        }

        public void EnablePlayerChase(Transform target)
        {
            chasePlayer = target != null;
            chaseTarget = target;
            if (chasePlayer)
            {
                startPosition = transform.position;
            }
        }

        public void TakeProjectileHit()
        {
            if (defeated)
            {
                return;
            }

            defeated = true;
            AudioOneShot.Play(defeatClip, transform.position, 0.85f);
            SpawnCoinDrop();
            Destroy(gameObject);
        }

        private void SpawnCoinDrop()
        {
            if (dropCoinSprite == null)
            {
                return;
            }

            var coinObject = new GameObject($"{name}_CoinDrop");
            coinObject.transform.position = transform.position + new Vector3(0f, 0.2f, 0f);

            var renderer = coinObject.AddComponent<SpriteRenderer>();
            renderer.sprite = dropCoinSprite;
            renderer.sortingOrder = 4;

            var collider = coinObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.24f;

            var collectible = coinObject.AddComponent<Collectible>();
            collectible.Configure(Collectible.CollectibleKind.Coin, 1, coinPickupClip, 0.82f);
        }
    }
}
