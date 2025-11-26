using UnityEngine;
using System.IO;
using System; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Progression")]
    [Range(0f, 100f)]
    public float currentMainQuestProgress = 0f;
    public bool isChinniPresent = true;

    // === Core Events ===
    public event Action OnGameProgressReached80; 
    public event Action OnGameDataUpdated; 
    public event Action OnSaveGame;
    public event Action OnLoadGame;

    // === Save System Settings ===
    private const string SAVE_FILE_NAME = "player_empire_data.json";
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadGame();
    }
    
    void Update()
    {
        CheckForChinniDeparture();
    }

    // --- Progression Logic ---
    
    public void UpdateProgress(float newProgress)
    {
        if (newProgress > currentMainQuestProgress)
        {
            currentMainQuestProgress = Mathf.Clamp(newProgress, 0f, 100f);
            OnGameDataUpdated?.Invoke(); 
        }
    }
    
    private void CheckForChinniDeparture()
    {
        const float CHINNI_DEPARTURE_THRESHOLD = 80f;
        
        if (isChinniPresent && currentMainQuestProgress >= CHINNI_DEPARTURE_THRESHOLD)
        {
            isChinniPresent = false;
            Debug.Log($"PROGRESS {currentMainQuestProgress}%: Chinni is leaving!");
            OnGameProgressReached80?.Invoke();
            SaveGame();
        }
    }

    // --- Save/Load System ---
    
    public void SaveGame()
    {
        OnSaveGame?.Invoke(); 

        PlayerData data = new PlayerData
        {
            progress = currentMainQuestProgress,
            // You MUST gather data from BaseManager, PlayerCombat, etc. here for a complete save
            // workersCount = BaseManager.Instance.allWorkers.Count, 
        };

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);

                currentMainQuestProgress = data.progress;
                isChinniPresent = data.progress < 80f; 
                
                OnLoadGame?.Invoke(); 

                OnGameDataUpdated?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}. Starting new game.");
            }
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public float progress;
    // Add other fields needed for saving here
}
