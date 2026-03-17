using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace TreasureTower.Editor
{
    public static class EventSystemScenePatcher
    {
        private const string InputActionsAssetPath = "Assets/InputSystem_Actions.inputactions";

        [MenuItem("Tools/Treasure Tower/Patch Event Systems For Input System UI")]
        public static void PatchAllScenes()
        {
            var inputActionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsAssetPath);
            if (inputActionsAsset == null)
            {
                throw new FileNotFoundException($"Unable to load Input Actions asset at '{InputActionsAssetPath}'.");
            }

            var scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            foreach (var sceneGuid in scenePaths)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var eventSystem = Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
                if (eventSystem == null)
                {
                    continue;
                }

                var inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                if (inputSystemModule == null)
                {
                    inputSystemModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }

                inputSystemModule.actionsAsset = inputActionsAsset;

                var standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
                if (standaloneModule != null)
                {
                    Object.DestroyImmediate(standaloneModule, true);
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Patched EventSystem components in all scenes for Input System UI.");
        }
    }
}
