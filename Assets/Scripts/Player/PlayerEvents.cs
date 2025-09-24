// PlayerEvents.cs
using UnityEngine;

public static class PlayerEvents
{
    // Attack Events
    public static event System.Action<AttackEventData> OnMeleeAttackStarted;
    public static event System.Action<AttackEventData> OnRangedAttackStarted;
    public static event System.Action OnAttackEnded;

    // Movement Events
    public static event System.Action<MovementEventData> OnMovementRequested;
    public static event System.Action OnMovementStop;

    // State Events  
    public static event System.Action<bool> OnGroundedChanged; // bool = isGrounded
    public static event System.Action<bool> OnDashStateChanged; // bool = isDashing
}

[System.Serializable]
public struct AttackEventData
{
    public AttackData attackData;
    public AttackType attackType;
    public MovementBehavior movementBehavior;
    public bool isGrounded;

    public AttackEventData(AttackData data, AttackType type, bool grounded)
    {
        attackData = data;
        attackType = type;
        isGrounded = grounded;
        movementBehavior = data.movementBehavior; // From extended AttackData
    }
}

[System.Serializable]
public struct MovementEventData
{
    public Vector2 requestedVelocity;
    public MovementType type;
    public bool overrideCurrent;

    public MovementEventData(Vector2 velocity, MovementType movType, bool overrides = false)
    {
        requestedVelocity = velocity;
        type = movType;
        overrideCurrent = overrides;
    }
}

public enum AttackType { Melee, Ranged }
public enum MovementType { Stop, SetVelocity, AddForce, Dash }
