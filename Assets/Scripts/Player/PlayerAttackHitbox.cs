using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private SpriteRenderer sr;

    private Collider2D col;
    private AttackData currentAttack;
    private Transform playerTransform;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;

        // Get player transform (parent or root)
        playerTransform = GetComponentInParent<PlayerMeleeAttack>()?.transform;
        if (playerTransform == null)
            playerTransform = transform.parent ?? transform;

        if (sr != null)
            sr.enabled = false;
    }

    public void SetAttack(AttackData attack)
    {
        currentAttack = attack;

        if (col is BoxCollider2D box)
        {
            box.size = attack.size;
            box.offset = attack.offset;
        }
    }

    public void Activate()
    {
        col.enabled = true;
        if (sr != null) sr.enabled = true;
    }

    public void Deactivate()
    {
        col.enabled = false;
        if (sr != null) sr.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;
        if (currentAttack == null) return;

        var health = other.GetComponent<IHealth>();
        if (health != null)
        {
            health.TakeDamage(currentAttack.meleeDamage, gameObject);

            // IMPROVED KNOCKBACK
            var rb = other.attachedRigidbody;
            if (rb != null && currentAttack.knockbackForce > 0f)
            {
                Vector2 knockbackDir = GetKnockbackDirection(other);
                rb.AddForce(knockbackDir * currentAttack.knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    private Vector2 GetKnockbackDirection(Collider2D target)
    {
        // Method 1: Use player's facing direction (most reliable for close combat)
        Vector2 playerFacing = new Vector2(playerTransform.localScale.x, 0f);

        // Method 2: Calculate from centers with fallback
        Vector2 targetCenter = target.bounds.center;
        Vector2 playerCenter = playerTransform.position;
        Vector2 dirFromPlayer = (targetCenter - playerCenter).normalized;

        // If direction is too weak (overlapping), use facing direction
        if (dirFromPlayer.magnitude < 0.1f || Mathf.Abs(dirFromPlayer.x) < 0.3f)
        {
            return playerFacing.normalized;
        }

        // Method 3: Hybrid approach - prefer horizontal knockback in player's facing direction
        Vector2 finalDir = new Vector2(
            Mathf.Sign(playerFacing.x), // Always knockback in facing direction
            dirFromPlayer.y * 0.3f      // Small upward component
        );

        return finalDir.normalized;
    }
}