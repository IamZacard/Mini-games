using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] private HealthConfig config;

    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => config != null ? config.maxHealth : 0;

    [Header("Events")]
    public UnityEvent<int, GameObject> OnDamaged;  // damage, source
    public UnityEvent<int> OnHealed;               // amount
    public UnityEvent<GameObject> OnDeath;         // killer

    private void Awake()
    {
        if (config == null)
        {
            Debug.LogError($"{name} has no HealthConfig assigned!");
            enabled = false;
            return;
        }
        currentHealth = config.maxHealth;
    }

    private void Update()
    {
        if (config.canRegen && currentHealth < MaxHealth)
        {
            currentHealth = Mathf.Min(
                MaxHealth,
                currentHealth + Mathf.CeilToInt(config.regenRate * Time.deltaTime)
            );
        }
    }

    public void TakeDamage(int amount, GameObject source = null)
    {
        if (amount <= 0) return;

        int effectiveDamage = Mathf.Max(amount - config.armor, 1);
        currentHealth -= effectiveDamage;

        OnDamaged?.Invoke(effectiveDamage, source);

        if (currentHealth <= 0)
        {
            Kill();
            OnDeath?.Invoke(source);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);
        OnHealed?.Invoke(amount);
    }

    public void Kill()
    {
        currentHealth = 0;
        Destroy(gameObject);

    }
}
