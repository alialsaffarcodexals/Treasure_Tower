using TreasureTower.Core;
using TreasureTower.Player;
using UnityEngine;

namespace TreasureTower.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class SimplePatrolEnemy : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private float patrolDistance = 2.5f;
        [SerializeField] private float stompVelocityThreshold = -0.1f;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioClip defeatClip;
        [SerializeField] private AudioClip playerHitClip;

        private Rigidbody2D body;
        private Vector3 startPosition;
        private bool defeated;
        private float normalizedTravel;
        private bool movingRight = true;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
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
            if (defeated || !collision.collider.TryGetComponent<PlayerController2D>(out var player))
            {
                return;
            }

            foreach (var contact in collision.contacts)
            {
                if (contact.normal.y <= -0.5f && player.Velocity.y <= stompVelocityThreshold)
                {
                    Defeat(player);
                    return;
                }
            }

            Systems.AudioOneShot.Play(playerHitClip, player.transform.position, 0.85f);
            GameManager.Instance?.HandlePlayerDeath(player);
        }

        private void Defeat(PlayerController2D player)
        {
            defeated = true;
            player.Bounce();
            Systems.AudioOneShot.Play(defeatClip, transform.position, 0.85f);
            Destroy(gameObject);
        }

        public void TakeProjectileHit()
        {
            if (defeated)
            {
                return;
            }

            defeated = true;
            Systems.AudioOneShot.Play(defeatClip, transform.position, 0.85f);
            Destroy(gameObject);
        }
    }
}
