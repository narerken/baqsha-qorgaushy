using UnityEngine;

/// <summary>
/// Side-scrolling platformer movement for the gardener. Horizontal speed
/// comes from the on-screen VirtualJoystick; jumping is triggered by the
/// on-screen jump Button calling OnJumpPressed() from its OnClick.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VirtualJoystick joystick;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 9f;

    [Header("Ground check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D _rb;
    private bool _isGrounded;
    private bool _jumpQueued;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _isGrounded = groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (_jumpQueued)
        {
            if (_isGrounded)
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpQueued = false;
        }

        FlipSprite();
    }

    private void FixedUpdate()
    {
        float h = joystick != null ? joystick.Horizontal : 0f;
        _rb.linearVelocity = new Vector2(h * moveSpeed, _rb.linearVelocity.y);
    }

    /// <summary>Wire this to the on-screen Jump button's OnClick().</summary>
    public void OnJumpPressed()
    {
        _jumpQueued = true;
    }

    private void FlipSprite()
    {
        float h = joystick != null ? joystick.Horizontal : 0f;
        if (Mathf.Abs(h) < 0.05f) return;
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(Mathf.Abs(s.x) * Mathf.Sign(h), s.y, s.z);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
