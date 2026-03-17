using System.Collections.Generic;
using System.Linq;
using TreasureTower.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TreasureTower.Editor
{
    public static class GameplaySceneUiPatcher
    {
        private static readonly string[] GameplayScenePaths =
        {
            "Assets/Scenes/Levels/TreasureTower_Level01.unity",
            "Assets/Scenes/Levels/TreasureTower_Level02.unity",
            "Assets/Scenes/Levels/TreasureTower_Level03.unity",
            "Assets/Scenes/Levels/TreasureTower_Level04.unity",
            "Assets/Scenes/Levels/TreasureTower_Level05.unity",
            "Assets/Scenes/Levels/TreasureTower_Level01_MiniBoss.unity",
            "Assets/Scenes/Levels/TreasureTower_Level03_MiniBoss.unity"
        };

        [MenuItem("Tools/Treasure Tower/Patch Gameplay HUD And Tiles")]
        public static void PatchScenes()
        {
            var sourceLayout = ReadLevelLabelLayout("Assets/Scenes/Levels/TreasureTower_Level01.unity");
            var topBarLayout = ReadTopBarLayout("Assets/Scenes/Levels/TreasureTower_Level04.unity");

            foreach (var scenePath in GameplayScenePaths)
            {
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                var changed = false;

                changed |= EnsureLevelLabel();
                changed |= ApplyLevelLabelLayout(sourceLayout);
                changed |= ApplyTopBarLayout(topBarLayout);

                if (scenePath.EndsWith("TreasureTower_Level04.unity"))
                {
                    changed |= NormalizeLevel04Tiles(scene);
                }

                if (changed)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Patched gameplay HUD level labels and Level 4 tile hierarchy.");
        }

        private static TopBarLayout ReadTopBarLayout(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var topBar = FindTransformByName("TopBar") as RectTransform;
            if (topBar == null)
            {
                return TopBarLayout.Default;
            }

            return new TopBarLayout
            {
                AnchorMin = topBar.anchorMin,
                AnchorMax = topBar.anchorMax,
                AnchoredPosition = topBar.anchoredPosition,
                SizeDelta = topBar.sizeDelta,
                Pivot = topBar.pivot
            };
        }

        private static LevelLabelLayout ReadLevelLabelLayout(string scenePath)
        {
            var sourceScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            EnsureLevelLabel();
            var levelText = FindTransformByName("LevelText") as RectTransform;
            if (levelText == null)
            {
                return LevelLabelLayout.Default;
            }

            return new LevelLabelLayout
            {
                AnchorMin = levelText.anchorMin,
                AnchorMax = levelText.anchorMax,
                AnchoredPosition = levelText.anchoredPosition,
                SizeDelta = levelText.sizeDelta,
                Pivot = levelText.pivot,
                SiblingIndex = levelText.GetSiblingIndex()
            };
        }

        private static bool EnsureLevelLabel()
        {
            var hud = Object.FindFirstObjectByType<GameplayHudController>(FindObjectsInactive.Include);
            if (hud == null)
            {
                return false;
            }

            var serializedHud = new SerializedObject(hud);
            serializedHud.Update();
            var levelTextProperty = serializedHud.FindProperty("levelText");
            if (levelTextProperty == null)
            {
                return false;
            }

            var topBar = FindTransformByName("TopBar");
            if (topBar == null)
            {
                return false;
            }

            Text levelText = levelTextProperty.objectReferenceValue as Text;
            if (levelText == null)
            {
                var templateText = topBar.GetComponentsInChildren<Text>(true).FirstOrDefault(text => text.name == "CoinsText")
                    ?? topBar.GetComponentsInChildren<Text>(true).FirstOrDefault();

                if (templateText == null)
                {
                    return false;
                }

                var levelObject = Object.Instantiate(templateText.gameObject, topBar);
                levelObject.name = "LevelText";
                levelText = levelObject.GetComponent<Text>();

                var rect = levelObject.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(-365f, 0f);
                rect.sizeDelta = new Vector2(130f, 30f);
                rect.SetSiblingIndex(0);
                levelText.alignment = TextAnchor.MiddleCenter;
                levelText.raycastTarget = false;
                levelText.text = "Level";
            }

            levelTextProperty.objectReferenceValue = levelText;
            serializedHud.ApplyModifiedProperties();
            return true;
        }

        private static bool ApplyLevelLabelLayout(LevelLabelLayout layout)
        {
            var levelText = FindTransformByName("LevelText") as RectTransform;
            if (levelText == null)
            {
                return false;
            }

            var changed = false;
            changed |= SetIfDifferent(levelText.anchorMin, layout.AnchorMin, value => levelText.anchorMin = value);
            changed |= SetIfDifferent(levelText.anchorMax, layout.AnchorMax, value => levelText.anchorMax = value);
            changed |= SetIfDifferent(levelText.pivot, layout.Pivot, value => levelText.pivot = value);
            changed |= SetIfDifferent(levelText.anchoredPosition, layout.AnchoredPosition, value => levelText.anchoredPosition = value);
            changed |= SetIfDifferent(levelText.sizeDelta, layout.SizeDelta, value => levelText.sizeDelta = value);

            if (levelText.GetSiblingIndex() != layout.SiblingIndex)
            {
                levelText.SetSiblingIndex(layout.SiblingIndex);
                changed = true;
            }

            return changed;
        }

        private static bool ApplyTopBarLayout(TopBarLayout layout)
        {
            var topBar = FindTransformByName("TopBar") as RectTransform;
            if (topBar == null)
            {
                return false;
            }

            var changed = false;
            changed |= SetIfDifferent(topBar.anchorMin, layout.AnchorMin, value => topBar.anchorMin = value);
            changed |= SetIfDifferent(topBar.anchorMax, layout.AnchorMax, value => topBar.anchorMax = value);
            changed |= SetIfDifferent(topBar.pivot, layout.Pivot, value => topBar.pivot = value);
            changed |= SetIfDifferent(topBar.anchoredPosition, layout.AnchoredPosition, value => topBar.anchoredPosition = value);
            changed |= SetIfDifferent(topBar.sizeDelta, layout.SizeDelta, value => topBar.sizeDelta = value);
            return changed;
        }

        private static bool NormalizeLevel04Tiles(Scene scene)
        {
            var tileObjects = scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .Select(transform => transform.gameObject)
                .Where(IsTileObject)
                .ToList();

            if (tileObjects.Count == 0)
            {
                return false;
            }

            var changed = false;
            var manualGroup = FindTransformByName("ManualTiles_Level04");
            if (manualGroup == null)
            {
                var groupObject = new GameObject("ManualTiles_Level04");
                manualGroup = groupObject.transform;
                changed = true;
            }

            foreach (var tileObject in tileObjects.Where(tile => tile.transform.parent == null))
            {
                tileObject.transform.SetParent(manualGroup, true);
                changed = true;
            }

            var parentGroups = tileObjects
                .Select(tile => tile.transform.parent)
                .Where(parent => parent != null)
                .Distinct()
                .ToList();

            foreach (var parent in parentGroups)
            {
                var tiles = parent.Cast<Transform>()
                    .Select(child => child.gameObject)
                    .Where(IsTileObject)
                    .OrderByDescending(tile => tile.transform.position.y)
                    .ThenBy(tile => tile.transform.position.x)
                    .ToList();

                for (var index = 0; index < tiles.Count; index++)
                {
                    var desiredName = $"Tile_{index}";
                    if (tiles[index].name != desiredName)
                    {
                        tiles[index].name = desiredName;
                        changed = true;
                    }
                }
            }

            if (manualGroup.childCount > 0)
            {
                var looseTiles = manualGroup.Cast<Transform>()
                    .Select(child => child.gameObject)
                    .Where(IsTileObject)
                    .OrderByDescending(tile => tile.transform.position.y)
                    .ThenBy(tile => tile.transform.position.x)
                    .ToList();

                for (var index = 0; index < looseTiles.Count; index++)
                {
                    var desiredName = $"Tile_{index}";
                    if (looseTiles[index].name != desiredName)
                    {
                        looseTiles[index].name = desiredName;
                        changed = true;
                    }
                }
            }

            return changed;
        }

        private static bool IsTileObject(GameObject gameObject)
        {
            return gameObject.layer == 6 &&
                   gameObject.name.StartsWith("Tile") &&
                   gameObject.GetComponent<SpriteRenderer>() != null &&
                   gameObject.GetComponent<BoxCollider2D>() != null;
        }

        private static Transform FindTransformByName(string objectName)
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                var result = FindTransformRecursive(root.transform, objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static Transform FindTransformRecursive(Transform parent, string objectName)
        {
            if (parent.name == objectName)
            {
                return parent;
            }

            for (var childIndex = 0; childIndex < parent.childCount; childIndex++)
            {
                var child = parent.GetChild(childIndex);
                var result = FindTransformRecursive(child, objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static bool SetIfDifferent(Vector2 currentValue, Vector2 newValue, System.Action<Vector2> setter)
        {
            if (currentValue == newValue)
            {
                return false;
            }

            setter(newValue);
            return true;
        }

        private struct LevelLabelLayout
        {
            public Vector2 AnchorMin;
            public Vector2 AnchorMax;
            public Vector2 AnchoredPosition;
            public Vector2 SizeDelta;
            public Vector2 Pivot;
            public int SiblingIndex;

            public static LevelLabelLayout Default => new()
            {
                AnchorMin = new Vector2(0.5f, 0.5f),
                AnchorMax = new Vector2(0.5f, 0.5f),
                AnchoredPosition = new Vector2(-365f, 0f),
                SizeDelta = new Vector2(130f, 30f),
                Pivot = new Vector2(0.5f, 0.5f),
                SiblingIndex = 0
            };
        }

        private struct TopBarLayout
        {
            public Vector2 AnchorMin;
            public Vector2 AnchorMax;
            public Vector2 AnchoredPosition;
            public Vector2 SizeDelta;
            public Vector2 Pivot;

            public static TopBarLayout Default => new()
            {
                AnchorMin = new Vector2(0.5f, 1f),
                AnchorMax = new Vector2(0.5f, 1f),
                AnchoredPosition = new Vector2(0f, -52f),
                SizeDelta = new Vector2(1000f, 64f),
                Pivot = new Vector2(0.5f, 1f)
            };
        }
    }
}
