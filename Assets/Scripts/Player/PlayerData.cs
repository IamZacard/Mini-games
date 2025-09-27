using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Stats")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 3.5f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1.5f;
    public float acceleration = 50f;
    public float deceleration = 40f;
    public float gravityScale = 4.5f;

    [Header("Ground Check")]
    public float groundCheckDistance = 0.2f;
    public float groundCheckWidth = 0.5f;
    public LayerMask groundLayer;

    [Header("Coyote Time")]
    public float coyoteTime = 0.1f;
    public float landingBuffer = 0.05f;
    public float airAccelerationMultiplier = 2f;

    [Header("Landing Detection")]
    public float softLandMaxSpeed = -8f;
    public float hardLandMinSpeed = -18f;
    public float quickLandMaxAirTime = 0.4f;
    public float longFallMinAirTime = 1.2f;

    [Header("Attack Stats")]
    public AttackData[] meleeAttacks;
    public RangedAttackData rangedAttack;

    [Header("Economy")]
    public int startingGold = 100;

    [Header("Animation Data")]
    public float landingAnimDuration = 0.3f;
    public float walkThreshold = 3f;
    public float runThreshold = 5f;

    // Animation state hashes (moved from PlayerAnimator for centralization)
    public static readonly int Idle1 = Animator.StringToHash("Idle1");
    public static readonly int Idle2 = Animator.StringToHash("Idle2");
    public static readonly int Walk = Animator.StringToHash("Walk");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int Fall = Animator.StringToHash("Fall");
    public static readonly int SoftLand = Animator.StringToHash("SoftLand");
    public static readonly int HardLand = Animator.StringToHash("HardLand");
    public static readonly int Dash = Animator.StringToHash("Dash");
    public static readonly int Attack1 = Animator.StringToHash("Attack1");
    public static readonly int Attack2 = Animator.StringToHash("Attack2");
    public static readonly int Attack3 = Animator.StringToHash("Attack3");
    public static readonly int RangedAttack = Animator.StringToHash("RangedAttack");

    // AudioData can be added here if needed in the future
    // [Header("Audio Data")]
    // public AudioClip jumpSound;
    // etc.
}

[System.Serializable]
public class AttackData
{
    public string name;

    [Header("Hitbox")]
    public Vector2 size;
    public Vector2 offset;

    [Header("Combat")]
    public int damage;
    public float cooldown = 0.7f;
    public float knockbackForce;

    [Header("Animation")]
    public AnimationClip animationClip;

    [Header("Movement Behavior")]
    public MovementBehavior movementBehavior = MovementBehavior.StopOnGround;
    public float movementModifier = 0f;
    public bool allowAirMovement = false;
}

[System.Serializable]
public class RangedAttackData
{
    public string name;

    [Header("Combat")]
    public int damage;
    public float cooldown = 1f;
    public float knockbackForce;
    public GameObject projectilePrefab;

    [Header("Animation")]
    public AnimationClip animationClip;
}

public enum MovementBehavior
{
    NoChange,
    StopOnGround,
    StopAlways,
    ReduceSpeed,
    LockMovement,
    CustomVelocity
}
