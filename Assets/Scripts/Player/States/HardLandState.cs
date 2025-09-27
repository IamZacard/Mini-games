using UnityEngine;

public class HardLandState : PlayerState
{
    private float timer;

    public HardLandState(PlayerManager player, StateMachine stateMachine, PlayerData data)
        : base(player, stateMachine, data) { }

    public override void Enter()
    {
        timer = data.landingAnimDuration;
    }

    public override void FixedUpdate()
    {
        // Stop movement during hard landing
        player.MovementController.UpdateMovement(0, false);
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            stateMachine.ChangeState(player.IdleState);
    }
}