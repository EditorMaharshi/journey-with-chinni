using UnityEngine;
using System.Collections;

public class FinalCinematicTrigger : MonoBehaviour
{
    [Header("Cinematic Settings")]
    public float chinniReturnDelay = 3f;
    public GameObject empireRisesVFX; 

    // Called by the BossAI_GreatBeastLord upon defeat
    public void StartEndingSequence()
    {
        StartCoroutine(EndingSequenceCoroutine());
    }

    private IEnumerator EndingSequenceCoroutine()
    {
        // === Step 1: Chinni Reappears ===
        yield return new WaitForSeconds(chinniReturnDelay);
        Debug.Log("Ending Cinematic: Chinni returns.");
        
        // Re-spawn Chinni or make her existing object active
        // Instantiate(chinniPrefab, PlayerPosition, Quaternion.identity); 
        
        // Chinni's final dialogue
        // UIManager.Instance.ShowDialogue("You have earned the treasure — a kingdom built with strength and mercy.");
        yield return new WaitForSeconds(4f); // Wait for dialogue

        // === Step 2: Empire Rises ===
        // Instantiate the final VFX/model representing the empire rising behind the player
        if(empireRisesVFX != null)
        {
            // Instantiate at the location where the final base will be shown
            Instantiate(empireRisesVFX, Vector3.zero, Quaternion.identity); 
        }

        yield return new WaitForSeconds(5f); // Wait for cinematic finish

        // === Step 3: Fade Out and End Game ===
        // UIManager.Instance.FadeToBlack();
        Debug.Log("FADE OUT: Game Completed.");
        
        // Final action: transition to a dedicated 'Empire Mode' scene or main menu
        // SceneLoader.Instance.LoadScene("S03_EmpireMode");
    }
}
