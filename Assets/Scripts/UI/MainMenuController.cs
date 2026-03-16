using System.Collections;
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

        private bool runtimeButtonsHooked;

        private void Awake()
        {
            HookRuntimeButtons();
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
            TryPlayMenuMusic();
            StartCoroutine(EnsureHomeViewNextFrame());
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

        private IEnumerator EnsureHomeViewNextFrame()
        {
            yield return null;
            ShowHomeImmediate();
            RefreshAudioSettings();
            RefreshDifficultySettings();
            TryPlayMenuMusic();
        }

        private void HookRuntimeButtons()
        {
            if (runtimeButtonsHooked)
            {
                return;
            }

            HookButton("SettingsBackButton", ShowHomeImmediate);
            HookButton("StoryBackButton", ShowHomeImmediate);
            HookButton("LeaderboardBackButton", ShowHomeImmediate);
            runtimeButtonsHooked = true;
        }

        private void HookButton(string objectName, UnityEngine.Events.UnityAction action)
        {
            var buttonTransform = FindChildRecursive(transform.root, objectName);
            if (buttonTransform == null || !buttonTransform.TryGetComponent<Button>(out var button))
            {
                return;
            }

            button.onClick.AddListener(action);
        }

        private void TryPlayMenuMusic()
        {
            var track = FindFirstObjectByType<MusicSceneTrack>(FindObjectsInactive.Include);
            track?.PlayNow();
        }

        private static Transform FindChildRecursive(Transform parent, string objectName)
        {
            if (parent == null)
            {
                return null;
            }

            if (parent.name == objectName)
            {
                return parent;
            }

            for (var childIndex = 0; childIndex < parent.childCount; childIndex++)
            {
                var child = parent.GetChild(childIndex);
                var result = FindChildRecursive(child, objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
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
