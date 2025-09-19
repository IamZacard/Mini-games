using System.Collections.Generic;
using UnityEngine;

public enum ResourceType { Gold, Potion, Arrow }

public class Inventory : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    private Dictionary<ResourceType, int> resources = new();

    private void Start()
    {
        if (stats != null)
        {
            resources[ResourceType.Gold] = stats.startingGold;
        }
    }

    public void Add(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;
        resources[type] += amount;
    }

    public bool Spend(ResourceType type, int amount)
    {
        if (resources.TryGetValue(type, out int current) && current >= amount)
        {
            resources[type] -= amount;
            return true;
        }
        return false;
    }

    public int Get(ResourceType type) =>
        resources.TryGetValue(type, out int value) ? value : 0;
}
