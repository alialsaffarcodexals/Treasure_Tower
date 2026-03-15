using TreasureTower.Core;
using TreasureTower.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TreasureTower.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float acceleration = 60f;
        [SerializeField] private float deceleration = 70f;
        [SerializeField] private float airAcceleration = 35f;

        [Header("Jump")]
        [SerializeField] private float jumpForce = 14f;
        [SerializeField] private float jumpCutMultiplier = 0.5f;
        [SerializeField] private float maxFallSpeed = 18f;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private float jumpVolume = 0.85f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Vector2 groundCheckSize = new(0.6f, 0.15f);
        [SerializeField] private LayerMask groundLayers = ~0;

        private Rigidbody2D body;
        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction crouchAction;

        private Vector2 moveInput;
        private bool facingRight = true;
        private bool inputEnabled = true;
        private Vector3 respawnPoint;

        public bool IsGrounded { get; private set; }
        public bool IsCrouching => inputEnabled && crouchAction != null && crouchAction.IsPressed();
        public bool IsFacingRight => facingRight;
        public float HorizontalInput => moveInput.x;
        public Vector2 Velocity => body.linearVelocity;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            playerInput = GetComponent<PlayerInput>();
            respawnPoint = transform.position;

            moveAction = playerInput.actions["Move"];
            jumpAction = playerInput.actions["Jump"];
            crouchAction = playerInput.actions["Crouch"];
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                GameManager.Instance?.TogglePause();
            }

            if (!inputEnabled || GameManager.Instance != null && GameManager.Instance.State != GameFlowState.Playing)
            {
                moveInput = Vector2.zero;
                return;
            }

            moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            UpdateGroundedState();
            HandleJump();
        }

        private void FixedUpdate()
        {
            UpdateGroundedState();

            var targetSpeed = inputEnabled ? moveInput.x * moveSpeed : 0f;
            var accel = IsGrounded
                ? Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration
                : airAcceleration;

            var velocity = body.linearVelocity;
            velocity.x = Mathf.MoveTowards(velocity.x, targetSpeed, accel * Time.fixedDeltaTime);

            if (velocity.y < -maxFallSpeed)
            {
                velocity.y = -maxFallSpeed;
            }

            body.linearVelocity = velocity;

            if (inputEnabled && Mathf.Abs(moveInput.x) > 0.01f)
            {
                SetFacing(moveInput.x > 0f);
            }
        }

        public void Bounce(float bounceForce = 10f)
        {
            var velocity = body.linearVelocity;
            velocity.y = bounceForce;
            body.linearVelocity = velocity;
        }

        public void SetRespawnPoint(Vector3 newRespawnPoint)
        {
            respawnPoint = newRespawnPoint;
        }

        public void Respawn()
        {
            transform.position = respawnPoint;
            body.linearVelocity = Vector2.zero;
            SetInputEnabled(true);
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            if (!enabled)
            {
                moveInput = Vector2.zero;
                body.linearVelocity = Vector2.zero;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }

        private void HandleJump()
        {
            if (!inputEnabled || jumpAction == null)
            {
                return;
            }

            if (jumpAction.WasPressedThisFrame() && IsGrounded)
            {
                var velocity = body.linearVelocity;
                velocity.y = jumpForce;
                body.linearVelocity = velocity;
                IsGrounded = false;
                AudioOneShot.Play(jumpClip, transform.position, jumpVolume);
            }

            if (jumpAction.WasReleasedThisFrame() && body.linearVelocity.y > 0f)
            {
                var velocity = body.linearVelocity;
                velocity.y *= jumpCutMultiplier;
                body.linearVelocity = velocity;
            }
        }

        private void UpdateGroundedState()
        {
            if (groundCheck == null)
            {
                IsGrounded = false;
                return;
            }

            Collider2D hit = Physics2D.OverlapBox(
                groundCheck.position,
                groundCheckSize,
                0f,
                groundLayers
            );

            if (hit != null && hit.gameObject != gameObject)
            {
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

        private void SetFacing(bool shouldFaceRight)
        {
            if (facingRight == shouldFaceRight)
            {
                return;
            }

            facingRight = shouldFaceRight;
            var localScale = transform.localScale;
            localScale.x = Mathf.Abs(localScale.x) * (facingRight ? 1f : -1f);
            transform.localScale = localScale;
        }
    }
}
