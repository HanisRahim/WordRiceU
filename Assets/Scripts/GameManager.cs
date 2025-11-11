using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_UI
using UnityEngine.UI;
#endif

#if TMP_PRESENT
using TMPro;
#endif

/// <summary>
/// Main game manager for Solo mode gameplay
/// Converted from Godot GameManager.gd for Unity 6.2+
/// Compatible with Unity 6.x versions
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    #if TMP_PRESENT
    public TMPro.TextMeshProUGUI timerLabel;
    public TMPro.TextMeshProUGUI scoreLabel;
    #else
    public UnityEngine.UI.Text timerLabel;
    public UnityEngine.UI.Text scoreLabel;
    #endif
    
    #if UNITY_UI
    public UnityEngine.UI.Slider progressBar;
    #endif
    public Transform taskDisplayParent;
    public Transform letterPoolParent;
    public Transform particleParent;

    [Header("Prefabs")]
    public GameObject letterTilePrefab;
    public GameObject riceParticlePrefab;

    [Header("Game Settings")]
    private const float GAME_TIME = 30f;
    private const int TILE_PADDING = 20;

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
        if (GlobalData.Instance.isGameActive)
        {
            UpdateTimer(Time.deltaTime);
        }
    }

    void CalculatePoolBoundaries()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        poolTop = screenHeight * 0.30f;
        poolBottom = screenHeight * 0.77f;  // 23% from bottom
        poolLeft = screenWidth * 0.12f;
        poolRight = screenWidth * 0.88f;  // 12% from right
    }

    void StartGame()
    {
        GlobalData.Instance.ResetGame();
        GlobalData.Instance.isGameActive = true;
        GenerateNewTask();
        UpdateUI();
    }

    void SetupTaskDisplay()
    {
        // Create 4 task slots
        for (int i = 0; i < 4; i++)
        {
            GameObject slot = new GameObject($"TaskSlot_{i}");
            slot.transform.SetParent(taskDisplayParent);
            
            // Add UI components for slot display (requires UI package)
            #if UNITY_UI
            UnityEngine.UI.Image slotImage = slot.AddComponent<UnityEngine.UI.Image>();
            slotImage.color = HexToColor("f5ecd7");
            #endif
            
            // Add label
            GameObject labelObj = new GameObject("Label");
            labelObj.transform.SetParent(slot.transform);
            
            #if TMP_PRESENT
            TMPro.TextMeshProUGUI label = labelObj.AddComponent<TMPro.TextMeshProUGUI>();
            label.fontSize = 48;
            label.alignment = TMPro.TextAlignmentOptions.Center;
            label.color = HexToColor("3d2817");
            #else
            UnityEngine.UI.Text label = labelObj.AddComponent<UnityEngine.UI.Text>();
            label.fontSize = 48;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = HexToColor("3d2817");
            #endif
            
            taskSlots.Add(slot);
        }
    }

    void GenerateNewTask()
    {
        string task = GlobalData.Instance.StartNewTask();
        
        // Display task in slots
        for (int i = 0; i < 4; i++)
        {
            #if TMP_PRESENT
            var label = taskSlots[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            #else
            var label = taskSlots[i].GetComponentInChildren<UnityEngine.UI.Text>();
            #endif
            
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

        // Create letter array (4 correct + 8 random)
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

        // Spawn tiles with collision avoidance
        List<Vector2> positions = new List<Vector2>();
        float minDistance = 100f;

        foreach (char letter in letters)
        {
            GameObject tileObj = Instantiate(letterTilePrefab, letterPoolParent);
            LetterTile tile = tileObj.GetComponent<LetterTile>();
            
            if (tile != null)
            {
                tile.SetLetter(letter);
                
                // Find valid position
                Vector2 pos = FindValidPosition(positions, minDistance);
                tileObj.transform.position = new Vector3(pos.x, pos.y, 0);
                
                positions.Add(pos);
                letterTiles.Add(tile);
                
                // Subscribe to click event
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
        if (selectedLetters.Count >= 4) return;

        string currentTask = GlobalData.Instance.currentTask;
        bool isValid = false;
        int targetSlotIdx = -1;

        // Check if letter is correct for any unfilled slot
        for (int i = 0; i < currentTask.Length; i++)
        {
            if (currentTask[i] == tile.Letter && !IsSlotFilled(i))
            {
                isValid = true;
                targetSlotIdx = i;
                break;
            }
        }

        if (isValid)
        {
            // Correct letter
            SpawnRiceParticles(tile.transform.position, 6);
            tile.MarkAsSelected();
            selectedLetters.Add(tile);
            
            // Animate to slot
            StartCoroutine(tile.FlyToSlot(taskSlots[targetSlotIdx].transform.position));
            
            // Mark slot as filled
            MarkSlotFilled(targetSlotIdx);

            if (selectedLetters.Count == 4)
            {
                StartCoroutine(CompleteTaskAfterDelay());
            }
        }
        else
        {
            // Wrong letter
            tile.PlayShakeAnimation();
            GlobalData.Instance.DeductPoints();
            UpdateUI();
        }
    }

    IEnumerator CompleteTaskAfterDelay()
    {
        yield return new WaitForSeconds(0.7f);
        CompleteTask();
    }

    void CompleteTask()
    {
        SpawnRiceBurst(taskDisplayParent.position, 25);
        
        float timeTaken = (Time.time * 1000f) - GlobalData.Instance.taskStartTime;
        GlobalData.Instance.CompleteTask(timeTaken / 1000f);
        
        // Track stats for AI learning
        if (GlobalData.Instance.gameMode == "solo")
        {
            GlobalData.Instance.UpdatePlayerStats(timeTaken / 1000f);
        }

        // Add time bonus
        GlobalData.Instance.timeLeft = Mathf.Min(GlobalData.Instance.timeLeft + 2f, GAME_TIME);
        
        // Flash progress bar green
        StartCoroutine(FlashProgressBarGreen());
        
        UpdateUI();
        
        StartCoroutine(GenerateNewTaskAfterDelay());
    }

    IEnumerator GenerateNewTaskAfterDelay()
    {
        yield return new WaitForSeconds(0.8f);
        if (GlobalData.Instance.isGameActive)
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
        // Timer
        int minutes = (int)(GlobalData.Instance.timeLeft / 60f);
        int seconds = (int)GlobalData.Instance.timeLeft % 60;
        timerLabel.text = $"{minutes}:{seconds:D2}";

        // Gradual color transition for last 10 seconds
        if (GlobalData.Instance.timeLeft <= 10f && GlobalData.Instance.timeLeft > 0)
        {
            Color timerColor;
            if (GlobalData.Instance.timeLeft > 5f)
            {
                // Transition from gold to red (10s to 5s)
                float transition = (10f - GlobalData.Instance.timeLeft) / 5f;
                timerColor = Color.Lerp(HexToColor("FFD700"), Color.red, transition);
            }
            else
            {
                timerColor = Color.red;
            }
            
            timerLabel.color = timerColor;

            // Pulsing for last 5 seconds
            if (GlobalData.Instance.timeLeft <= 5f)
            {
                float pulseScale = 1f + (Mathf.Sin(Time.time * 6.67f) * 0.1f);
                timerLabel.transform.localScale = Vector3.one * pulseScale;
            }
        }
        else
        {
            timerLabel.color = HexToColor("FFD700");
            timerLabel.transform.localScale = Vector3.one;
        }

        // Progress bar
        #if UNITY_UI
        if (progressBar != null)
        {
            progressBar.value = (GlobalData.Instance.timeLeft / GAME_TIME) * 100f;
        }
        #endif

        // Score
        if (scoreLabel != null)
        {
            scoreLabel.text = GlobalData.Instance.currentScore.ToString("000");
        }
    }

    void SpawnRiceParticles(Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject particle = Instantiate(riceParticlePrefab, particleParent);
            particle.transform.position = position;
            
            float angle = (Mathf.PI * 2f * i) / count + Random.Range(-0.25f, 0.25f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(150f, 250f);
            velocity.y -= 100f;  // Upward bias
            
            RiceParticle particleScript = particle.GetComponent<RiceParticle>();
            if (particleScript != null)
            {
                particleScript.SetVelocity(velocity);
            }
        }
    }

    void SpawnRiceBurst(Vector3 center, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject particle = Instantiate(riceParticlePrefab, particleParent);
            particle.transform.position = center;
            
            float angle = (Mathf.PI * 2f * i) / count + Random.Range(-0.2f, 0.2f);
            Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Random.Range(300f, 600f);
            velocity.y -= 150f;  // Strong upward explosion
            
            RiceParticle particleScript = particle.GetComponent<RiceParticle>();
            if (particleScript != null)
            {
                particleScript.SetVelocity(velocity);
            }
        }
    }

    IEnumerator FlashProgressBarGreen()
    {
        #if UNITY_UI
        if (progressBar != null && progressBar.fillRect != null)
        {
            UnityEngine.UI.Image fillImage = progressBar.fillRect.GetComponent<UnityEngine.UI.Image>();
            if (fillImage != null)
            {
                Color originalColor = fillImage.color;
                Color greenColor = new Color(0.145f, 0.867f, 0.294f, 1f);
                
                // Flash to green
                fillImage.color = greenColor;
                yield return new WaitForSeconds(0.35f);
                
                // Fade back
                float elapsed = 0f;
                while (elapsed < 1f)
                {
                    elapsed += Time.deltaTime;
                    fillImage.color = Color.Lerp(greenColor, originalColor, elapsed);
                    yield return null;
                }
            }
        }
        #else
        yield return null;
        #endif
    }

    void EndGame()
    {
        if (GlobalData.Instance.gameMode == "solo")
        {
            GlobalData.Instance.playerTotalGames++;
            GlobalData.Instance.SavePlayerStats();
        }
        
        // Load score screen
        UnityEngine.SceneManagement.SceneManager.LoadScene("ScoreScreen");
    }

    bool IsSlotFilled(int index)
    {
        // Check if slot is already filled
        // Implementation depends on your slot marking system
        return selectedLetters.Count > index;
    }

    void MarkSlotFilled(int index)
    {
        // Mark slot as filled with golden color
        #if UNITY_UI
        UnityEngine.UI.Image slotImage = taskSlots[index].GetComponent<UnityEngine.UI.Image>();
        if (slotImage != null)
        {
            slotImage.color = HexToColor("ffd966");
        }
        #endif
    }

    // Utility method to convert hex to Color
    Color HexToColor(string hex)
    {
        if (hex.StartsWith("#")) hex = hex.Substring(1);
        
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        
        return new Color32(r, g, b, 255);
    }
}

