using UnityEngine;

public class BaseEntranceTrigger : MonoBehaviour
{
    // This function is called when another object enters the trigger collider
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the object that entered the trigger is the Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the Base entrance area. Loading Home Base...");

            // 2. Execute the scene transition logic
            // This is the line that calls the method in the SceneLoader script:
            SceneLoader.Instance.GoToHomeBase();
        }
    }
}
