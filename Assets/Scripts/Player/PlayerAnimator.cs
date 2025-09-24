using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerController controller;
    private PlayerMeleeAttack meleeAttack;
    private PlayerRangedAttack rangedAttack;
    private Rigidbody2D rb;

    private int currentState;
    private int previousState;
    private bool wasInAir;
    private bool isInLandingState; 
    private float landingStateTimer;

    [Header("Animation Settings")]
    [SerializeField] private float landingAnimDuration = 0.3f;

    [Header("Walking Settings")]
    [SerializeField] private float walkThreshold = 3f;
    [SerializeField] private float runThreshold = 5f;

    // Movement state hashes
    private static readonly int Idle1 = Animator.StringToHash("Idle1");
    private static readonly int Idle2 = Animator.StringToHash("Idle2");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Fall = Animator.StringToHash("Fall");
    private static readonly int SoftLand = Animator.StringToHash("SoftLand");
    private static readonly int HardLand = Animator.StringToHash("HardLand");
    private static readonly int Dash = Animator.StringToHash("Dash");

    // Attack state hashes
    private static readonly int Attack1 = Animator.StringToHash("Attack1");
    private static readonly int Attack2 = Animator.StringToHash("Attack2");
    private static readonly int Attack3 = Animator.StringToHash("Attack3");

    private static readonly int RangedAttack = Animator.StringToHash("RangedAttack");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        meleeAttack = GetComponent<PlayerMeleeAttack>();
        rangedAttack = GetComponent<PlayerRangedAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (controller == null || animator == null) return;

        int nextState = GetNextState();

        if (isInLandingState)
        {
            landingStateTimer -= Time.deltaTime;
            if (landingStateTimer <= 0f)
            {
                isInLandingState = false;
            }
        }

        if (nextState != currentState)
        {
            animator.CrossFade(nextState, 0.05f, 0);
            previousState = currentState;
            currentState = nextState;
        }
    }

    private int GetNextState()
    {
        // ATTACK PRIORITY - highest priority
        if (meleeAttack != null && meleeAttack.IsAttacking())
        {
            int comboIndex = meleeAttack.GetCurrentComboIndex();
            int attackIndex = comboIndex == 0 ? 2 : comboIndex - 1;

            return attackIndex switch
            {
                0 => Attack1,
                1 => Attack2,
                2 => Attack3,
                _ => Attack1
            };
        }

        if (rangedAttack != null && rangedAttack.IsAttacking())
            return RangedAttack;

        bool isGrounded = controller.IsGrounded();

        // Dash priority
        if (controller.IsDashing())
            return Dash;

        // Stay in landing animation if we're still playing it
        if (isInLandingState && (currentState == SoftLand || currentState == HardLand))
        {
            return currentState;
        }

        // Landing detection
        if (isGrounded && wasInAir && !isInLandingState)
        {
            wasInAir = false;

            bool shouldHardLand = controller.ShouldHardLand();
            bool shouldSoftLand = controller.ShouldSoftLand();

            Debug.Log($"Landing - Hard: {shouldHardLand}, Soft: {shouldSoftLand}, AirTime: {controller.GetCurrentAirTime()}, Speed: {controller.GetMaxFallSpeed()}");

            // Reset tracking AFTER we've determined landing type
            controller.ResetAirTracking();

            if (shouldHardLand)
            {
                isInLandingState = true;
                landingStateTimer = landingAnimDuration;
                return HardLand;
            }
            else if (shouldSoftLand)
            {
                isInLandingState = true;
                landingStateTimer = landingAnimDuration;
                return SoftLand;
            }
        }

        // Air states
        if (!isGrounded)
        {
            wasInAir = true;
            if (!controller.HasReachedApex())
                return Jump;
            else if (rb.linearVelocity.y < -0.1f)
                return Fall;
        }

        // Ground movement
        float absVelX = Mathf.Abs(rb.linearVelocity.x);
        if (controller.IsRunning() && absVelX > runThreshold)
            return Run;
        else if (absVelX > walkThreshold)
            return Walk;

        return GetIdleVariant();
    }

    private int GetIdleVariant()
    {
        if (currentState != Idle1 && currentState != Idle2)
            return Random.value < 0.5f ? Idle1 : Idle2;
        return currentState;
    }

    public int CurrentState => currentState;
    public int PreviousState => previousState;
}