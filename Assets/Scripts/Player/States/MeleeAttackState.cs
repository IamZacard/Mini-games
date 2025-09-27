public class MeleeAttackState : PlayerState
{
    public MeleeAttackState(PlayerManager player, StateMachine stateMachine, PlayerData data)
    : base(player, stateMachine, data) { }
    public override void Enter()
    {
        player.AttackManager.TryMeleeAttack(true);
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
        // Limited movement if allowed by attack data
        // Assuming current attack data accessible, else minimal movement
        player.MovementController.UpdateMovement(player.InputHandler.Horizontal * 0.5f, false); // Example: reduced
    }
}