using System.IO;
using TreasureTower.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TreasureTower.Editor
{
    public static class ManualUiVolumeScenePatcher
    {
        private const string MainMenuScenePath = "Assets/Scenes/Menus/MainMenu.unity";
        private const string PauseMenuSourceScenePath = "Assets/Scenes/Levels/TreasureTower_Level05.unity";

        [MenuItem("Tools/Treasure Tower/Make UI Volume Controls Manual")]
        public static void PatchScenes()
        {
            PatchMainMenu(MainMenuScenePath);

            foreach (var scenePath in Directory.GetFiles("Assets/Scenes/Levels", "*.unity"))
            {
                PatchGameplayScene(scenePath.Replace('\\', '/'));
            }

            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Treasure Tower/Apply Level 5 Pause Menu To All Levels")]
        public static void ApplyLevelFivePauseMenuToAllLevels()
        {
            var sourceScene = EditorSceneManager.OpenScene(PauseMenuSourceScenePath, OpenSceneMode.Single);
            var sourcePausePanel = FindGameObjectInScene(sourceScene, "PausePanel");
            var sourceCanvas = FindComponentInScene<Canvas>(sourceScene, "Canvas");
            if (sourcePausePanel == null || sourceCanvas == null)
            {
                throw new UnityException("Could not find source pause menu objects in Level 5.");
            }

            foreach (var targetPath in Directory.GetFiles("Assets/Scenes/Levels", "*.unity"))
            {
                var normalizedPath = targetPath.Replace('\\', '/');
                if (normalizedPath == PauseMenuSourceScenePath)
                {
                    FixGameplayUiSliderBinding(sourceScene);
                    EditorSceneManager.SaveScene(sourceScene);
                    continue;
                }

                var targetScene = EditorSceneManager.OpenScene(normalizedPath, OpenSceneMode.Additive);
                var targetCanvas = FindComponentInScene<Canvas>(targetScene, "Canvas");
                var targetHud = FindFirstComponentInScene<GameplayHudController>(targetScene);
                if (targetCanvas == null || targetHud == null)
                {
                    EditorSceneManager.CloseScene(targetScene, true);
                    continue;
                }

                var existingPausePanel = FindGameObjectInScene(targetScene, "PausePanel");
                var siblingIndex = existingPausePanel != null ? existingPausePanel.transform.GetSiblingIndex() : 0;
                if (existingPausePanel != null)
                {
                    Object.DestroyImmediate(existingPausePanel);
                }

                var clonedPausePanel = Object.Instantiate(sourcePausePanel, targetCanvas.transform);
                clonedPausePanel.name = "PausePanel";
                clonedPausePanel.transform.SetSiblingIndex(siblingIndex);

                var serializedHud = new SerializedObject(targetHud);
                var pauseMusic = FindSliderInChildren(clonedPausePanel.transform, "PauseMusicSlider");
                var pauseSfx = FindSliderInChildren(clonedPausePanel.transform, "PauseSfxSlider");
                var pauseUi = FindSliderInChildren(clonedPausePanel.transform, "PauseUiSfxSlider");
                serializedHud.FindProperty("pausePanel").objectReferenceValue = clonedPausePanel;
                serializedHud.FindProperty("pauseMusicSlider").objectReferenceValue = pauseMusic;
                serializedHud.FindProperty("pauseSfxSlider").objectReferenceValue = pauseSfx;
                serializedHud.FindProperty("pauseUiSfxSlider").objectReferenceValue = pauseUi;
                serializedHud.ApplyModifiedPropertiesWithoutUndo();
                FixSliderBinding(pauseUi, targetHud.SetUiSfxVolume);

                EditorSceneManager.MarkSceneDirty(targetScene);
                EditorSceneManager.SaveScene(targetScene);
                EditorSceneManager.CloseScene(targetScene, true);
            }

            AssetDatabase.SaveAssets();
        }

        public static void PatchScenesBatchMode()
        {
            PatchScenes();
            EditorApplication.Exit(0);
        }

        public static void ApplyLevelFivePauseMenuToAllLevelsBatchMode()
        {
            ApplyLevelFivePauseMenuToAllLevels();
            EditorApplication.Exit(0);
        }

        private static void PatchMainMenu(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var controller = Object.FindFirstObjectByType<MainMenuController>(FindObjectsInactive.Include);
            if (controller == null)
            {
                return;
            }

            var serializedObject = new SerializedObject(controller);
            var sfxSliderProp = serializedObject.FindProperty("sfxSlider");
            var uiSfxSliderProp = serializedObject.FindProperty("uiSfxSlider");
            var sfxSlider = sfxSliderProp.objectReferenceValue as Slider;
            var uiSfxSlider = uiSfxSliderProp.objectReferenceValue as Slider;

            if (sfxSlider != null && uiSfxSlider == null)
            {
                uiSfxSlider = DuplicateSlider(sfxSlider, "UiSfxSlider", new Vector2(0f, -78f));
                DuplicateLabel(FindSiblingTextByName(sfxSlider.transform.parent, "SfxLabel"), "UiSfxLabel", "Menu / UI", new Vector2(0f, -78f));
                uiSfxSliderProp.objectReferenceValue = uiSfxSlider;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.MarkSceneDirty(scene);
            }

            if (uiSfxSlider != null)
            {
                FixSliderBinding(uiSfxSlider, controller.SetUiSfxVolume);
            }

            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchGameplayScene(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            FixGameplayUiSliderBinding(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void FixGameplayUiSliderBinding(Scene scene)
        {
            var controller = FindFirstComponentInScene<GameplayHudController>(scene);
            if (controller == null)
            {
                return;
            }

            var serializedObject = new SerializedObject(controller);
            var sfxSliderProp = serializedObject.FindProperty("pauseSfxSlider");
            var uiSfxSliderProp = serializedObject.FindProperty("pauseUiSfxSlider");
            var sfxSlider = sfxSliderProp.objectReferenceValue as Slider;
            var uiSfxSlider = uiSfxSliderProp.objectReferenceValue as Slider;

            if (sfxSlider != null && uiSfxSlider == null)
            {
                uiSfxSlider = DuplicateSlider(sfxSlider, "PauseUiSfxSlider", new Vector2(0f, -78f));
                DuplicateLabel(FindSiblingTextByName(sfxSlider.transform.parent, "PauseSfxLabel"), "PauseUiSfxLabel", "Menu / UI", new Vector2(0f, -78f));
                uiSfxSliderProp.objectReferenceValue = uiSfxSlider;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.MarkSceneDirty(scene);
            }

            if (uiSfxSlider != null)
            {
                FixSliderBinding(uiSfxSlider, controller.SetUiSfxVolume);
            }
        }

        private static void FixSliderBinding(Slider slider, UnityEngine.Events.UnityAction<float> callback)
        {
            if (slider == null)
            {
                return;
            }

            while (slider.onValueChanged.GetPersistentEventCount() > 0)
            {
                UnityEventTools.RemovePersistentListener(slider.onValueChanged, 0);
            }

            UnityEventTools.AddPersistentListener(slider.onValueChanged, callback);
            EditorUtility.SetDirty(slider);
        }

        private static T FindComponentInScene<T>(Scene scene, string objectName) where T : Component
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var component in root.GetComponentsInChildren<T>(true))
                {
                    if (component.gameObject.name == objectName)
                    {
                        return component;
                    }
                }
            }

            return null;
        }

        private static GameObject FindGameObjectInScene(Scene scene, string objectName)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                {
                    if (transform.gameObject.name == objectName)
                    {
                        return transform.gameObject;
                    }
                }
            }

            return null;
        }

        private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var component = root.GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        private static Slider FindSliderInChildren(Transform root, string objectName)
        {
            foreach (var slider in root.GetComponentsInChildren<Slider>(true))
            {
                if (slider.gameObject.name == objectName)
                {
                    return slider;
                }
            }

            return null;
        }

        private static Slider DuplicateSlider(Slider source, string name, Vector2 offset)
        {
            var cloneObject = Object.Instantiate(source.gameObject, source.transform.parent);
            cloneObject.name = name;
            var sourceRect = source.GetComponent<RectTransform>();
            var cloneRect = cloneObject.GetComponent<RectTransform>();
            if (sourceRect != null && cloneRect != null)
            {
                cloneRect.anchorMin = sourceRect.anchorMin;
                cloneRect.anchorMax = sourceRect.anchorMax;
                cloneRect.pivot = sourceRect.pivot;
                cloneRect.sizeDelta = sourceRect.sizeDelta;
                cloneRect.anchoredPosition = sourceRect.anchoredPosition + offset;
                cloneRect.localScale = sourceRect.localScale;
            }

            return cloneObject.GetComponent<Slider>();
        }

        private static void DuplicateLabel(Text source, string name, string labelText, Vector2 offset)
        {
            if (source == null)
            {
                return;
            }

            var cloneObject = Object.Instantiate(source.gameObject, source.transform.parent);
            cloneObject.name = name;
            var sourceRect = source.GetComponent<RectTransform>();
            var cloneRect = cloneObject.GetComponent<RectTransform>();
            if (sourceRect != null && cloneRect != null)
            {
                cloneRect.anchorMin = sourceRect.anchorMin;
                cloneRect.anchorMax = sourceRect.anchorMax;
                cloneRect.pivot = sourceRect.pivot;
                cloneRect.sizeDelta = sourceRect.sizeDelta;
                cloneRect.anchoredPosition = sourceRect.anchoredPosition + offset;
                cloneRect.localScale = sourceRect.localScale;
            }

            var label = cloneObject.GetComponent<Text>();
            if (label != null)
            {
                label.text = labelText;
            }
        }

        private static Text FindSiblingTextByName(Transform parent, string name)
        {
            if (parent == null)
            {
                return null;
            }

            foreach (Transform child in parent)
            {
                if (child.name != name)
                {
                    continue;
                }

                return child.GetComponent<Text>();
            }

            return null;
        }
    }
}
