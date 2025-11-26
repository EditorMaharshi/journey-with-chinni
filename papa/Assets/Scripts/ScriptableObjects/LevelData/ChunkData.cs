using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewChunk", menuName = "LevelData/Biome Chunk")]
public class ChunkData : ScriptableObject
{
    [Header("Chunk Identity")]
    // The prefab containing the terrain mesh, trees, paths, etc.
    public GameObject chunkPrefab; 
    public string biomeType; // e.g., "ShallowJungle", "ThickRuins", "DarkForestCore"
    
    [Header("Generation Parameters")]
    [Tooltip("The minimum player progress (0-100) required for this chunk to appear.")]
    [Range(0f, 100f)]
    public float minProgressRequired = 0f;
    
    [Tooltip("List of Monster Types that can spawn in this chunk.")]
    public List<ScriptableMonsterData> possibleMonsters;
    
    [Tooltip("Points on the chunk prefab where other chunks can connect.")]
    public List<Transform> connectionPoints; 
    
    [Tooltip("Location on the chunk for a major feature (e.g., a Treasure Hint Room).")]
    public Transform majorFeatureSpawnPoint;
}
