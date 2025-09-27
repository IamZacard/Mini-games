using UnityEngine;
public class GroundDetector : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D playerCollider;
    private PlayerData data;
    public bool IsGrounded { get; private set; }
    public float LastGroundedTime { get; private set; }
    public void Initialize(PlayerData data, CapsuleCollider2D collider)
    {
        this.data = data;
        this.playerCollider = collider;
    }
    public void CheckGround()
    {
        Bounds bounds = playerCollider.bounds;
        Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, 0.1f);
        Vector2 boxCenter = new Vector2(bounds.center.x, bounds.min.y - 0.05f);
        RaycastHit2D groundHit = Physics2D.BoxCast(
        boxCenter,
        boxSize,
        0f,
        Vector2.down,
        data.groundCheckDistance,
        data.groundLayer
        );
        bool wasGrounded = IsGrounded;
        IsGrounded = groundHit.collider != null;
        if (IsGrounded && !wasGrounded)
        {
            LastGroundedTime = Time.time;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (playerCollider != null)
        {
            Gizmos.color = Color.red;
            Bounds bounds = playerCollider.bounds;

            Vector3 boxCenter = new Vector3(bounds.center.x, bounds.min.y - 0.05f, 0f);
            Vector3 boxSize = new Vector3(bounds.size.x * 0.9f, 0.1f, 0f);

            Gizmos.DrawWireCube(boxCenter + Vector3.down * data.groundCheckDistance * 0.5f, boxSize);
        }
    }
}