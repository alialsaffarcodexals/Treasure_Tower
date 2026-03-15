using UnityEngine;

namespace TreasureTower.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private PlayerController2D controller;
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite walkSpriteA;
        [SerializeField] private Sprite walkSpriteB;
        [SerializeField] private Sprite jumpSprite;
        [SerializeField] private Sprite fallSprite;
        [SerializeField] private Sprite crouchSprite;
        [SerializeField] private Sprite skidSprite;
        [SerializeField] private float walkAnimationRate = 0.12f;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            controller ??= GetComponent<PlayerController2D>();
        }

        private void Update()
        {
            if (controller == null)
            {
                return;
            }

            spriteRenderer.sprite = ResolveSprite();
        }

        private Sprite ResolveSprite()
        {
            if (controller.IsCrouching && crouchSprite != null && controller.IsGrounded)
            {
                return crouchSprite;
            }

            if (!controller.IsGrounded)
            {
                return controller.Velocity.y >= 0f ? jumpSprite ?? idleSprite : fallSprite ?? jumpSprite ?? idleSprite;
            }

            var horizontal = controller.HorizontalInput;
            if (Mathf.Abs(horizontal) > 0.01f)
            {
                if (Mathf.Sign(horizontal) != Mathf.Sign(controller.Velocity.x) && Mathf.Abs(controller.Velocity.x) > 0.6f && skidSprite != null)
                {
                    return skidSprite;
                }

                if (walkSpriteA != null && walkSpriteB != null)
                {
                    var pingPong = Mathf.PingPong(Time.time / walkAnimationRate, 2f);
                    return pingPong < 1f ? walkSpriteA : walkSpriteB;
                }

                return walkSpriteA ?? idleSprite;
            }

            return idleSprite;
        }
    }
}
