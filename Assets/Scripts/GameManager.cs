using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main game manager for Solo mode gameplay
/// Converted from Godot GameManager.gd for Unity 6.2+
/// Simplified version without UI package dependencies
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI References - Assign in Inspector")]
    public GameObject timerText;
    public GameObject scoreText;
    public Transform taskDisplayParent;
    public Transform letterPoolParent;
    public Transform particleParent;

    [Header("Prefabs")]
    public GameObject letterTilePrefab;
    public GameObject riceParticlePrefab;

    [Header("Game Settings")]
    private const float GAME_TIME = 30f;

    // Game state
    private List<GameObject> taskSlots = new List<GameObject>();
    private List<LetterTile> selectedLetters = new List<LetterTile>();
    private List<LetterTile> letterTiles = new List<LetterTile>();

    // Dynamic pool boundaries
    private float poolTop;
    private float poolBottom;
    private float poolLeft;
    private float poolRight;

    void Start()
    {
        CalculatePoolBoundaries();
        SetupTaskDisplay();
        StartGame();
    }

    void Update()
    {
        if (GlobalData.Instance != null && GlobalData.Instance.isGameActive)
        {
            UpdateTimer(Time.deltaTime);
        }
    }

    void CalculatePoolBoundaries()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        poolTop = screenHeight * 0.30f;
        poolBottom = screenHeight * 0.77f;
        poolLeft = screenWidth * 0.12f;
        poolRight = screenWidth * 0.88f;
    }

    void StartGame()
    {
        if (GlobalData.Instance != null)
        {
            GlobalData.Instance.ResetGame();
            GlobalData.Instance.isGameActive = true;
            GenerateNewTask();
            UpdateUI();
        }
    }

    void SetupTaskDisplay()
    {
        // Create 4 simple task slot GameObjects
        for (int i = 0; i < 4; i++)
        {
            GameObject slot = new GameObject($"TaskSlot_{i}");
            slot.transform.SetParent(taskDisplayParent);
            slot.transform.localPosition = new Vector3(i * 90, 0, 0);
            
            // Add TextMesh for label (basic Unity component)
            TextMesh textMesh = slot.AddComponent<TextMesh>();
            textMesh.text = "?";
            textMesh.fontSize = 48;
            textMesh.color = new Color(0.24f, 0.16f, 0.09f, 1f);
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            
            taskSlots.Add(slot);
        }
    }

    void GenerateNewTask()
    {
        if (GlobalData.Instance == null) return;
        
        string task = GlobalData.Instance.StartNewTask();
        
        // Display task in slots
        for (int i = 0; i < 4 && i < task.Length; i++)
        {
            TextMesh label = taskSlots[i].GetComponent<TextMesh>();
            if (label != null)
            {
                label.text = task[i].ToString();
            }
        }
        
        selectedLetters.Clear();
        SpawnLetterTiles(task);
    }

    void SpawnLetterTiles(string task)
    {
        // Clear existing tiles
        foreach (var tile in letterTiles)
        {
            if (tile != null) Destroy(tile.gameObject);
        }
        letterTiles.Clear();

        // Create letter array
        List<char> letters = new List<char>();
        foreach (char c in task)
        {
            letters.Add(c);
        }

        // Add 8 random letters
        string allLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < 8; i++)
        {
            letters.Add(allLetters[Random.Range(0, allLetters.Length)]);
        }

        // Shuffle
        for (int i = 0; i < letters.Count; i++)
        {
            int randomIndex = Random.Range(0, letters.Count);
            char temp = letters[i];
            letters[i] = letters[randomIndex];
            letters[randomIndex] = temp;
        }

        // Spawn tiles
        List<Vector2> positions = new List<Vector2>();
        float minDistance = 100f;

        foreach (char letter in letters)
        {
            if (letterTilePrefab == null) continue;
            
            GameObject tileObj = Instantiate(letterTilePrefab, letterPoolParent);
            LetterTile tile = tileObj.GetComponent<LetterTile>();
            
            if (tile != null)
            {
                tile.SetLetter(letter);
                
                Vector2 pos = FindValidPosition(positions, minDistance);
                tileObj.transform.position = new Vector3(pos.x, pos.y, 0);
                
                positions.Add(pos);
                letterTiles.Add(tile);
                
                tile.OnTileClicked += OnLetterTileClicked;
            }
        }
    }

    Vector2 FindValidPosition(List<Vector2> existingPositions, float minDistance)
    {
        Vector2 pos = Vector2.zero;
        bool validPosition = false;
        int attempts = 0;

        while (!validPosition && attempts < 100)
        {
            pos = new Vector2(
                Random.Range(poolLeft, poolRight),
                Random.Range(poolTop, poolBottom)
            );

            validPosition = true;
            foreach (Vector2 existingPos in existingPositions)
            {
                if (Vector2.Distance(pos, existingPos) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }
            attempts++;
        }

        return pos;
    }

    void OnLetterTileClicked(LetterTile tile)
    {
        if (selectedLetters.Count >= 4 || GlobalData.Instance == null) return;

        string currentTask = GlobalData.Instance.currentTask;
        bool isValid = false;
        int targetSlotIdx = -1;

        for (int i = 0; i < currentTask.Length; i++)
        {
            if (currentTask[i] == tile.Letter && selectedLetters.Count == i)
            {
                isValid = true;
                targetSlotIdx = i;
                break;
            }
        }

        if (isValid)
        {
            SpawnRiceParticles(tile.transform.position, 6);
            tile.MarkAsSelected();
            selectedLetters.Add(tile);
            
            StartCoroutine(tile.FlyToSlot(taskSlots[targetSlotIdx].transform.position));

            if (selectedLetters.Count == 4)
            {
                StartCoroutine(CompleteTaskAfterDelay());
            }
        }
        else
        {
            tile.PlayShakeAnimation();
            if (GlobalData.Instance != null)
            {
                GlobalData.Instance.DeductPoints();
                UpdateUI();
            }
        }
    }

    IEnumerator CompleteTaskAfterDelay()
    {
        yield return new WaitForSeconds(0.7f);
        CompleteTask();
    }

    void CompleteTask()
    {
        if (GlobalData.Instance == null) return;
        
        SpawnRiceParticles(taskDisplayParent.position, 25);
        
        float timeTaken = (Time.time - (GlobalData.Instance.taskStartTime / 1000f));
        GlobalData.Instance.CompleteTask(timeTaken);
        
        if (GlobalData.Instance.gameMode == "solo")
        {
            GlobalData.Instance.UpdatePlayerStats(timeTaken);
        }

        GlobalData.Instance.timeLeft = Mathf.Min(GlobalData.Instance.timeLeft + 2f, GAME_TIME);
        
        UpdateUI();
        StartCoroutine(GenerateNewTaskAfterDelay());
    }

    IEnumerator GenerateNewTaskAfterDelay()
    {
        yield return new WaitForSeconds(0.8f);
        if (GlobalData.Instance != null && GlobalData.Instance.isGameActive)
        {
            GenerateNewTask();
        }
    }

    void UpdateTimer(float delta)
    {
        GlobalData.Instance.timeLeft -= delta;

        if (GlobalData.Instance.timeLeft <= 0)
        {
            GlobalData.Instance.timeLeft = 0;
            GlobalData.Instance.isGameActive = false;
            EndGame();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (GlobalData.Instance == null) return;
        
        // Timer
        int minutes = (int)(GlobalData.Instance.timeLeft / 60f);
        int seconds = (int)GlobalData.Instance.timeLeft % 60;
        string timerString = $"{minutes}:{seconds:D2}";
        
        if (timerText != null)
        {
            TextMesh tm = timerText.GetComponent<TextMesh>();
            if (tm != null) tm.text = timerString;
        }

        // Score
        if (scoreText != null)
        {
            TextMesh sm = scoreText.GetComponent<TextMesh>();
            if (sm != null) sm.text = GlobalData.Instance.currentScore.ToString("000");
        }
    }

    void SpawnRiceParticles(Vector3 position, int count)
    {
        if (riceParticlePrefab == null || particleParent == null) return;
        
        for (int i = 0; i < count; i++)
        {
            GameObject particle = Instantiate(riceParticlePrefab, particleParent);
            particle.transform.position = position;
            
            float angle = (Mathf.PI * 2f * i) / count + Random.Range(-0.25f, 0.25f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(150f, 250f);
            velocity.y -= 100f;
            
            RiceParticle particleScript = particle.GetComponent<RiceParticle>();
            if (particleScript != null)
            {
                particleScript.SetVelocity(velocity);
            }
        }
    }

    void EndGame()
    {
        if (GlobalData.Instance != null && GlobalData.Instance.gameMode == "solo")
        {
            GlobalData.Instance.playerTotalGames++;
            GlobalData.Instance.SavePlayerStats();
        }
        
        // Load score screen - requires scene to be created
        // UnityEngine.SceneManagement.SceneManager.LoadScene("ScoreScreen");
        Debug.Log("Game Over! Score: " + (GlobalData.Instance != null ? GlobalData.Instance.currentScore : 0));
    }
}
