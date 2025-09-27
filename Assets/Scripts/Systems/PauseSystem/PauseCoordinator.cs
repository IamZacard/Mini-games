using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class PauseCoordinator : MonoBehaviour
{
    public static PauseCoordinator Instance { get; private set; }

    [SerializeField] private PauseConfigSO config;
    private Dictionary<string, List<IPausable>> pauseGroups = new();
    private Dictionary<IPausable, int> pauseRefCounts = new();  // For stacking pauses

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshPausables();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (config != null)
            {
                foreach (var group in config.pauseGroups)
                {
                    pauseGroups[group.groupName] = group.pausables;
                }
            }
            FindAllPausables();
        }
        else
        {
            Destroy(gameObject);
        }

        if (config != null)
        {
            foreach (var group in config.pauseGroups)
            {
                pauseGroups[group.groupName] = group.pausables;
            }
        }
        // Optional: Auto-populate if SO empty (find IPausables in scene)
        FindAllPausables();
    }

    private void FindAllPausables()
    {
        var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var allPausables = allMonoBehaviours.OfType<IPausable>().ToList();

        // Auto-populate MovementOnly with player controllers
        if (pauseGroups.ContainsKey("MovementOnly") && pauseGroups["MovementOnly"].Count == 0)
        {
            //var playerControllers = allPausables.OfType<PlayerController>().Cast<IPausable>().ToList();
            //pauseGroups["MovementOnly"] = playerControllers;
            //Debug.Log($"Auto-populated MovementOnly with {playerControllers.Count} controllers");
        }

        if (!pauseGroups.ContainsKey("Default"))
            pauseGroups["Default"] = allPausables;
    }

    public void PauseGroup(string groupName)
    {
        // Refresh if group is empty (scene might have changed)
        if (!pauseGroups.ContainsKey(groupName) || pauseGroups[groupName].Count == 0)
        {
            RefreshPausables();
        }

        if (!pauseGroups.ContainsKey(groupName))
        {
            Debug.LogError($"PauseCoordinator: Group '{groupName}' not found!");
            return;
        }

        var groupPausables = pauseGroups[groupName];

        foreach (var pausable in groupPausables)
        {
            if (pausable == null || (pausable as MonoBehaviour) == null) continue;

            pauseRefCounts.TryGetValue(pausable, out int count);
            pauseRefCounts[pausable] = ++count;

            if (count == 1)
            {
                pausable.Pause();
            }
        }
    }

    public void RefreshPausables()
    {
        pauseGroups.Clear();
        pauseRefCounts.Clear();

        var allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var allPausables = allMonoBehaviours.OfType<IPausable>().ToList();

        // Filter out destroyed/null objects
        allPausables = allPausables.Where(p => p != null && (p as MonoBehaviour) != null).ToList();

        // Movement group (PlayerController)
        //var playerControllers = allPausables.OfType<PlayerController>().Cast<IPausable>().ToList();
        //pauseGroups["MovementOnly"] = playerControllers;

        // Default group (everything)
        pauseGroups["Default"] = allPausables;

        //Debug.Log($"PauseCoordinator: Refreshed - MovementOnly: {playerControllers.Count}, Default: {allPausables.Count}");
    }

    public void ResumeGroup(string groupName)
    {
        if (!pauseGroups.ContainsKey(groupName)) return;

        foreach (var pausable in pauseGroups[groupName])
        {
            if (!pauseRefCounts.ContainsKey(pausable)) continue;

            pauseRefCounts[pausable]--;

            if (pauseRefCounts[pausable] <= 0)  // Last resume triggers
            {
                pausable.Resume();
                pauseRefCounts.Remove(pausable);
            }
        }
    }

    public bool IsGroupPaused(string groupName)
    {
        if (!pauseGroups.ContainsKey(groupName)) return false;
        return pauseGroups[groupName].Any(p => p.IsPaused);
    }
}