using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager Instance { get; private set; }

    [SerializeField] private Inventory Inventory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartMiniGame(MiniGameDefinition definition, Action onComplete = null)
    {
        Debug.Log($"Starting mini-game: {definition.miniGameName}");

        switch (definition.launchType)
        {
            case MiniGameLaunchType.LoadScene:
                if (!string.IsNullOrEmpty(definition.sceneName))
                    SceneManager.LoadScene(definition.sceneName, LoadSceneMode.Single);
                else
                    Debug.LogError("MiniGameDefinition missing sceneName.");
                break;

            case MiniGameLaunchType.SpawnPrefab:
                if (definition.prefab != null)
                    Instantiate(definition.prefab, Vector3.zero, Quaternion.identity);
                else
                    Debug.LogError("MiniGameDefinition missing prefab.");
                break;

            case MiniGameLaunchType.ChangeState:
                break;
        }

        // save onComplete callback if needed
    }

    public void FinishMiniGame(MiniGameDefinition definition, bool success)
    {
        Debug.Log($"Mini-game finished: {definition.miniGameName}, success: {success}");

        if (success)
        {
            Inventory.Add(ResourceType.Gold, definition.rewardGold);
        }

        // Return to festival hub scene
        SceneManager.LoadScene("FestivalHub", LoadSceneMode.Single);
    }
}
