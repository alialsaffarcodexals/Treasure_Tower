using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TreasureTower.Systems
{
    public sealed class UiButtonClickFallback : MonoBehaviour
    {
        private readonly List<RaycastResult> raycastResults = new();
        private readonly List<GraphicRaycaster> raycasters = new();

        private EventSystem cachedEventSystem;

        private void Awake()
        {
            cachedEventSystem = GetComponent<EventSystem>();
            RefreshRaycasters();
        }

        private void Update()
        {
            if (Application.isEditor)
            {
                return;
            }

            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (cachedEventSystem == null)
            {
                cachedEventSystem = EventSystem.current;
            }

            if (cachedEventSystem == null)
            {
                return;
            }

            if (raycasters.Count == 0)
            {
                RefreshRaycasters();
            }

            var pointerData = new PointerEventData(cachedEventSystem)
            {
                position = Mouse.current.position.ReadValue()
            };

            for (var index = 0; index < raycasters.Count; index++)
            {
                var raycaster = raycasters[index];
                if (raycaster == null || !raycaster.isActiveAndEnabled)
                {
                    continue;
                }

                raycastResults.Clear();
                raycaster.Raycast(pointerData, raycastResults);
                for (var resultIndex = 0; resultIndex < raycastResults.Count; resultIndex++)
                {
                    var button = raycastResults[resultIndex].gameObject.GetComponentInParent<Button>();
                    if (button == null || !button.IsActive() || !button.IsInteractable())
                    {
                        continue;
                    }

                    button.onClick.Invoke();
                    return;
                }
            }
        }

        private void RefreshRaycasters()
        {
            raycasters.Clear();
            raycasters.AddRange(FindObjectsByType<GraphicRaycaster>(FindObjectsInactive.Exclude, FindObjectsSortMode.None));
        }
    }
}
