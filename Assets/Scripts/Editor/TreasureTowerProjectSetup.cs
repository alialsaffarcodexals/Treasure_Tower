using System.IO;
using TreasureTower.Core;
using TreasureTower.Enemies;
using TreasureTower.Environment;
using TreasureTower.Player;
using TreasureTower.Systems;
using TreasureTower.UI;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace TreasureTower.Editor
{
    public static class TreasureTowerProjectSetup
    {
        private const int GroundLayer = 6;
        private const int PlayerLayer = 7;

        [MenuItem("Tools/Treasure Tower/Build Initial Game")]
        public static void BuildInitialGame()
        {
            EnsureFolders();
            CreateMainMenuScene();
            CreateLevelOneScene();
            CreateLevelOneMiniBossScene();
            CreateLevelTwoScene();
            CreateLevelThreeScene();
            CreateLevelThreeMiniBossScene();
            CreateLevelFourScene();
            CreateLevelFiveScene();
            UpdateBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Treasure Tower initial scenes created.");
        }

        public static void BuildInitialGameBatch()
        {
            BuildInitialGame();
            EditorApplication.Exit(0);
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory("Assets/Scenes/Menus");
            Directory.CreateDirectory("Assets/Scenes/Levels");
        }

        private static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MainMenu";

            CreateMainCamera(new Color(0.76f, 0.86f, 0.93f));
            CreateEventSystem();
            var canvas = CreateCanvas("Canvas");
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_title.mp3", 0.34f);
            CreateMainMenuBackground();

            var menuController = new GameObject("MainMenuController").AddComponent<MainMenuController>();
            var homePanel = CreatePanel("HomePanel", canvas.transform, Vector2.zero, new Vector2(1920f, 1080f), new Color(1f, 1f, 1f, 0f));
            StretchFullScreen(homePanel.GetComponent<RectTransform>());

            var title = CreateText("Title", homePanel.transform, "TREASURE TOWER", 48, new Vector2(0f, 205f), new Vector2(760f, 100f));
            title.alignment = TextAnchor.MiddleCenter;
            title.color = new Color(0.17f, 0.22f, 0.29f);

            var subtitle = CreateText("Subtitle", homePanel.transform, "Climb, collect, and reach the top.", 22, new Vector2(0f, 155f), new Vector2(760f, 70f));
            subtitle.alignment = TextAnchor.MiddleCenter;
            subtitle.color = new Color(0.25f, 0.31f, 0.38f);

            var credit = CreateText("Credit", homePanel.transform, "Created by Ali Alsaffar 202301152", 20, new Vector2(0f, 116f), new Vector2(760f, 50f));
            credit.alignment = TextAnchor.MiddleCenter;
            credit.color = new Color(0.28f, 0.34f, 0.41f);

            var startButton = CreateButton("StartButton", homePanel.transform, "Start Game", new Vector2(-140f, 25f));
            UnityEventTools.AddPersistentListener(startButton.onClick, menuController.StartGame);

            var storyButton = CreateButton("StoryButton", homePanel.transform, "Story", new Vector2(140f, 25f));
            UnityEventTools.AddPersistentListener(storyButton.onClick, menuController.ShowStory);

            var leaderboardButton = CreateButton("LeaderboardButton", homePanel.transform, "Leaderboard", new Vector2(-140f, -55f));
            UnityEventTools.AddPersistentListener(leaderboardButton.onClick, menuController.ShowLeaderboard);

            var settingsButton = CreateButton("SettingsButton", homePanel.transform, "Settings", new Vector2(140f, -55f));
            UnityEventTools.AddPersistentListener(settingsButton.onClick, menuController.ShowSettings);

            var quitButton = CreateButton("QuitButton", homePanel.transform, "Quit", new Vector2(0f, -135f));
            UnityEventTools.AddPersistentListener(quitButton.onClick, menuController.QuitGame);

            var controls = CreatePanel("ControlsPanel", homePanel.transform, new Vector2(0f, -270f), new Vector2(760f, 190f), new Color(1f, 1f, 1f, 0.8f));
            var controlsTitle = CreateText("ControlsTitle", controls.transform, "Controls", 26, new Vector2(0f, 62f), new Vector2(260f, 40f));
            controlsTitle.color = new Color(0.2f, 0.25f, 0.32f);
            CreateControlTextRow(controls.transform, "MoveRow", "Move", "A / D  or  Left / Right Arrow", new Vector2(0f, 38f));
            CreateControlTextRow(controls.transform, "JumpRow", "Jump", "Space", new Vector2(0f, -28f));
            CreateControlTextRow(controls.transform, "ShootRow", "Mini Boss Shoot", "F", new Vector2(0f, -8f));
            CreateControlTextRow(controls.transform, "PauseRow", "Pause", "Esc", new Vector2(0f, -54f));

            var storyPanel = CreateOverlay(canvas.transform, "StoryPanel", "Story Of The Game", new Color(0.08f, 0.12f, 0.18f, 0.82f));
            var storyCard = storyPanel.transform.Find("Card");
            storyCard.GetComponent<RectTransform>().sizeDelta = new Vector2(860f, 520f);
            storyCard.Find("Title").GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 190f);
            var storyText = CreateText(
                "StoryText",
                storyCard,
                "A hidden treasure tower rises above the kingdom. Ali, the young climber, must cross five dangerous floors, collect the tower gems, and unlock the final summit door before the ancient treasure is lost forever.\n\nSome floors hide risky mini-boss doors. If the player enters one, they must defeat a stronger enemy in a separate arena. Winning opens a special exit that skips ahead to a later level, but losing costs a life.\n\nEvery level tests a different skill: timing, courage, careful jumps, and survival against ground and flying enemies. Collect more coins, finish faster, and die less to improve your record.\n\nGame Development subject: IT8101\nSupervised by Doctor Albaraa Janahi\nCourse Coordinator: Haetham Alhadad\nSection 2, Year Semester 2, 2025-2026",
                22,
                new Vector2(0f, 30f),
                new Vector2(760f, 280f));
            storyText.alignment = TextAnchor.UpperLeft;
            storyText.color = new Color(0.2f, 0.25f, 0.32f);
            var storyBackButton = CreateButton("StoryBackButton", storyCard, "Back", new Vector2(0f, -180f));
            UnityEventTools.AddPersistentListener(storyBackButton.onClick, menuController.ShowHome);

            var leaderboardPanel = CreateOverlay(canvas.transform, "LeaderboardPanel", "Completed Attempts", new Color(0.08f, 0.12f, 0.18f, 0.82f));
            var leaderboardCard = leaderboardPanel.transform.Find("Card");
            leaderboardCard.GetComponent<RectTransform>().sizeDelta = new Vector2(860f, 520f);
            var leaderboardText = CreateText("LeaderboardText", leaderboardCard, "No completed attempts yet.", 22, new Vector2(0f, 20f), new Vector2(760f, 320f));
            leaderboardText.alignment = TextAnchor.UpperLeft;
            leaderboardText.color = new Color(0.2f, 0.25f, 0.32f);
            var leaderboardBackButton = CreateButton("LeaderboardBackButton", leaderboardCard, "Back", new Vector2(0f, -180f));
            UnityEventTools.AddPersistentListener(leaderboardBackButton.onClick, menuController.ShowHome);

            var settingsPanel = CreateOverlay(canvas.transform, "SettingsPanel", "Audio Settings", new Color(0.08f, 0.12f, 0.18f, 0.82f));
            var settingsCard = settingsPanel.transform.Find("Card");
            settingsCard.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, 460f);
            settingsCard.Find("Title").GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 150f);
            var settingsHint = CreateText("SettingsHint", settingsCard, "Adjust music and sound effects volume.", 22, new Vector2(0f, 98f), new Vector2(520f, 40f));
            settingsHint.color = new Color(0.2f, 0.25f, 0.32f);
            var musicLabel = CreateText("MusicLabel", settingsCard, "Music", 22, new Vector2(-210f, 26f), new Vector2(140f, 34f));
            musicLabel.alignment = TextAnchor.MiddleLeft;
            var musicSlider = CreateSlider("MusicSlider", settingsCard, new Vector2(70f, 26f), new Vector2(340f, 30f), 0.75f);
            UnityEventTools.AddPersistentListener(musicSlider.onValueChanged, menuController.SetMusicVolume);
            var sfxLabel = CreateText("SfxLabel", settingsCard, "Sound FX", 22, new Vector2(-210f, -36f), new Vector2(140f, 34f));
            sfxLabel.alignment = TextAnchor.MiddleLeft;
            var sfxSlider = CreateSlider("SfxSlider", settingsCard, new Vector2(70f, -36f), new Vector2(340f, 30f), 0.85f);
            UnityEventTools.AddPersistentListener(sfxSlider.onValueChanged, menuController.SetSfxVolume);
            var settingsBackButton = CreateButton("SettingsBackButton", settingsCard, "Back", new Vector2(0f, -150f));
            UnityEventTools.AddPersistentListener(settingsBackButton.onClick, menuController.ShowHome);

            storyPanel.SetActive(false);
            leaderboardPanel.SetActive(false);
            settingsPanel.SetActive(false);

            SetObjectReference(menuController, "homePanel", homePanel);
            SetObjectReference(menuController, "storyPanel", storyPanel);
            SetObjectReference(menuController, "leaderboardPanel", leaderboardPanel);
            SetObjectReference(menuController, "settingsPanel", settingsPanel);
            SetObjectReference(menuController, "leaderboardText", leaderboardText);
            SetObjectReference(menuController, "musicSlider", musicSlider);
            SetObjectReference(menuController, "sfxSlider", sfxSlider);

            EditorSceneManager.SaveScene(scene, SceneIds.MainMenu);
        }

        private static void CreateLevelOneScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TreasureTower_Level01";

            var camera = CreateMainCamera(new Color(0.53f, 0.84f, 0.99f));
            CreateEventSystem();
            CreateGameplayHud();
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_ingame.mp3", 0.32f);
            CreateBackground();
            CreateLevelOneWorld(camera.transform);

            EditorSceneManager.SaveScene(scene, SceneIds.Level01);
        }

        private static void CreateLevelTwoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TreasureTower_Level02";

            var camera = CreateMainCamera(new Color(0.56f, 0.58f, 0.78f));
            CreateEventSystem();
            CreateGameplayHud();
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_ingame.mp3", 0.32f);
            CreateLevelTwoBackground();
            CreateLevelTwoWorld(camera.transform);

            EditorSceneManager.SaveScene(scene, SceneIds.Level02);
        }

        private static void CreateLevelOneMiniBossScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TreasureTower_Level01_MiniBoss";

            var camera = CreateMainCamera(new Color(0.25f, 0.28f, 0.36f));
            CreateEventSystem();
            CreateGameplayHud();
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_boss.mp3", 0.34f);
            CreateLevelOneMiniBossBackground();
            var boss = CreateLevelOneMiniBossWorld(camera.transform);
            CreateMiniBossHud(boss);

            EditorSceneManager.SaveScene(scene, SceneIds.Level01MiniBoss);
        }

        private static void CreateLevelThreeScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TreasureTower_Level03";

            var camera = CreateMainCamera(new Color(0.28f, 0.31f, 0.42f));
            CreateEventSystem();
            CreateGameplayHud();
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_ingame.mp3", 0.32f);
            CreateLevelThreeBackground();
            CreateLevelThreeWorld(camera.transform);

            EditorSceneManager.SaveScene(scene, SceneIds.Level03);
        }

        private static void CreateLevelFourScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TreasureTower_Level04";

            var camera = CreateMainCamera(new Color(0.74f, 0.88f, 0.97f));
            CreateEventSystem();
            CreateGameplayHud();
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_ingame.mp3", 0.32f);
            CreateLevelFourBackground();
            CreateLevelFourWorld(camera.transform);

            EditorSceneManager.SaveScene(scene, SceneIds.Level04);
        }

        private static void CreateLevelThreeMiniBossScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TreasureTower_Level03_MiniBoss";

            var camera = CreateMainCamera(new Color(0.19f, 0.16f, 0.22f));
            CreateEventSystem();
            CreateGameplayHud();
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_boss.mp3", 0.34f);
            CreateLevelThreeMiniBossBackground();
            var boss = CreateLevelThreeMiniBossWorld(camera.transform);
            CreateMiniBossHud(boss);

            EditorSceneManager.SaveScene(scene, SceneIds.Level03MiniBoss);
        }

        private static void CreateLevelFiveScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "TreasureTower_Level05";

            var camera = CreateMainCamera(new Color(0.96f, 0.78f, 0.55f));
            CreateEventSystem();
            CreateGameplayHud();
            CreateSceneMusic("Assets/Audio/Music/OpenGameArt/PlatformerMusic/lsc_boss.mp3", 0.34f);
            CreateLevelFiveBackground();
            CreateLevelFiveWorld(camera.transform);

            EditorSceneManager.SaveScene(scene, SceneIds.Level05);
        }

        private static void CreateBackground()
        {
            var cloudSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/cloud1.png");
            var backdropSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/castle.png");

            CreateDecorSprite("CloudLeft", cloudSprite, new Vector3(-7.5f, 3.4f, 0f), new Vector3(0.95f, 0.95f, 1f), new Color(1f, 1f, 1f, 0.9f), -5);
            CreateDecorSprite("CloudMiddle", cloudSprite, new Vector3(0.5f, 2.9f, 0f), new Vector3(0.9f, 0.9f, 1f), new Color(1f, 1f, 1f, 0.8f), -5);
            CreateDecorSprite("CloudRight", cloudSprite, new Vector3(7.8f, 3.2f, 0f), new Vector3(1.05f, 1.05f, 1f), new Color(1f, 1f, 1f, 0.85f), -5);
            CreateDecorSprite("Backdrop", backdropSprite, new Vector3(8.25f, -1.45f, 0f), new Vector3(1.25f, 1.25f, 1f), new Color(1f, 1f, 1f, 0.72f), -4);
        }

        private static void CreateMainMenuBackground()
        {
            var cloudsSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/clouds1.png");
            var hillsSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/hills1.png");
            var castleSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/castle_beige.png");
            var towerSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tower_beige.png");

            CreateDecorSprite("MenuCloudsLeft", cloudsSprite, new Vector3(-6.5f, 3.6f, 0f), new Vector3(1.2f, 1.05f, 1f), new Color(1f, 1f, 1f, 0.72f), -6);
            CreateDecorSprite("MenuCloudsRight", cloudsSprite, new Vector3(6.2f, 3.4f, 0f), new Vector3(1.35f, 1.08f, 1f), new Color(1f, 1f, 1f, 0.68f), -6);
            CreateDecorSprite("MenuHills", hillsSprite, new Vector3(0.2f, -1.95f, 0f), new Vector3(2.8f, 1.9f, 1f), new Color(0.88f, 0.94f, 0.86f, 0.78f), -5);
            CreateDecorSprite("MenuCastle", castleSprite, new Vector3(6.9f, -0.75f, 0f), new Vector3(1.45f, 1.45f, 1f), new Color(1f, 1f, 1f, 0.56f), -4);
            CreateDecorSprite("MenuTower", towerSprite, new Vector3(-7.2f, -0.55f, 0f), new Vector3(1.2f, 1.2f, 1f), new Color(1f, 1f, 1f, 0.48f), -4);
        }

        private static void CreateLevelOneWorld(Transform cameraTransform)
        {
            var worldRoot = new GameObject("World");
            var player = CreatePlayer(worldRoot.transform, new Vector3(-8.4f, -2.2f, 0f));
            CreatePlatforms(worldRoot.transform);
            CreateCollectibles(worldRoot.transform);
            CreateEnemy(worldRoot.transform);
            CreateEnemy(worldRoot.transform, "MiniDoorGuardA", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/snail.png", new Vector3(-2.8f, -2.12f, 0f), 1.3f, 1.8f);
            CreateEnemy(worldRoot.transform, "MiniDoorGuardB", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/worm.png", new Vector3(0.2f, -2.12f, 0f), 1.2f, 2.0f);
            CreateHazards(worldRoot.transform);
            CreateMiniBossBranchDoor(worldRoot.transform, "MiniBossDoor_Level01", new Vector3(-1.25f, -2.12f, 0f), SceneIds.Level01MiniBoss, SceneIds.Level01, SceneIds.Level03);
            CreateGoal(worldRoot.transform, new Vector3(8.4f, 2.08f, 0f));
            CreateFallDeathZone(worldRoot.transform, -4.15f, 26f);
            CreateBoundaries(worldRoot.transform);
            CreateCameraFollow(cameraTransform, player.transform, new Vector2(-1.5f, 0f), new Vector2(1.5f, 0f));
        }

        private static void CreateLevelTwoWorld(Transform cameraTransform)
        {
            var worldRoot = new GameObject("World");
            var player = CreatePlayer(worldRoot.transform, new Vector3(-12.2f, -2.2f, 0f));

            var terrainSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/terrain_purple_block_center.png");
            CreatePlatformRow(worldRoot.transform, "Ground", terrainSprite, new Vector3(-13.2f, -3f, 0f), 36);
            CreatePlatformRow(worldRoot.transform, "RiseA", terrainSprite, new Vector3(-10.2f, -1.75f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "RiseB", terrainSprite, new Vector3(-6.1f, -0.45f, 0f), 4);
            CreatePlatformRow(worldRoot.transform, "RiseC", terrainSprite, new Vector3(-1.2f, 0.85f, 0f), 4);
            CreatePlatformRow(worldRoot.transform, "RiseD", terrainSprite, new Vector3(3.9f, -0.1f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "RiseE", terrainSprite, new Vector3(7.6f, 1.15f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "GoalLedge", terrainSprite, new Vector3(11.1f, 2.4f, 0f), 4);

            var coinSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/coin_gold.png");
            var gemSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/gem_blue.png");
            CreateCollectible(worldRoot.transform, "CoinA", coinSprite, new Vector3(-9.4f, -0.9f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinB", coinSprite, new Vector3(-5.1f, 0.35f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinC", coinSprite, new Vector3(-0.15f, 1.7f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinD", coinSprite, new Vector3(4.8f, 0.85f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinE", coinSprite, new Vector3(9.0f, 2.15f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "GemA", gemSprite, new Vector3(-0.1f, 2.7f, 0f), Collectible.CollectibleKind.Gem);

            CreateEnemy(worldRoot.transform, "BatEnemy", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bat_fly.png", new Vector3(-3.6f, 1.55f, 0f), 1.4f, 1.9f);
            CreateEnemy(worldRoot.transform, "BeeEnemy", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(5.8f, 0.7f, 0f), 1.6f, 2.3f);
            CreateEnemy(worldRoot.transform, "FlyEnemy", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/fly_fly.png", new Vector3(10.0f, 2.0f, 0f), 1.2f, 2.5f);

            CreateSpikes(worldRoot.transform, new Vector3(-7.15f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(2.1f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(6.25f, -2.44f, 0f));
            CreateGoal(worldRoot.transform, new Vector3(12.35f, 3.2f, 0f));
            CreateFallDeathZone(worldRoot.transform, -4.15f, 30f);
            CreateBoundaries(worldRoot.transform, new Vector2(-13.8f, 13.8f), new Vector2(-4.55f, 4.75f));
            CreateCameraFollow(cameraTransform, player.transform, new Vector2(-4.3f, 0f), new Vector2(4.3f, 0.3f));
        }

        private static void CreateLevelThreeWorld(Transform cameraTransform)
        {
            var worldRoot = new GameObject("World");
            var player = CreatePlayer(worldRoot.transform, new Vector3(-15.4f, -2.2f, 0f));

            var terrainSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/terrain_stone_block_center.png");
            var gemSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/gem_red.png");
            var coinSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/coin_gold.png");

            CreatePlatformRow(worldRoot.transform, "Ground", terrainSprite, new Vector3(-16.4f, -3f, 0f), 46);
            CreatePlatformRow(worldRoot.transform, "RiseA", terrainSprite, new Vector3(-12.8f, -1.95f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "RiseB", terrainSprite, new Vector3(-9.1f, -0.95f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "RiseC", terrainSprite, new Vector3(-5.2f, 0.15f, 0f), 4);
            CreatePlatformRow(worldRoot.transform, "RiseD", terrainSprite, new Vector3(0.2f, 1.25f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "DropTrap", terrainSprite, new Vector3(4.9f, -0.35f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "RiseE", terrainSprite, new Vector3(8.7f, 1.0f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "RiseF", terrainSprite, new Vector3(12.8f, 2.0f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "GoalLedge", terrainSprite, new Vector3(17.2f, 3.0f, 0f), 4);

            CreateCollectible(worldRoot.transform, "CoinA", coinSprite, new Vector3(-11.9f, -1.1f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinB", coinSprite, new Vector3(-8.2f, -0.1f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinC", coinSprite, new Vector3(-4.1f, 1.0f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinD", coinSprite, new Vector3(1.2f, 2.1f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinE", coinSprite, new Vector3(6.0f, 0.4f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinF", coinSprite, new Vector3(10.0f, 1.85f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinG", coinSprite, new Vector3(14.0f, 2.8f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "GemA", gemSprite, new Vector3(1.0f, 3.25f, 0f), Collectible.CollectibleKind.Gem);

            CreateEnemy(worldRoot.transform, "BatEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bat_fly.png", new Vector3(-6.8f, 1.25f, 0f), 1.5f, 2.3f);
            CreateEnemy(worldRoot.transform, "BeeEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(5.9f, 0.45f, 0f), 1.7f, 2.5f);
            CreateEnemy(worldRoot.transform, "FlyEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/fly_fly.png", new Vector3(11.2f, 1.7f, 0f), 1.4f, 2.7f);
            CreateEnemy(worldRoot.transform, "BatEnemy_B", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bat_fly.png", new Vector3(16.0f, 2.7f, 0f), 1.3f, 2.9f);
            CreateEnemy(worldRoot.transform, "MiniDoorGuardC", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/mouse.png", new Vector3(3.6f, -2.12f, 0f), 1.6f, 2.1f);
            CreateEnemy(worldRoot.transform, "MiniDoorGuardD", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/snail.png", new Vector3(6.4f, -2.12f, 0f), 1.4f, 1.9f);

            CreateSpikes(worldRoot.transform, new Vector3(-10.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(-1.4f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(6.8f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(15.0f, -2.44f, 0f));
            CreateMiniBossBranchDoor(worldRoot.transform, "MiniBossDoor_Level03", new Vector3(5.05f, -2.12f, 0f), SceneIds.Level03MiniBoss, SceneIds.Level03, SceneIds.Level05);
            CreateGoal(worldRoot.transform, new Vector3(18.45f, 3.85f, 0f));
            CreateFallDeathZone(worldRoot.transform, -4.15f, 38f);
            CreateBoundaries(worldRoot.transform, new Vector2(-17.0f, 19.0f), new Vector2(-4.55f, 5.1f));
            CreateCameraFollow(cameraTransform, player.transform, new Vector2(-7.0f, 0.2f), new Vector2(10.5f, 1.4f));
        }

        private static void CreateLevelFourWorld(Transform cameraTransform)
        {
            var worldRoot = new GameObject("World");
            var player = CreatePlayer(worldRoot.transform, new Vector3(-18.2f, -2.25f, 0f));

            var terrainSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/terrain_snow_block_center.png");
            var coinSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/coin_gold.png");
            var gemSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/gem_green.png");

            CreatePlatformRow(worldRoot.transform, "Ground", terrainSprite, new Vector3(-19.2f, -3f, 0f), 56);
            CreatePlatformRow(worldRoot.transform, "LaunchA", terrainSprite, new Vector3(-15.3f, -1.8f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "LaunchB", terrainSprite, new Vector3(-10.7f, -0.55f, 0f), 4);
            CreatePlatformRow(worldRoot.transform, "BridgeA", terrainSprite, new Vector3(-5.4f, -0.1f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "BridgeB", terrainSprite, new Vector3(-0.8f, 0.85f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "ClimbA", terrainSprite, new Vector3(4.2f, 1.8f, 0f), 2);
            CreatePlatformRow(worldRoot.transform, "ClimbB", terrainSprite, new Vector3(8.3f, 2.65f, 0f), 2);
            CreatePlatformRow(worldRoot.transform, "SummitA", terrainSprite, new Vector3(12.2f, 3.0f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "SummitB", terrainSprite, new Vector3(17.0f, 1.25f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "GoalLedge", terrainSprite, new Vector3(21.8f, 3.35f, 0f), 4);

            CreateCollectible(worldRoot.transform, "CoinA", coinSprite, new Vector3(-16.0f, -2.0f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinB", coinSprite, new Vector3(-12.9f, -0.95f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinC", coinSprite, new Vector3(-8.6f, 0.3f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinD", coinSprite, new Vector3(-4.0f, 0.8f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinE", coinSprite, new Vector3(0.2f, 1.7f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinF", coinSprite, new Vector3(5.2f, 2.65f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinG", coinSprite, new Vector3(9.1f, 3.45f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinH", coinSprite, new Vector3(13.4f, 3.95f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinI", coinSprite, new Vector3(18.0f, 2.2f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "GemA", gemSprite, new Vector3(9.6f, 4.05f, 0f), Collectible.CollectibleKind.Gem);

            CreateEnemy(worldRoot.transform, "WormEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/worm.png", new Vector3(-13.4f, -2.12f, 0f), 1.4f, 2.0f);
            CreateEnemy(worldRoot.transform, "SnailEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/snail.png", new Vector3(-6.2f, -2.12f, 0f), 1.6f, 1.9f);
            CreateEnemy(worldRoot.transform, "MouseEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/mouse.png", new Vector3(0.9f, -2.12f, 0f), 1.5f, 2.2f);
            CreateEnemy(worldRoot.transform, "BeeEnemy_B", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(6.0f, 2.25f, 0f), 1.8f, 2.9f);
            CreateEnemy(worldRoot.transform, "FlyEnemy_B", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/fly_fly.png", new Vector3(13.3f, 3.0f, 0f), 1.7f, 3.0f);
            CreateEnemy(worldRoot.transform, "BatEnemy_C", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bat_fly.png", new Vector3(18.2f, 2.2f, 0f), 1.6f, 3.1f);
            CreateEnemy(worldRoot.transform, "BeeEnemy_C", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(22.0f, 4.15f, 0f), 1.2f, 3.2f);

            CreateSpikes(worldRoot.transform, new Vector3(-12.2f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(-7.2f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(-1.9f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(4.8f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(11.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(19.1f, -2.44f, 0f));

            CreateGoal(worldRoot.transform, new Vector3(23.2f, 4.2f, 0f));
            CreateFallDeathZone(worldRoot.transform, -4.15f, 46f);
            CreateBoundaries(worldRoot.transform, new Vector2(-20.0f, 24.2f), new Vector2(-4.55f, 5.65f));
            CreateCameraFollow(cameraTransform, player.transform, new Vector2(-9.2f, 0.2f), new Vector2(14.8f, 1.9f));
        }

        private static void CreateLevelFiveWorld(Transform cameraTransform)
        {
            var worldRoot = new GameObject("World");
            var player = CreatePlayer(worldRoot.transform, new Vector3(-21.6f, -2.25f, 0f));

            var terrainSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/terrain_sand_block_center.png");
            var coinSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/coin_gold.png");
            var gemSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/gem_yellow.png");

            CreatePlatformRow(worldRoot.transform, "Ground", terrainSprite, new Vector3(-23.8f, -3f, 0f), 70);
            CreatePlatformRow(worldRoot.transform, "StepA", terrainSprite, new Vector3(-19.2f, -1.9f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "StepB", terrainSprite, new Vector3(-14.3f, -0.75f, 0f), 4);
            CreatePlatformRow(worldRoot.transform, "StepC", terrainSprite, new Vector3(-8.4f, 0.35f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "MidBridgeA", terrainSprite, new Vector3(-2.4f, 1.35f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "MidBridgeB", terrainSprite, new Vector3(2.3f, 0.2f, 0f), 2);
            CreatePlatformRow(worldRoot.transform, "UpperRunA", terrainSprite, new Vector3(6.5f, 1.75f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "UpperRunB", terrainSprite, new Vector3(11.4f, 2.85f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "UpperRunC", terrainSprite, new Vector3(16.2f, 1.4f, 0f), 2);
            CreatePlatformRow(worldRoot.transform, "SkyA", terrainSprite, new Vector3(20.5f, 2.7f, 0f), 2);
            CreatePlatformRow(worldRoot.transform, "SkyB", terrainSprite, new Vector3(24.6f, 3.8f, 0f), 2);
            CreatePlatformRow(worldRoot.transform, "SkyC", terrainSprite, new Vector3(28.7f, 4.8f, 0f), 2);
            CreatePlatformRow(worldRoot.transform, "GoalLedge", terrainSprite, new Vector3(32.8f, 5.85f, 0f), 4);

            CreateCollectible(worldRoot.transform, "CoinA", coinSprite, new Vector3(-20.1f, -0.95f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinB", coinSprite, new Vector3(-15.0f, 0.2f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinC", coinSprite, new Vector3(-9.0f, 1.3f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinD", coinSprite, new Vector3(-3.3f, 2.35f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinE", coinSprite, new Vector3(3.2f, 1.2f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinF", coinSprite, new Vector3(7.5f, 2.75f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinG", coinSprite, new Vector3(12.3f, 3.85f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinH", coinSprite, new Vector3(17.1f, 2.35f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinI", coinSprite, new Vector3(21.4f, 3.7f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinJ", coinSprite, new Vector3(25.4f, 4.8f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinK", coinSprite, new Vector3(29.6f, 5.8f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "CoinL", coinSprite, new Vector3(33.8f, 6.95f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(worldRoot.transform, "GemA", gemSprite, new Vector3(13.8f, 5.05f, 0f), Collectible.CollectibleKind.Gem);

            CreateEnemy(worldRoot.transform, "WormEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/worm.png", new Vector3(-18.2f, -1.98f, 0f), 1.7f, 2.2f);
            CreateEnemy(worldRoot.transform, "SnailEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/snail.png", new Vector3(-11.7f, -1.98f, 0f), 1.8f, 2.0f);
            CreateEnemy(worldRoot.transform, "MouseEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/mouse.png", new Vector3(-5.8f, -1.98f, 0f), 1.7f, 2.4f);
            CreateEnemy(worldRoot.transform, "SlimeEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/slimeGreen.png", new Vector3(0.6f, -1.98f, 0f), 1.7f, 2.3f);
            CreateEnemy(worldRoot.transform, "BeeEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(7.9f, 2.6f, 0f), 1.9f, 3.0f);
            CreateEnemy(worldRoot.transform, "FlyEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/fly_fly.png", new Vector3(13.2f, 3.65f, 0f), 1.8f, 3.1f);
            CreateEnemy(worldRoot.transform, "BatEnemy_A", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bat_fly.png", new Vector3(18.4f, 2.3f, 0f), 1.8f, 3.2f);
            CreateEnemy(worldRoot.transform, "BeeEnemy_B", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(22.9f, 3.9f, 0f), 1.6f, 3.3f);
            CreateEnemy(worldRoot.transform, "FlyEnemy_B", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/fly_fly.png", new Vector3(26.9f, 5.0f, 0f), 1.5f, 3.35f);
            CreateEnemy(worldRoot.transform, "BatEnemy_B", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bat_fly.png", new Vector3(31.0f, 6.0f, 0f), 1.4f, 3.4f);
            CreateEnemy(worldRoot.transform, "BeeEnemy_C", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(34.2f, 6.9f, 0f), 1.2f, 3.5f);

            CreateSpikes(worldRoot.transform, new Vector3(-16.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(-9.5f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(-3.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(4.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(10.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(15.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(20.0f, -2.44f, 0f));
            CreateSpikes(worldRoot.transform, new Vector3(27.0f, -2.44f, 0f));

            CreateGoal(worldRoot.transform, new Vector3(34.3f, 6.7f, 0f));
            CreateFallDeathZone(worldRoot.transform, -4.2f, 70f);
            CreateBoundaries(worldRoot.transform, new Vector2(-24.5f, 35.1f), new Vector2(-4.55f, 7.4f));
            CreateCameraFollow(cameraTransform, player.transform, new Vector2(-12.0f, 0.3f), new Vector2(26.0f, 3.4f));
        }

        private static MiniBossController CreateLevelOneMiniBossWorld(Transform cameraTransform)
        {
            var worldRoot = new GameObject("World");
            var player = CreatePlayer(worldRoot.transform, new Vector3(-9.4f, -2.2f, 0f));
            AttachPlayerGun(player, "Assets/Art/Kenney/TopDownShooter/PNG/weapon_gun.png");

            var terrainSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/terrain_grass_block_center.png");
            var coinSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/coin_gold.png");
            var gemSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/gem_blue.png");

            CreatePlatformRow(worldRoot.transform, "Ground", terrainSprite, new Vector3(-10.2f, -3f, 0f), 26);
            CreatePlatformRow(worldRoot.transform, "LedgeA", terrainSprite, new Vector3(-6.2f, -1.3f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "LedgeB", terrainSprite, new Vector3(0.4f, 0.15f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "ExitLedge", terrainSprite, new Vector3(7.8f, -1.05f, 0f), 3);

            var boss = CreateMiniBoss(
                worldRoot.transform,
                "Alien Warden",
                "Assets/Art/Kenney/PlatformerExtendedEnemies/Alien sprites/alienGreen_stand.png",
                new Vector3(3.4f, -1.95f, 0f),
                6,
                2.4f,
                8f,
                3,
                true,
                coinSprite,
                gemSprite,
                null,
                0,
                0f,
                0,
                0f);

            CreateBossExitDoor(worldRoot.transform, new Vector3(8.6f, -1.15f, 0f));
            CreateFallDeathZone(worldRoot.transform, -4.15f, 24f);
            CreateBoundaries(worldRoot.transform, new Vector2(-10.8f, 10.5f), new Vector2(-4.55f, 3.8f));
            CreateCameraFollow(cameraTransform, player.transform, new Vector2(-2.5f, -0.15f), new Vector2(2.5f, -0.15f));
            return boss;
        }

        private static MiniBossController CreateLevelThreeMiniBossWorld(Transform cameraTransform)
        {
            var worldRoot = new GameObject("World");
            var player = CreatePlayer(worldRoot.transform, new Vector3(-11.8f, -2.2f, 0f));
            AttachPlayerGun(player, "Assets/Art/Kenney/TopDownShooter/PNG/weapon_silencer.png");

            var terrainSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/terrain_stone_block_center.png");
            var coinSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/coin_gold.png");
            var gemSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/gem_red.png");
            var minionTemplate = CreateEnemyTemplate(worldRoot.transform, "BossMinionTemplate", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/slimeBlue.png", 1.8f, 2.2f);

            CreatePlatformRow(worldRoot.transform, "Ground", terrainSprite, new Vector3(-12.6f, -3f, 0f), 34);
            CreatePlatformRow(worldRoot.transform, "BossBridgeA", terrainSprite, new Vector3(-6.8f, -1.55f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "BossBridgeB", terrainSprite, new Vector3(-1.1f, 0.2f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "BossBridgeC", terrainSprite, new Vector3(4.8f, 1.6f, 0f), 3);
            CreatePlatformRow(worldRoot.transform, "ExitLedge", terrainSprite, new Vector3(10.7f, -0.95f, 0f), 3);

            var boss = CreateMiniBoss(
                worldRoot.transform,
                "Crimson Overseer",
                "Assets/Art/Kenney/PlatformerExtendedEnemies/Alien sprites/alienYellow_stand.png",
                new Vector3(5.4f, -1.8f, 0f),
                10,
                1.8f,
                9.5f,
                5,
                true,
                coinSprite,
                gemSprite,
                minionTemplate,
                2,
                10f,
                3,
                22f);

            CreateBossExitDoor(worldRoot.transform, new Vector3(11.6f, -1.05f, 0f));
            CreateFallDeathZone(worldRoot.transform, -4.15f, 30f);
            CreateBoundaries(worldRoot.transform, new Vector2(-13.2f, 13.0f), new Vector2(-4.55f, 4.6f));
            CreateCameraFollow(cameraTransform, player.transform, new Vector2(-3.5f, 0.1f), new Vector2(5.4f, 0.75f));
            return boss;
        }

        private static void CreateLevelTwoBackground()
        {
            var cloudsSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/clouds2.png");
            var hillsSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/hills2.png");
            var castleSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/castle_grey.png");
            var towerSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tower_grey.png");

            CreateDecorSprite("CloudBandLeft", cloudsSprite, new Vector3(-10.8f, 3.35f, 0f), new Vector3(1.15f, 1.08f, 1f), new Color(0.98f, 0.98f, 1f, 0.62f), -6);
            CreateDecorSprite("CloudBandCenter", cloudsSprite, new Vector3(1.5f, 3.05f, 0f), new Vector3(1.3f, 1.08f, 1f), new Color(0.98f, 0.98f, 1f, 0.54f), -6);
            CreateDecorSprite("CloudBandRight", cloudsSprite, new Vector3(12.0f, 3.3f, 0f), new Vector3(1.12f, 1.06f, 1f), new Color(0.98f, 0.98f, 1f, 0.58f), -6);
            CreateDecorSprite("Hills", hillsSprite, new Vector3(1.8f, -1.55f, 0f), new Vector3(3.15f, 2.0f, 1f), new Color(0.63f, 0.67f, 0.83f, 0.52f), -5);
            CreateDecorSprite("TowerLeft", towerSprite, new Vector3(-11.5f, -0.45f, 0f), new Vector3(1.2f, 1.2f, 1f), new Color(0.94f, 0.95f, 1f, 0.44f), -4);
            CreateDecorSprite("CastleRight", castleSprite, new Vector3(12.8f, -0.35f, 0f), new Vector3(1.6f, 1.6f, 1f), new Color(0.94f, 0.95f, 1f, 0.42f), -4);
        }

        private static void CreateLevelThreeBackground()
        {
            var cloudSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/clouds2.png");
            var hillsSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/hills2.png");
            var towerSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tower_grey.png");
            var castleSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/castle_grey.png");

            CreateDecorSprite("CloudBandLeft", cloudSprite, new Vector3(-10.8f, 3.35f, 0f), new Vector3(1.2f, 1.2f, 1f), new Color(0.95f, 0.97f, 1f, 0.68f), -6);
            CreateDecorSprite("CloudBandRight", cloudSprite, new Vector3(8.5f, 3.15f, 0f), new Vector3(1.35f, 1.2f, 1f), new Color(0.95f, 0.97f, 1f, 0.62f), -6);
            CreateDecorSprite("Hills", hillsSprite, new Vector3(1.8f, -1.35f, 0f), new Vector3(2.6f, 1.8f, 1f), new Color(0.62f, 0.68f, 0.79f, 0.52f), -5);
            CreateDecorSprite("TowerLeft", towerSprite, new Vector3(-11.7f, -0.25f, 0f), new Vector3(1.2f, 1.2f, 1f), new Color(0.92f, 0.95f, 1f, 0.54f), -4);
            CreateDecorSprite("CastleBackdrop", castleSprite, new Vector3(14.9f, -0.35f, 0f), new Vector3(1.8f, 1.8f, 1f), new Color(0.92f, 0.95f, 1f, 0.46f), -4);
        }

        private static void CreateLevelFourBackground()
        {
            var cloudSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/clouds1.png");
            var hillsSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/hills1.png");
            var treeSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tree05.png");
            var towerSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tower_beige.png");

            CreateDecorSprite("CloudStripLeft", cloudSprite, new Vector3(-13.5f, 3.7f, 0f), new Vector3(1.25f, 1.15f, 1f), new Color(1f, 1f, 1f, 0.78f), -6);
            CreateDecorSprite("CloudStripCenter", cloudSprite, new Vector3(0.8f, 3.3f, 0f), new Vector3(1.45f, 1.15f, 1f), new Color(1f, 1f, 1f, 0.72f), -6);
            CreateDecorSprite("CloudStripRight", cloudSprite, new Vector3(16.3f, 3.8f, 0f), new Vector3(1.2f, 1.15f, 1f), new Color(1f, 1f, 1f, 0.8f), -6);
            CreateDecorSprite("Hills", hillsSprite, new Vector3(3.8f, -1.55f, 0f), new Vector3(3.2f, 2.0f, 1f), new Color(0.84f, 0.9f, 0.97f, 0.65f), -5);
            CreateDecorSprite("TreeLeft", treeSprite, new Vector3(-16.8f, -0.8f, 0f), new Vector3(1.25f, 1.25f, 1f), new Color(1f, 1f, 1f, 0.5f), -4);
            CreateDecorSprite("TreeCenter", treeSprite, new Vector3(2.7f, -0.7f, 0f), new Vector3(1.15f, 1.15f, 1f), new Color(1f, 1f, 1f, 0.45f), -4);
            CreateDecorSprite("TowerRight", towerSprite, new Vector3(20.8f, -0.1f, 0f), new Vector3(1.65f, 1.65f, 1f), new Color(1f, 1f, 1f, 0.48f), -4);
        }

        private static void CreateLevelFiveBackground()
        {
            var cloudSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/clouds2.png");
            var hillsSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/hills1.png");
            var castleSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/castle_beige.png");
            var towerSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tower_beige.png");
            var treeSprite = LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tree11.png");

            CreateDecorSprite("CloudLeft", cloudSprite, new Vector3(-17.0f, 3.9f, 0f), new Vector3(1.35f, 1.15f, 1f), new Color(1f, 0.98f, 0.96f, 0.72f), -6);
            CreateDecorSprite("CloudCenter", cloudSprite, new Vector3(4.8f, 3.6f, 0f), new Vector3(1.7f, 1.2f, 1f), new Color(1f, 0.97f, 0.95f, 0.68f), -6);
            CreateDecorSprite("CloudRight", cloudSprite, new Vector3(26.5f, 4.1f, 0f), new Vector3(1.45f, 1.2f, 1f), new Color(1f, 0.98f, 0.95f, 0.74f), -6);
            CreateDecorSprite("Hills", hillsSprite, new Vector3(7.5f, -1.45f, 0f), new Vector3(4.2f, 2.3f, 1f), new Color(0.98f, 0.87f, 0.67f, 0.54f), -5);
            CreateDecorSprite("CastleFar", castleSprite, new Vector3(29.5f, -0.5f, 0f), new Vector3(2.2f, 2.2f, 1f), new Color(1f, 0.95f, 0.88f, 0.4f), -4);
            CreateDecorSprite("TowerMid", towerSprite, new Vector3(13.0f, -0.25f, 0f), new Vector3(1.8f, 1.8f, 1f), new Color(1f, 0.94f, 0.86f, 0.44f), -4);
            CreateDecorSprite("TreeLeft", treeSprite, new Vector3(-20.0f, -0.65f, 0f), new Vector3(1.4f, 1.4f, 1f), new Color(1f, 0.96f, 0.9f, 0.38f), -4);
            CreateDecorSprite("TreeMid", treeSprite, new Vector3(-0.5f, -0.75f, 0f), new Vector3(1.2f, 1.2f, 1f), new Color(1f, 0.96f, 0.9f, 0.34f), -4);
        }

        private static GameObject CreatePlayer(Transform parent, Vector3 position)
        {
            var playerObject = new GameObject("Player");
            playerObject.transform.SetParent(parent);
            playerObject.transform.position = position;
            playerObject.layer = PlayerLayer;

            var spriteRenderer = playerObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_idle.png");
            spriteRenderer.sortingOrder = 5;

            var rigidbody = playerObject.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 3.2f;
            rigidbody.freezeRotation = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = playerObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.62f, 0.92f);
            collider.offset = new Vector2(0f, -0.04f);

            var groundCheck = new GameObject("GroundCheck").transform;
            groundCheck.SetParent(playerObject.transform);
            groundCheck.localPosition = new Vector3(0f, -0.54f, 0f);
            groundCheck.gameObject.layer = PlayerLayer;

            var playerInput = playerObject.AddComponent<PlayerInput>();
            playerInput.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            playerInput.defaultActionMap = "Player";
            playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;

            var controller = playerObject.AddComponent<PlayerController2D>();
            SetObjectReference(controller, "groundCheck", groundCheck);
            SetLayerMask(controller, "groundLayers", 1 << GroundLayer);

            var animationController = playerObject.AddComponent<PlayerAnimationController>();
            SetObjectReference(animationController, "controller", controller);
            SetObjectReference(animationController, "idleSprite", LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_idle.png"));
            SetObjectReference(animationController, "walkSpriteA", LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_walk1.png"));
            SetObjectReference(animationController, "walkSpriteB", LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_walk2.png"));
            SetObjectReference(animationController, "jumpSprite", LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_jump.png"));
            SetObjectReference(animationController, "fallSprite", LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_fall.png"));
            SetObjectReference(animationController, "crouchSprite", LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_duck.png"));
            SetObjectReference(animationController, "skidSprite", LoadSprite("Assets/Art/Sprites/Player/Kenney/Poses/player_skid.png"));
            SetObjectReference(controller, "jumpClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/NewPlatformerPack/sfx_jump.ogg"));
            SetFloat(controller, "jumpVolume", 0.85f);
            return playerObject;
        }

        private static void CreatePlatforms(Transform parent)
        {
            var terrainSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/terrain_grass_block_center.png");
            CreatePlatformRow(parent, "Ground", terrainSprite, new Vector3(-8.95f, -3f, 0f), 24);
            CreatePlatformRow(parent, "StartLedge", terrainSprite, new Vector3(-7.0f, -2.0f, 0f), 3);
            CreatePlatformRow(parent, "StepA", terrainSprite, new Vector3(-3.1f, -1.05f, 0f), 4);
            CreatePlatformRow(parent, "StepB", terrainSprite, new Vector3(1.25f, 0.1f, 0f), 4);
            CreatePlatformRow(parent, "GoalLedge", terrainSprite, new Vector3(6.55f, 1.25f, 0f), 4);
        }

        private static void CreatePlatformRow(Transform parent, string name, Sprite sprite, Vector3 start, int length)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent);
            var spriteSize = sprite != null ? sprite.bounds.size : Vector3.one;
            var tileStride = new Vector3(spriteSize.x, 0f, 0f);

            for (var index = 0; index < length; index++)
            {
                var tile = new GameObject($"Tile_{index}");
                tile.transform.SetParent(root.transform);
                tile.transform.position = start + tileStride * index;
                tile.layer = GroundLayer;

                var renderer = tile.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = 1;

                tile.AddComponent<BoxCollider2D>().size = new Vector2(spriteSize.x, spriteSize.y);
            }
        }

        private static void CreateCollectibles(Transform parent)
        {
            var coinSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/coin_gold.png");
            var gemSprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/gem_blue.png");

            CreateCollectible(parent, "CoinA", coinSprite, new Vector3(-6.15f, -1.0f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(parent, "CoinB", coinSprite, new Vector3(-1.55f, -0.05f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(parent, "CoinC", coinSprite, new Vector3(3.0f, 1.08f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(parent, "CoinD", coinSprite, new Vector3(5.9f, 2.2f, 0f), Collectible.CollectibleKind.Coin);
            CreateCollectible(parent, "GemA", gemSprite, new Vector3(8.15f, 3.0f, 0f), Collectible.CollectibleKind.Gem);
        }

        private static void CreateCollectible(Transform parent, string name, Sprite sprite, Vector3 position, Collectible.CollectibleKind kind)
        {
            var collectibleObject = new GameObject(name);
            collectibleObject.transform.SetParent(parent);
            collectibleObject.transform.position = position;

            var renderer = collectibleObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 3;

            var collider = collectibleObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 0.3f;

            var collectible = collectibleObject.AddComponent<Collectible>();
            SetEnum(collectible, "kind", (int)kind);
            SetObjectReference(
                collectible,
                "pickupClip",
                AssetDatabase.LoadAssetAtPath<AudioClip>(kind == Collectible.CollectibleKind.Coin
                    ? "Assets/Audio/SFX/Kenney/NewPlatformerPack/sfx_coin.ogg"
                    : "Assets/Audio/SFX/Kenney/NewPlatformerPack/sfx_gem.ogg"));
            SetFloat(collectible, "pickupVolume", kind == Collectible.CollectibleKind.Coin ? 0.82f : 0.92f);
        }

        private static void CreateEnemy(Transform parent)
        {
            CreateEnemy(parent, "BeeEnemy", "Assets/Art/Kenney/PlatformerExtendedEnemies/Enemy sprites/bee_fly.png", new Vector3(5.1f, 1.05f, 0f), 1.1f, 2.2f);
        }

        private static void CreateEnemy(Transform parent, string name, string spritePath, Vector3 position, float patrolDistance, float speed)
        {
            var enemyObject = new GameObject(name);
            enemyObject.transform.SetParent(parent);
            enemyObject.transform.position = position;

            var renderer = enemyObject.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(spritePath);
            renderer.sortingOrder = 3;

            var rigidbody = enemyObject.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.freezeRotation = true;

            enemyObject.AddComponent<BoxCollider2D>().size = new Vector2(0.7f, 0.5f);
            var enemy = enemyObject.AddComponent<SimplePatrolEnemy>();
            SetFloat(enemy, "speed", speed);
            SetFloat(enemy, "patrolDistance", patrolDistance);
            SetObjectReference(enemy, "spriteRenderer", renderer);
            SetObjectReference(enemy, "defeatClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactPunch_medium_002.ogg"));
            SetObjectReference(enemy, "playerHitClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactPunch_heavy_001.ogg"));
        }

        private static void CreateHazards(Transform parent)
        {
            CreateSpikes(parent, new Vector3(4.0f, -2.44f, 0f));
        }

        private static void CreateSpikes(Transform parent, Vector3 position)
        {
            var hazard = new GameObject($"Spikes_{position.x:0.0}_{position.y:0.0}");
            hazard.transform.SetParent(parent);
            hazard.transform.position = position;

            var renderer = hazard.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/spikes.png");
            renderer.sortingOrder = 2;

            var collider = hazard.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.55f, 0.24f);
            collider.offset = new Vector2(0f, 0.08f);

            var hazardComponent = hazard.AddComponent<Hazard>();
            SetObjectReference(hazardComponent, "deathClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/error_003.ogg"));
        }

        private static void CreateGoal(Transform parent, Vector3 position)
        {
            var goal = new GameObject("GoalDoor");
            goal.transform.SetParent(parent);
            goal.transform.position = position;

            var doorBottom = new GameObject("DoorBottom");
            doorBottom.transform.SetParent(goal.transform);
            doorBottom.transform.localPosition = new Vector3(0f, -0.42f, 0f);
            var bottomRenderer = doorBottom.AddComponent<SpriteRenderer>();
            bottomRenderer.sprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed.png");
            bottomRenderer.sortingOrder = 3;

            var doorTop = new GameObject("DoorTop");
            doorTop.transform.SetParent(goal.transform);
            doorTop.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            var topRenderer = doorTop.AddComponent<SpriteRenderer>();
            topRenderer.sprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed_top.png");
            topRenderer.sortingOrder = 4;

            var collider = goal.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.82f, 1.2f);
            collider.offset = new Vector2(0f, -0.12f);

            var goalComponent = goal.AddComponent<GoalFlag>();
            SetObjectReference(goalComponent, "doorBottomRenderer", bottomRenderer);
            SetObjectReference(goalComponent, "doorTopRenderer", topRenderer);
            SetObjectReference(goalComponent, "closedDoorBottom", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed.png"));
            SetObjectReference(goalComponent, "openDoorBottom", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_open.png"));
            SetObjectReference(goalComponent, "closedDoorTop", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed_top.png"));
            SetObjectReference(goalComponent, "openDoorTop", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_open_top.png"));
            SetInt(goalComponent, "requiredGems", 1);
            SetObjectReference(goalComponent, "unlockClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/confirmation_002.ogg"));
            SetObjectReference(goalComponent, "completeClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/confirmation_004.ogg"));
        }

        private static void CreateMiniBossBranchDoor(Transform parent, string name, Vector3 position, string miniBossScenePath, string retryLevelPath, string skipLevelPath)
        {
            var door = new GameObject(name);
            door.transform.SetParent(parent);
            door.transform.position = position;

            var doorBottom = new GameObject("DoorBottom");
            doorBottom.transform.SetParent(door.transform);
            doorBottom.transform.localPosition = new Vector3(0f, -0.42f, 0f);
            var bottomRenderer = doorBottom.AddComponent<SpriteRenderer>();
            bottomRenderer.sprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed.png");
            bottomRenderer.sortingOrder = 3;

            var doorTop = new GameObject("DoorTop");
            doorTop.transform.SetParent(door.transform);
            doorTop.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            var topRenderer = doorTop.AddComponent<SpriteRenderer>();
            topRenderer.sprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed_top.png");
            topRenderer.sortingOrder = 4;

            var collider = door.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.82f, 1.2f);
            collider.offset = new Vector2(0f, -0.12f);

            var doorComponent = door.AddComponent<MiniBossDoor>();
            SetObjectReference(doorComponent, "doorBottomRenderer", bottomRenderer);
            SetObjectReference(doorComponent, "doorTopRenderer", topRenderer);
            SetObjectReference(doorComponent, "closedDoorBottom", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed.png"));
            SetObjectReference(doorComponent, "openDoorBottom", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_open.png"));
            SetObjectReference(doorComponent, "closedDoorTop", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed_top.png"));
            SetObjectReference(doorComponent, "openDoorTop", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_open_top.png"));
            SetInt(doorComponent, "requiredGems", 1);
            SetString(doorComponent, "miniBossScenePath", miniBossScenePath);
            SetString(doorComponent, "retryLevelPath", retryLevelPath);
            SetString(doorComponent, "skipLevelPath", skipLevelPath);
            SetObjectReference(doorComponent, "unlockClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/confirmation_002.ogg"));
            SetObjectReference(doorComponent, "enterClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/confirmation_004.ogg"));
        }

        private static void CreateBossExitDoor(Transform parent, Vector3 position)
        {
            var door = new GameObject("BossExitDoor");
            door.transform.SetParent(parent);
            door.transform.position = position;

            var doorBottom = new GameObject("DoorBottom");
            doorBottom.transform.SetParent(door.transform);
            doorBottom.transform.localPosition = new Vector3(0f, -0.42f, 0f);
            var bottomRenderer = doorBottom.AddComponent<SpriteRenderer>();
            bottomRenderer.sprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed.png");
            bottomRenderer.sortingOrder = 3;

            var doorTop = new GameObject("DoorTop");
            doorTop.transform.SetParent(door.transform);
            doorTop.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            var topRenderer = doorTop.AddComponent<SpriteRenderer>();
            topRenderer.sprite = LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed_top.png");
            topRenderer.sortingOrder = 4;

            var collider = door.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.82f, 1.2f);
            collider.offset = new Vector2(0f, -0.12f);

            var doorComponent = door.AddComponent<BossExitDoor>();
            SetObjectReference(doorComponent, "doorBottomRenderer", bottomRenderer);
            SetObjectReference(doorComponent, "doorTopRenderer", topRenderer);
            SetObjectReference(doorComponent, "closedDoorBottom", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed.png"));
            SetObjectReference(doorComponent, "openDoorBottom", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_open.png"));
            SetObjectReference(doorComponent, "closedDoorTop", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_closed_top.png"));
            SetObjectReference(doorComponent, "openDoorTop", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/door_open_top.png"));
            SetInt(doorComponent, "requiredGemDelta", 1);
            SetObjectReference(doorComponent, "unlockClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/confirmation_002.ogg"));
            SetObjectReference(doorComponent, "enterClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/confirmation_004.ogg"));
        }

        private static MiniBossController CreateMiniBoss(
            Transform parent,
            string bossName,
            string spritePath,
            Vector3 position,
            int maxHealth,
            float shootCooldown,
            float projectileSpeed,
            int rewardCoins,
            bool rewardGem,
            Sprite coinSprite,
            Sprite gemSprite,
            GameObject minionTemplate,
            int smallWaveCount,
            float smallWaveInterval,
            int largeWaveCount,
            float largeWaveInterval)
        {
            var bossObject = new GameObject(bossName);
            bossObject.transform.SetParent(parent);
            bossObject.transform.position = position;

            var renderer = bossObject.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(spritePath);
            renderer.sortingOrder = 6;

            var rigidbody = bossObject.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.freezeRotation = true;

            bossObject.AddComponent<BoxCollider2D>().size = new Vector2(0.9f, 1.1f);

            var boss = bossObject.AddComponent<MiniBossController>();
            SetString(boss, "bossName", bossName);
            SetInt(boss, "maxHealth", maxHealth);
            SetFloat(boss, "shootCooldown", shootCooldown);
            SetFloat(boss, "projectileSpeed", projectileSpeed);
            SetInt(boss, "rewardCoins", rewardCoins);
            SetObjectReference(boss, "spriteRenderer", renderer);
            SetObjectReference(boss, "projectileSprite", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/fireball.png"));
            SetObjectReference(boss, "shootClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactMetal_light_002.ogg"));
            SetObjectReference(boss, "hitClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactGeneric_light_002.ogg"));
            SetObjectReference(boss, "deathClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactPunch_heavy_001.ogg"));
            SetObjectReference(boss, "projectileHitClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactGlass_light_001.ogg"));
            SetLayerMask(boss, "projectileGroundLayers", 1 << GroundLayer);
            SetObjectReference(boss, "coinSprite", coinSprite);
            SetObjectReference(boss, "gemSprite", gemSprite);
            SetObjectReference(boss, "coinClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/NewPlatformerPack/sfx_coin.ogg"));
            SetObjectReference(boss, "gemClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/NewPlatformerPack/sfx_gem.ogg"));
            SetObjectReference(boss, "minionTemplate", minionTemplate);
            SetInt(boss, "smallWaveCount", smallWaveCount);
            SetFloat(boss, "smallWaveInterval", smallWaveInterval);
            SetInt(boss, "largeWaveCount", largeWaveCount);
            SetFloat(boss, "largeWaveInterval", largeWaveInterval);
            return boss;
        }

        private static GameObject CreateEnemyTemplate(Transform parent, string name, string spritePath, float speed, float patrolDistance)
        {
            var template = new GameObject(name);
            template.transform.SetParent(parent);
            template.transform.position = new Vector3(0f, -50f, 0f);

            var renderer = template.AddComponent<SpriteRenderer>();
            renderer.sprite = LoadSprite(spritePath);
            renderer.sortingOrder = 5;

            var rigidbody = template.AddComponent<Rigidbody2D>();
            rigidbody.gravityScale = 0f;
            rigidbody.freezeRotation = true;

            template.AddComponent<BoxCollider2D>().size = new Vector2(0.7f, 0.5f);
            var enemy = template.AddComponent<SimplePatrolEnemy>();
            SetFloat(enemy, "speed", speed);
            SetFloat(enemy, "patrolDistance", patrolDistance);
            SetObjectReference(enemy, "spriteRenderer", renderer);
            SetObjectReference(enemy, "defeatClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactPunch_medium_002.ogg"));
            SetObjectReference(enemy, "playerHitClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactPunch_heavy_001.ogg"));
            template.SetActive(false);
            return template;
        }

        private static void AttachPlayerGun(GameObject playerObject, string gunSpritePath)
        {
            var gunPivot = new GameObject("GunPivot");
            gunPivot.transform.SetParent(playerObject.transform);
            gunPivot.transform.localPosition = new Vector3(0.58f, 0.04f, 0f);

            var gunRenderer = gunPivot.AddComponent<SpriteRenderer>();
            gunRenderer.sprite = LoadSprite(gunSpritePath);
            gunRenderer.sortingOrder = 8;

            var gunController = playerObject.AddComponent<PlayerGunController>();
            SetObjectReference(gunController, "gunRenderer", gunRenderer);
            SetObjectReference(gunController, "gunSprite", LoadSprite(gunSpritePath));
            SetObjectReference(gunController, "projectileSprite", LoadSprite("Assets/Art/Kenney/NewPlatformerPack/GeneratedTextures/Tiles/fireball.png"));
            SetObjectReference(gunController, "shootClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactMetal_light_000.ogg"));
            SetObjectReference(gunController, "projectileHitClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/ImpactSounds/impactGlass_light_002.ogg"));
            SetFloat(gunController, "fireCooldown", 2f);
            SetFloat(gunController, "projectileSpeed", 10f);
            SetLayerMask(gunController, "groundLayers", 1 << GroundLayer);
        }

        private static void CreateMiniBossHud(MiniBossController boss)
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                return;
            }

            var root = new GameObject("MiniBossHud");
            root.transform.SetParent(canvas.transform, false);
            var controller = root.AddComponent<MiniBossHudController>();

            var panel = CreatePanel("BossPanel", root.transform, new Vector2(210f, -70f), new Vector2(440f, 44f), new Color(0.08f, 0.1f, 0.14f, 0.86f));
            AnchorTo(panel.GetComponent<RectTransform>(), new Vector2(0f, 1f), new Vector2(0f, 1f));
            var nameText = CreateText("BossNameText", panel.transform, boss != null ? boss.BossName : "Mini Boss", 17, new Vector2(-130f, 0f), new Vector2(150f, 24f));
            nameText.alignment = TextAnchor.MiddleLeft;
            nameText.color = Color.white;
            var hpText = CreateText("BossHpText", panel.transform, "", 17, new Vector2(175f, 0f), new Vector2(56f, 24f));
            hpText.color = Color.white;

            var fillBackground = CreatePanel("BossHealthBackground", panel.transform, new Vector2(20f, 0f), new Vector2(190f, 14f), new Color(0.22f, 0.24f, 0.28f, 1f));
            var fillObject = new GameObject("BossHealthFill");
            fillObject.transform.SetParent(fillBackground.transform, false);
            var fillRect = fillObject.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = new Vector2(3f, 3f);
            fillRect.offsetMax = new Vector2(-3f, -3f);
            var fillImage = fillObject.AddComponent<Image>();
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.color = new Color(0.89f, 0.21f, 0.21f, 1f);

            SetObjectReference(controller, "panel", panel);
            SetObjectReference(controller, "bossNameText", nameText);
            SetObjectReference(controller, "healthText", hpText);
            SetObjectReference(controller, "healthFill", fillImage);
            SetObjectReference(controller, "boss", boss);
        }

        private static void CreateFallDeathZone(Transform parent, float yPosition, float width)
        {
            var deathZone = new GameObject("FallDeathZone");
            deathZone.transform.SetParent(parent);
            deathZone.transform.position = new Vector3(0f, yPosition, 0f);

            var collider = deathZone.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(width, 1.2f);

            var hazard = deathZone.AddComponent<Hazard>();
            SetObjectReference(hazard, "deathClip", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/Kenney/InterfaceSounds/error_003.ogg"));
        }

        private static void CreateLevelOneMiniBossBackground()
        {
            CreateDecorSprite("BossClouds", LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/clouds2.png"), new Vector3(0f, 3.6f, 0f), new Vector3(1.8f, 1.15f, 1f), new Color(1f, 1f, 1f, 0.36f), -6);
            CreateDecorSprite("BossTower", LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/tower_grey.png"), new Vector3(7.8f, -0.8f, 0f), new Vector3(1.6f, 1.6f, 1f), new Color(1f, 1f, 1f, 0.34f), -5);
        }

        private static void CreateLevelThreeMiniBossBackground()
        {
            CreateDecorSprite("BossCastle", LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/castle_grey.png"), new Vector3(8.4f, -0.9f, 0f), new Vector3(1.75f, 1.75f, 1f), new Color(1f, 1f, 1f, 0.28f), -5);
            CreateDecorSprite("BossCloudBand", LoadSprite("Assets/Art/Kenney/BackgroundElements/PNG/Flat/clouds2.png"), new Vector3(-1.4f, 3.7f, 0f), new Vector3(1.55f, 1.1f, 1f), new Color(0.95f, 0.92f, 1f, 0.28f), -6);
        }

        private static void CreateBoundaries(Transform parent)
        {
            CreateBoundaries(parent, new Vector2(-9.55f, 9.55f), new Vector2(-4.55f, 4.55f));
        }

        private static void CreateBoundaries(Transform parent, Vector2 horizontalBounds, Vector2 verticalBounds)
        {
            CreateBoundary(parent, "LeftBoundary", new Vector2(horizontalBounds.x, 0f), new Vector2(0.5f, 10f));
            CreateBoundary(parent, "RightBoundary", new Vector2(horizontalBounds.y, 0f), new Vector2(0.5f, 10f));
            CreateBoundary(parent, "TopBoundary", new Vector2(0f, verticalBounds.y), new Vector2(40f, 0.5f));
            CreateBoundary(parent, "BottomBoundary", new Vector2(0f, verticalBounds.x), new Vector2(40f, 0.5f));
        }

        private static void CreateBoundary(Transform parent, string name, Vector2 position, Vector2 size)
        {
            var boundary = new GameObject(name);
            boundary.transform.SetParent(parent);
            boundary.transform.position = position;
            boundary.layer = GroundLayer;
            boundary.AddComponent<BoxCollider2D>().size = size;
        }

        private static void CreateGameplayHud()
        {
            var canvas = CreateCanvas("Canvas");

            var hudObject = new GameObject("GameplayHud");
            hudObject.transform.SetParent(canvas.transform, false);
            var hud = hudObject.AddComponent<GameplayHudController>();

            var topBar = CreatePanel("TopBar", canvas.transform, new Vector2(0f, -44f), new Vector2(900f, 64f), new Color(1f, 1f, 1f, 0.82f));
            AnchorTo(topBar.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));

            var coinsText = CreateText("CoinsText", topBar.transform, "Coins 00", 20, new Vector2(-310f, 0f), new Vector2(120f, 30f));
            var gemsText = CreateText("GemsText", topBar.transform, "Gems 00", 20, new Vector2(-190f, 0f), new Vector2(120f, 30f));
            var timerText = CreateText("TimerText", topBar.transform, "Time 00:00", 20, new Vector2(-45f, 0f), new Vector2(160f, 30f));
            var livesText = CreateText("LivesText", topBar.transform, "Lives 3/3", 20, new Vector2(110f, 0f), new Vector2(130f, 30f));
            var deathsText = CreateText("DeathsText", topBar.transform, "Deaths 0", 20, new Vector2(250f, 0f), new Vector2(120f, 30f));
            var restartButton = CreateButton("RestartHudButton", topBar.transform, "Restart", new Vector2(370f, 0f), new Vector2(130f, 42f), 18);
            UnityEventTools.AddPersistentListener(restartButton.onClick, hud.RestartLevel);

            SetObjectReference(hud, "coinsText", coinsText);
            SetObjectReference(hud, "gemsText", gemsText);
            SetObjectReference(hud, "timerText", timerText);
            SetObjectReference(hud, "livesText", livesText);
            SetObjectReference(hud, "deathsText", deathsText);

            var transitionPanel = CreateOverlay(canvas.transform, "TransitionPanel", "Level 2", new Color(0.06f, 0.1f, 0.16f, 0.86f));
            var transitionCard = transitionPanel.transform.Find("Card");
            var transitionText = CreateText("TransitionText", transitionCard, "Level 2", 40, new Vector2(0f, 24f), new Vector2(320f, 70f));
            transitionText.alignment = TextAnchor.MiddleCenter;
            transitionText.color = new Color(0.18f, 0.23f, 0.29f);
            var transitionHint = CreateText("TransitionHint", transitionCard, "Climb higher. Survive longer.", 20, new Vector2(0f, -32f), new Vector2(320f, 44f));
            transitionHint.alignment = TextAnchor.MiddleCenter;
            transitionHint.color = new Color(0.28f, 0.34f, 0.41f);

            var pausePanel = CreateOverlay(canvas.transform, "PausePanel", "Paused", new Color(0.08f, 0.12f, 0.18f, 0.78f));
            var pauseCard = pausePanel.transform.Find("Card");
            pauseCard.GetComponent<RectTransform>().sizeDelta = new Vector2(560f, 470f);
            pauseCard.Find("Title").GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 150f);
            var pauseResumeButton = CreateButton("ResumeButton", pauseCard, "Resume", new Vector2(0f, 70f));
            UnityEventTools.AddPersistentListener(pauseResumeButton.onClick, hud.ResumeGame);
            var pauseMusicLabel = CreateText("PauseMusicLabel", pauseCard, "Music", 22, new Vector2(-180f, -6f), new Vector2(120f, 34f));
            pauseMusicLabel.alignment = TextAnchor.MiddleLeft;
            var pauseMusicSlider = CreateSlider("PauseMusicSlider", pauseCard, new Vector2(45f, -6f), new Vector2(280f, 30f), 0.75f);
            UnityEventTools.AddPersistentListener(pauseMusicSlider.onValueChanged, hud.SetMusicVolume);
            var pauseSfxLabel = CreateText("PauseSfxLabel", pauseCard, "Sound FX", 22, new Vector2(-180f, -70f), new Vector2(120f, 34f));
            pauseSfxLabel.alignment = TextAnchor.MiddleLeft;
            var pauseSfxSlider = CreateSlider("PauseSfxSlider", pauseCard, new Vector2(45f, -70f), new Vector2(280f, 30f), 0.85f);
            UnityEventTools.AddPersistentListener(pauseSfxSlider.onValueChanged, hud.SetSfxVolume);
            var pauseMenuButton = CreateButton("PauseMenuButton", pauseCard, "Main Menu", new Vector2(0f, -150f));
            UnityEventTools.AddPersistentListener(pauseMenuButton.onClick, hud.ReturnToMenu);

            var gameOverPanel = CreateOverlay(canvas.transform, "GameOverPanel", "Game Over", new Color(0.18f, 0.08f, 0.08f, 0.82f));
            var gameOverCard = gameOverPanel.transform.Find("Card");
            gameOverCard.GetComponent<RectTransform>().sizeDelta = new Vector2(520f, 380f);
            gameOverCard.Find("Title").GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 128f);
            var gameOverText = CreateText("GameOverText", gameOverCard, "You lost a life.", 22, new Vector2(0f, 48f), new Vector2(380f, 92f));
            gameOverText.alignment = TextAnchor.MiddleCenter;
            var retryButton = CreateButton("RetryButton", gameOverCard, "Retry Level", new Vector2(0f, -46f));
            UnityEventTools.AddPersistentListener(retryButton.onClick, hud.RetryAfterGameOver);
            var gameOverMenuButton = CreateButton("GameOverMenuButton", gameOverCard, "Main Menu", new Vector2(0f, -118f));
            UnityEventTools.AddPersistentListener(gameOverMenuButton.onClick, hud.ReturnToMenu);
            var retryButtonLabel = retryButton.transform.Find("Label").GetComponent<Text>();

            var victoryPanel = CreateOverlay(canvas.transform, "VictoryPanel", "Congratulations", new Color(0.1f, 0.18f, 0.08f, 0.82f));
            var victoryCard = victoryPanel.transform.Find("Card");
            victoryCard.GetComponent<RectTransform>().sizeDelta = new Vector2(620f, 420f);
            var victoryTitleText = victoryCard.Find("Title").GetComponent<Text>();
            var summary = CreateText("Summary", victoryCard, "You finished the game.", 22, new Vector2(0f, 82f), new Vector2(520f, 150f));
            summary.alignment = TextAnchor.MiddleCenter;
            var leaderboardSummary = CreateText("LeaderboardSummary", victoryCard, "Leaderboard will appear here.", 20, new Vector2(0f, -12f), new Vector2(520f, 120f));
            leaderboardSummary.alignment = TextAnchor.UpperLeft;
            var playAgainButton = CreateButton("VictoryRetryButton", victoryCard, "Play Again", new Vector2(-145f, -150f));
            UnityEventTools.AddPersistentListener(playAgainButton.onClick, hud.RestartLevel);
            var victoryMenuButton = CreateButton("VictoryMenuButton", victoryCard, "Main Menu", new Vector2(145f, -150f));
            UnityEventTools.AddPersistentListener(victoryMenuButton.onClick, hud.ReturnToMenu);

            transitionPanel.SetActive(false);
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            victoryPanel.SetActive(false);

            SetObjectReference(hud, "transitionPanel", transitionPanel);
            SetObjectReference(hud, "transitionText", transitionText);
            SetObjectReference(hud, "pausePanel", pausePanel);
            SetObjectReference(hud, "pauseMusicSlider", pauseMusicSlider);
            SetObjectReference(hud, "pauseSfxSlider", pauseSfxSlider);
            SetObjectReference(hud, "gameOverPanel", gameOverPanel);
            SetObjectReference(hud, "gameOverText", gameOverText);
            SetObjectReference(hud, "retryButton", retryButton);
            SetObjectReference(hud, "retryButtonText", retryButtonLabel);
            SetObjectReference(hud, "victoryPanel", victoryPanel);
            SetObjectReference(hud, "victoryTitleText", victoryTitleText);
            SetObjectReference(hud, "victorySummaryText", summary);
            SetObjectReference(hud, "victoryLeaderboardText", leaderboardSummary);
        }

        private static GameObject CreateOverlay(Transform parent, string name, string title, Color color)
        {
            var overlay = CreatePanel(name, parent, Vector2.zero, new Vector2(1920f, 1080f), color);
            var rect = overlay.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var card = CreatePanel("Card", overlay.transform, new Vector2(0f, 10f), new Vector2(420f, 300f), new Color(1f, 1f, 1f, 0.92f));
            var titleText = CreateText("Title", card.transform, title, 34, new Vector2(0f, 92f), new Vector2(320f, 50f));
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = new Color(0.18f, 0.23f, 0.29f);
            return overlay;
        }

        private static Canvas CreateCanvas(string name)
        {
            var canvasObject = new GameObject(name);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static Camera CreateMainCamera(Color backgroundColor)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5.2f;
            camera.backgroundColor = backgroundColor;
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            cameraObject.AddComponent<AudioListener>();
            return camera;
        }

        private static void CreateCameraFollow(Transform cameraTransform, Transform target, Vector2 minBounds, Vector2 maxBounds)
        {
            var cameraFollow = cameraTransform.gameObject.AddComponent<Systems.CameraFollow2D>();
            SetObjectReference(cameraFollow, "target", target);
            SetVector2(cameraFollow, "minBounds", minBounds);
            SetVector2(cameraFollow, "maxBounds", maxBounds);
        }

        private static GameObject CreatePanel(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);

            var rect = panel.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var image = panel.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return panel;
        }

        private static Slider CreateSlider(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, float defaultValue)
        {
            var sliderObject = new GameObject(name);
            sliderObject.transform.SetParent(parent, false);

            var rect = sliderObject.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var background = new GameObject("Background");
            background.transform.SetParent(sliderObject.transform, false);
            var backgroundRect = background.AddComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0f, 0.25f);
            backgroundRect.anchorMax = new Vector2(1f, 0.75f);
            backgroundRect.offsetMin = Vector2.zero;
            backgroundRect.offsetMax = Vector2.zero;
            var backgroundImage = background.AddComponent<Image>();
            backgroundImage.color = new Color(0.84f, 0.88f, 0.92f, 1f);

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObject.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
            fillAreaRect.offsetMin = new Vector2(8f, 0f);
            fillAreaRect.offsetMax = new Vector2(-8f, 0f);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.98f, 0.76f, 0.12f, 1f);

            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObject.transform, false);
            var handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(8f, 0f);
            handleAreaRect.offsetMax = new Vector2(-8f, 0f);

            var handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            var handleRect = handle.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(22f, 34f);
            var handleImage = handle.AddComponent<Image>();
            handleImage.color = new Color(0.2f, 0.25f, 0.32f, 1f);

            var slider = sliderObject.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = defaultValue;
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            return slider;
        }

        private static void CreateControlTextRow(Transform parent, string name, string label, string value, Vector2 anchoredPosition)
        {
            var row = new GameObject(name);
            row.transform.SetParent(parent, false);
            var rect = row.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(640f, 46f);
            rect.anchoredPosition = anchoredPosition;

            var labelText = CreateText("Label", row.transform, label, 20, new Vector2(-250f, 0f), new Vector2(120f, 34f));
            labelText.alignment = TextAnchor.MiddleLeft;
            var valueText = CreateText("Value", row.transform, value, 20, new Vector2(70f, 0f), new Vector2(360f, 34f));
            valueText.alignment = TextAnchor.MiddleLeft;
        }

        private static Text CreateText(string name, Transform parent, string content, int fontSize, Vector2 anchoredPosition, Vector2 size)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            var rect = textObject.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var text = textObject.AddComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = new Color(0.19f, 0.24f, 0.3f);
            text.alignment = TextAnchor.MiddleCenter;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Vector2 anchoredPosition)
        {
            return CreateButton(name, parent, label, anchoredPosition, new Vector2(260f, 60f), 24);
        }

        private static Button CreateButton(string name, Transform parent, string label, Vector2 anchoredPosition, Vector2 size, int fontSize)
        {
            var buttonObject = new GameObject(name);
            buttonObject.transform.SetParent(parent, false);

            var rect = buttonObject.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var image = buttonObject.AddComponent<Image>();
            image.sprite = LoadSprite("Assets/Art/UI/Kenney/GeneratedTextures/Yellow/button_rectangle_flat.png");
            image.type = Image.Type.Sliced;
            image.color = Color.white;

            var button = buttonObject.AddComponent<Button>();

            var text = CreateText("Label", buttonObject.transform, label, fontSize, Vector2.zero, size - new Vector2(24f, 16f));
            text.raycastTarget = false;

            return button;
        }

        private static void CreateDecorSprite(string name, Sprite sprite, Vector3 position, Vector3 scale, Color color, int sortingOrder)
        {
            if (sprite == null)
            {
                return;
            }

            var decor = new GameObject(name);
            decor.transform.position = position;
            decor.transform.localScale = scale;

            var renderer = decor.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(SceneIds.MainMenu, true),
                new EditorBuildSettingsScene(SceneIds.Level01, true),
                new EditorBuildSettingsScene(SceneIds.Level01MiniBoss, true),
                new EditorBuildSettingsScene(SceneIds.Level02, true),
                new EditorBuildSettingsScene(SceneIds.Level03, true),
                new EditorBuildSettingsScene(SceneIds.Level03MiniBoss, true),
                new EditorBuildSettingsScene(SceneIds.Level04, true),
                new EditorBuildSettingsScene(SceneIds.Level05, true)
            };
        }

        private static Sprite LoadSprite(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static void SetObjectReference(Object target, string fieldName, Object value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetLayerMask(Object target, string fieldName, LayerMask value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                return;
            }

            property.intValue = value.value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetEnum(Object target, string fieldName, int value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                return;
            }

            property.enumValueIndex = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetFloat(Object target, string fieldName, float value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                return;
            }

            property.floatValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetInt(Object target, string fieldName, int value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                return;
            }

            property.intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetString(Object target, string fieldName, string value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                return;
            }

            property.stringValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetVector2(Object target, string fieldName, Vector2 value)
        {
            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty(fieldName);
            if (property == null)
            {
                return;
            }

            property.vector2Value = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AnchorTo(RectTransform rectTransform, Vector2 min, Vector2 max)
        {
            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private static void StretchFullScreen(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private static void CreateSceneMusic(string clipPath, float volume)
        {
            var musicObject = new GameObject("SceneMusic");
            var musicTrack = musicObject.AddComponent<Systems.MusicSceneTrack>();
            SetObjectReference(musicTrack, "musicClip", AssetDatabase.LoadAssetAtPath<AudioClip>(clipPath));
            SetFloat(musicTrack, "volume", volume);
        }
    }
}
