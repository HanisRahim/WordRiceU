using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Global game state manager - Singleton pattern
/// Converted from Godot Global.gd for Unity 6.2+
/// Compatible with Unity 6.x versions
/// </summary>
public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance { get; private set; }

    [Header("Game Mode")]
    public string gameMode = "solo"; // "solo" or "vs"
    public string opponentName = "";
    public int opponentDifficulty = 1; // 1-5

    [Header("Game State")]
    public int currentScore = 0;
    public int wrongClicks = 0;
    public int tasksCompleted = 0;
    public List<TaskData> completedTasks = new List<TaskData>();
    public string currentTask = "";
    public float taskStartTime = 0f;
    public float timeLeft = 30f;
    public bool isGameActive = false;

    [Header("AI Stats")]
    public float playerAvgTime = 0f;
    public int playerTotalGames = 0;
    public List<float> playerTaskTimes = new List<float>();

    [Header("Leaderboard")]
    private const int LEADERBOARD_SIZE = 10;
    public List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();

    [Header("Word Dictionary")]
    public string[] wordDictionary = new string[]
    {
        "WORD", "RICE", "GAME", "PLAY", "TIME", "LOVE", "HATE", "LIKE", "MAKE", "TAKE",
        "COME", "SOME", "THEM", "THEN", "THAN", "THAT", "THIS", "WITH", "HAVE", "FROM",
        "THEY", "BEEN", "WERE", "WILL", "YOUR", "MORE", "WHEN", "WORK", "ALSO", "WELL",
        "VERY", "YEAR", "BACK", "CALL", "CAME", "EACH", "EVEN", "FEEL", "FIND", "GIVE",
        "GOOD", "HAND", "HIGH", "KEEP", "LAST", "LEFT", "LIFE", "LIVE", "LONG", "LOOK",
        "MADE", "MANY", "MUST", "NAME", "NEED", "NEXT", "ONLY", "OPEN", "OVER", "PART",
        "REAL", "SAID", "SAME", "SEEM", "SHOW", "SIDE", "SUCH", "SURE", "TELL", "TURN",
        "USED", "WANT", "WAYS", "WEEK", "WENT", "WHAT", "WILD", "WISE", "ZERO", "ZONE"
    };

    [Header("Malaysian Names")]
    public string[] malaysianNames = new string[]
    {
        "Ahmad", "Siti", "Muhammad", "Nurul", "Aminah", "Hassan", "Fatimah", "Ibrahim",
        "Aziz", "Nora", "Khairul", "Zainab", "Ismail", "Rafiq", "Farah", "Rizal",
        "Aishah", "Amir", "Nazir", "Hana", "Azman", "Laila", "Hafiz", "Maryam",
        "Razak", "Sofia", "Kamal", "Yasmin", "Jalil", "Aisyah", "Firdaus", "Azlina",
        "Rashid", "Nadia", "Zulkifli", "Hidayah", "Halim", "Suraya", "Arif", "Melissa",
        "Hakim", "Salmah", "Ramli", "Zura", "Kamarul", "Noraini", "Shahrul", "Azura"
    };

    // Difficulty multipliers
    private readonly Dictionary<int, float> difficultyMultipliers = new Dictionary<int, float>
    {
        { 1, 1.00f },   // Normal
        { 2, 0.90f },   // Slightly Hard
        { 3, 0.80f },   // Medium Hard
        { 4, 0.70f },   // Hard
        { 5, 0.67f }    // Very Hard (50% faster)
    };

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLeaderboard();
            LoadPlayerStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetGame()
    {
        currentScore = 0;
        wrongClicks = 0;
        tasksCompleted = 0;
        completedTasks.Clear();
        timeLeft = 30f;
        isGameActive = false;
    }

    public string StartNewTask()
    {
        // Shuffle and pick random word
        int randomIndex = UnityEngine.Random.Range(0, wordDictionary.Length);
        currentTask = wordDictionary[randomIndex];
        taskStartTime = Time.time * 1000f;
        return currentTask;
    }

    public int CompleteTask(float timeTaken)
    {
        int taskScore = (int)(200f - (timeTaken * 25f));
        taskScore = Mathf.Max(30, taskScore);

        completedTasks.Add(new TaskData
        {
            word = currentTask,
            time = timeTaken,
            score = taskScore
        });

        tasksCompleted++;
        currentScore += taskScore;
        return taskScore;
    }

    public void DeductPoints()
    {
        wrongClicks++;
        currentScore = Mathf.Max(0, currentScore - 20);
    }

    public string GetRandomMalaysianName()
    {
        return malaysianNames[UnityEngine.Random.Range(0, malaysianNames.Length)];
    }

    public int GetRandomDifficulty()
    {
        return UnityEngine.Random.Range(1, 6); // 1-5
    }

    public float GetAITaskTime()
    {
        float difficultyMult = difficultyMultipliers.ContainsKey(opponentDifficulty) 
            ? difficultyMultipliers[opponentDifficulty] 
            : 1.0f;

        float baseTime;
        if (playerTotalGames == 0 || playerAvgTime == 0)
        {
            baseTime = UnityEngine.Random.Range(3.0f, 4.5f);
        }
        else
        {
            baseTime = playerAvgTime;
        }

        float adjustedTime = baseTime * difficultyMult;
        float variance = adjustedTime * 0.15f;
        
        return UnityEngine.Random.Range(adjustedTime - variance, adjustedTime + variance);
    }

    public void UpdatePlayerStats(float taskTime)
    {
        playerTaskTimes.Add(taskTime);

        if (playerTaskTimes.Count > 50)
        {
            playerTaskTimes.RemoveAt(0);
        }

        if (playerTaskTimes.Count > 0)
        {
            float total = 0f;
            foreach (float time in playerTaskTimes)
            {
                total += time;
            }
            playerAvgTime = total / playerTaskTimes.Count;
        }

        SavePlayerStats();
    }

    public bool IsTopTen()
    {
        if (leaderboard.Count < LEADERBOARD_SIZE)
            return true;
        return currentScore > leaderboard[LEADERBOARD_SIZE - 1].score;
    }

    public void AddToLeaderboard(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Rice";

        leaderboard.Add(new LeaderboardEntry
        {
            name = playerName,
            score = currentScore,
            date = DateTime.Now.ToString()
        });

        leaderboard.Sort((a, b) => b.score.CompareTo(a.score));

        if (leaderboard.Count > LEADERBOARD_SIZE)
        {
            leaderboard.RemoveRange(LEADERBOARD_SIZE, leaderboard.Count - LEADERBOARD_SIZE);
        }

        SaveLeaderboard();
    }

    void LoadLeaderboard()
    {
        string path = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            if (data != null && data.entries != null)
            {
                leaderboard = new List<LeaderboardEntry>(data.entries);
            }
        }
    }

    void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData { entries = leaderboard.ToArray() };
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        File.WriteAllText(path, json);
    }

    void LoadPlayerStats()
    {
        string path = Path.Combine(Application.persistentDataPath, "player_stats.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(json);
            if (data != null)
            {
                playerAvgTime = data.avgTime;
                playerTotalGames = data.totalGames;
                playerTaskTimes = new List<float>(data.taskTimes);
            }
        }
    }

    public void SavePlayerStats()
    {
        PlayerStatsData data = new PlayerStatsData
        {
            avgTime = playerAvgTime,
            totalGames = playerTotalGames,
            taskTimes = playerTaskTimes.ToArray()
        };
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, "player_stats.json");
        File.WriteAllText(path, json);
    }
}

[Serializable]
public class TaskData
{
    public string word;
    public float time;
    public int score;
}

[Serializable]
public class LeaderboardEntry
{
    public string name;
    public int score;
    public string date;
}

[Serializable]
public class LeaderboardData
{
    public LeaderboardEntry[] entries;
}

[Serializable]
public class PlayerStatsData
{
    public float avgTime;
    public int totalGames;
    public float[] taskTimes;
}

