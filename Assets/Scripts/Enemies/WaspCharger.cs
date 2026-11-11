using UnityEngine;
using System.Collections;

/// <summary>
/// "Charger" enemy type from the brief. Winds up with a visible shake, then
/// dashes in a straight line at high speed towards where the player was
/// when the wind-up finished. Overrides TickTelegraph() to add the shake
/// and lock in a dash direction, and DoAttack() to run the dash as a
/// coroutine (so FinishAttack() is only called once the dash is over).
/// </summary>
public class WaspCharger : EnemyBase
{
    [Header("Charge")]
    [SerializeField] private float dashSpeed = 9f;
    [SerializeField] private float dashDuration = 0.35f;
    [SerializeField] private float telegraphShakeAmount = 0.06f;
    [SerializeField] private float hitRadius = 0.6f;

    private Vector2 _dashDirection = Vector2.right;

    protected override void TickTelegraph(bool seesPlayer)
    {
        // Small random jitter so the wind-up reads clearly before the dash.
        transform.position += (Vector3)(Random.insideUnitCircle * telegraphShakeAmount);

        if (player != null)
            _dashDirection = ((Vector2)player.position - (Vector2)transform.position).normalized;

        base.TickTelegraph(seesPlayer);
    }

    protected override void DoAttack()
    {
        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        float elapsed = 0f;
        bool hasHitPlayer = false;

        while (elapsed < dashDuration && !hasHitPlayer)
        {
            Move(_dashDirection * dashSpeed);
            elapsed += Time.deltaTime;

            if (player != null &&
                Vector2.Distance(transform.position, player.position) <= hitRadius)
            {
                PlayerHealth target = player.GetComponent<PlayerHealth>();
                if (target != null) target.TakeHit(damage, transform.position);
                hasHitPlayer = true;
                // Stop the dash the instant contact lands instead of continuing
                // to physically plow through the player for the rest of the dash.
                break;
            }
            yield return null;
        }

        Move(Vector2.zero);
        FinishAttack();
    }
}
