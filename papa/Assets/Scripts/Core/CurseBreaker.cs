using UnityEngine;

public class CurseBreaker : MonoBehaviour
{
    public static CurseBreaker Instance;
    
    // Assign a friendly/ally prefab here
    public GameObject allyMonsterPrefab; 
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Executes the final story moment: monsters returning as allies fully.
    /// </summary>
    public void BreakCurseAndFreeWorkers()
    {
        Debug.Log("CURSE BREAKER: Initiating mass transformation...");

        // 1. Iterate through all captured workers in the BaseManager
        BaseManager bm = BaseManager.Instance;

        // Combine all workers and managers for transformation
        WorkerMonster[] allMonsters = bm.allWorkers.ToArray(); 
        
        // Clear the old worker lists immediately
        bm.allWorkers.Clear();
        bm.managers.Clear();
        
        // 2. Transformation Logic
        foreach (WorkerMonster worker in allMonsters)
        {
            Vector3 spawnPos = worker.transform.position;
            
            // a) Play transformation VFX (e.g., golden glow)
            // ...
            
            // b) Destroy the old worker object
            Destroy(worker.gameObject);
            
            // c) Instantiate the new, friendly ally version
            // This new ally monster should have a friendly AI/logic component
            Instantiate(allyMonsterPrefab, spawnPos, Quaternion.identity);
        }
        
        Debug.Log($"CURSE BROKEN: {allMonsters.Length} workers freed and transformed into allies.");
    }
}
