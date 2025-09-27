using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class MovementController : MonoBehaviour
{
    private PlayerData data;
    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;
    private GroundDetector groundDetector;
    private JumpController jumpController;
    private DashController dashController;
    private bool isRunningMode = true;
    public bool IsRunning => isRunningMode && Mathf.Abs(rb.linearVelocity.x) > 0.1f;
    public void Initialize(PlayerData data)
    {
        this.data = data;
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        rb.gravityScale = data.gravityScale;
        groundDetector = gameObject.AddComponent<GroundDetector>();
        groundDetector.Initialize(data, playerCollider);
        jumpController = gameObject.AddComponent<JumpController>();
        jumpController.Initialize(data, rb, groundDetector);
        dashController = gameObject.AddComponent<DashController>();
        dashController.Initialize(data, rb, transform);
    }
    public void UpdateMovement(float horizontalInput, bool toggleRun)
    {
        groundDetector.CheckGround();
        jumpController.UpdateAirTime();
        jumpController.UpdateApex();
        if (toggleRun)
            isRunningMode = !isRunningMode;
        if (dashController.IsDashing)
        {
            dashController.UpdateDash();
            return;
        }
        float currentMoveSpeed = isRunningMode ? data.runSpeed : data.walkSpeed;
        float targetSpeed = horizontalInput * currentMoveSpeed;
        float accel = data.acceleration;
        if (!groundDetector.IsGrounded)
            accel *= data.airAccelerationMultiplier;
        float step = (Mathf.Abs(horizontalInput) > 0.1f) ? accel : data.deceleration;
        step *= Time.fixedDeltaTime;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, step);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        if (horizontalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
    }
    public void TryJump(bool jumpPressed)
    {
        if (jumpPressed && (groundDetector.IsGrounded || Time.time - groundDetector.LastGroundedTime <= data.coyoteTime))
        {
            jumpController.TryJump();
        }
    }
    public void TryDash(bool dashPressed)
    {
        if (dashPressed && dashController.CanDash())
        {
            dashController.StartDash();
        }
    }
    // Expose properties
    public bool IsGrounded => groundDetector.IsGrounded;
    public bool IsJumping => jumpController.IsJumping;
    public bool HasReachedApex => jumpController.HasReachedApex;
    public float CurrentAirTime => jumpController.CurrentAirTime;
    public float MaxFallSpeed => jumpController.MaxFallSpeedThisAir;
    public bool WasJumpInitiated => jumpController.WasJumpInitiated;
    public bool IsDashing => dashController.IsDashing;
    public float LastDashTime => dashController.LastDashTime;
    public bool ShouldHardLand() => jumpController.ShouldHardLand();
    public bool ShouldSoftLand() => jumpController.ShouldSoftLand();
    public void ResetAirTracking() => jumpController.ResetAirTracking();
    public float GetLastGroundedTime() => groundDetector.LastGroundedTime;
}