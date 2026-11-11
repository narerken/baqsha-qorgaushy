using UnityEngine;

/// <summary>
/// Shared skeleton used by all three enemy types.
///
/// State machine: Patrol -> Chase -> Telegraph -> Attack -> Cooldown.
///   Patrol    - walks back and forth between waypoints until it sees the player.
///   Chase     - moves towards the player while it can see (or recently saw) them.
///   Telegraph - stops and visibly winds up (color flash) for telegraphDuration
///               seconds BEFORE doing any damage - this is what makes the
///               attack "fair" per the assignment brief.
///   Attack    - the actual hit happens here. Each enemy overrides DoAttack()
///               to decide what that means (melee bite, projectile, dash).
///   Cooldown  - can't attack again until cooldownDuration has passed.
///
/// Movement/vision/state-switching lives here so all three enemies behave
/// consistently and share one place to tune/debug. Each concrete enemy
/// (SnailSentry, BeetleShooter, WaspCharger) only overrides the parts that
/// make it unique.
/// </summary>
[RequireComponent(typeof(EnemyVision))]
public abstract class EnemyBase : MonoBehaviour
{
    public enum State { Patrol, Chase, Telegraph, Attack, Cooldown }

    [Header("References")]
    [SerializeField] protected Transform[] waypoints;
    [Tooltip("Leave empty to auto-find the GameObject tagged 'Player'.")]
    [SerializeField] protected Transform player;

    [Header("Movement")]
    [SerializeField] protected float patrolSpeed = 1.5f;
    [SerializeField] protected float chaseSpeed = 3f;
    [SerializeField] protected float waypointReachDistance = 0.15f;

    [Header("Combat tuning (all serialized - no magic numbers)")]
    [SerializeField] protected float attackRange = 1.2f;
    [Tooltip("Visible wind-up before the hit lands. Assignment requires 0.3-0.8s.")]
    [SerializeField] protected float telegraphDuration = 0.5f;
    [SerializeField] protected float cooldownDuration = 1.5f;
    [SerializeField] protected float loseInterestTime = 2.5f;
    [SerializeField] protected int damage = 1;

    [Header("Telegraph visual cue")]
    [SerializeField] protected SpriteRenderer bodySprite;
    [SerializeField] protected Color telegraphColor = Color.red;

    protected EnemyVision vision;
    protected Rigidbody2D rb;
    protected Color originalColor;

    public State CurrentState { get; protected set; } = State.Patrol;

    private int _waypointIndex;
    private float _timeSinceSawPlayer;
    private float _stateTimer;

    protected virtual void Awake()
    {
        vision = GetComponent<EnemyVision>();
        rb = GetComponent<Rigidbody2D>();
        if (bodySprite != null) originalColor = bodySprite.color;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    protected virtual void Update()
    {
        bool seesPlayer = vision.CanSeePlayer(player, out Vector2 dirToPlayer);
        if (seesPlayer)
        {
            _timeSinceSawPlayer = 0f;
            FaceTowards(dirToPlayer.x);
        }
        else
        {
            _timeSinceSawPlayer += Time.deltaTime;
        }

        switch (CurrentState)
        {
            case State.Patrol: TickPatrol(seesPlayer); break;
            case State.Chase: TickChase(seesPlayer); break;
            case State.Telegraph: TickTelegraph(seesPlayer); break;
            case State.Attack: break; // handled by DoAttack()/coroutines started on entry
            case State.Cooldown: TickCooldown(seesPlayer); break;
        }
    }

    protected virtual void TickPatrol(bool seesPlayer)
    {
        if (seesPlayer) { ChangeState(State.Chase); return; }
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[_waypointIndex];
        Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;

        if (toTarget.magnitude <= waypointReachDistance)
        {
            _waypointIndex = (_waypointIndex + 1) % waypoints.Length;
        }
        else
        {
            Move(toTarget.normalized * patrolSpeed);
            FaceTowards(toTarget.x);
        }
    }

    protected virtual void TickChase(bool seesPlayer)
    {
        if (player == null) { ChangeState(State.Patrol); return; }

        if (!seesPlayer && _timeSinceSawPlayer > loseInterestTime)
        {
            ChangeState(State.Patrol);
            return;
        }

        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distance = toPlayer.magnitude;

        // Once already in Chase, point-blank range is enough to attack even if
        // this frame's raycast briefly loses sight (grazing angles / self-occlusion
        // right next to the target) - otherwise the enemy can ram straight through
        // the player forever instead of stopping to telegraph its hit.
        if (distance <= attackRange)
        {
            ChangeState(State.Telegraph);
            return;
        }

        Move(toPlayer.normalized * chaseSpeed);
    }

    protected virtual void TickTelegraph(bool seesPlayer)
    {
        // Enemy holds still and flashes color - no movement, no damage yet.
        _stateTimer += Time.deltaTime;
        if (_stateTimer >= telegraphDuration)
        {
            ChangeState(State.Attack);
        }
    }

    protected virtual void TickCooldown(bool seesPlayer)
    {
        _stateTimer += Time.deltaTime;
        if (_stateTimer < cooldownDuration) return;

        if (seesPlayer && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            ChangeState(distance <= attackRange ? State.Telegraph : State.Chase);
        }
        else
        {
            ChangeState(State.Chase);
        }
    }

    protected virtual void ChangeState(State next)
    {
        Debug.Log("[DamageDebug] " + gameObject.name + " state " + CurrentState + " -> " + next);
        CurrentState = next;
        _stateTimer = 0f;

        // Telegraph/Attack/Cooldown don'''t call Move() every frame, and with no
        // linear damping any leftover chase/patrol velocity would otherwise just
        // keep coasting (and shoving the player) for the whole wind-up/cooldown.
        if (next == State.Telegraph || next == State.Attack || next == State.Cooldown || next == State.Patrol)
            Move(Vector2.zero);

        if (bodySprite != null)
            bodySprite.color = (next == State.Telegraph) ? telegraphColor : originalColor;

        if (next == State.Attack)
        {
            DoAttack();
        }
    }

    /// <summary>
    /// Called exactly once, the instant the telegraph finishes. Each enemy
    /// implements its own kind of attack here and must eventually call
    /// FinishAttack() (immediately for an instant hit, or after a coroutine
    /// for something like a dash).
    /// </summary>
    protected abstract void DoAttack();

    protected void FinishAttack()
    {
        ChangeState(State.Cooldown);
    }

    protected void Move(Vector2 velocity)
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);
        else
            transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    protected void FaceTowards(float dirX)
    {
        if (Mathf.Abs(dirX) < 0.01f) return;
        int facing = dirX > 0 ? 1 : -1;
        vision.facingDirection = facing;
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * facing, s.y, s.z);
    }
}
