using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerManager player, StateMachine stateMachine, PlayerData data)
        : base(player, stateMachine, data) { }

    public override void FixedUpdate()
    {
        player.MovementController.UpdateMovement(0, player.InputHandler.RunTogglePressed);
    }

    public override void Update()
    {
        if (Mathf.Abs(player.InputHandler.Horizontal) > 0.1f)
            stateMachine.ChangeState(player.MoveState);

        CheckForTransitions();
    }

    private void CheckForTransitions()
    {
        if (player.InputHandler.JumpPressed)
            stateMachine.ChangeState(player.JumpState);
        if (player.InputHandler.DashPressed)
            stateMachine.ChangeState(player.DashState);
        if (player.InputHandler.MeleeAttackPressed)
            stateMachine.ChangeState(player.MeleeAttackState);
        if (player.InputHandler.RangedAttackPressed)
            stateMachine.ChangeState(player.RangedAttackState);
        if (!player.MovementController.IsGrounded)
            stateMachine.ChangeState(player.FallState);
    }
}