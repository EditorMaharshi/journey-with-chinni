using UnityEngine;

// Define the asset creation menu item in Unity's editor
[CreateAssetMenu(fileName = "NewMonsterData", menuName = "GameData/Monster Data")]
public class ScriptableMonsterData : ScriptableObject
{
    [Header("Identity")]
    public string monsterName = "Gloomling";
    public string monsterType = "SmallMonster"; 
    public GameObject monsterPrefab; // The visual model used in the jungle

    [Header("Combat Stats")]
    public int maxHealth = 50;
    public int baseDamage = 5;
    public float movementSpeed = 3.5f;
    public int experienceGained = 10;
    
    [Header("Worker Stats (Base Management)")]
    [Tooltip("The monster's inherent skill level for Woodcutting (1-100)")]
    [Range(1, 100)]
    public int woodcuttingEfficiency = 20;

    [Tooltip("The monster's inherent skill level for Forging (1-100)")]
    [Range(1, 100)]
    public int forgingEfficiency = 10;

    [Tooltip("The monster's inherent skill level for Research (1-100)")]
    [Range(1, 100)]
    public int researchEfficiency = 5;
    
    // Add other task efficiencies (Farming, Mining, etc.) here
}
