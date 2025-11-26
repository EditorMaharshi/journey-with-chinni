using UnityEngine;

public class BossAI_GreatBeastLord : MonoBehaviour
{
    [Header("Boss Stats")]
    public float maxHealth = 5000f;
    private float currentHealth;
    public int attackDamage = 50;

    [Header("Boss Phases")]
    public float phaseTwoThreshold = 60f; // 60% health
    public float phaseThreeThreshold = 30f; // 30% health
    private int currentPhase = 1;

    [Header("Ending Reference")]
    public ChinniAI chinniPrefab; // Used to instantiate Chinni for the ending
    public FinalCinematicTrigger finalTrigger;

    void Start()
    {
        currentHealth = maxHealth;
        // UIManager.Instance.ShowBossHealthBar(maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        // UIManager.Instance.UpdateBossHealthBar(currentHealth);

        if (currentHealth <= 0)
        {
            DefeatBoss();
        }
        else
        {
            CheckPhaseTransition();
        }
    }

    private void CheckPhaseTransition()
    {
        float currentPct = (currentHealth / maxHealth) * 100f;

        if (currentPhase == 1 && currentPct <= phaseTwoThreshold)
        {
            currentPhase = 2;
            Debug.Log("BOSS: Entering Phase 2 - Unleashing new attack pattern!");
            // Trigger new attack animations/logic here
        }
        else if (currentPhase == 2 && currentPct <= phaseThreeThreshold)
        {
            currentPhase = 3;
            Debug.Log("BOSS: Entering FINAL Phase 3 - Desperate last stand!");
            // Trigger ultimate attack pattern
        }
    }

    private void DefeatBoss()
    {
        Debug.Log("BOSS DEFEATED! Curse is breaking.");
        // Play Boss death animation/VFX

        // 1. Trigger the curse break event
        GameManager.Instance.UpdateProgress(100f); // Set game progress to 100%
        CurseBreaker.Instance.BreakCurseAndFreeWorkers();
        
        // 2. Trigger the final scene sequence
        finalTrigger.StartEndingSequence();

        Destroy(gameObject); // Remove the defeated boss
    }
}
