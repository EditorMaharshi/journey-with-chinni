using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("HUD References")]
    public Slider healthSlider;
    public Text healthText;
    public Text chinniStatusText;
    
    [Header("Resource Display")]
    // NOTE: You must map TaskType to a UI Text object manually in the Inspector
    public Dictionary<TaskType, Text> resourceTexts = new Dictionary<TaskType, Text>();
    public Text woodText; // Placeholder for mapping

    [Header("Base Management Screen")]
    public GameObject baseManagementPanel;
    public Transform workerContentParent; 
    private bool isBasePanelOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeResourceDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        GameManager.Instance.OnGameDataUpdated += UpdateAllUI;
        UpdateAllUI(); 
        baseManagementPanel?.SetActive(false);
    }
    
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameDataUpdated -= UpdateAllUI;
        }
    }

    private void InitializeResourceDictionary()
    {
        // Must be fully defined by dragging UI Text objects to public fields
        if (woodText != null) resourceTexts.Add(TaskType.Woodcutting, woodText);
        // Add other mappings here...
    }

    public void UpdateAllUI()
    {
        PlayerCombat pc = FindObjectOfType<PlayerCombat>();
        if (pc != null)
        {
            UpdateHealth(pc.currentHealth, pc.maxHealth);
        }
        UpdateResources();
        chinniStatusText.text = GameManager.Instance.isChinniPresent ? "Companion: Active" : "Companion: Gone";
    }

    public void UpdateHealth(float current, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
        healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
    
    private void UpdateResources()
    {
        if (BaseManager.Instance == null) return;
        
        foreach (var kvp in BaseManager.Instance.resources)
        {
            if (resourceTexts.TryGetValue(kvp.Key, out Text display))
            {
                display.text = $"{kvp.Key}: {kvp.Value:F1}";
            }
        }
    }

    public void ToggleBaseUI()
    {
        isBasePanelOpen = !isBasePanelOpen;
        baseManagementPanel.SetActive(isBasePanelOpen);
        
        if (isBasePanelOpen)
        {
            Time.timeScale = 0f; 
            PopulateWorkerList();
        }
        else
        {
            Time.timeScale = 1f;
            ClearWorkerList();
        }
    }

    private void PopulateWorkerList()
    {
        if (BaseManager.Instance == null) return;
        
        ClearWorkerList();
        
        // NOTE: This requires a Prefab named "WorkerEntryPrefab" in Resources/Prefabs/UI
        GameObject workerEntryPrefab = Resources.Load<GameObject>("Prefabs/UI/WorkerEntryPrefab");
        
        foreach (WorkerMonster worker in BaseManager.Instance.allWorkers)
        {
            GameObject entry = Instantiate(workerEntryPrefab, workerContentParent);
            entry.GetComponent<WorkerUIEntry>().Setup(worker);
        }
    }
    
    private void ClearWorkerList()
    {
        foreach (Transform child in workerContentParent)
        {
            Destroy(child.gameObject);
        }
    }
}
