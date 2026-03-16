using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TreasureTower.Player;
using TreasureTower.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TreasureTower.Core
{
    public enum GameFlowState
    {
        MainMenu,
        Playing,
        Transition,
        Paused,
        Victory,
        GameOver
    }

    [Serializable]
    public sealed class AttemptRecord
    {
        public int attemptNumber;
        public int coins;
        public int deaths;
        public float timeSeconds;
    }

    [Serializable]
    public sealed class AttemptRecordCollection
    {
        public List<AttemptRecord> records = new();
    }

    public sealed class GameManager : MonoBehaviour
    {
        private const string LeaderboardKey = "TreasureTower.Leaderboard";
        private const int MaxLeaderboardEntries = 8;

        public static GameManager Instance { get; private set; }

        public event Action<GameFlowState> StateChanged;
        public event Action<int, int> ScoreChanged;
        public event Action<string> TransitionMessageChanged;
        public event Action RunStatsChanged;

        public GameFlowState State { get; private set; } = GameFlowState.MainMenu;
        public int Coins { get; private set; }
        public int Gems { get; private set; }
        public int LivesRemaining { get; private set; } = DifficultySettings.LivesPerStage;
        public int Deaths { get; private set; }
        public int AttemptNumber { get; private set; }
        public float ElapsedTime { get; private set; }
        public string TransitionMessage { get; private set; } = string.Empty;
        public string GameOverMessage { get; private set; } = "You fell.";
        public string VictoryTitle { get; private set; } = "Tower Cleared";
        public string VictoryMessage { get; private set; } = "You found the exit door.";
        public string VictoryStats { get; private set; } = string.Empty;
        public bool HasLivesRemaining => LivesRemaining > 0;
        public int MaxLivesPerStage => DifficultySettings.LivesPerStage;

        private readonly List<AttemptRecord> leaderboard = new();

        private string currentLevelPath = SceneIds.Level01;
        private string retryScenePath = SceneIds.Level01;
        private string miniBossSkipTargetPath = string.Empty;
        private string miniBossReturnLevelPath = string.Empty;
        private bool resetLivesOnSceneLoad = true;
        private bool miniBossRunActive;
        private int storedLevelLivesBeforeMiniBoss = DifficultySettings.LivesPerStage;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureInstance()
        {
            if (Instance != null)
            {
                return;
            }

            var gameManagerObject = new GameObject(nameof(GameManager));
            gameManagerObject.AddComponent<GameManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboard();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            if (State != GameFlowState.Playing)
            {
                return;
            }

            ElapsedTime += Time.unscaledDeltaTime;
            RunStatsChanged?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance != this)
            {
                return;
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            Time.timeScale = 1f;
            Instance = null;
        }

        public void StartNewGame()
        {
            AttemptNumber++;
            ResetRunProgress();
            resetLivesOnSceneLoad = true;
            Time.timeScale = 1f;
            LoadScene(SceneIds.Level01);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            LoadScene(currentLevelPath);
        }

        public void RetryAfterGameOver()
        {
            Time.timeScale = 1f;

            if (miniBossRunActive && IsMiniBossScene(currentLevelPath))
            {
                if (HasLivesRemaining)
                {
                    resetLivesOnSceneLoad = false;
                    retryScenePath = currentLevelPath;
                    LoadScene(currentLevelPath);
                    return;
                }

                var remainingLevelLives = Mathf.Max(0, storedLevelLivesBeforeMiniBoss - 1);
                miniBossRunActive = false;
                miniBossSkipTargetPath = string.Empty;

                if (remainingLevelLives > 0 && !string.IsNullOrWhiteSpace(miniBossReturnLevelPath))
                {
                    storedLevelLivesBeforeMiniBoss = remainingLevelLives;
                    LivesRemaining = remainingLevelLives;
                    resetLivesOnSceneLoad = false;
                    retryScenePath = miniBossReturnLevelPath;
                    var returnLevelPath = miniBossReturnLevelPath;
                    miniBossReturnLevelPath = string.Empty;
                    LoadScene(returnLevelPath);
                    return;
                }

                StartNewGame();
                return;
            }

            if (HasLivesRemaining)
            {
                LoadScene(retryScenePath);
                return;
            }

            StartNewGame();
        }

        public void ReturnToMainMenu()
        {
            miniBossRunActive = false;
            miniBossReturnLevelPath = string.Empty;
            miniBossSkipTargetPath = string.Empty;
            Time.timeScale = 1f;
            LoadScene(SceneIds.MainMenu);
        }

        public void TogglePause()
        {
            if (State == GameFlowState.Victory || State == GameFlowState.GameOver)
            {
                return;
            }

            if (State == GameFlowState.Paused)
            {
                ResumeGame();
            }
            else if (State == GameFlowState.Playing)
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            if (State != GameFlowState.Playing)
            {
                return;
            }

            Time.timeScale = 0f;
            SetState(GameFlowState.Paused);
        }

        public void ResumeGame()
        {
            if (State != GameFlowState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            SetState(GameFlowState.Playing);
        }

        public void RegisterCoin(int amount)
        {
            Coins += Mathf.Max(0, amount);
            ScoreChanged?.Invoke(Coins, Gems);
            RunStatsChanged?.Invoke();
        }

        public void RegisterGem(int amount)
        {
            Gems += Mathf.Max(0, amount);
            ScoreChanged?.Invoke(Coins, Gems);
            RunStatsChanged?.Invoke();
        }

        public void EnterMiniBossWorld(string miniBossScenePath, string retryLevelPath, string skipLevelPath)
        {
            if (string.IsNullOrWhiteSpace(miniBossScenePath))
            {
                return;
            }

            miniBossRunActive = true;
            storedLevelLivesBeforeMiniBoss = Mathf.Max(0, LivesRemaining);
            miniBossReturnLevelPath = retryLevelPath;
            miniBossSkipTargetPath = skipLevelPath;
            retryScenePath = miniBossScenePath;
            LivesRemaining = MaxLivesPerStage;
            resetLivesOnSceneLoad = false;
            Time.timeScale = 1f;
            LoadScene(miniBossScenePath);
        }

        public void CompleteMiniBossSkip()
        {
            if (string.IsNullOrWhiteSpace(miniBossSkipTargetPath))
            {
                return;
            }

            miniBossRunActive = false;
            miniBossReturnLevelPath = string.Empty;
            var targetScenePath = miniBossSkipTargetPath;
            miniBossSkipTargetPath = string.Empty;
            storedLevelLivesBeforeMiniBoss = 3;
            var transitionLabel = targetScenePath == SceneIds.Level03 ? "Level 3" : "Level 5";
            StartCoroutine(TransitionToLevel(targetScenePath, transitionLabel, true));
        }

        public void HandlePlayerDeath(PlayerController2D player)
        {
            if (player == null || State == GameFlowState.GameOver || State == GameFlowState.Victory)
            {
                return;
            }

            player.SetInputEnabled(false);
            Deaths++;
            LivesRemaining = Mathf.Max(0, LivesRemaining - 1);

            if (miniBossRunActive && IsMiniBossScene(currentLevelPath))
            {
                GameOverMessage = HasLivesRemaining
                    ? $"You lost a mini boss life.\nMini boss lives left: {LivesRemaining}/{MaxLivesPerStage}\nRetry the mini boss stage."
                    : storedLevelLivesBeforeMiniBoss > 1
                        ? $"All mini boss lives are gone.\nYou return to the current level.\nLevel lives left: {storedLevelLivesBeforeMiniBoss - 1}/{MaxLivesPerStage}"
                        : $"All mini boss lives are gone.\nNo level lives remain.\nAll {MaxLivesPerStage} lives are gone.\nYour next attempt restarts from Level 1.";
            }
            else
            {
                GameOverMessage = HasLivesRemaining
                    ? $"You lost a life.\nLives left: {LivesRemaining}/{MaxLivesPerStage}\nRetry this level and keep climbing."
                    : $"All {MaxLivesPerStage} lives are gone.\nYour next attempt restarts from Level 1.";
            }

            Time.timeScale = 0f;
            RunStatsChanged?.Invoke();
            SetState(GameFlowState.GameOver);
        }

        public void CompleteLevel()
        {
            if (State == GameFlowState.Victory)
            {
                return;
            }

            if (currentLevelPath == SceneIds.Level01)
            {
                StartCoroutine(TransitionToLevel(SceneIds.Level02, "Level 2", true));
                return;
            }

            if (currentLevelPath == SceneIds.Level02)
            {
                StartCoroutine(TransitionToLevel(SceneIds.Level03, "Level 3", true));
                return;
            }

            if (currentLevelPath == SceneIds.Level03)
            {
                StartCoroutine(TransitionToLevel(SceneIds.Level04, "Level 4", true));
                return;
            }

            if (currentLevelPath == SceneIds.Level04)
            {
                StartCoroutine(TransitionToLevel(SceneIds.Level05, "Level 5", true));
                return;
            }

            RegisterCompletedAttempt();
            VictoryTitle = "Congratulations";
            VictoryMessage = "You finished all 5 levels of Treasure Tower.";
            VictoryStats =
                $"Attempt #{AttemptNumber}\nCoins: {Coins}\nDeaths: {Deaths}\nTime: {FormatTime(ElapsedTime)}\n\nYou conquered the final door.";
            Time.timeScale = 0f;
            SetState(GameFlowState.Victory);
        }

        public string GetLeaderboardText()
        {
            if (leaderboard.Count == 0)
            {
                return "No completed attempts yet.\nFinish the game to create the first record.";
            }

            var builder = new StringBuilder();
            for (var i = 0; i < leaderboard.Count; i++)
            {
                var record = leaderboard[i];
                builder.Append(i + 1)
                    .Append(". Attempt #")
                    .Append(record.attemptNumber)
                    .Append("  Coins ")
                    .Append(record.coins)
                    .Append("  Deaths ")
                    .Append(record.deaths)
                    .Append("  Time ")
                    .Append(FormatTime(record.timeSeconds));

                if (i < leaderboard.Count - 1)
                {
                    builder.Append('\n');
                }
            }

            return builder.ToString();
        }

        public string GetControlSummary()
        {
            return $"Collect coins across all levels, save your {MaxLivesPerStage} lives in each stage, and finish faster with fewer deaths.";
        }

        public static string FormatTime(float timeSeconds)
        {
            var totalSeconds = Mathf.Max(0, Mathf.FloorToInt(timeSeconds));
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }

        private void LoadScene(string scenePath)
        {
            SceneManager.LoadScene(scenePath);
        }

        private IEnumerator TransitionToLevel(string scenePath, string transitionLabel, bool resetLives)
        {
            resetLivesOnSceneLoad = resetLives;
            Time.timeScale = 1f;
            TransitionMessage = transitionLabel;
            TransitionMessageChanged?.Invoke(TransitionMessage);
            SetState(GameFlowState.Transition);
            yield return new WaitForSecondsRealtime(1.35f);
            LoadScene(scenePath);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            currentLevelPath = scene.path;

            if (scene.path == SceneIds.MainMenu)
            {
                retryScenePath = SceneIds.Level01;
                miniBossSkipTargetPath = string.Empty;
                miniBossReturnLevelPath = string.Empty;
                miniBossRunActive = false;
                TransitionMessage = string.Empty;
                SetState(GameFlowState.MainMenu);
                Time.timeScale = 1f;
                return;
            }

            if (!IsGameplayScene(scene.path))
            {
                return;
            }

            if (IsMainTowerLevel(scene.path))
            {
                retryScenePath = scene.path;

                if (!miniBossRunActive)
                {
                    miniBossSkipTargetPath = string.Empty;
                    miniBossReturnLevelPath = string.Empty;
                    storedLevelLivesBeforeMiniBoss = MaxLivesPerStage;
                }
            }

            if (resetLivesOnSceneLoad)
            {
                LivesRemaining = MaxLivesPerStage;
                resetLivesOnSceneLoad = false;
            }

            TransitionMessage = string.Empty;
            TransitionMessageChanged?.Invoke(TransitionMessage);
            Time.timeScale = 1f;
            SetState(GameFlowState.Playing);
            ScoreChanged?.Invoke(Coins, Gems);
            RunStatsChanged?.Invoke();
        }

        private void ResetRunProgress()
        {
            Coins = 0;
            Gems = 0;
            Deaths = 0;
            ElapsedTime = 0f;
            LivesRemaining = MaxLivesPerStage;
            retryScenePath = SceneIds.Level01;
            miniBossSkipTargetPath = string.Empty;
            miniBossReturnLevelPath = string.Empty;
            miniBossRunActive = false;
            storedLevelLivesBeforeMiniBoss = MaxLivesPerStage;
            GameOverMessage = "You lost a life.";
            VictoryTitle = "Congratulations";
            VictoryMessage = "You finished the game.";
            VictoryStats = string.Empty;
            ScoreChanged?.Invoke(Coins, Gems);
            RunStatsChanged?.Invoke();
        }

        private void RegisterCompletedAttempt()
        {
            var record = new AttemptRecord
            {
                attemptNumber = AttemptNumber,
                coins = Coins,
                deaths = Deaths,
                timeSeconds = ElapsedTime
            };

            leaderboard.Insert(0, record);
            if (leaderboard.Count > MaxLeaderboardEntries)
            {
                leaderboard.RemoveRange(MaxLeaderboardEntries, leaderboard.Count - MaxLeaderboardEntries);
            }

            SaveLeaderboard();
        }

        private void LoadLeaderboard()
        {
            leaderboard.Clear();
            var json = PlayerPrefs.GetString(LeaderboardKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            var data = JsonUtility.FromJson<AttemptRecordCollection>(json);
            if (data?.records == null)
            {
                return;
            }

            leaderboard.AddRange(data.records);
        }

        private void SaveLeaderboard()
        {
            var data = new AttemptRecordCollection { records = leaderboard };
            PlayerPrefs.SetString(LeaderboardKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
        }

        private static bool IsGameplayScene(string scenePath)
        {
            return scenePath == SceneIds.Level01 ||
                   scenePath == SceneIds.Level02 ||
                   scenePath == SceneIds.Level03 ||
                   scenePath == SceneIds.Level04 ||
                   scenePath == SceneIds.Level05 ||
                   scenePath == SceneIds.Level01MiniBoss ||
                   scenePath == SceneIds.Level03MiniBoss;
        }

        private static bool IsMainTowerLevel(string scenePath)
        {
            return scenePath == SceneIds.Level01 ||
                   scenePath == SceneIds.Level02 ||
                   scenePath == SceneIds.Level03 ||
                   scenePath == SceneIds.Level04 ||
                   scenePath == SceneIds.Level05;
        }

        private static bool IsMiniBossScene(string scenePath)
        {
            return scenePath == SceneIds.Level01MiniBoss ||
                   scenePath == SceneIds.Level03MiniBoss;
        }

        private void SetState(GameFlowState newState)
        {
            State = newState;
            StateChanged?.Invoke(State);
        }
    }
}
