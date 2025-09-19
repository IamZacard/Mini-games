using UnityEngine;
using System;

public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerAttackHitbox hitbox;

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTime = 5f; // Your 5 second requirement
    [SerializeField] private float comboContinueWindow = 1.5f; // Window to continue combo

    private int comboIndex;
    private float nextAttackTime;
    private bool isAttacking;
    private float lastAttackTime;
    private float comboWindowEndTime;

    // Events
    public event Action<AttackData> OnAttackTriggered;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        HandleInput();
        HandleComboReset();
        UpdateAttackState();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            TryAttack();
    }

    private void HandleComboReset()
    {
        // Reset combo if too much time passed since last attack
        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboIndex = 0;
        }

        // Reset combo if we missed the continue window and not currently attacking
        if (!isAttacking && Time.time > comboWindowEndTime && comboIndex > 0)
        {
            comboIndex = 0;
        }
    }

    private void UpdateAttackState()
    {
        if (!isAttacking) return;

        // Check if current attack animation finished
        if (IsCurrentAttackFinished())
        {
            isAttacking = false;
            hitbox.Deactivate();

            // Set combo continue window
            comboWindowEndTime = Time.time + comboContinueWindow;
        }
    }

    private bool IsCurrentAttackFinished()
    {
        if (stats?.attacks == null || comboIndex >= stats.attacks.Length)
            return true;

        var currentAttack = stats.attacks[comboIndex == 0 ? stats.attacks.Length - 1 : comboIndex - 1];
        if (currentAttack.animationClip == null)
            return true;

        // Check if enough time passed for the animation to complete
        float animationDuration = currentAttack.animationClip.length;
        return Time.time - lastAttackTime >= animationDuration * 0.9f; // 90% completion
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime || stats?.attacks == null || stats.attacks.Length == 0)
            return;

        // If combo window expired and we're not attacking, reset combo
        if (!isAttacking && Time.time > comboWindowEndTime)
            comboIndex = 0;

        var attackData = stats.attacks[comboIndex];
        if (attackData.animationClip == null) return;

        PerformAttack(attackData);
    }

    private void PerformAttack(AttackData attackData)
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        // Stop movement immediately when attack starts
        SetVelocityToZero();

        hitbox.SetAttack(attackData);

        // Trigger the attack event (PlayerAttackAnimator listens to this)
        OnAttackTriggered?.Invoke(attackData);

        nextAttackTime = Time.time + attackData.meleeAttackCooldown;

        // Advance combo (0->1->2->0)
        comboIndex = (comboIndex + 1) % stats.attacks.Length;
    }

    private void SetVelocityToZero()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    public void SetVelo0() => SetVelocityToZero();

    // Animation Events (called directly from Animation Timeline)
    public void ActivateHitbox() => hitbox?.Activate();
    public void DeactivateHitbox() => hitbox?.Deactivate();

    // Public getters
    public bool IsAttacking() => isAttacking;
    public int GetCurrentComboIndex() => comboIndex;
    public float GetTimeSinceLastAttack() => Time.time - lastAttackTime;
    public bool IsInComboWindow() => Time.time <= comboWindowEndTime;
}