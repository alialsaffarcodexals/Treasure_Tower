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

        private void Awake()
        {
            EnsureUiSfxControls();
            HookButtonSounds();
        }

        private void OnEnable()
        {
            ShowHome();
            RefreshLeaderboard();
            RefreshAudioSettings();

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

            EnsureUiSfxControls();
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

        private void EnsureUiSfxControls()
        {
            if (uiSfxSlider != null || sfxSlider == null)
            {
                return;
            }

            var sliderObject = Instantiate(sfxSlider.gameObject, sfxSlider.transform.parent);
            sliderObject.name = "UiSfxSlider";
            var sliderRect = sliderObject.GetComponent<RectTransform>();
            var sourceRect = sfxSlider.GetComponent<RectTransform>();
            if (sliderRect != null && sourceRect != null)
            {
                sliderRect.anchorMin = sourceRect.anchorMin;
                sliderRect.anchorMax = sourceRect.anchorMax;
                sliderRect.pivot = sourceRect.pivot;
                sliderRect.sizeDelta = sourceRect.sizeDelta;
                sliderRect.anchoredPosition = sourceRect.anchoredPosition + new Vector2(0f, -78f);
                sliderRect.localScale = sourceRect.localScale;
            }

            uiSfxSlider = sliderObject.GetComponent<Slider>();
            uiSfxSlider.onValueChanged.RemoveAllListeners();
            uiSfxSlider.onValueChanged.AddListener(SetUiSfxVolume);

            var templateLabel = FindClosestLabel(sfxSlider);
            if (templateLabel != null)
            {
                var labelObject = Instantiate(templateLabel.gameObject, templateLabel.transform.parent);
                labelObject.name = "UiSfxLabel";
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
