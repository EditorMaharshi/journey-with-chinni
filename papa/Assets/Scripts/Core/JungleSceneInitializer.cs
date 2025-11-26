using UnityEngine;

public class JungleSceneInitializer : MonoBehaviour
{
    [Header("Scene Dependencies")]
    // Assign the LevelGenerator script instance found in the scene
    public LevelGenerator levelGenerator; 

    [Header("Player Spawn")]
    // Assign the player prefab here
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;

    void Start()
    {
        // 1. Ensure essential Singletons are present before proceeding
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found! Cannot initialize scene. Loading required core systems...");
            // In a robust game, you'd load the GameManager prefab additively here.
            // For now, assume it's loaded via DontDestroyOnLoad or is the bootstrap scene.
            return; 
        }

        // 2. Load/Spawn the Player
        SpawnPlayer();

        // 3. Generate the Level
        if (levelGenerator != null)
        {
            Debug.Log("Jungle Scene Initialized. Calling Level Generator to build the map.");
            levelGenerator.GenerateNewLevel();
        }
        else
        {
            Debug.LogError("LevelGenerator not assigned! The jungle will not be built.");
        }
        
        // 4. Update UI for the new scene
        // HUDManager.Instance.UpdateAllUI();
    }

    private void SpawnPlayer()
    {
        // Check if the player is already in the scene (e.g., if loaded from a save state)
        if (FindObjectOfType<PlayerController>() == null)
        {
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                // Instantiate the player at the defined start point
                Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                Debug.Log("Player spawned successfully.");
            }
            else
            {
                Debug.LogError("Player Prefab or Spawn Point is missing!");
            }
        }
        else
        {
            Debug.Log("Player already exists in the scene (re-entering from base). Skipping spawn.");
        }
    }
}
