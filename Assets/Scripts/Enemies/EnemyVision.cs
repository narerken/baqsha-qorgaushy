using UnityEngine;

/// <summary>
/// Attach to any enemy. Answers one question every frame: "can I see the player
/// right now?" — checking distance, a field-of-view angle, and a Raycast2D that
/// a wall can block. Assignment 10 requirement: "at least one enemy checks
/// distance, angle and a line of sight with a Raycast; walls block it."
/// All three enemies in this project use this same component.
/// </summary>
public class EnemyVision : MonoBehaviour
{
    [Header("Vision tuning")]
    [SerializeField] private float viewRadius = 6f;
    [Tooltip("Full cone width in degrees, centered on the facing direction.")]
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Facing (set automatically by the enemy script)")]
    [Tooltip("1 = enemy currently faces right, -1 = faces left.")]
    public int facingDirection = 1;

    /// <summary>
    /// True if the player is inside view radius, inside the view cone,
    /// and nothing on obstacleLayer sits between us and them.
    /// </summary>
    private float _lastDebugLog = -999f;

    public bool CanSeePlayer(Transform player, out Vector2 directionToPlayer)
    {
        directionToPlayer = Vector2.zero;
        bool debugNow = Time.time - _lastDebugLog > 1f;
        if (debugNow) _lastDebugLog = Time.time;

        if (player == null)
        {
            if (debugNow) Debug.Log("[VisionDebug] " + gameObject.name + " player ref is null");
            return false;
        }

        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distance = toPlayer.magnitude;
        if (distance > viewRadius || distance < 0.001f)
        {
            if (debugNow) Debug.Log("[VisionDebug] " + gameObject.name + " out of range. distance=" + distance + " viewRadius=" + viewRadius);
            return false;
        }

        Vector2 facing = new Vector2(facingDirection, 0f);
        float angle = Vector2.Angle(facing, toPlayer);
        if (angle > viewAngle * 0.5f)
        {
            if (debugNow) Debug.Log("[VisionDebug] " + gameObject.name + " out of FOV. angle=" + angle + " half=" + (viewAngle * 0.5f) + " facingDir=" + facingDirection);
            return false;
        }

        // Cast towards the player on both layers together: whichever is
        // actually hit first tells us if a wall is in the way.
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, toPlayer.normalized, distance, obstacleLayer | playerLayer);

        if (hit.collider == null)
        {
            if (debugNow) Debug.Log("[VisionDebug] " + gameObject.name + " raycast hit nothing. distance=" + distance + " mask=" + (obstacleLayer.value | playerLayer.value));
            return false;
        }
        bool hitIsPlayer = ((1 << hit.collider.gameObject.layer) & playerLayer.value) != 0;
        if (!hitIsPlayer)
        {
            if (debugNow) Debug.Log("[VisionDebug] " + gameObject.name + " raycast hit non-player first: " + hit.collider.name + " layer=" + hit.collider.gameObject.layer);
            return false; // a wall was hit first - line of sight blocked
        }

        directionToPlayer = toPlayer.normalized;
        if (debugNow) Debug.Log("[VisionDebug] " + gameObject.name + " SEES PLAYER");
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 facing = new Vector3(facingDirection, 0f, 0f);
        Vector3 left = Quaternion.Euler(0, 0, viewAngle * 0.5f) * facing;
        Vector3 right = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * facing;
        Gizmos.DrawLine(transform.position, transform.position + left * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + right * viewRadius);
    }
}
