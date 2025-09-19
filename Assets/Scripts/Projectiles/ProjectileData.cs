using UnityEngine;

[CreateAssetMenu(menuName = "Data/Projectile")]
public class ProjectileData : ScriptableObject
{
    [Header("General")]
    public string projectileName = "New Projectile";
    public Sprite sprite;
    public GameObject prefab;

    [Header("Stats")]
    public float speed = 15f;
    public float lifetime = 3f;
    public float damage = 10f;
    public bool pierce = false;   // keep going after hitting an enemy?
}
