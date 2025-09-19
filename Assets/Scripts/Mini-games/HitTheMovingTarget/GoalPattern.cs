using UnityEngine;

public enum GoalPatternType
{
    Linear,
    Teleport,
    SinWave,
    Circle,    
    Custom // for future
}

[CreateAssetMenu(menuName = "Data/GoalPattern")]
public class GoalPattern : ScriptableObject
{
    public string patternName = "New Pattern";
    public GoalPatternType type;

    [Header("General")]
    public float speed = 5f;                 // movement speed
    public Vector3[] points;                 // for Linear, SinWave, ZigZag
    public Vector3 defaultPoint = Vector3.zero; // point to return after hit

    [Header("SinWave / Circle")]
    public float amplitude = 1f;
    public float frequency = 1f;

    [Header("Teleport")]
    public float teleportInterval = 1f;
}
