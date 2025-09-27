public class FallState : PlayerState
{
    public FallState(PlayerManager player, StateMachine stateMachine, PlayerData data)
    : base(player, stateMachine, data) { }
    public override void FixedUpdate()
    {
        player.MovementController.UpdateMovement(player.InputHandler.Horizontal, false);
    }
    public override void Update()
    {
        if (player.MovementController.IsGrounded)
        {
            if (player.MovementController.ShouldHardLand())
                stateMachine.ChangeState(player.HardLandState);
            else if (player.MovementController.ShouldSoftLand())
                stateMachine.ChangeState(player.SoftLandState);
            else
                stateMachine.ChangeState(player.IdleState);
        }
        if (player.InputHandler.MeleeAttackPressed)
            stateMachine.ChangeState(player.MeleeAttackState);
        if (player.InputHandler.RangedAttackPressed)
            stateMachine.ChangeState(player.RangedAttackState);
    }
}