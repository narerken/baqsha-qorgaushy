using UnityEngine;

/// <summary>
/// Simple straight-line projectile fired by BeetleShooter. Needs a
/// Collider2D (set Is Trigger) and optionally a Rigidbody2D (Kinematic) on
/// the prefab for 2D trigger events to fire reliably.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifeTime = 4f;

    private Vector2 _velocity;
    private int _damage;

    public void Launch(Vector2 direction, float speed, int damage)
    {
        _velocity = direction * speed;
        _damage = damage;
        Destroy(gameObject, lifeTime);

        // Point the sprite the way it's travelling.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        transform.position += (Vector3)(_velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeHit(_damage, transform.position);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
