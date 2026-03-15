using TreasureTower.Core;
using UnityEngine;

namespace TreasureTower.Environment
{
    public sealed class GoalFlag : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer doorBottomRenderer;
        [SerializeField] private SpriteRenderer doorTopRenderer;
        [SerializeField] private Sprite closedDoorBottom;
        [SerializeField] private Sprite openDoorBottom;
        [SerializeField] private Sprite closedDoorTop;
        [SerializeField] private Sprite openDoorTop;
        [SerializeField] private int requiredGems = 1;
        [SerializeField] private Color lockedTint = Color.white;
        [SerializeField] private Color unlockedTint = new(1f, 0.96f, 0.72f);
        [SerializeField] private AudioClip unlockClip;
        [SerializeField] private AudioClip completeClip;

        private bool isUnlocked;
        private int startingGems;

        private void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                ApplyDoorState(false);
                return;
            }

            startingGems = GameManager.Instance.Gems;
            GameManager.Instance.ScoreChanged += OnScoreChanged;
            ApplyDoorState(GameManager.Instance.Gems >= startingGems + requiredGems);
        }

        private void OnDisable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.ScoreChanged -= OnScoreChanged;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isUnlocked || !other.TryGetComponent<TreasureTower.Player.PlayerController2D>(out _))
            {
                return;
            }

            Systems.AudioOneShot.Play(completeClip, transform.position, 0.95f);
            GameManager.Instance?.CompleteLevel();
        }

        private void Update()
        {
            if (!isUnlocked)
            {
                return;
            }

            var pulse = 0.92f + Mathf.PingPong(Time.unscaledTime * 0.75f, 0.08f);
            transform.localScale = new Vector3(pulse, pulse, 1f);
        }

        private void OnScoreChanged(int coins, int gems)
        {
            ApplyDoorState(gems >= startingGems + requiredGems);
        }

        private void ApplyDoorState(bool unlocked)
        {
            var wasUnlocked = isUnlocked;
            isUnlocked = unlocked;
            transform.localScale = Vector3.one;

            if (doorBottomRenderer != null)
            {
                doorBottomRenderer.sprite = unlocked ? openDoorBottom : closedDoorBottom;
                doorBottomRenderer.color = unlocked ? unlockedTint : lockedTint;
            }

            if (doorTopRenderer != null)
            {
                doorTopRenderer.sprite = unlocked ? openDoorTop : closedDoorTop;
                doorTopRenderer.color = unlocked ? unlockedTint : lockedTint;
            }

            if (unlocked && !wasUnlocked)
            {
                Systems.AudioOneShot.Play(unlockClip, transform.position, 0.9f);
            }
        }
    }
}
