using UnityEngine;

public interface IHealth
{
    int CurrentHealth { get; }
    int MaxHealth { get; }

    void TakeDamage(int amount, GameObject source = null);
    void Heal(int amount);
    void Kill();
}
