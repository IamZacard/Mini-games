public class JumpState : PlayerState
{
    public JumpState(PlayerManager player, StateMachine stateMachine, PlayerData data)
    : base(player, stateMachine, data) { }
    public override void Enter()
    {
        player.MovementController.TryJump(true);
    }
    public override void FixedUpdate()
    {
        player.MovementController.UpdateMovement(player.InputHandler.Horizontal, false);
    }
    public override void Update()
    {
        if (player.MovementController.HasReachedApex)
            stateMachine.ChangeState(player.FallState);
        if (player.InputHandler.MeleeAttackPressed)
            stateMachine.ChangeState(player.MeleeAttackState);
        if (player.InputHandler.RangedAttackPressed)
            stateMachine.ChangeState(player.RangedAttackState);
    }
}