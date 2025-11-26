using UnityEngine;

public class WorkerMonster : MonoBehaviour
{
    // === Core Identity ===
    public string monsterType; 
    public int currentLevel;
    public bool isManager = false;
    
    [Header("Workload")]
    public TaskType assignedTask = TaskType.Woodcutting;
    private ScriptableMonsterData baseData; 
    
    private float workXP = 0f;
    private const float XP_TO_LEVEL = 100f; 
    private const int WORKER_TO_MANAGER_COUNT = 10; 
    
    private float workTimer = 0f;
    private const float WORK_INTERVAL = 5f; // Time (seconds) between resource generation ticks

    public void Initialize(string type, int level)
    {
        monsterType = type;
        currentLevel = level;
        
        // Load the corresponding Scriptable Data to get stats (you must load it via Resources.Load or a central data manager)
        // baseData = Resources.Load<ScriptableMonsterData>("ScriptableObjects/MonsterData/MD_" + type);
        // NOTE: For now, you must ensure you have a reference to the ScriptableMonsterData to get efficiencies.
    }
    
    public void AssignTask(TaskType task)
    {
        assignedTask = task;
    }
    
    void Update()
    {
        if (isManager) return; 
        
        workTimer += Time.deltaTime;
        
        if (workTimer >= WORK_INTERVAL)
        {
            DoWork();
            workTimer = 0f;
        }
    }
    
    private void DoWork()
    {
        if (BaseManager.Instance == null) return;
        
        // NOTE: baseData is required here to get the efficiency stat!
        // int efficiency = GetEfficiencyForTask(assignedTask); 
        int efficiency = 20; // Placeholder until baseData is properly loaded

        // Output calculation: Base efficiency * current discipline level
        float output = (efficiency * currentLevel) / 10f; 
        
        // Add resource (e.g., Woodcutting task adds Wood resource)
        BaseManager.Instance.AddResource(assignedTask, output);
        
        // Gain Work XP (discipline)
        workXP += 5 * currentLevel; 
        
        if (workXP >= XP_TO_LEVEL)
        {
            LevelUpWorkDiscipline();
        }
    }
    
    private void LevelUpWorkDiscipline()
    {
        workXP -= XP_TO_LEVEL;
        currentLevel++;

        if (currentLevel % WORKER_TO_MANAGER_COUNT == 0)
        {
            BaseManager.Instance.TryPromoteWorker(this);
        }
    }
    
    public void PromoteToManager()
    {
        isManager = true;
        // Logic to boost other workers or apply passive bonus
    }
}
