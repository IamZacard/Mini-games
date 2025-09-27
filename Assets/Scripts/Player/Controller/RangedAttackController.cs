using UnityEngine;
using System;
public class RangedAttackController : MonoBehaviour
{
    [SerializeField] private ProjectileData defaultProjectile; // If still needed, assign in Inspector or move to data
    private PlayerData data;
    private Rigidbody2D rb;
    private MovementController movementController;
    private float nextFireTime;
    private float lastAttackTime;
    private Transform projectileSpawnPoint;
    public bool IsAttacking { get; private set; }
    // Events
    public event Action<RangedAttackData> OnAttackTriggered;
    public void Initialize(PlayerData data, Rigidbody2D rb, MovementController movementController, Transform projectileSpawnPoint)
    {
        this.data = data;
        this.rb = rb;
        this.movementController = movementController;
        this.projectileSpawnPoint = projectileSpawnPoint;
    }
    public void UpdateAttack()
    {
        if (!IsAttacking) return;
        if (IsAttackFinished())
        {
            IsAttacking = false;
        }
    }
    public bool TryAttack(bool attackPressed)
    {
        if (!attackPressed || Time.time < nextFireTime || IsAttacking)
            return false;
        if (data.rangedAttack.projectilePrefab == null)
            return false;
        PerformAttack();
        return true;
    }
    private void PerformAttack()
    {
        IsAttacking = true;
        lastAttackTime = Time.time;
        nextFireTime = Time.time + data.rangedAttack.cooldown;
        if (movementController.IsGrounded)
        {
            rb.linearVelocity = Vector2.zero;
        }
        OnAttackTriggered?.Invoke(data.rangedAttack);
        // FireProjectile() called from animation event
    }
    public void FireProjectile()
    {
        if (data.rangedAttack.projectilePrefab == null)
            return;
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        GameObject projGO = Instantiate(data.rangedAttack.projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        Projectile proj = projGO.GetComponent<Projectile>();
        proj.Initialize(defaultProjectile, direction); // If defaultProjectile null, use data or remove
    }
    private bool IsAttackFinished()
    {
        if (data.rangedAttack.animationClip == null) return true;
        float duration = data.rangedAttack.animationClip.length;
        return Time.time - lastAttackTime >= duration * 0.9f;
    }
}