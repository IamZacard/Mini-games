using UnityEngine;
public class DashController : MonoBehaviour
{
    private PlayerData data;
    private Rigidbody2D rb;
    private Transform playerTransform;
    public bool IsDashing { get; private set; }
    public float LastDashTime { get; private set; }
    private float dashTimer;
    public void Initialize(PlayerData data, Rigidbody2D rb, Transform playerTransform)
    {
        this.data = data;
        this.rb = rb;
        this.playerTransform = playerTransform;
    }
    public bool CanDash() => Time.time - LastDashTime >= data.dashCooldown;
    public void StartDash()
    {
        IsDashing = true;
        dashTimer = data.dashDuration;
        LastDashTime = Time.time;
        Vector2 dashDir = new Vector2(playerTransform.localScale.x, 0);
        rb.linearVelocity = dashDir * data.dashForce;
    }
    public void UpdateDash()
    {
        if (!IsDashing) return;
        if (dashTimer > 0f)
        {
            Vector2 dashDir = new Vector2(playerTransform.localScale.x, 0);
            rb.linearVelocity = new Vector2(dashDir.x * data.dashForce, rb.linearVelocity.y);
        }
        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f)
            IsDashing = false;
    }
}