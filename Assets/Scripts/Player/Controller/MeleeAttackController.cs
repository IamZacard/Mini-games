using UnityEngine;
using System;
public class MeleeAttackController : MonoBehaviour
{
    private PlayerData data;
    private ComboSystem comboSystem;
    private Rigidbody2D rb;
    private float nextAttackTime;
    private PlayerAttackHitbox hitbox;
    public bool IsAttacking { get; private set; }
    // Events
    public event Action<AttackData> OnAttackTriggered;
    public void Initialize(PlayerData data, Rigidbody2D rb, ComboSystem comboSystem, PlayerAttackHitbox hitbox)
    {
        this.data = data;
        this.rb = rb;
        this.comboSystem = comboSystem;
        this.hitbox = hitbox;
    }
    public void UpdateAttack()
    {
        comboSystem.UpdateCombo();
        if (!IsAttacking) return;
        if (IsCurrentAttackFinished())
        {
            IsAttacking = false;
            hitbox.Deactivate();
            comboSystem.StartComboWindow();
        }
    }
    public bool TryAttack(bool attackPressed)
    {
        if (!attackPressed || Time.time < nextAttackTime || data.meleeAttacks == null || data.meleeAttacks.Length == 0)
            return false;
        // If combo window expired, reset combo
        if (!comboSystem.IsInComboWindow())
            comboSystem.ResetCombo();
        int index = comboSystem.GetNextComboIndex();
        var attackData = data.meleeAttacks[index];
        if (attackData.animationClip == null) return false;
        PerformAttack(attackData);
        return true;
    }
    private void PerformAttack(AttackData attackData)
    {
        IsAttacking = true;
        comboSystem.StartComboWindow(); // Update last attack time early
                                        // Handle movement behavior
        HandleMovementBehavior(attackData);
        hitbox.SetAttack(attackData);
        OnAttackTriggered?.Invoke(attackData);
        nextAttackTime = Time.time + attackData.cooldown;
        comboSystem.AdvanceCombo();
    }
    private void HandleMovementBehavior(AttackData attackData)
    {
        bool isGrounded = GetComponent<MovementController>().IsGrounded;
        switch (attackData.movementBehavior)
        {
            case MovementBehavior.StopOnGround:
                if (isGrounded) rb.linearVelocity = Vector2.zero;
                break;
            case MovementBehavior.StopAlways:
                rb.linearVelocity = Vector2.zero;
                break;
            case MovementBehavior.LockMovement:
                // Handled in state (no input)
                break;
            case MovementBehavior.ReduceSpeed:
                rb.linearVelocity *= attackData.movementModifier;
                break;
            case MovementBehavior.CustomVelocity:
                rb.linearVelocity = new Vector2(attackData.movementModifier * Mathf.Sign(transform.localScale.x), rb.linearVelocity.y);
                break;
        }
    }
    private bool IsCurrentAttackFinished()
    {
        int index = comboSystem.GetNextComboIndex() == 0 ? data.meleeAttacks.Length - 1 : comboSystem.GetNextComboIndex() - 1;
        var currentAttack = data.meleeAttacks[index];
        if (currentAttack.animationClip == null) return true;
        float duration = currentAttack.animationClip.length;
        return comboSystem.TimeSinceLastAttack() >= duration * 0.9f;
    }
    // Animation Events
    public void ActivateHitbox() => hitbox?.Activate();
    public void DeactivateHitbox() => hitbox?.Deactivate();
}