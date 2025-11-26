using UnityEngine;
using UnityEngine.UI;

public class WorkerUIEntry : MonoBehaviour
{
    // === UI Component References ===
    [Header("UI References")]
    public Text nameText;
    public Text levelText;
    public Text statusText; // Shows "Worker" or "Manager"
    public Button promoteButton;
    
    // === Task Assignment Buttons (Assign in Inspector) ===
    public Button assignWoodcutButton;
    public Button assignForgeButton;
    public Button assignResearchButton;

    // === Data Reference ===
    private WorkerMonster currentWorker; 
    
    // Call this method when the entry is instantiated in HUDManager.PopulateWorkerList()
    public void Setup(WorkerMonster worker)
    {
        currentWorker = worker;
        
        // 1. Initial Display
        nameText.text = worker.monsterType;
        levelText.text = $"Discipline Lvl: {worker.currentLevel}";
        
        // 2. Set Status and Promote Button
        if (worker.isManager)
        {
            statusText.text = "MANAGER";
            promoteButton.gameObject.SetActive(false); // Hide button if already a manager
        }
        else
        {
            statusText.text = $"TASK: {worker.assignedTask}";
            // Show promote button only if the promotion condition is met (e.g., Lvl 10)
            promoteButton.gameObject.SetActive(worker.currentLevel >= 10);
            promoteButton.onClick.RemoveAllListeners();
            promoteButton.onClick.AddListener(OnPromoteClicked);
        }

        // 3. Setup Task Buttons
        // Clear all listeners first to prevent duplicates
        assignWoodcutButton.onClick.RemoveAllListeners();
        assignForgeButton.onClick.RemoveAllListeners();
        assignResearchButton.onClick.RemoveAllListeners();

        // Add listeners for task assignment
        assignWoodcutButton.onClick.AddListener(() => AssignNewTask(TaskType.Woodcutting));
        assignForgeButton.onClick.AddListener(() => AssignNewTask(TaskType.Forging));
        assignResearchButton.onClick.AddListener(() => AssignNewTask(TaskType.Research));
        
        // Highlight the currently assigned task button
        HighlightCurrentTaskButton(worker.assignedTask);
    }

    private void AssignNewTask(TaskType newTask)
    {
        if (currentWorker == null || currentWorker.isManager) return;
        
        // 1. Tell the WorkerMonster script to change its job
        currentWorker.AssignTask(newTask);
        
        // 2. Update the UI entry
        statusText.text = $"TASK: {newTask}";
        HighlightCurrentTaskButton(newTask);
        
        // Close the Base UI or provide confirmation feedback
    }

    private void OnPromoteClicked()
    {
        if (currentWorker == null || currentWorker.currentLevel < 10) return;
        
        // Trigger the promotion logic in the Base Manager
        BaseManager.Instance.TryPromoteWorker(currentWorker);
        
        // Update the display for the new manager status
        Setup(currentWorker); 
        
        // The HUDManager will need to re-populate the list to correctly remove/move the entry
        // HUDManager.Instance.PopulateWorkerList(); 
    }
    
    private void HighlightCurrentTaskButton(TaskType currentTask)
    {
        // Simple visual feedback: you'll need to implement logic (e.g., changing button color/outline)
        // This is a placeholder for visual logic
        Color activeColor = Color.green;
        Color inactiveColor = Color.white;
        
        assignWoodcutButton.GetComponent<Image>().color = (currentTask == TaskType.Woodcutting) ? activeColor : inactiveColor;
        assignForgeButton.GetComponent<Image>().color = (currentTask == TaskType.Forging) ? activeColor : inactiveColor;
        assignResearchButton.GetComponent<Image>().color = (currentTask == TaskType.Research) ? activeColor : inactiveColor;
    }
}
