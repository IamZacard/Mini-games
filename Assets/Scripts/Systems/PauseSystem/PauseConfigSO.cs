using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PauseConfig", menuName = "Pause System/Pause Config")]
public class PauseConfigSO : ScriptableObject
{
    [System.Serializable]
    public class PauseGroup
    {
        public string groupName;  // e.g., "MovementOnly"
        public List<IPausable> pausables = new();  // Assign in inspector
        public int priority = 1;  // Higher = overrides lower (for stacking)
    }

    public List<PauseGroup> pauseGroups = new();
}