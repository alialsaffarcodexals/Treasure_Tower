using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace TreasureTower.Systems
{
    public static class UiEventSystemBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EnsureInputSystemModule();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EnsureInputSystemModule();
        }

        private static void EnsureInputSystemModule()
        {
            var eventSystem = Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
            if (eventSystem == null)
            {
                return;
            }

            var inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            // Always reassign the default UI actions at runtime so stale serialized
            // action references in scene files cannot break clicks in standalone builds.
            inputModule.AssignDefaultActions();
            inputModule.enabled = false;
            inputModule.enabled = true;

            var legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacyModule != null)
            {
                Object.Destroy(legacyModule);
            }

            if (!Application.isEditor && eventSystem.GetComponent<UiButtonClickFallback>() == null)
            {
                eventSystem.gameObject.AddComponent<UiButtonClickFallback>();
            }
        }
    }
}
