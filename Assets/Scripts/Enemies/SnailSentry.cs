using UnityEngine;

/// <summary>
/// "Patroller" enemy type from the brief. Slow and persistent: patrols a
/// fixed route, and once it spots the player it closes in and bites at
/// close range. States: Patrol -> Chase -> Telegraph -> Attack -> Cooldown
/// (inherited from EnemyBase) - this script only defines what the Attack
/// state actually does.
/// </summary>
public class SnailSentry : EnemyBase
{
    [Header("Snail bite")]
    [SerializeField] private float biteRange = 1.0f;

    protected override void DoAttack()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= biteRange)
            {
                PlayerHealth target = player.GetComponent<PlayerHealth>();
                if (target != null)
                    target.TakeHit(damage, transform.position);
            }
        }
        FinishAttack();
    }
}
