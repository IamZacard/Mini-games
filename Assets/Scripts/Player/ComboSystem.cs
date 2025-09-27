using UnityEngine;
using System;
public class ComboSystem : MonoBehaviour
{
    private PlayerData data;
    private int comboIndex;
    private float lastAttackTime;
    private float comboWindowEndTime;
    // Combo settings moved to PlayerData if needed, but hardcoded for now as per original
    private float comboResetTime = 5f;
    private float comboContinueWindow = 1.5f;
    public event Action<int> OnComboAdvanced;
    public void Initialize(PlayerData data)
    {
        this.data = data;
    }
    public int GetNextComboIndex() => comboIndex;
    public void AdvanceCombo()
    {
        comboIndex = (comboIndex + 1) % data.meleeAttacks.Length;
        OnComboAdvanced?.Invoke(comboIndex);
    }
    public void UpdateCombo()
    {
        // Reset combo if too much time passed since last attack
        if (Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
        // Reset combo if we missed the continue window
        if (Time.time > comboWindowEndTime && comboIndex > 0)
        {
            ResetCombo();
        }
    }
    public void StartComboWindow()
    {
        lastAttackTime = Time.time;
        comboWindowEndTime = Time.time + comboContinueWindow;
    }
    public void ResetCombo()
    {
        comboIndex = 0;
    }
    public bool IsInComboWindow() => Time.time <= comboWindowEndTime;
    public float TimeSinceLastAttack() => Time.time - lastAttackTime;
}