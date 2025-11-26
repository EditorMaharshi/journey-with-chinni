using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    // === Scene Names (Match these exactly in Unity's Build Settings) ===
    private const string JUNGLE_SCENE = "S01_Jungle";
    private const string HOME_BASE_SCENE = "S02_HomeBase";

    public static SceneLoader Instance; // Singleton reference

    [Header("UI Reference")]
    public GameObject loadingScreenUI; // A canvas to show during loading transitions

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Ensures the SceneLoader persists across scene loads
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // === Core Load/Transition Methods ===

    /// <summary>
    /// Starts the async loading process for the target scene.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Loads the Jungle exploration scene. Called at game start or when leaving the Base.
    /// </summary>
    public void GoToJungle()
    {
        LoadScene(JUNGLE_SCENE);
    }
    
    /// <summary>
    /// Loads the Home Base management scene. Called when player interacts with the base trigger.
    /// </summary>
    public void GoToHomeBase()
    {
        // Save the game state before leaving the exploration area
        GameManager.Instance.SaveGame(); 
        LoadScene(HOME_BASE_SCENE);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingScreenUI != null)
        {
            loadingScreenUI.SetActive(true);
            // Optionally, show a progress bar in the UI
        }
        
        // Unload the current active scene before loading the new one
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
            while (!unloadOp.isDone)
            {
                // Update progress if needed
                yield return null;
            }
        }

        // Load the new scene additively (or single, based on project needs. Single is simpler here)
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!loadOp.isDone)
        {
            // Update loading progress bar (loadOp.progress)
            yield return null;
        }

        if (loadingScreenUI != null)
        {
            loadingScreenUI.SetActive(false);
        }
        
        Debug.Log($"Scene loaded: {sceneName}");
        
        // Re-initialize systems that need a scene context (e.g., Level Generator)
        if (sceneName == JUNGLE_SCENE)
        {
            LevelGenerator generator = FindObjectOfType<LevelGenerator>();
            if (generator != null)
            {
                generator.GenerateNewLevel(); // Ensure the jungle is built when entering
            }
        }
    }
}
