using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent agent;
    private Transform playerTarget;
    private MonsterCapture monsterCapture;

    [Header("Behavior Settings")]
    public float sightRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    
    private float lastAttackTime;
    public enum AIState { Patrol, Chase, Attack }
    public AIState currentState = AIState.Patrol;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        monsterCapture = GetComponent<MonsterCapture>();
        playerTarget = GameObject.FindWithTag(Constants.TAG_PLAYER)?.transform;
    }

    void Update()
    {
        if (playerTarget == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        bool playerInSight = distanceToPlayer <= sightRange;
        bool playerInAttackRange = distanceToPlayer <= attackRange;

        if (playerInSight && !playerInAttackRange)
        {
            currentState = AIState.Chase;
        }
        else if (playerInSight && playerInAttackRange)
        {
            currentState = AIState.Attack;
        }
        else
        {
            currentState = AIState.Patrol;
        }

        ExecuteState(currentState);
    }

    void ExecuteState(AIState state)
    {
        switch (state)
        {
            case AIState.Chase:
                agent.SetDestination(playerTarget.position);
                break;
            case AIState.Attack:
                AttackPlayer();
                break;
            case AIState.Patrol:
                // Implement Patrol/Wander logic here
                break;
        }
    }

    void AttackPlayer()
    {
        agent.ResetPath(); 
        
        // Face the player
        Vector3 direction = (playerTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            
            PlayerCombat pc = playerTarget.GetComponent<PlayerCombat>();
            if (pc != null)
            {
                // NOTE: Using baseDamage from the monster's loaded Scriptable Data is necessary here
                pc.TakeDamage(10); // Placeholder damage value
            }
        }
    }
}
