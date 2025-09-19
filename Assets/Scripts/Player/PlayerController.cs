using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerController : MonoBehaviour, IPausable
{
    [Header("References")]
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Transform groundCheck;

    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckWidth = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    private bool wasJumpInitiated;
    private float currentAirTime;
    private float maxFallSpeedThisAir;

    private bool isGrounded;
    private float lastGroundedTime;
    private bool isJumping;

    [Header("Landing Detection")]
    [SerializeField] private float softLandMaxSpeed = -8f;
    [SerializeField] private float hardLandMinSpeed = -18f;
    [SerializeField] private float quickLandMaxAirTime = 0.4f;
    [SerializeField] private float longFallMinAirTime = 1.2f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float landingBuffer = 0.05f;
    [SerializeField] private float airAccelerationMultiplier = 2f;
    private float lastLandingTime;

    [Header("Dash Settings")]
    [SerializeField] private float dashCooldown = 1.5f;
    private bool isDashing;
    private float dashTimer;
    private float lastDashTime;

    private float horizontalInput;
    private bool isRunningMode = true;
    private bool jumpPressed;
    private bool runPressed;
    private bool reachedApex;

    [Header("Pause State")]
    private bool isPaused;

    public bool IsPaused => isPaused;
    public event Action OnPausedChanged;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();

        if (stats != null)
            rb.gravityScale = stats.gravityScale;
    }

    private void Update()
    {
        if (isPaused) return;

        HandleGroundCheck();
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (isPaused) return;

        HandleMovement();
        HandleJump();
        UpdateJumpApex();
        HandleDash();
    }

    private void HandleGroundCheck()
    {
        Bounds bounds = playerCollider.bounds;

        Vector2 boxSize = new Vector2(bounds.size.x * 0.9f, 0.1f);
        Vector2 boxCenter = new Vector2(bounds.center.x, bounds.min.y - 0.05f);

        RaycastHit2D groundHit = Physics2D.BoxCast(
            boxCenter,
            boxSize,
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        bool wasGrounded = isGrounded;
        isGrounded = groundHit.collider != null;

        if (!isGrounded)
        {
            currentAirTime += Time.deltaTime;
            if (rb.linearVelocity.y < maxFallSpeedThisAir)
                maxFallSpeedThisAir = rb.linearVelocity.y;
        }

        if (isGrounded && !wasGrounded)
        {
            lastGroundedTime = Time.time;
            if (Time.time - lastLandingTime > landingBuffer)
            {
                isJumping = false;
                lastLandingTime = Time.time;
                wasJumpInitiated = false;
            }
        }

        if (!isGrounded && wasGrounded)
        {
            currentAirTime = 0f;
            maxFallSpeedThisAir = 0f;
            wasJumpInitiated = false;
        }
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunningMode = !isRunningMode;

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || Time.time - lastGroundedTime <= coyoteTime))
            jumpPressed = true;

        if (Input.GetKeyDown(KeyCode.LeftControl) && Time.time - lastDashTime >= dashCooldown)
            TryStartDash();
    }

    private void HandleMovement()
    {
        if (isDashing || stats == null) return;

        var meleeAttack = GetComponent<PlayerMeleeAttack>();
        if (meleeAttack != null && meleeAttack.IsAttacking()) return;

        var rangedAttack = GetComponent<PlayerRangedAttack>();
        if (isGrounded && rangedAttack != null && rangedAttack.IsAttacking()) return;


        float currentMoveSpeed = isRunningMode ? stats.runSpeed : stats.walkSpeed;
        float targetSpeed = horizontalInput * currentMoveSpeed;

        float accel = stats.acceleration;
        if (!isGrounded)
            accel *= airAccelerationMultiplier;

        float step = (Mathf.Abs(horizontalInput) > 0.1f) ? accel : stats.deceleration;
        step *= Time.fixedDeltaTime;

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, step);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        if (horizontalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
    }


    private void HandleJump()
    {
        if (jumpPressed && (isGrounded || Time.time - lastGroundedTime <= coyoteTime))
        {
            float jumpVelocity = Mathf.Sqrt(2f * -Physics2D.gravity.y * stats.gravityScale * stats.jumpHeight);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            isJumping = true;
            wasJumpInitiated = true;
            reachedApex = false;
        }
        jumpPressed = false;
    }

    private void UpdateJumpApex()
    {
        if (rb.linearVelocity.y > 0.1f)
            reachedApex = false;

        if (rb.linearVelocity.y < -0.1f)
            reachedApex = true;

        if (isGrounded)
            isJumping = false;
    }

    public bool ShouldHardLand()
    {
        // Hard land if fall speed exceeds threshold (more than 15 = faster than -15)
        bool hardLand = maxFallSpeedThisAir <= hardLandMinSpeed || currentAirTime >= longFallMinAirTime;
        Debug.Log($"ShouldHardLand: {hardLand} | MaxFallSpeed: {maxFallSpeedThisAir} | AirTime: {currentAirTime}");
        return hardLand;
    }

    public bool ShouldSoftLand()
    {
        bool softLand = wasJumpInitiated && currentAirTime < quickLandMaxAirTime && maxFallSpeedThisAir >= softLandMaxSpeed;
        Debug.Log($"ShouldSoftLand: {softLand} | WasJump: {wasJumpInitiated} | AirTime: {currentAirTime} | MaxFallSpeed: {maxFallSpeedThisAir}");
        return softLand;
    }

    private void TryStartDash()
    {
        isDashing = true;
        dashTimer = stats.dashDuration;
        lastDashTime = Time.time;

        Vector2 dashDir = new Vector2(transform.localScale.x, 0);
        rb.linearVelocity = dashDir * stats.dashForce;
    }

    private void HandleDash()
    {
        if (!isDashing) return;

        if (dashTimer > 0f)
        {
            Vector2 dashDir = new Vector2(transform.localScale.x, 0);
            rb.linearVelocity = new Vector2(dashDir.x * stats.dashForce, rb.linearVelocity.y);
        }

        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f)
            isDashing = false;
    }

    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;
            OnPausedChanged?.Invoke();
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            OnPausedChanged?.Invoke();
        }
    }

    public bool IsGrounded() => isGrounded;
    public bool IsJumping() => isJumping;
    public bool IsRunning() => isRunningMode && Mathf.Abs(horizontalInput) > 0.1f;
    public bool HasReachedApex() => reachedApex;
    public float GetCurrentAirTime() => currentAirTime;
    public float GetMaxFallSpeed() => maxFallSpeedThisAir;
    public bool WasJumpInitiated() => wasJumpInitiated;
    public float GetLastGroundedTime() => lastGroundedTime;
    public bool IsDashing() => isDashing;
    public float GetLastDashTime() => lastDashTime;

    // Call this after landing detection is complete
    public void ResetAirTracking()
    {
        currentAirTime = 0f;
        maxFallSpeedThisAir = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Vector2 boxSize = new Vector2(groundCheckWidth, 0.1f);
            Gizmos.DrawWireCube(groundCheck.position + Vector3.down * groundCheckDistance * 0.5f, boxSize);
        }
    }
}