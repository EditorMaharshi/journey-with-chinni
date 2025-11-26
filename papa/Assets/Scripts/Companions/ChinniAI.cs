using UnityEngine;
using UnityEngine.AI;
using System;

public enum ChinniState { Following, Fighting, Healing, Leaving }

public class ChinniAI : MonoBehaviour
{
    // === Core Components ===
    public ChinniState currentState = ChinniState.Following;
    public NavMeshAgent agent;
    public Transform playerTransform;
    private PlayerCombat playerCombat; 

    // === Settings ===
    public float followDistance = 5f;
    public float fightRange = 3f;
    public float healThreshold = 50f; 
    public float healRate = 5f; // Health restored per second
    
    // === Vfx/Sfx ===
    public GameObject goldenLightEffectPrefab; 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            playerTransform = playerGO.transform;
            playerCombat = playerGO.GetComponent<PlayerCombat>();
        }
        
        // Listen for the emotional turning point
        GameManager.Instance.OnGameProgressReached80 += StartDeparture;
    }

    void Update()
    {
        if (playerTransform == null || playerCombat == null) return;

        switch (currentState)
        {
            case ChinniState.Following:
                FollowPlayer();
                CheckForDanger();
                CheckForHeal();
                break;
            case ChinniState.Fighting:
                // Auto-fight logic: move toward and attack nearest small monster
                // (Implementation omitted for brevity, focusing on state flow)
                currentState = ChinniState.Following; // Immediately return to following for simple logic
                break;
            case ChinniState.Healing:
                HealPlayer();
                break;
            case ChinniState.Leaving:
                // Wait for departure cinematic logic to finish
                break;
        }
    }

    void FollowPlayer()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance > followDistance)
        {
            agent.SetDestination(playerTransform.position);
        }
        else
        {
            agent.ResetPath(); 
        }
    }

    void CheckForDanger()
    {
        // Simple proximity check for small monsters
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, fightRange, LayerMask.GetMask("Enemy"));
        if (hitColliders.Length > 0)
        {
            currentState = ChinniState.Fighting;
        }
    }

    void CheckForHeal()
    {
        if (playerCombat.currentHealth < (playerCombat.maxHealth * healThreshold / 100f))
        {
            currentState = ChinniState.Healing;
        }
    }

    void HealPlayer()
    {
        agent.ResetPath();
        
        // Logic to restore player health over time
        playerCombat.Heal(healRate * Time.deltaTime); 
        
        if (playerCombat.currentHealth >= playerCombat.maxHealth)
        {
            currentState = ChinniState.Following;
        }
    }

    // === Emotional Turning Point ===
    void StartDeparture()
    {
        currentState = ChinniState.Leaving;
        Debug.Log("Chinni is starting her departure sequence.");
        StartCoroutine(DepartSequence());
    }

    IEnumerator DepartSequence()
    {
        // Delay for dialogue/cutscene to play
        yield return new WaitForSeconds(3f); 

        if(goldenLightEffectPrefab != null) 
            Instantiate(goldenLightEffectPrefab, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
        GameManager.Instance.OnGameProgressReached80 -= StartDeparture;
        Debug.Log("Chinni has left the player.");
    }
}
