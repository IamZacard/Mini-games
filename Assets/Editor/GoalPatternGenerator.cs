using UnityEngine;
using UnityEditor;

public class GoalPatternGenerator : EditorWindow
{
    [Header("Settings")]
    public string folderPath = "Assets/GoalPatterns";
    public int numberOfPatterns = 40;

    [Header("Range Settings")]
    public Vector2 speedRange = new Vector2(2f, 5f);
    public Vector2 amplitudeRange = new Vector2(0.5f, 3f);
    public Vector2 frequencyRange = new Vector2(0.5f, 2f);
    public Vector2 teleportIntervalRange = new Vector2(0.5f, 2f);
    public int minPoints = 2;
    public int maxPoints = 5;

    [Header("Default Point Settings")]
    public Vector3 defaultPoint = new Vector3(14f, 5f, 0f);
    public float maxOffsetX = 4f;
    public float maxOffsetY = 4f;

    [MenuItem("Tools/Goal Pattern Generator")]
    public static void ShowWindow()
    {
        GetWindow<GoalPatternGenerator>("Goal Pattern Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Goal Pattern Generator", EditorStyles.boldLabel);

        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
        numberOfPatterns = EditorGUILayout.IntField("Number of Patterns", numberOfPatterns);

        if (GUILayout.Button("Generate Patterns"))
        {
            GeneratePatterns();
        }
    }

    private void GeneratePatterns()
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets", "GoalPatterns");

        for (int i = 0; i < numberOfPatterns; i++)
        {
            GoalPattern pattern = ScriptableObject.CreateInstance<GoalPattern>();
            pattern.patternName = $"Pattern_{i + 1}";
            pattern.speed = Random.Range(speedRange.x, speedRange.y);
            pattern.defaultPoint = defaultPoint;

            // Randomly pick a type
            pattern.type = (GoalPatternType)Random.Range(0, 5); // Linear, Teleport, SinWave, Circle, ZigZag

            int pointsCount = Random.Range(minPoints, maxPoints + 1);
            pattern.points = new Vector3[pointsCount];

            for (int j = 0; j < pointsCount; j++)
            {
                float offsetX = Random.Range(-maxOffsetX, maxOffsetX);
                float offsetY = Random.Range(-maxOffsetY, maxOffsetY);
                pattern.points[j] = defaultPoint + new Vector3(offsetX, offsetY, 0f);
            }

            pattern.amplitude = Random.Range(amplitudeRange.x, amplitudeRange.y);
            pattern.frequency = Random.Range(frequencyRange.x, frequencyRange.y);
            pattern.teleportInterval = Random.Range(teleportIntervalRange.x, teleportIntervalRange.y);

            AssetDatabase.CreateAsset(pattern, $"{folderPath}/GoalPattern_{i + 1}.asset");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"{numberOfPatterns} Goal Patterns generated in {folderPath}");
    }
}
