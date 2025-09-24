using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 5f;        // Base walking speed
    public float runSpeed = 8f;         // Running speed (with LShift)
    public float jumpForce = 12f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float acceleration = 50f;
    public float deceleration = 40f;

    public float gravityScale = 4.5f;
    public float jumpHeight = 3.5f;

    [Header("Combat")]   
    public AttackData[] attacks;

    public RangedAttackData rangedAttacks;

    [Header("Economy")]
    public int startingGold = 100;
}

[System.Serializable]
public class AttackData
{
    public string name;

    [Header("Hitbox")]
    public Vector2 size;
    public Vector2 offset;

    [Header("Combat")]
    public int meleeDamage;
    public float meleeAttackCooldown = 0.7f;
    public float knockbackForce;

    [Header("Animation")]
    public AnimationClip animationClip;

    [Header("Movement Behavior")]
    public MovementBehavior movementBehavior = MovementBehavior.StopOnGround;
    public float movementModifier = 0f; // For partial speed, dash force, etc.
    public bool allowAirMovement = false; // Can move in air during attack
}

[System.Serializable]
public class RangedAttackData
{
    public string name;

    [Header("Combat")]
    public int rangedDamage;
    public float rangedAttackCooldown = 1f;
    public float knockbackForce;

    public GameObject projectilePrefab;

    [Header("Animation")]
    public AnimationClip rangedAttackClip;
}

[System.Serializable]
public enum MovementBehavior
{
    NoChange,           // Don't affect movement at all
    StopOnGround,       // Stop only when grounded  
    StopAlways,         // Stop regardless of ground state
    ReduceSpeed,        // Reduce speed by movementModifier %
    LockMovement,       // Lock all movement input during attack
    CustomVelocity      // Set specific velocity from movementModifier
}