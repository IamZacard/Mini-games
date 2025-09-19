using UnityEngine;
using System;

public class PlayerRangedAttack : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private PlayerController controller;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private ProjectileData defaultProjectile;

    private bool isAttacking;
    private float lastAttackTime;
    private float nextFireTime;

    // Events
    public event Action<ProjectileData> OnRangedAttackTriggered;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleInput();
        UpdateAttackState();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(1)) // Right Mouse Button
            TryShoot();
    }

    private void UpdateAttackState()
    {
        if (!isAttacking) return;

        if (IsAttackAnimationFinished())
        {
            isAttacking = false;
        }
    }

    private bool IsAttackAnimationFinished()
    {
        // Assuming ranged attack has 1 animation
        if (stats.rangedAttacks.rangedAttackClip == null)
            return true;

        float duration = stats.rangedAttacks.rangedAttackClip.length;
        return Time.time - lastAttackTime >= duration * 0.9f; // allow chaining slightly before end
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime || isAttacking)
            return;

        if (defaultProjectile == null || defaultProjectile.prefab == null)
        {
            Debug.LogWarning("No projectile assigned!");
            return;
        }

        PerformShoot(defaultProjectile);
    }

    private void PerformShoot(ProjectileData projectileData)
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        nextFireTime = Time.time + stats.rangedAttacks.rangedAttackCooldown;

        if (controller != null && controller.IsGrounded())
        {
            SetVelocityToZero();
        }
        

        // Trigger animator / event
        OnRangedAttackTriggered?.Invoke(projectileData);

        // Spawn projectile (Animation Event calls FireProjectile at the right frame)
        // If you don't want animation events: call FireProjectile() directly here.
    }

    public void FireProjectile()
    {
        if (defaultProjectile == null || defaultProjectile.prefab == null)
            return;

        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        GameObject projGO = Instantiate(defaultProjectile.prefab, projectileSpawnPoint.position, Quaternion.identity);
        Projectile proj = projGO.GetComponent<Projectile>();
        proj.Initialize(defaultProjectile, direction);
    }

    private void SetVelocityToZero()
    {
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    // Public API
    public bool IsAttacking() => isAttacking;
    public float GetTimeSinceLastAttack() => Time.time - lastAttackTime;
}
