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
        [SerializeField] private Slider pauseUiSfxSlider;
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
            EnsurePauseUiSfxControls();
            HookButtonSounds();
        }

        private void OnEnable()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            EnsurePauseUiSfxControls();
            GameManager.Instance.ScoreChanged += OnScoreChanged;
            GameManager.Instance.StateChanged += OnStateChanged;
            GameManager.Instance.TransitionMessageChanged += OnTransitionMessageChanged;
            GameManager.Instance.RunStatsChanged += RefreshStats;
            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.MusicVolumeChanged += OnMusicVolumeChanged;
                AudioSettingsManager.Instance.SfxVolumeChanged += OnSfxVolumeChanged;
                AudioSettingsManager.Instance.UiSfxVolumeChanged += OnUiSfxVolumeChanged;
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
                AudioSettingsManager.Instance.UiSfxVolumeChanged -= OnUiSfxVolumeChanged;
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

        public void SetUiSfxVolume(float value)
        {
            AudioSettingsManager.Instance?.SetUiSfxVolume(value);
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

        private void EnsurePauseUiSfxControls()
        {
            if (pauseUiSfxSlider != null || pauseSfxSlider == null)
            {
                return;
            }

            var sliderObject = Instantiate(pauseSfxSlider.gameObject, pauseSfxSlider.transform.parent);
            sliderObject.name = "PauseUiSfxSlider";
            var sliderRect = sliderObject.GetComponent<RectTransform>();
            var sourceRect = pauseSfxSlider.GetComponent<RectTransform>();
            if (sliderRect != null && sourceRect != null)
            {
                sliderRect.anchorMin = sourceRect.anchorMin;
                sliderRect.anchorMax = sourceRect.anchorMax;
                sliderRect.pivot = sourceRect.pivot;
                sliderRect.sizeDelta = sourceRect.sizeDelta;
                sliderRect.anchoredPosition = sourceRect.anchoredPosition + new Vector2(0f, -78f);
                sliderRect.localScale = sourceRect.localScale;
            }

            pauseUiSfxSlider = sliderObject.GetComponent<Slider>();
            pauseUiSfxSlider.onValueChanged.RemoveAllListeners();
            pauseUiSfxSlider.onValueChanged.AddListener(SetUiSfxVolume);

            var templateLabel = FindClosestLabel(pauseSfxSlider);
            if (templateLabel != null)
            {
                var labelObject = Instantiate(templateLabel.gameObject, templateLabel.transform.parent);
                labelObject.name = "PauseUiSfxLabel";
                var labelRect = labelObject.GetComponent<RectTransform>();
                var templateRect = templateLabel.GetComponent<RectTransform>();
                if (labelRect != null && templateRect != null)
                {
                    labelRect.anchorMin = templateRect.anchorMin;
                    labelRect.anchorMax = templateRect.anchorMax;
                    labelRect.pivot = templateRect.pivot;
                    labelRect.sizeDelta = templateRect.sizeDelta;
                    labelRect.anchoredPosition = templateRect.anchoredPosition + new Vector2(0f, -78f);
                    labelRect.localScale = templateRect.localScale;
                }

                var labelText = labelObject.GetComponent<Text>();
                if (labelText != null)
                {
                    labelText.text = "Menu / UI";
                }
            }
        }

        private Text FindClosestLabel(Slider slider)
        {
            var sliderRect = slider.GetComponent<RectTransform>();
            if (sliderRect == null || slider.transform.parent == null)
            {
                return null;
            }

            Text closestLabel = null;
            var bestScore = float.MaxValue;
            foreach (var label in slider.transform.parent.GetComponentsInChildren<Text>(true))
            {
                var labelRect = label.GetComponent<RectTransform>();
                if (labelRect == null)
                {
                    continue;
                }

                if (labelRect.anchoredPosition.x >= sliderRect.anchoredPosition.x)
                {
                    continue;
                }

                var score = Mathf.Abs(labelRect.anchoredPosition.y - sliderRect.anchoredPosition.y);
                if (score < bestScore)
                {
                    bestScore = score;
                    closestLabel = label;
                }
            }

            return closestLabel;
        }

        private void OnMusicVolumeChanged(float value)
        {
            RefreshAudioSettings();
        }

        private void OnSfxVolumeChanged(float value)
        {
            RefreshAudioSettings();
        }

        private void OnUiSfxVolumeChanged(float value)
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

            if (pauseUiSfxSlider != null)
            {
                pauseUiSfxSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.UiSfxVolume);
            }
        }
    }
}
