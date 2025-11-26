using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewSkillTree", menuName = "GameData/Skill Tree")]
public class ScriptableSkillTree : ScriptableObject
{
    [Header("Tree Definition")]
    public string treeName = "Player Combat and Empire Progression";

    [Tooltip("List of all individual skills (nodes) in the entire tree.")]
    public List<SkillNode> allNodes = new List<SkillNode>();
    
    // Dictionary for quick lookup at runtime
    private Dictionary<string, SkillNode> nodeLookup;

    /// <summary>
    /// Builds a dictionary for O(1) lookup speed for skill nodes by their ID.
    /// </summary>
    public void BuildLookup()
    {
        if (nodeLookup != null) return;
        
        nodeLookup = new Dictionary<string, SkillNode>();
        foreach (SkillNode node in allNodes)
        {
            if (!nodeLookup.ContainsKey(node.skillID))
            {
                nodeLookup.Add(node.skillID, node);
            }
            else
            {
                Debug.LogError($"Duplicate Skill ID found: {node.skillID}");
            }
        }
    }

    /// <summary>
    /// Retrieves a skill node definition by its ID.
    /// </summary>
    public SkillNode GetNodeByID(string id)
    {
        if (nodeLookup == null) BuildLookup();
        
        if (nodeLookup.TryGetValue(id, out SkillNode node))
        {
            return node;
        }
        return null;
    }
}
