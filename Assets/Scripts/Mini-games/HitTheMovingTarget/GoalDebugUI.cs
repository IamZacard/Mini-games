using TMPro;
using UnityEngine;

public class GoalDebugUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro debugText;
    [SerializeField] private GoalController goal;

    private string currentState;
    private int currentHits;

    private void Awake()
    {
        if (debugText == null)
            debugText = GetComponentInChildren<TextMeshPro>();
    }

    private void Update()
    {
        if (goal != null)
        {
            currentState = goal.CurrentState.ToString();
            currentHits = goal.Hits;
        }

        debugText.text =
            $"State: {currentState}\n" +
            $"Hits: {currentHits}";
    }
}
