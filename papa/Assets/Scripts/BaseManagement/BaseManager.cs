using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Added for Dictionary methods

public enum TaskType { Woodcutting, Farming, Forging, Construction, Research, Guarding }

public class BaseManager : MonoBehaviour
{
    // ... (Awake and resourceTexts dictionary setup remains the same) ...

    // --- Core Resource Methods ---

    // Public method to add resources (called by WorkerMonster.cs and ResourceNode.cs)
    public void AddResource(TaskType source, float amount)
    {
        // We use TaskType as the key for the resource it generates (e.g., Research generates Research Points)
        if (resources.ContainsKey(source))
        {
            resources[source] += amount;
            // GameManager.Instance.OnGameDataUpdated?.Invoke(); // Notify UI
        }
    }

    /// <summary>
    /// Checks if the Base has enough of a specific resource.
    /// </summary>
    public bool HasResource(TaskType resourceType, float amountNeeded)
    {
        if (resources.ContainsKey(resourceType))
        {
            return resources[resourceType] >= amountNeeded;
        }
        return false;
    }

    /// <summary>
    /// Attempts to spend a resource. Returns true on success, false otherwise.
    /// </summary>
    public bool TrySpendResource(TaskType resourceType, float amountToSpend)
    {
        if (HasResource(resourceType, amountToSpend))
        {
            resources[resourceType] -= amountToSpend;
            // GameManager.Instance.OnGameDataUpdated?.Invoke(); // Notify UI
            Debug.Log($"Spent {amountToSpend:F1} {resourceType}. Remaining: {resources[resourceType]:F1}");
            return true;
        }
        
        Debug.LogWarning($"Insufficient {resourceType}. Needed {amountToSpend:F1}, Have {resources[resourceType]:F1}");
        return false;
    }
    
    // ... (AddSlaveWorker, TryPromoteWorker, and other methods remain the same) ...
}
