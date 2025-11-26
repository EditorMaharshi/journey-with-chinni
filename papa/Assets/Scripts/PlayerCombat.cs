using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public int attackDamage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    
    private float lastAttackTime;

    void Start()
    {
        currentHealth = maxHealth;
        // Link to UI to show starting health
        // UIManager.Instance.UpdateHealth(currentHealth, maxHealth); 
    }

    public void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            
            // Play attack animation (e.g., swinging the wooden stick)
            // GetComponent<Animator>().SetTrigger("Attack"); 
            
            // Check for enemies within the attack radius
            Collider[] hitEnemies = Physics.OverlapSphere(transform.position + transform.forward * 0.5f, attackRange, LayerMask.GetMask("Enemy"));

            foreach (Collider enemy in hitEnemies)
            {
                // Try to get the MonsterCapture component (which has the TakeDamage method)
                MonsterCapture monster = enemy.GetComponent<MonsterCapture>();
                if (monster != null)
                {
                    monster.TakeDamage(attackDamage);
                    Debug.Log($"Player hit {enemy.name} for {attackDamage} damage.");
                }
            }
        }
    }

    // Called by monsters or traps when the player takes damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        // UIManager.Instance.UpdateHealth(currentHealth, maxHealth); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    // Called by ChinniAI.cs
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        // UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player has fallen. Game Over or Respawn logic here.");
        // GameManager.Instance.LoadLastCheckpoint();
    }
}
