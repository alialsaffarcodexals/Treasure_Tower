using TreasureTower.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TreasureTower.Editor
{
    public static class MainMenuDifficultyScenePatcher
    {
        private const string MainMenuScenePath = "Assets/Scenes/Menus/MainMenu.unity";

        [MenuItem("Tools/Treasure Tower/Patch Main Menu Difficulty UI")]
        public static void PatchMainMenuDifficultyUi()
        {
            var scene = EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            var controller = Object.FindObjectOfType<MainMenuController>(true);
            if (controller == null)
            {
                throw new MissingReferenceException("MainMenuController was not found in MainMenu scene.");
            }

            var settingsPanel = FindSceneTransform("SettingsPanel");
            var settingsCard = FindChild(settingsPanel, "Card") as RectTransform;
            var settingsBackButton = FindChild(settingsCard, "SettingsBackButton") as RectTransform;
            var uiSfxSlider = FindChild(settingsCard, "UiSfxSlider") as RectTransform;

            if (settingsCard == null || settingsBackButton == null || uiSfxSlider == null)
            {
                throw new MissingReferenceException("Settings panel objects were not found.");
            }

            var difficultyLabel = GetOrCreateDifficultyLabel(settingsCard, uiSfxSlider);
            var difficultyValue = GetOrCreateDifficultyValue(settingsCard, difficultyLabel);
            var easyButton = GetOrCreateDifficultyButton(settingsCard, settingsBackButton, "EasyDifficultyButton", "Easy", new Vector2(-120f, -78f));
            var hardButton = GetOrCreateDifficultyButton(settingsCard, settingsBackButton, "HardDifficultyButton", "Hard", new Vector2(120f, -78f));

            SetAnchoredPosition(settingsBackButton, new Vector2(0f, -210f));

            var cardTransform = settingsCard.GetComponent<RectTransform>();
            cardTransform.sizeDelta = new Vector2(cardTransform.sizeDelta.x, 620f);

            BindDifficultyButton(easyButton.GetComponent<Button>(), controller, nameof(MainMenuController.SetEasyDifficulty));
            BindDifficultyButton(hardButton.GetComponent<Button>(), controller, nameof(MainMenuController.SetHardDifficulty));

            var serializedController = new SerializedObject(controller);
            SetObjectReference(serializedController, "difficultyValueText", difficultyValue.GetComponent<Text>());
            SetObjectReference(serializedController, "easyDifficultyButton", easyButton.GetComponent<Button>());
            SetObjectReference(serializedController, "hardDifficultyButton", hardButton.GetComponent<Button>());
            serializedController.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static RectTransform GetOrCreateDifficultyLabel(RectTransform parent, RectTransform uiSfxSlider)
        {
            var existing = FindChild(parent, "DifficultyLabel") as RectTransform;
            if (existing != null)
            {
                SetAnchoredPosition(existing, new Vector2(-120f, uiSfxSlider.anchoredPosition.y - 76f));
                return existing;
            }

            var templateLabel = FindChild(parent, "UiSfxLabel") as RectTransform;
            var clone = Object.Instantiate(templateLabel.gameObject, parent, false);
            clone.name = "DifficultyLabel";
            var rect = clone.GetComponent<RectTransform>();
            SetAnchoredPosition(rect, new Vector2(-120f, uiSfxSlider.anchoredPosition.y - 76f));
            clone.GetComponent<Text>().text = "Difficulty";
            return rect;
        }

        private static RectTransform GetOrCreateDifficultyValue(RectTransform parent, RectTransform difficultyLabel)
        {
            var existing = FindChild(parent, "DifficultyValueText") as RectTransform;
            if (existing != null)
            {
                SetAnchoredPosition(existing, new Vector2(85f, difficultyLabel.anchoredPosition.y));
                return existing;
            }

            var templateLabel = FindChild(parent, "UiSfxLabel") as RectTransform;
            var clone = Object.Instantiate(templateLabel.gameObject, parent, false);
            clone.name = "DifficultyValueText";
            var rect = clone.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(260f, rect.sizeDelta.y);
            SetAnchoredPosition(rect, new Vector2(85f, difficultyLabel.anchoredPosition.y));
            var text = clone.GetComponent<Text>();
            text.alignment = TextAnchor.MiddleLeft;
            text.text = "Difficulty: Hard (3 Lives)";
            return rect;
        }

        private static RectTransform GetOrCreateDifficultyButton(RectTransform parent, RectTransform templateButton, string objectName, string label, Vector2 anchoredPosition)
        {
            var existing = FindChild(parent, objectName) as RectTransform;
            if (existing != null)
            {
                SetAnchoredPosition(existing, anchoredPosition);
                UpdateButtonLabel(existing, label);
                return existing;
            }

            var clone = Object.Instantiate(templateButton.gameObject, parent, false);
            clone.name = objectName;
            var rect = clone.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(185f, rect.sizeDelta.y);
            SetAnchoredPosition(rect, anchoredPosition);
            UpdateButtonLabel(rect, label);
            return rect;
        }

        private static void BindDifficultyButton(Button button, MainMenuController controller, string methodName)
        {
            if (button == null)
            {
                return;
            }

            button.onClick = new Button.ButtonClickedEvent();
            if (methodName == nameof(MainMenuController.SetEasyDifficulty))
            {
                UnityEventTools.AddPersistentListener(button.onClick, controller.SetEasyDifficulty);
            }
            else
            {
                UnityEventTools.AddPersistentListener(button.onClick, controller.SetHardDifficulty);
            }
        }

        private static void UpdateButtonLabel(RectTransform buttonRect, string label)
        {
            var textTransform = FindChild(buttonRect, "Text") ?? FindChild(buttonRect, "Label");
            if (textTransform != null)
            {
                var text = textTransform.GetComponent<Text>();
                if (text != null)
                {
                    text.text = label;
                }
            }
        }

        private static void SetAnchoredPosition(RectTransform rectTransform, Vector2 anchoredPosition)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
        }

        private static void SetObjectReference(SerializedObject serializedObject, string propertyName, Object value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.objectReferenceValue = value;
            }
        }

        private static Transform FindSceneTransform(string objectName)
        {
            var allTransforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var transform in allTransforms)
            {
                if (transform.name == objectName && transform.gameObject.scene.path == MainMenuScenePath)
                {
                    return transform;
                }
            }

            return null;
        }

        private static Transform FindChild(Component parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            return FindChild(parent.transform, childName);
        }

        private static Transform FindChild(Transform parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }

                var nested = FindChild(child, childName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }
    }
}
