using UnityEngine;
[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private PlayerData data;
    private MovementController movementController;
    private AttackManager attackManager;
    private int currentStateHash;
    private int previousStateHash;
    private bool wasInAir;
    private bool isInLandingState;
    private float landingStateTimer;
    public void Initialize(PlayerData data, MovementController movement, AttackManager attack)
    {
        this.data = data;
        this.movementController = movement;
        this.attackManager = attack;
        animator = GetComponent<Animator>();
    }
    public void UpdateAnimation()
    {
        int nextState = GetNextStateHash();
        if (isInLandingState)
        {
            landingStateTimer -= Time.deltaTime;
            if (landingStateTimer <= 0f)
            {
                isInLandingState = false;
            }
        }
        if (nextState != currentStateHash)
        {
            animator.CrossFade(nextState, 0.05f, 0);
            previousStateHash = currentStateHash;
            currentStateHash = nextState;
        }
    }
    private int GetNextStateHash()
    {
        // Attack priority
        if (attackManager.IsAttacking)
        {
            // Melee
            if (attackManager.TryMeleeAttack(false)) // Check if melee is active
            {
                int comboIndex = attackManager.GetComponent<ComboSystem>().GetNextComboIndex(); // Assume exposed
                int attackIndex = comboIndex == 0 ? 2 : comboIndex - 1;
                return attackIndex switch
                {
                    0 => PlayerData.Attack1,
                    1 => PlayerData.Attack2,
                    2 => PlayerData.Attack3,
                    _ => PlayerData.Attack1
                };
            }
            // Ranged
            if (attackManager.TryRangedAttack(false))
                return PlayerData.RangedAttack;
        }
        bool isGrounded = movementController.IsGrounded;
        // Dash priority
        if (movementController.IsDashing)
            return PlayerData.Dash;
        // Landing
        if (isInLandingState && (currentStateHash == PlayerData.SoftLand || currentStateHash == PlayerData.HardLand))
            return currentStateHash;
        if (isGrounded && wasInAir && !isInLandingState)
        {
            wasInAir = false;
            bool hardLand = movementController.ShouldHardLand();
            bool softLand = movementController.ShouldSoftLand();
            movementController.ResetAirTracking();
            if (hardLand)
            {
                isInLandingState = true;
                landingStateTimer = data.landingAnimDuration;
                return PlayerData.HardLand;
            }
            else if (softLand)
            {
                isInLandingState = true;
                landingStateTimer = data.landingAnimDuration;
                return PlayerData.SoftLand;
            }
        }
        // Air states
        if (!isGrounded)
        {
            wasInAir = true;
            if (!movementController.HasReachedApex)
                return PlayerData.Jump;
            else if (GetComponent<Rigidbody2D>().linearVelocity.y < -0.1f)
                return PlayerData.Fall;
        }
        // Ground movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float absVelX = Mathf.Abs(rb.linearVelocity.x);
        if (movementController.IsRunning && absVelX > data.runThreshold)
            return PlayerData.Run;
        else if (absVelX > data.walkThreshold)
            return PlayerData.Walk;
        return GetIdleVariant();
    }
    private int GetIdleVariant()
    {
        if (currentStateHash != PlayerData.Idle1 && currentStateHash != PlayerData.Idle2)
            return Random.value < 0.5f ? PlayerData.Idle1 : PlayerData.Idle2;
        return currentStateHash;
    }
    public int CurrentStateHash => currentStateHash;
    public int PreviousStateHash => previousStateHash;
}