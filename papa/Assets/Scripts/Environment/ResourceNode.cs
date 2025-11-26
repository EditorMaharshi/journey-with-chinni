using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Resource Details")]
    // The type of task this node represents (e.g., Woodcutting for Wood)
    public TaskType resourceType = TaskType.Woodcutting; 
    public float baseYield = 5f;
    public int health = 3; // How many hits (interactions) it takes to deplete
    
    [Header("Visuals")]
    public GameObject depletionEffectPrefab; // VFX for chopping/mining
    
    // Tracks whether the node is depleted and should be inactive
    private bool isDepleted = false; 

    /// <summary>
    /// Called when the player successfully interacts with the node (e.g., a stick swing hits it).
    /// </summary>
    public void Interact(float playerStrengthModifier = 1f)
    {
        if (isDepleted) return;

        // 1. Consume the node's health
        health--;

        // 2. Add resource yield based on a modifier (e.g., weapon strength)
        float finalYield = baseYield * playerStrengthModifier;
        
        // Use the appropriate resource type for the resource being collected
        BaseManager.Instance.AddResource(resourceType, finalYield);
        
        Debug.Log($"Collected {finalYield:F1} {resourceType}. Node hits remaining: {health}");
        
        // 3. Play visual feedback
        if (depletionEffectPrefab != null)
        {
            // Spawn a small effect at the interaction point
            Instantiate(depletionEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        }

        if (health <= 0)
        {
            DepleteNode();
        }
    }

    private void DepleteNode()
    {
        isDepleted = true;
        
        // Play final depletion VFX/SFX (e.g., tree falls, rock shatters)
        // ...
        
        // Make the node visually inactive (e.g., disable renderer, sink into ground)
        GetComponent<Collider>().enabled = false;
        // GetComponent<MeshRenderer>().enabled = false; 

        Debug.Log($"{gameObject.name} depleted.");
        
        // Optional: Start a timer to respawn the node if needed for infinite play
        // StartCoroutine(RespawnTimer(300f)); 
    }
}
