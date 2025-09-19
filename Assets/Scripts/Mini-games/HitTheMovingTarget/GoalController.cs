using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GoalState
{
    Basic,
    PreGame,
    InGame
}


public class GoalController : MonoBehaviour
{
    [Header("Patterns")]
    public List<GoalPattern> patterns;

    [Header("Game Settings")]
    public int hitsToWin = 20;

    private int currentHits = 0;
    private GoalPattern currentPattern;
    private Tweener moveTweener;
    private Coroutine patternCoroutine;
    private GoalPatternType lastPatternType = GoalPatternType.Linear;

    private GoalState currentState = GoalState.Basic;

    private void Start()
    {
        if (patterns.Count > 0)
        {
            transform.position = patterns[0].defaultPoint;
        }

        //EnterState(GoalState.Basic);
        EnterState(GoalState.InGame);
    }

    // === STATE MACHINE ===
    private void EnterState(GoalState newState)
    {
        ExitState(currentState);
        currentState = newState;

        switch (newState)
        {
            case GoalState.Basic:
                // Idle at default point
                StopCurrentPattern();
                if (patterns.Count > 0)
                    transform.position = patterns[0].defaultPoint;
                break;

            case GoalState.PreGame:
                StopCurrentPattern();
                // TODO: later cutscene, animations, scaling effects, etc.
                Debug.Log("Goal is in Pre-Game state");
                break;

            case GoalState.InGame:
                currentHits = 0;
                NextPattern();
                break;
        }
    }

    private void ExitState(GoalState state)
    {
        switch (state)
        {
            case GoalState.InGame:
                StopCurrentPattern();
                break;
        }
    }

    // === PUBLIC CONTROLS ===
    public void SetState(GoalState newState) => EnterState(newState);

    public void HitByPlayer()
    {
        if (currentState != GoalState.InGame) return;

        currentHits++;
        if (currentHits >= hitsToWin)
        {
            Debug.Log("Mini-game complete!");
            EnterState(GoalState.Basic);
            return;
        }

        transform.position = currentPattern.defaultPoint;
        NextPattern();
    }

    // === PATTERN LOGIC (unchanged, but gated to InGame) ===
    private void StopCurrentPattern()
    {
        if (moveTweener != null) moveTweener.Kill();
        if (patternCoroutine != null) StopCoroutine(patternCoroutine);
    }

    private void NextPattern()
    {
        StopCurrentPattern();

        if (patterns.Count == 0) return;

        var availablePatterns = patterns.Where(p => p.type != lastPatternType).ToList();
        if (availablePatterns.Count == 0)
            availablePatterns = patterns;

        currentPattern = availablePatterns[Random.Range(0, availablePatterns.Count)];
        lastPatternType = currentPattern.type;

        switch (currentPattern.type)
        {
            case GoalPatternType.Linear:
                if (currentPattern.points.Length > 0)
                {
                    transform.position = currentPattern.points[0];
                    moveTweener = transform.DOPath(
                        currentPattern.points,
                        currentPattern.points.Length / currentPattern.speed,
                        PathType.Linear
                    )
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.Linear);
                }
                break;

            case GoalPatternType.Teleport:
                patternCoroutine = StartCoroutine(TeleportRoutine());
                break;

            case GoalPatternType.SinWave:
                if (currentPattern.points.Length > 1)
                    patternCoroutine = StartCoroutine(SinWaveRoutine());
                break;

            case GoalPatternType.Circle:
                patternCoroutine = StartCoroutine(CircleRoutine());
                break;
        }
    }

    #region Coroutines
    private IEnumerator TeleportRoutine()
    {
        Vector3[] pts = currentPattern.points;
        while (true)
        {
            if (pts.Length == 0) yield break;

            int index = Random.Range(0, pts.Length);
            transform.position = pts[index];
            yield return new WaitForSeconds(currentPattern.teleportInterval);
        }
    }

    private IEnumerator SinWaveRoutine()
    {
        Vector3[] pts = currentPattern.points;
        int index = 0;
        Vector3 startPos = pts[index];
        float elapsed = 0f;

        while (true)
        {
            Vector3 target = pts[(index + 1) % pts.Length];
            float distance = Vector3.Distance(transform.position, target);
            float duration = distance / currentPattern.speed;
            elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 pos = Vector3.Lerp(startPos, target, t);
                pos.y += Mathf.Sin(t * Mathf.PI * 2f * currentPattern.frequency) * currentPattern.amplitude;
                transform.position = pos;
                yield return null;
            }

            index = (index + 1) % pts.Length;
            startPos = transform.position;
        }
    }

    private IEnumerator CircleRoutine()
    {
        Vector3 center = transform.position;
        float radius = currentPattern.amplitude;
        float angle = 0f;

        while (true)
        {
            angle += Time.deltaTime * currentPattern.speed;
            float rad = angle;
            Vector3 pos = center + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
            transform.position = pos;
            yield return null;
        }
    }
    #endregion

    public GoalState CurrentState => currentState;
    public int Hits => currentHits;
}
