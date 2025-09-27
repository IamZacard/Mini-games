using UnityEngine;
public class MoveState : PlayerState
{
    public MoveState(PlayerManager player, StateMachine stateMachine, PlayerData data)
    : base(player, stateMachine, data) { }
    public override void FixedUpdate()
    {
        player.MovementController.UpdateMovement(player.InputHandler.Horizontal, player.InputHandler.RunTogglePressed);
    }
    public override void Update()
    {
        if (player.InputHandler.Horizontal == 0)
            stateMachine.ChangeState(player.IdleState);
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