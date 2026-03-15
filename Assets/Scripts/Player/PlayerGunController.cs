using TreasureTower.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TreasureTower.Player
{
    public sealed class PlayerGunController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer gunRenderer;
        [SerializeField] private Sprite gunSprite;
        [SerializeField] private Sprite projectileSprite;
        [SerializeField] private AudioClip shootClip;
        [SerializeField] private AudioClip projectileHitClip;
        [SerializeField] private float fireCooldown = 2f;
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private Vector2 muzzleOffset = new(0.6f, 0.05f);
        [SerializeField] private LayerMask groundLayers;

        private PlayerController2D controller;
        private float nextFireTime;

        private void Awake()
        {
            controller = GetComponent<PlayerController2D>();
        }

        private void Update()
        {
            if (controller == null || !Application.isPlaying)
            {
                return;
            }

            if (gunRenderer != null)
            {
                gunRenderer.sprite = gunSprite;
                gunRenderer.flipX = !controller.IsFacingRight;
                gunRenderer.transform.localPosition = new Vector3(controller.IsFacingRight ? muzzleOffset.x : -muzzleOffset.x, muzzleOffset.y, 0f);
            }

            if (Time.time < nextFireTime)
            {
                return;
            }

            var wantsToShoot =
                (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame) ||
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);

            if (!wantsToShoot)
            {
                return;
            }

            Fire();
        }

        private void Fire()
        {
            nextFireTime = Time.time + fireCooldown;
            var projectileObject = new GameObject("PlayerProjectile");
            projectileObject.transform.position = transform.position + new Vector3(controller.IsFacingRight ? muzzleOffset.x : -muzzleOffset.x, muzzleOffset.y, 0f);
            projectileObject.layer = gameObject.layer;

            var renderer = projectileObject.AddComponent<SpriteRenderer>();
            renderer.sprite = projectileSprite;
            renderer.sortingOrder = 7;

            var collider = projectileObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.14f;

            projectileObject.AddComponent<Rigidbody2D>().gravityScale = 0f;
            var projectile = projectileObject.AddComponent<Projectile2D>();
            var direction = controller.IsFacingRight ? Vector2.right : Vector2.left;
            projectile.Initialize(direction, projectileSpeed, Projectile2D.ProjectileOwner.Player, groundLayers, projectileHitClip);

            AudioOneShot.Play(shootClip, transform.position, 0.8f);
        }
    }
}
