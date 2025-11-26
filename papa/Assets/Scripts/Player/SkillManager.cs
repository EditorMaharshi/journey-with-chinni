using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    
    [Header("Data References")]
    // Assign your ST_MainProgression Scriptable Object here
    public ScriptableSkillTree skillTreeData; 
    
    [Header("Progression Tracking")]
    // Key: Skill ID, Value: Current Level (Max 1 usually, higher for multi-level skills)
    private Dictionary<string, int> unlockedSkills = new Dictionary<string, int>();

    // === Dependencies ===
    private PlayerCombat playerCombat;
    private BaseManager baseManager; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Get references after systems are guaranteed to be initialized
        playerCombat = FindObjectOfType<PlayerCombat>();
        baseManager = BaseManager.Instance;
        
        if (skillTreeData != null)
        {
            skillTreeData.BuildLookup(); // Prepare the dictionary for fast lookups
        }
        
        // **TODO: Load unlockedSkills from GameManager.Instance.LoadGame()**
        
        // Apply initial buffs from any loaded skills
        ApplyAllActiveBuffs();
    }
    
    /// <summary>
    /// Attempts to unlock or level up a skill node.
    /// </summary>
    public bool UnlockSkill(string skillID)
    {
        if (skillTreeData == null) return false;

        ScriptableSkillNode node = skillTreeData.GetNodeByID(skillID);
        if (node == null)
        {
            Debug.LogError($"Skill ID not found: {skillID}");
            return false;
        }

        // 1. Check prerequisites and cost
        if (!CanUnlock(node))
        {
            Debug.Log("Cannot unlock skill: Prerequisites or cost not met.");
            return false;
        }

        // 2. Consume Resources
        // In a real implementation, you'd subtract the cost from the BaseManager resources:
        // bool success = baseManager.TrySpendResource(TaskType.Research, node.costMagicResearchPoints);
        // For now, we assume success:
        
        // 3. Update Tracking
        int currentLevel = unlockedSkills.ContainsKey(skillID) ? unlockedSkills[skillID] : 0;

        if (currentLevel < node.maxLevel)
        {
            unlockedSkills[skillID] = currentLevel + 1;
            
            // 4. Apply Buff
            ApplyBuff(node, 1); // Apply the buff for the new level
            
            // 5. Notify and Save
            GameManager.Instance.SaveGame();
            Debug.Log($"Skill Unlocked/Leveled: {node.skillName} to Level {unlockedSkills[skillID]}");
            return true;
        }
        
        Debug.Log("Skill is already at max level.");
        return false;
    }

    private bool CanUnlock(ScriptableSkillNode node)
    {
        // A. Check Resource Cost (TODO: Implement proper BaseManager resource checking)
        // if (baseManager.resources[TaskType.Research] < node.costMagicResearchPoints) return false;

        // B. Check Player Level Requirement
        // if (playerCombat.playerLevel < node.requiredPlayerLevel) return false; 
        
        // C. Check Prerequisites
        foreach (string prerequisiteID in node.prerequisites)
        {
            if (!unlockedSkills.ContainsKey(prerequisiteID))
            {
                return false; // Missing prerequisite skill
            }
        }
        
        return true;
    }

    /// <summary>
    /// Applies a specific buff to the player or base systems.
    /// </summary>
    private void ApplyBuff(ScriptableSkillNode node, int levelIncrease)
    {
        float value = node.baseValueIncrease * levelIncrease;
        
        switch (node.effectType)
        {
            case SkillEffectType.PlayerDamage:
                if (playerCombat != null) 
                    playerCombat.attackDamage += Mathf.RoundToInt(value);
                break;

            case SkillEffectType.PlayerMaxHealth:
                if (playerCombat != null) 
                    playerCombat.maxHealth += value;
                break;
                
            case SkillEffectType.WorkerEfficiencyBoost:
                // Global base boost, might affect BaseManager calculations
                // BaseManager.Instance.GlobalEfficiencyModifier += value; 
                break;

            // Add other cases for PlayerMovementSpeed, BaseDefenseStrength, etc.
            default:
                break;
        }
        
        // Force the player to update their health display, etc.
        playerCombat?.UpdateStatsAfterSkill();
    }
    
    // Call this on game load to ensure all buffs are active
    private void ApplyAllActiveBuffs()
    {
        foreach(var kvp in unlockedSkills)
        {
            ScriptableSkillNode node = skillTreeData.GetNodeByID(kvp.Key);
            if (node != null)
            {
                // Apply the full effect up to the current level
                ApplyBuff(node, kvp.Value); 
            }
        }
    }
}
