using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class GoalHitDetector : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnHit; 

    private GoalController goalController;

    private void Awake()
    {
        goalController = GetComponent<GoalController>();

        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if it was hit by a projectile
        if (other.CompareTag("Projectile"))
        {
            goalController?.HitByPlayer();

            OnHit?.Invoke();

            // Destroy(other.gameObject);
        }
    }
}
