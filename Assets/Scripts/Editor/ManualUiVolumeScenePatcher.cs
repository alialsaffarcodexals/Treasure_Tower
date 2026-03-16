using System.IO;
using TreasureTower.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TreasureTower.Editor
{
    public static class ManualUiVolumeScenePatcher
    {
        [MenuItem("Tools/Treasure Tower/Make UI Volume Controls Manual")]
        public static void PatchScenes()
        {
            PatchMainMenu("Assets/Scenes/Menus/MainMenu.unity");

            foreach (var scenePath in Directory.GetFiles("Assets/Scenes/Levels", "*.unity"))
            {
                PatchGameplayScene(scenePath.Replace('\\', '/'));
            }

            AssetDatabase.SaveAssets();
        }

        public static void PatchScenesBatchMode()
        {
            PatchScenes();
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

            EditorSceneManager.SaveScene(scene);
        }

        private static void PatchGameplayScene(string scenePath)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var controller = Object.FindFirstObjectByType<GameplayHudController>(FindObjectsInactive.Include);
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

            EditorSceneManager.SaveScene(scene);
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
