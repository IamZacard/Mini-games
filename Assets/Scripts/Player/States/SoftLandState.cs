using UnityEngine;

public class SoftLandState : PlayerState
{
    private float timer;

    public SoftLandState(PlayerManager player, StateMachine stateMachine, PlayerData data)
        : base(player, stateMachine, data) { }

    public override void Enter()
    {
        timer = data.landingAnimDuration;
    }

    public override void FixedUpdate()
    {
        // Stop movement during landing
        player.MovementController.UpdateMovement(0, false);
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            stateMachine.ChangeState(player.IdleState);
    }
}