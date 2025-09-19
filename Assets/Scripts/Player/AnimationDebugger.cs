using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AnimationDebugger : MonoBehaviour
{
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private PlayerMeleeAttack meleeAttack;
    [SerializeField] private TextMeshPro debugText;

    private Dictionary<int, string> stateNames;
    private Animator animator;

    private void Start()
    {
        if (!playerAnimator)
            playerAnimator = GetComponent<PlayerAnimator>();
        if (!meleeAttack)
            meleeAttack = GetComponent<PlayerMeleeAttack>();
        if (!debugText)
            debugText = GetComponent<TextMeshPro>();

        animator = GetComponent<Animator>();
        InitializeStateNames();
    }

    private void InitializeStateNames()
    {
        stateNames = new Dictionary<int, string>
        {
            { Animator.StringToHash("Idle1"), "Idle1" },
            { Animator.StringToHash("Idle2"), "Idle2" },
            { Animator.StringToHash("Walk"), "Walk" },
            { Animator.StringToHash("Run"), "Run" },
            { Animator.StringToHash("Jump"), "Jump" },
            { Animator.StringToHash("Fall"), "Fall" },
            { Animator.StringToHash("SoftLand"), "SoftLand" },
            { Animator.StringToHash("HardLand"), "HardLand" },
            { Animator.StringToHash("Dash"), "Dash" },
            { Animator.StringToHash("Attack1"), "Attack1" },
            { Animator.StringToHash("Attack2"), "Attack2" },
            { Animator.StringToHash("Attack3"), "Attack3" }
        };
    }

    private void Update()
    {
        if (!debugText || !playerAnimator) return;

        string currentStateName = GetStateName(playerAnimator.CurrentState);
        string previousStateName = GetStateName(playerAnimator.PreviousState);

        // Get current animator info
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime;
        bool isAttacking = meleeAttack ? meleeAttack.IsAttacking() : false;

        debugText.text = $"Current: {currentStateName}\n" +
                        $"Previous: {previousStateName}\n" +
                        $"Normalized Time: {normalizedTime:F2}\n" +
                        $"Is Attacking: {isAttacking}\n" +
                        $"State Length: {stateInfo.length:F2}s";
    }

    private string GetStateName(int stateHash)
    {
        return stateNames.TryGetValue(stateHash, out string name) ? name : "Unknown";
    }
}