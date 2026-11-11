using UnityEngine;

/// <summary>
/// Smoothly follows the player horizontally (and optionally vertically),
/// clamped to the level bounds so the camera never shows empty space
/// past the edges of the background/level.
/// Attach this to Main Camera and assign the Player's Transform as target.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // drag Player here

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private Vector2 offset = new Vector2(0f, 1f);
    [SerializeField] private bool followY = false; // platformers usually only follow X

    [Header("Level Bounds (optional, world units)")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private Vector3 _velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + offset.x;
        float targetY = followY ? target.position.y + offset.y : transform.position.y;

        Vector3 desiredPosition = new Vector3(targetX, targetY, transform.position.z);

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, smoothTime);
    }
}
