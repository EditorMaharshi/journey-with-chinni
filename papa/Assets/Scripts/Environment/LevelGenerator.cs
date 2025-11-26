using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

public class LevelGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    public int maxChunksToGenerate = 10;
    public float chunkSize = 50f; // The physical size of one chunk
    
    private List<ChunkData> allChunkData; 
    private List<GameObject> activeChunks = new List<GameObject>();
    
    void Start()
    {
        // Load all defined chunk data assets (Requires folder Resources/LevelData/Chunks)
        allChunkData = Resources.LoadAll<ChunkData>("LevelData/Chunks").ToList();
    }

    public void GenerateNewLevel()
    {
        ClearCurrentLevel();
        
        ChunkData startChunk = GetViableChunk(0f);
        if (startChunk == null) return;
        
        GameObject firstChunk = Instantiate(startChunk.chunkPrefab, Vector3.zero, Quaternion.identity, transform);
        activeChunks.Add(firstChunk);
        
        for (int i = 1; i < maxChunksToGenerate; i++)
        {
            float currentProgress = GameManager.Instance.currentMainQuestProgress;
            ChunkData nextChunkData = GetViableChunk(currentProgress);
            
            if (nextChunkData != null)
            {
                ConnectNextChunk(nextChunkData, activeChunks.Last()); 
            }
        }
    }

    private ChunkData GetViableChunk(float progress)
    {
        List<ChunkData> viableChunks = allChunkData
            .Where(c => c.minProgressRequired <= progress)
            .ToList();

        if (viableChunks.Count == 0) return null;

        return viableChunks[UnityEngine.Random.Range(0, viableChunks.Count)];
    }

    private void ConnectNextChunk(ChunkData nextChunkData, GameObject previousChunk)
    {
        // Simple example: place the new chunk a fixed distance away
        Vector3 newPosition = previousChunk.transform.position + new Vector3(chunkSize, 0, 0);
        GameObject newChunk = Instantiate(nextChunkData.chunkPrefab, newPosition, Quaternion.identity, transform);
        activeChunks.Add(newChunk);
        
        SpawnMonsters(nextChunkData, newChunk.transform);
    }
    
    private void SpawnMonsters(ChunkData chunkData, Transform parent)
    {
        int monsterCount = Mathf.CeilToInt(4 + chunkData.minProgressRequired / 10f); 
        
        for(int i = 0; i < monsterCount; i++)
        {
            if (chunkData.possibleMonsters.Count == 0) continue;

            ScriptableMonsterData monsterType = chunkData.possibleMonsters[UnityEngine.Random.Range(0, chunkData.possibleMonsters.Count)];
            
            // Instantiate the monster prefab at a random valid point within the chunk
            Vector3 spawnPos = parent.position + new Vector3(UnityEngine.Random.Range(-20f, 20f), 0, UnityEngine.Random.Range(-20f, 20f));
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
