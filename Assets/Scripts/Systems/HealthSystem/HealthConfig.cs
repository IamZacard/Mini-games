using UnityEngine;

[CreateAssetMenu(menuName = "Stats/HealthConfig")]
public class HealthConfig : ScriptableObject
{
    [Header("Base Stats")]
    public int maxHealth = 100;
    public int armor = 0;          // reduces damage
    public bool canRegen = false;
    public float regenRate = 0f;   // hp/sec
}
