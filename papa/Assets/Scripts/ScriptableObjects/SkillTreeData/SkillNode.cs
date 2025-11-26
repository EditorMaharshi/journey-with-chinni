using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SkillNode
{
    [Header("Identity")]
    public string skillID; // Unique ID (e.g., "PC1_HeavySwing")
    public string skillName;
    [TextArea(3, 5)]
    public string description;
    public int maxLevel = 1;

    [Header("Requirements & Costs")]
    public int requiredPlayerLevel = 1;
    public int costMagicResearchPoints = 100;

    [Header("Effect & Buffs")]
    // The type of stat this skill modifies
    public SkillEffectType effectType; 
    // The value added per level (e.g., +10 to Damage, or +0.1f to Speed)
    public float baseValueIncrease; 

    [Header("Tree Structure")]
    // IDs of the skills that must be unlocked BEFORE this skill
    public List<string> prerequisites = new List<string>(); 
}

// Define the types of stats the skill tree can affect
public enum SkillEffectType
{
    None,
    PlayerDamage,
    PlayerMaxHealth,
    PlayerMovementSpeed,
    WorkerEfficiencyBoost, 
    WorkerPromotionRate,
    ChinniHealRate,
    BaseDefenseStrength
}
