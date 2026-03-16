using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace TreasureTower.Editor
{
    public static class EventSystemScenePatcher
    {
        [MenuItem("Tools/Treasure Tower/Patch Event Systems For Standalone")]
        public static void PatchAllScenes()
        {
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
                if (inputSystemModule != null)
                {
                    Object.DestroyImmediate(inputSystemModule, true);
                }

                if (eventSystem.GetComponent<StandaloneInputModule>() == null)
                {
                    eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Patched EventSystem components in all scenes.");
        }
    }
}
