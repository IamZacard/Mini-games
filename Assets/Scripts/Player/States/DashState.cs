public class DashState : PlayerState
{
    public DashState(PlayerManager player, StateMachine stateMachine, PlayerData data)
    : base(player, stateMachine, data) { }
    public override void Enter()
    {
        player.MovementController.TryDash(true);
    }
    public override void FixedUpdate()
    {
        player.MovementController.UpdateMovement(0, false); // No horizontal during dash
    }
    public override void Update()
    {
        if (!player.MovementController.IsDashing)
        {
            if (player.MovementController.IsGrounded)
                stateMachine.ChangeState(player.IdleState);
            else
                stateMachine.ChangeState(player.FallState);
        }
    }
}