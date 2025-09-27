using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerManager : MonoBehaviour, IPausable
{
    [SerializeField] private PlayerData data;
    [SerializeField] private PlayerAttackHitbox meleeHitbox;
    [SerializeField] private Transform rangedSpawnPoint;
    public InputHandler InputHandler { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public MovementController MovementController { get; private set; }
    public AttackManager AttackManager { get; private set; }
    public AnimationController AnimationController { get; private set; }
    // States
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public JumpState JumpState { get; private set; }
    public FallState FallState { get; private set; }
    public DashState DashState { get; private set; }
    public MeleeAttackState MeleeAttackState { get; private set; }
    public RangedAttackState RangedAttackState { get; private set; }
    public SoftLandState SoftLandState { get; private set; }
    public HardLandState HardLandState { get; private set; }
    private bool isPaused;
    public bool IsPaused => isPaused;
    public event System.Action OnPausedChanged;
    private void Awake()
    {
        InputHandler = gameObject.AddComponent<InputHandler>();
        StateMachine = gameObject.AddComponent<StateMachine>();
        MovementController = gameObject.AddComponent<MovementController>();
        MovementController.Initialize(data);
        AttackManager = gameObject.AddComponent<AttackManager>();
        AttackManager.Initialize(data, meleeHitbox, rangedSpawnPoint);
        AnimationController = gameObject.AddComponent<AnimationController>();
        AnimationController.Initialize(data, MovementController, AttackManager);
        // Initialize states
        IdleState = new IdleState(this, StateMachine, data);
        MoveState = new MoveState(this, StateMachine, data);
        JumpState = new JumpState(this, StateMachine, data);
        FallState = new FallState(this, StateMachine, data);
        DashState = new DashState(this, StateMachine, data);
        MeleeAttackState = new MeleeAttackState(this, StateMachine, data);
        RangedAttackState = new RangedAttackState(this, StateMachine, data);
        SoftLandState = new SoftLandState(this, StateMachine, data);
        HardLandState = new HardLandState(this, StateMachine, data);
        StateMachine.Initialize(IdleState);
    }
    private void Update()
    {
        if (isPaused) return;
        // StateMachine.Update() handles state updates
        AttackManager.UpdateAttacks();
        AnimationController.UpdateAnimation();
        // Consume inputs after processing
        InputHandler.ConsumeInputs();
    }
    private void FixedUpdate()
    {
        if (isPaused) return;
        // StateMachine.FixedUpdate() handles physics
    }
    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;
            OnPausedChanged?.Invoke();
        }
    }
    public void Resume()
    {
        if (isPaused)
        {
            isPaused = false;
            OnPausedChanged?.Invoke();
        }
    }
}