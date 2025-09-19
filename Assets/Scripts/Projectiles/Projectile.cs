using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private ProjectileData data;
    private Vector2 direction;
    private float timer;

    public void Initialize(ProjectileData data, Vector2 direction)
    {
        this.data = data;
        this.direction = direction.normalized;
        rb = GetComponent<Rigidbody2D>();

        // Set sprite (if prefab uses SpriteRenderer)
        if (data.sprite != null)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = data.sprite;
        }

        timer = data.lifetime;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * data.speed;

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Example: detect enemies
        if (other.CompareTag("Enemy"))
        {
            // Deal damage if enemy has health component
            /*if (other.TryGetComponent(out EnemyHealth health))
            {
                health.TakeDamage(data.damage);
            }*/

            if (!data.pierce)
            {
                Destroy(gameObject);
            }
        }
    }
}
