using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for LINQ usage

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    
    [Header("Generation Settings")]
    public int maxChunksToGenerate = 10;
    public float chunkSize = 50f; // The physical size of one chunk
    
    // A list of all ChunkData ScriptableObjects, loaded from Resources
    private List<ChunkData> allChunkData; 
    private List<GameObject> activeChunks = new List<GameObject>();
    
    void Start()
    {
        // Load all defined chunk data assets at startup
        allChunkData = Resources.LoadAll<ChunkData>("LevelData/Chunks").ToList();
        
        // Start the level generation process
        GenerateNewLevel();
    }

    public void GenerateNewLevel()
    {
        // 1. Clear previous level (optional, but good for memory)
        ClearCurrentLevel();
        
        // 2. Start with the initial chunk (e.g., the one with minProgressRequired = 0)
        ChunkData startChunk = GetViableChunk(0f);
        if (startChunk == null) return;
        
        // Instantiate the first chunk at the origin
        GameObject firstChunk = Instantiate(startChunk.chunkPrefab, Vector3.zero, Quaternion.identity, transform);
        activeChunks.Add(firstChunk);
        
        // 3. Iteratively add subsequent chunks
        for (int i = 1; i < maxChunksToGenerate; i++)
        {
            float currentProgress = GameManager.Instance.currentMainQuestProgress;
            ChunkData nextChunkData = GetViableChunk(currentProgress);
            
            if (nextChunkData != null)
            {
                // This is where you connect the chunks using the connectionPoints
                ConnectNextChunk(nextChunkData, activeChunks.Last()); 
            }
        }
    }

    /// <summary>
    /// Selects a random chunk whose minimum required progress is met or exceeded.
    /// </summary>
    private ChunkData GetViableChunk(float progress)
    {
        // Filter the available chunks based on player progression
        List<ChunkData> viableChunks = allChunkData
            .Where(c => c.minProgressRequired <= progress)
            .ToList();

        if (viableChunks.Count == 0) return null;

        // Return a random chunk from the viable list
        return viableChunks[Random.Range(0, viableChunks.Count)];
    }

    /// <summary>
    /// Placeholder for the complex geometric logic of connecting chunk prefabs.
    /// </summary>
    private void ConnectNextChunk(ChunkData nextChunkData, GameObject previousChunk)
    {
        // ... (Logic to select available connection points on both prefabs) ...
        
        // Simple example: place the new chunk a fixed distance away
        Vector3 newPosition = previousChunk.transform.position + new Vector3(chunkSize, 0, 0);
        GameObject newChunk = Instantiate(nextChunkData.chunkPrefab, newPosition, Quaternion.identity, transform);
        activeChunks.Add(newChunk);
        
        // Trigger monster spawning within the new chunk
        SpawnMonsters(nextChunkData, newChunk.transform);
    }
    
    private void SpawnMonsters(ChunkData chunkData, Transform parent)
    {
        int monsterCount = Mathf.CeilToInt(4 + chunkData.minProgressRequired / 10f); // Scale count with progress
        
        for(int i = 0; i < monsterCount; i++)
        {
            // Select a random monster type from the chunkData's possibleMonsters list
            ScriptableMonsterData monsterType = chunkData.possibleMonsters[Random.Range(0, chunkData.possibleMonsters.Count)];
            
            // Instantiate the monster prefab at a random valid point within the chunk
            Vector3 spawnPos = parent.position + new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
            Instantiate(monsterType.monsterPrefab, spawnPos, Quaternion.identity, parent);
        }
    }

    private void ClearCurrentLevel()
    {
        foreach (GameObject chunk in activeChunks)
        {
            Destroy(chunk);
        }
        activeChunks.Clear();
    }
}
