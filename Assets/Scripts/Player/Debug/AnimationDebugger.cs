using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AnimationDebugger : MonoBehaviour
{
    [SerializeField] private AnimationController animationController;
    [SerializeField] private AttackManager attackManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private TextMeshPro debugText;
    [SerializeField] private bool showStateInfo = true;
    [SerializeField] private bool showMovementInfo = true;
    [SerializeField] private bool showAttackInfo = true;

    private Dictionary<int, string> stateNames;
    private Animator animator;
    private Rigidbody2D rb;

    private void Start()
    {
        // Auto-find components
        if (!animationController)
            animationController = GetComponent<AnimationController>();
        if (!attackManager)
            attackManager = GetComponent<AttackManager>();
        if (!playerManager)
            playerManager = GetComponent<PlayerManager>();
        if (!debugText)
            debugText = GetComponent<TextMeshPro>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        InitializeStateNames();

        // Setup debug text appearance
        if (debugText)
        {
            debugText.fontSize = 3f;
            debugText.color = Color.white;
            debugText.alignment = TextAlignmentOptions.TopLeft;
        }
    }

    private void InitializeStateNames()
    {
        stateNames = new Dictionary<int, string>
        {
            { PlayerData.Idle1, "Idle1" },
            { PlayerData.Idle2, "Idle2" },
            { PlayerData.Walk, "Walk" },
            { PlayerData.Run, "Run" },
            { PlayerData.Jump, "Jump" },
            { PlayerData.Fall, "Fall" },
            { PlayerData.SoftLand, "SoftLand" },
            { PlayerData.HardLand, "HardLand" },
            { PlayerData.Dash, "Dash" },
            { PlayerData.Attack1, "Attack1" },
            { PlayerData.Attack2, "Attack2" },
            { PlayerData.Attack3, "Attack3" },
            { PlayerData.RangedAttack, "RangedAttack" }
        };
    }

    private void Update()
    {
        if (!debugText) return;

        string debugInfo = "";

        if (showStateInfo && animationController)
        {
            debugInfo += GetAnimationInfo();
        }

        if (showMovementInfo && playerManager)
        {
            debugInfo += GetMovementInfo();
        }

        if (showAttackInfo && attackManager)
        {
            debugInfo += GetAttackInfo();
        }

        debugText.text = debugInfo;
    }

    private string GetAnimationInfo()
    {
        string currentStateName = GetStateName(animationController.CurrentStateHash);
        string previousStateName = GetStateName(animationController.PreviousStateHash);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime;
        bool isTransitioning = animator.IsInTransition(0);

        return $"<color=yellow>ANIMATION</color>\n" +
               $"Current: {currentStateName}\n" +
               $"Previous: {previousStateName}\n" +
               $"Norm Time: {normalizedTime:F2}\n" +
               $"Length: {stateInfo.length:F2}s\n" +
               $"Transitioning: {isTransitioning}\n\n";
    }

    private string GetMovementInfo()
    {
        var stateMachine = playerManager.StateMachine;
        var movementController = playerManager.MovementController;
        var inputHandler = playerManager.InputHandler;

        string currentState = stateMachine.CurrentState?.GetType().Name ?? "None";
        Vector2 velocity = rb ? rb.linearVelocity : Vector2.zero;
        float horizontalInput = inputHandler ? inputHandler.Horizontal : 0f;
        bool isGrounded = movementController ? movementController.IsGrounded : false;
        bool isRunning = movementController ? movementController.IsRunning : false;

        return $"<color=cyan>MOVEMENT</color>\n" +
               $"State: {currentState}\n" +
               $"Velocity: ({velocity.x:F1}, {velocity.y:F1})\n" +
               $"Input: {horizontalInput:F2}\n" +
               $"Grounded: {isGrounded}\n" +
               $"Running: {isRunning}\n\n";
    }

    private string GetAttackInfo()
    {
        bool isAttacking = attackManager.IsAttacking;

        string attackInfo = $"<color=red>ATTACK</color>\n" +
                           $"Is Attacking: {isAttacking}\n";

        // Add combo info if available
        var comboSystem = attackManager.GetComponent<ComboSystem>();
        if (comboSystem)
        {
            attackInfo += $"Combo Index: {comboSystem.GetNextComboIndex()}\n" +
                         $"Time Since Last: {comboSystem.TimeSinceLastAttack():F2}s\n" +
                         $"In Combo Window: {comboSystem.IsInComboWindow()}\n";
        }

        return attackInfo + "\n";
    }

    private string GetStateName(int stateHash)
    {
        return stateNames.TryGetValue(stateHash, out string name) ? name : $"Unknown({stateHash})";
    }

    // Toggle debug info with keys
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            showStateInfo = !showStateInfo;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            showMovementInfo = !showMovementInfo;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            showAttackInfo = !showAttackInfo;
    }
}