using UnityEngine;
using System; // Required for Action event

public class MonsterCapture : MonoBehaviour
{
    // === NEW: Event for XP Gain ===
    public static event Action<int> OnMonsterDefeated;
    
    // Attach this to the Monster Prefab
    // ... (rest of the fields) ...

    void DieAndConvert()
    {
        // 1. Fire the XP event
        // The PlayerLevel script is listening for this event
        if (OnMonsterDefeated != null) 
        {
            // Use the XP value from the monster's data
            OnMonsterDefeated.Invoke(monsterData.experienceGained); 
        }

        // 2. Add the monster to the Base Manager's pool
        // ... (rest of the conversion logic) ...
        Destroy(gameObject);
    }
}
