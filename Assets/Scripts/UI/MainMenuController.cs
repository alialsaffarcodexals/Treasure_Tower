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

        private bool buttonSoundsHooked;
        private bool sliderPreviewHooked;

        private void Awake()
        {
            HookButtonSounds();
            HookSliderPreviewSounds();
        }

        private void OnEnable()
        {
            ShowHome();
            RefreshLeaderboard();
            RefreshAudioSettings();
            HookSliderPreviewSounds();

            if (AudioSettingsManager.Instance != null)
            {
                AudioSettingsManager.Instance.UiSfxVolumeChanged -= OnUiSfxVolumeChanged;
                AudioSettingsManager.Instance.UiSfxVolumeChanged += OnUiSfxVolumeChanged;
            }
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
            GameManager.Instance?.StartNewGame();
        }

        public void ShowHome()
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

        public void ShowStory()
        {
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

        public void QuitGame()
        {
            Application.Quit();
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

        private void HookSliderPreviewSounds()
        {
            if (sliderPreviewHooked)
            {
                return;
            }

            HookSliderPreview(musicSlider);
            HookSliderPreview(sfxSlider);
            HookSliderPreview(uiSfxSlider);
            sliderPreviewHooked = true;
        }

        private void HookSliderPreview(Slider slider)
        {
            if (slider == null)
            {
                return;
            }

            slider.onValueChanged.AddListener(OnSliderPreviewChanged);
        }

        private void OnSliderPreviewChanged(float value)
        {
            UiClickSfx.PlayPreview();
        }

        private void OnUiSfxVolumeChanged(float value)
        {
            RefreshAudioSettings();
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
    }
}
