using UnityEngine;
public class StateMachine : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }
    public void Initialize(PlayerState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }
    public void ChangeState(PlayerState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
    private void Update()
    {
        CurrentState.HandleInput();
        CurrentState.Update();
    }
    private void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }
}