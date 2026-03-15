using TreasureTower.Core;
using UnityEngine;

namespace TreasureTower.Environment
{
    public sealed class MiniBossDoor : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer doorBottomRenderer;
        [SerializeField] private SpriteRenderer doorTopRenderer;
        [SerializeField] private Sprite closedDoorBottom;
        [SerializeField] private Sprite openDoorBottom;
        [SerializeField] private Sprite closedDoorTop;
        [SerializeField] private Sprite openDoorTop;
        [SerializeField] private int requiredGems = 1;
        [SerializeField] private string miniBossScenePath;
        [SerializeField] private string retryLevelPath;
        [SerializeField] private string skipLevelPath;
        [SerializeField] private AudioClip unlockClip;
        [SerializeField] private AudioClip enterClip;

        private bool isUnlocked;
        private int startingGems;

        private void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                ApplyState(false);
                return;
            }

            startingGems = GameManager.Instance.Gems;
            GameManager.Instance.ScoreChanged += OnScoreChanged;
            ApplyState(GameManager.Instance.Gems >= startingGems + requiredGems);
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

            Systems.AudioOneShot.Play(enterClip, transform.position, 0.9f);
            GameManager.Instance?.EnterMiniBossWorld(miniBossScenePath, retryLevelPath, skipLevelPath);
        }

        private void OnScoreChanged(int coins, int gems)
        {
            ApplyState(gems >= startingGems + requiredGems);
        }

        private void ApplyState(bool unlocked)
        {
            var wasUnlocked = isUnlocked;
            isUnlocked = unlocked;

            if (doorBottomRenderer != null)
            {
                doorBottomRenderer.sprite = unlocked ? openDoorBottom : closedDoorBottom;
            }

            if (doorTopRenderer != null)
            {
                doorTopRenderer.sprite = unlocked ? openDoorTop : closedDoorTop;
            }

            if (unlocked && !wasUnlocked)
            {
                Systems.AudioOneShot.Play(unlockClip, transform.position, 0.9f);
            }
        }
    }
}
