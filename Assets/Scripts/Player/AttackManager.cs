using UnityEngine;
public class AttackManager : MonoBehaviour
{
    private PlayerData data;
    private Rigidbody2D rb;
    private MovementController movementController;
    private ComboSystem comboSystem;
    private MeleeAttackController meleeController;
    private RangedAttackController rangedController;
    private PlayerAttackHitbox meleeHitbox;
    private Transform rangedSpawnPoint;
    public bool IsAttacking => (meleeController != null && meleeController.IsAttacking) || (rangedController != null && rangedController.IsAttacking);
    public void Initialize(PlayerData data, PlayerAttackHitbox meleeHitbox, Transform rangedSpawnPoint)
    {
        this.data = data;
        this.meleeHitbox = meleeHitbox;
        this.rangedSpawnPoint = rangedSpawnPoint;
        rb = GetComponent<Rigidbody2D>();
        movementController = GetComponent<MovementController>();
        comboSystem = gameObject.AddComponent<ComboSystem>();
        comboSystem.Initialize(data);
        meleeController = gameObject.AddComponent<MeleeAttackController>();
        meleeController.Initialize(data, rb, comboSystem, meleeHitbox);
        rangedController = gameObject.AddComponent<RangedAttackController>();
        rangedController.Initialize(data, rb, movementController, rangedSpawnPoint);
    }
    public void UpdateAttacks()
    {
        meleeController?.UpdateAttack();
        rangedController?.UpdateAttack();
    }
    public bool TryMeleeAttack(bool pressed) => meleeController != null && meleeController.TryAttack(pressed);
    public bool TryRangedAttack(bool pressed) => rangedController != null && rangedController.TryAttack(pressed);
    // Expose for animation events
    public void ActivateMeleeHitbox() => meleeController?.ActivateHitbox();
    public void DeactivateMeleeHitbox() => meleeController?.DeactivateHitbox();
    public void FireRangedProjectile() => rangedController?.FireProjectile();
}