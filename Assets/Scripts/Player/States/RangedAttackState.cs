public class RangedAttackState : PlayerState
{
    public RangedAttackState(PlayerManager player, StateMachine stateMachine, PlayerData data)
    : base(player, stateMachine, data) { }
    public override void Enter()
    {
        player.AttackManager.TryRangedAttack(true);
    }
    public override void Update()
    {
        player.AttackManager.UpdateAttacks();
        if (!player.AttackManager.IsAttacking)
        {
            if (player.MovementController.IsGrounded)
                stateMachine.ChangeState(player.IdleState);
            else
                stateMachine.ChangeState(player.FallState);
        }
    }
    public override void FixedUpdate()
    {
        // No movement during ranged if grounded, as per original
        if (player.MovementController.IsGrounded)
            player.MovementController.UpdateMovement(0, false);
        else
            player.MovementController.UpdateMovement(player.InputHandler.Horizontal, false);
    }
}