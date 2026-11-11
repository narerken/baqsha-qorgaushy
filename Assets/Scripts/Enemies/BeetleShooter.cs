using UnityEngine;

/// <summary>
/// "Shooter" enemy type from the brief. Prefers to keep its distance and
/// fire a seed/pebble projectile rather than walk into melee. Overrides
/// TickChase() so it stops approaching once in range (instead of the base
/// class's "walk right up to the player" behaviour), then fires in DoAttack().
/// </summary>
public class BeetleShooter : EnemyBase
{
    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 6f;

    protected override void TickChase(bool seesPlayer)
    {
        if (player == null) { ChangeState(State.Patrol); return; }

        if (!seesPlayer)
        {
            base.TickChase(false);
            return;
        }

        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distance = toPlayer.magnitude;

        if (distance <= attackRange)
        {
            ChangeState(State.Telegraph);
            return;
        }

        // Keep closing in, but only until just inside attack range -
        // beetles don't want to melee.
        if (distance > attackRange * 1.2f)
            Move(toPlayer.normalized * chaseSpeed);
    }

    protected override void DoAttack()
    {
        if (projectilePrefab != null && firePoint != null && player != null)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;
            GameObject go = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile proj = go.GetComponent<Projectile>();
            if (proj != null)
                proj.Launch(dir, projectileSpeed, damage);
        }
        FinishAttack();
    }
}
