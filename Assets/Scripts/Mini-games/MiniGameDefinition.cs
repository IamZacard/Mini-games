using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameDefinition", menuName = "Festival/MiniGame")]
public class MiniGameDefinition : ScriptableObject
{
    public string miniGameName;
    [TextArea] public string description;

    public int entryCost;
    public int rewardGold;

    [Header("Launch Settings")]
    public MiniGameLaunchType launchType;

    [Tooltip("Used if launchType = LoadScene")]
    public string sceneName;

    [Tooltip("Used if launchType = SpawnPrefab")]
    public GameObject prefab;

    [Tooltip("Used if launchType = ChangeState")]
    public string stateId; // or enum, depending on how your GameState system works
}

public enum MiniGameLaunchType
{
    LoadScene,
    SpawnPrefab,
    ChangeState
}