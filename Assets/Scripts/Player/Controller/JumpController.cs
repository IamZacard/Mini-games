using UnityEngine;
public class JumpController : MonoBehaviour
{
    private PlayerData data;
    private Rigidbody2D rb;
    private GroundDetector groundDetector;
    public bool IsJumping { get; private set; }
    public bool HasReachedApex { get; private set; }
    public bool WasJumpInitiated { get; private set; }
    public float CurrentAirTime { get; private set; }
    public float MaxFallSpeedThisAir { get; private set; }
    public void Initialize(PlayerData data, Rigidbody2D rb, GroundDetector groundDetector)
    {
        this.data = data;
        this.rb = rb;
        this.groundDetector = groundDetector;
    }
    public void UpdateAirTime()
    {
        if (!groundDetector.IsGrounded)
        {
            CurrentAirTime += Time.deltaTime;
            if (rb.linearVelocity.y < MaxFallSpeedThisAir)
                MaxFallSpeedThisAir = rb.linearVelocity.y;
        }
        else
        {
            CurrentAirTime = 0f;
            MaxFallSpeedThisAir = 0f;
            WasJumpInitiated = false;
        }
    }
    public void TryJump()
    {
        float jumpVelocity = Mathf.Sqrt(2f * -Physics2D.gravity.y * data.gravityScale * data.jumpHeight);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        IsJumping = true;
        WasJumpInitiated = true;
        HasReachedApex = false;
    }
    public void UpdateApex()
    {
        if (rb.linearVelocity.y > 0.1f)
            HasReachedApex = false;
        if (rb.linearVelocity.y < -0.1f)
            HasReachedApex = true;
        if (groundDetector.IsGrounded)
            IsJumping = false;
    }
    public bool ShouldHardLand()
    {
        bool hardLand = MaxFallSpeedThisAir <= data.hardLandMinSpeed || CurrentAirTime >= data.longFallMinAirTime;
        return hardLand;
    }
    public bool ShouldSoftLand()
    {
        bool softLand = (WasJumpInitiated && CurrentAirTime < data.quickLandMaxAirTime) ||
        (MaxFallSpeedThisAir >= data.softLandMaxSpeed && CurrentAirTime < data.quickLandMaxAirTime);
        return softLand;
    }
    public void ResetAirTracking()
    {
        CurrentAirTime = 0f;
        MaxFallSpeedThisAir = 0f;
    }
}