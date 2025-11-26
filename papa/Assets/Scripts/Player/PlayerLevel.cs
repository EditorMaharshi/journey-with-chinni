using UnityEngine;
using System;

public class PlayerLevel : MonoBehaviour
{
    // This is a component attached to the Player GameObject

    [Header("Leveling Stats")]
    public int currentLevel = 1;
    public float currentXP = 0f;
    public float baseXPToNextLevel = 100f;
    public float xpCurveFactor = 1.25f; // How much harder each level gets

    public event Action<int> OnLevelUp; // Event triggered when level changes

    void Start()
    {
        // Subscribe to an event that the MonsterCapture script can fire
        // This is ideal for clean, decoupled XP distribution
        MonsterCapture.OnMonsterDefeated += GainXP;
        
        // Ensure the initial progress is set (e.g., if loading a new game)
        UpdateGameProgressBasedOnLevel();
    }
    
    void OnDestroy()
    {
        MonsterCapture.OnMonsterDefeated -= GainXP;
    }

    /// <summary>
    /// Calculates the required XP for the current level to reach the next.
    /// </summary>
    public float GetRequiredXPForNextLevel()
    {
        // Simple exponential curve: Base * (Factor ^ CurrentLevel)
        return baseXPToNextLevel * Mathf.Pow(xpCurveFactor, currentLevel - 1);
    }

    /// <summary>
    /// Adds XP gained from external sources (e.g., defeating a monster).
    /// </summary>
    public void GainXP(int amount)
    {
        currentXP += amount;
        
        while (currentXP >= GetRequiredXPForNextLevel())
        {
            LevelUp();
        }
        
        // UIManager.Instance.UpdateXPBar(currentXP, GetRequiredXPForNextLevel());
        UpdateGameProgressBasedOnLevel();
    }

    private void LevelUp()
    {
        float requiredXP = GetRequiredXPForNextLevel();
        
        // Remove the required XP, keeping the remainder (spillover)
        currentXP -= requiredXP; 
        currentLevel++;

        Debug.Log($"PLAYER LEVELED UP! New Level: {currentLevel}");

        // Trigger the level up event
        OnLevelUp?.Invoke(currentLevel); 
        
        // Immediately save the major progression change
        GameManager.Instance.SaveGame(); 
        
        // (Optional) Award skill points or unlock new base features here
        // SkillManager.Instance.AddSkillPoints(1);
    }

    /// <summary>
    /// Links Player Level to the Main Quest Progress to trigger story events.
    /// This is an estimate based on your Level Flow design (L80+ is final stage).
    /// </summary>
    private void UpdateGameProgressBasedOnLevel()
    {
        // Assume Level 100 is required for 100% progress. 
        // A simple linear mapping: Progress = (Current Level / Max Level) * 100
        const int MAX_LEVEL = 100;
        float progress = Mathf.Clamp((float)currentLevel / MAX_LEVEL, 0f, 1f) * 100f;
        
        GameManager.Instance.UpdateProgress(progress);
    }
}
