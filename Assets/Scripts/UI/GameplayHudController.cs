using TreasureTower.Core;
using TreasureTower.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureTower.UI
{
    public sealed class GameplayHudController : MonoBehaviour
    {
        [SerializeField] private Text coinsText;
        [SerializeField] private Text gemsText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text livesText;
        [SerializeField] private Text deathsText;
        [SerializeField] private GameObject transitionPanel;
        [SerializeField] private Text transitionText;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Slider pauseMusicSlider;
        [SerializeField] private Slider pauseSfxSlider;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text gameOverText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Text retryButtonText;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private Text victoryTitleText;
        [SerializeField] private Text victorySummaryText;
        [SerializeField] private Text victoryLeaderboardText;

        private bool buttonSoundsHooked;

        private void Awake()
        {
            HookButtonSounds();
        }

        private void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.ScoreChanged += OnScoreChanged;
            GameManager.Instance.StateChanged += OnStateChanged;
            GameManager.Instance.TransitionMessageChanged += OnTransitionMessageChanged;
            GameManager.Instance.RunStatsChanged += RefreshStats;
            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.MusicVolumeChanged += OnMusicVolumeChanged;
                AudioSettingsManager.Instance.SfxVolumeChanged += OnSfxVolumeChanged;
            }
            OnScoreChanged(GameManager.Instance.Coins, GameManager.Instance.Gems);
            OnTransitionMessageChanged(GameManager.Instance.TransitionMessage);
            RefreshStats();
            OnStateChanged(GameManager.Instance.State);
            RefreshAudioSettings();
        }

        private void OnDisable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            GameManager.Instance.ScoreChanged -= OnScoreChanged;
            GameManager.Instance.StateChanged -= OnStateChanged;
            GameManager.Instance.TransitionMessageChanged -= OnTransitionMessageChanged;
            GameManager.Instance.RunStatsChanged -= RefreshStats;
            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.MusicVolumeChanged -= OnMusicVolumeChanged;
                AudioSettingsManager.Instance.SfxVolumeChanged -= OnSfxVolumeChanged;
            }
        }

        private void Update()
        {
            RefreshStats();
        }

        public void ResumeGame()
        {
            GameManager.Instance?.ResumeGame();
        }

        public void RestartLevel()
        {
            GameManager.Instance?.RestartLevel();
        }

        public void RetryAfterGameOver()
        {
            GameManager.Instance?.RetryAfterGameOver();
        }

        public void ReturnToMenu()
        {
            GameManager.Instance?.ReturnToMainMenu();
        }

        public void SetMusicVolume(float value)
        {
            AudioSettingsManager.Instance?.SetMusicVolume(value);
        }

        public void SetSfxVolume(float value)
        {
            AudioSettingsManager.Instance?.SetSfxVolume(value);
        }

        private void OnScoreChanged(int coins, int gems)
        {
            if (coinsText != null)
            {
                coinsText.text = $"Coins {coins:00}";
            }

            if (gemsText != null)
            {
                gemsText.text = $"Gems {gems:00}";
            }
        }

        private void OnStateChanged(GameFlowState state)
        {
            if (transitionPanel != null)
            {
                transitionPanel.SetActive(state == GameFlowState.Transition);
            }

            if (pausePanel != null)
            {
                pausePanel.SetActive(state == GameFlowState.Paused);
            }

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(state == GameFlowState.GameOver);
            }

            if (victoryPanel != null)
            {
                victoryPanel.SetActive(state == GameFlowState.Victory);
            }

            RefreshStats();
        }

        private void OnTransitionMessageChanged(string message)
        {
            if (transitionText != null)
            {
                transitionText.text = string.IsNullOrWhiteSpace(message) ? "Get Ready" : message;
            }
        }

        private void RefreshStats()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            if (timerText != null)
            {
                timerText.text = $"Time {GameManager.FormatTime(GameManager.Instance.ElapsedTime)}";
            }

            if (livesText != null)
            {
                livesText.text = $"Lives {GameManager.Instance.LivesRemaining}/3";
            }

            if (deathsText != null)
            {
                deathsText.text = $"Deaths {GameManager.Instance.Deaths}";
            }

            if (gameOverText != null)
            {
                gameOverText.text = GameManager.Instance.GameOverMessage;
            }

            if (retryButtonText != null)
            {
                retryButtonText.text = GameManager.Instance.HasLivesRemaining ? "Retry Level" : "Restart Game";
            }

            if (victoryTitleText != null)
            {
                victoryTitleText.text = GameManager.Instance.VictoryTitle;
            }

            if (victorySummaryText != null)
            {
                victorySummaryText.text = $"{GameManager.Instance.VictoryMessage}\n\n{GameManager.Instance.VictoryStats}";
            }

            if (victoryLeaderboardText != null)
            {
                victoryLeaderboardText.text = GameManager.Instance.GetLeaderboardText();
            }
        }

        private void HookButtonSounds()
        {
            if (buttonSoundsHooked)
            {
                return;
            }

            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                button.onClick.AddListener(UiClickSfx.Play);
            }

            buttonSoundsHooked = true;
        }

        private void OnMusicVolumeChanged(float value)
        {
            RefreshAudioSettings();
        }

        private void OnSfxVolumeChanged(float value)
        {
            RefreshAudioSettings();
        }

        private void RefreshAudioSettings()
        {
            if (AudioSettingsManager.Instance == null)
            {
                return;
            }

            if (pauseMusicSlider != null)
            {
                pauseMusicSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.MusicVolume);
            }

            if (pauseSfxSlider != null)
            {
                pauseSfxSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.SfxVolume);
            }
        }
    }
}
