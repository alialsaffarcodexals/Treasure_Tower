using TreasureTower.Core;
using TreasureTower.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureTower.UI
{
    public sealed class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject homePanel;
        [SerializeField] private GameObject storyPanel;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Text leaderboardText;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider uiSfxSlider;
        [SerializeField] private Text difficultyValueText;
        [SerializeField] private Button easyDifficultyButton;
        [SerializeField] private Button hardDifficultyButton;

        private void Awake()
        {
            ShowHomeImmediate();
        }

        private void OnEnable()
        {
            ShowHomeImmediate();
            RefreshLeaderboard();
            RefreshAudioSettings();
            RefreshDifficultySettings();

            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.UiSfxVolumeChanged -= OnUiSfxVolumeChanged;
                AudioSettingsManager.Instance.UiSfxVolumeChanged += OnUiSfxVolumeChanged;
            }
        }

        private void Start()
        {
            ShowHomeImmediate();
            RefreshAudioSettings();
            RefreshDifficultySettings();
        }

        private void OnDisable()
        {
            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.UiSfxVolumeChanged -= OnUiSfxVolumeChanged;
            }
        }

        public void StartGame()
        {
            UiClickSfx.Play();
            GameManager.Instance?.StartNewGame();
        }

        public void ShowHome()
        {
            UiClickSfx.Play();
            ShowHomeImmediate();
        }

        public void ShowStory()
        {
            UiClickSfx.Play();

            if (homePanel != null)
            {
                homePanel.SetActive(false);
            }

            if (storyPanel != null)
            {
                storyPanel.SetActive(true);
            }

            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(false);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        public void ShowLeaderboard()
        {
            UiClickSfx.Play();

            if (homePanel != null)
            {
                homePanel.SetActive(false);
            }

            if (storyPanel != null)
            {
                storyPanel.SetActive(false);
            }

            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(true);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }

            RefreshLeaderboard();
        }

        public void ShowSettings()
        {
            UiClickSfx.Play();

            if (homePanel != null)
            {
                homePanel.SetActive(false);
            }

            if (storyPanel != null)
            {
                storyPanel.SetActive(false);
            }

            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(false);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }

            RefreshAudioSettings();
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

        public void SetEasyDifficulty()
        {
            UiClickSfx.Play();
            DifficultySettings.SetMode(DifficultyMode.Easy);
            RefreshDifficultySettings();
        }

        public void SetHardDifficulty()
        {
            UiClickSfx.Play();
            DifficultySettings.SetMode(DifficultyMode.Hard);
            RefreshDifficultySettings();
        }

        public void QuitGame()
        {
            UiClickSfx.Play();
            Application.Quit();
        }

        private void OnUiSfxVolumeChanged(float value)
        {
            RefreshAudioSettings();
        }

        private void ShowHomeImmediate()
        {
            if (homePanel != null)
            {
                homePanel.SetActive(true);
            }

            if (storyPanel != null)
            {
                storyPanel.SetActive(false);
            }

            if (leaderboardPanel != null)
            {
                leaderboardPanel.SetActive(false);
            }

            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        private void RefreshLeaderboard()
        {
            if (leaderboardText == null || GameManager.Instance == null)
            {
                return;
            }

            leaderboardText.text = GameManager.Instance.GetLeaderboardText();
        }

        private void RefreshAudioSettings()
        {
            if (AudioSettingsManager.Instance == null)
            {
                return;
            }

            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.MusicVolume);
            }

            if (sfxSlider != null)
            {
                sfxSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.SfxVolume);
            }

            if (uiSfxSlider != null)
            {
                uiSfxSlider.SetValueWithoutNotify(AudioSettingsManager.Instance.UiSfxVolume);
            }
        }

        private void RefreshDifficultySettings()
        {
            var mode = DifficultySettings.CurrentMode;

            if (difficultyValueText != null)
            {
                difficultyValueText.text = $"Difficulty: {mode} ({DifficultySettings.LivesPerStage} Lives)";
            }

            if (easyDifficultyButton != null)
            {
                easyDifficultyButton.interactable = mode != DifficultyMode.Easy;
            }

            if (hardDifficultyButton != null)
            {
                hardDifficultyButton.interactable = mode != DifficultyMode.Hard;
            }
        }
    }
}
