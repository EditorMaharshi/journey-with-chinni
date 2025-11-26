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
    }

    public void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            
            // Check for enemies/resources within the attack radius
            Collider[] hitTargets = Physics.OverlapSphere(transform.position + transform.forward * 0.5f, attackRange); 

            foreach (Collider target in hitTargets)
            {
                // Hit a Monster?
                MonsterCapture monster = target.GetComponent<MonsterCapture>();
                if (monster != null)
                {
                    monster.TakeDamage(attackDamage);
                    continue; 
                }

                // Hit a Resource Node?
                ResourceNode node = target.GetComponent<ResourceNode>();
                if (node != null)
                {
                    node.Interact(1f); // Fixed modifier for basic stick
                }
            }
        }
    }

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
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        // UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void UpdateStatsAfterSkill()
    {
        // Call this after SkillManager updates attackDamage or maxHealth
        Debug.Log($"Player stats updated. New Damage: {attackDamage}");
        // UIManager.Instance.UpdateHealth(currentHealth, maxHealth); 
    }

    private void Die()
    {
        Debug.Log("Player has fallen.");
        // GameManager.Instance.LoadLastCheckpoint();
    }
}
