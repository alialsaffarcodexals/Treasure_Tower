using System.IO;
using TreasureTower.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TreasureTower.Editor
{
    public static class PlayerPhysicsScenePatcher
    {
        private const string MaterialFolder = "Assets/Physics";
        private const string MaterialPath = "Assets/Physics/PlayerNoFriction.physicsMaterial2D";

        [MenuItem("Tools/Treasure Tower/Patch Player Physics")]
        public static void PatchPlayerPhysics()
        {
            var material = EnsurePlayerMaterial();

            foreach (var scenePath in Directory.GetFiles("Assets/Scenes/Levels", "*.unity"))
            {
                PatchScene(scenePath.Replace('\\', '/'), material);
            }

            AssetDatabase.SaveAssets();
        }

        public static void PatchPlayerPhysicsBatchMode()
        {
            PatchPlayerPhysics();
            EditorApplication.Exit(0);
        }

        private static void PatchScene(string scenePath, PhysicsMaterial2D material)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var player = Object.FindFirstObjectByType<PlayerController2D>(FindObjectsInactive.Include);
            if (player == null)
            {
                return;
            }

            var changed = false;
            var playerCollider = player.GetComponent<BoxCollider2D>();
            if (playerCollider != null)
            {
                if (playerCollider.sharedMaterial != material)
                {
                    playerCollider.sharedMaterial = material;
                    changed = true;
                }

                if (Mathf.Abs(playerCollider.edgeRadius - 0.05f) > 0.0001f)
                {
                    playerCollider.edgeRadius = 0.05f;
                    changed = true;
                }
            }

            var body = player.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                if (body.interpolation != RigidbodyInterpolation2D.Interpolate)
                {
                    body.interpolation = RigidbodyInterpolation2D.Interpolate;
                    changed = true;
                }

                if (body.sleepMode != RigidbodySleepMode2D.NeverSleep)
                {
                    body.sleepMode = RigidbodySleepMode2D.NeverSleep;
                    changed = true;
                }

                if (body.collisionDetectionMode != CollisionDetectionMode2D.Continuous)
                {
                    body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                    changed = true;
                }

                if (!body.freezeRotation)
                {
                    body.freezeRotation = true;
                    changed = true;
                }
            }

            if (!changed)
            {
                return;
            }

            EditorUtility.SetDirty(player);
            if (playerCollider != null)
            {
                EditorUtility.SetDirty(playerCollider);
            }

            if (body != null)
            {
                EditorUtility.SetDirty(body);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static PhysicsMaterial2D EnsurePlayerMaterial()
        {
            if (!AssetDatabase.IsValidFolder(MaterialFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Physics");
            }

            var material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(MaterialPath);
            if (material == null)
            {
                material = new PhysicsMaterial2D("PlayerNoFriction")
                {
                    friction = 0f,
                    bounciness = 0f
                };
                AssetDatabase.CreateAsset(material, MaterialPath);
            }
            else
            {
                material.friction = 0f;
                material.bounciness = 0f;
                EditorUtility.SetDirty(material);
            }

            return material;
        }
    }
}
