using UnityEngine;
public abstract class PlayerState
{
    protected PlayerManager player;
    protected PlayerData data;
    protected StateMachine stateMachine;
    public PlayerState(PlayerManager player, StateMachine stateMachine, PlayerData data)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.data = data;
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void HandleInput() { }
}