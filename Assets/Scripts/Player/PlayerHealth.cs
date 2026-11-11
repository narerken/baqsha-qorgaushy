using UnityEngine;
using System.Collections;

/// <summary>
/// Player's hit points, tied into the existing HUDController hearts. Any
/// enemy or projectile calls TakeHit() on contact. Grants a short
/// invulnerability + flicker after each hit so one touch can't drain all
/// three lives in one frame.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private float knockbackForce = 4f;
    [SerializeField] private SpriteRenderer sprite;

    private bool _invulnerable;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void TakeHit(int damage, Vector2 fromPosition)
    {
        Debug.Log("[DamageDebug] TakeHit called. invulnerable=" + _invulnerable + " HUDInstance=" + (HUDController.Instance != null));
        if (_invulnerable) return;

        if (HUDController.Instance != null)
            HUDController.Instance.LoseLife();

        if (_rb != null)
        {
            Vector2 away = ((Vector2)transform.position - fromPosition).normalized;
            _rb.AddForce(away * knockbackForce, ForceMode2D.Impulse);
        }

        StartCoroutine(InvulnerabilityFlicker());
    }

    private IEnumerator InvulnerabilityFlicker()
    {
        _invulnerable = true;
        float elapsed = 0f;
        while (elapsed < invulnerabilityDuration)
        {
            if (sprite != null) sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        if (sprite != null) sprite.enabled = true;
        _invulnerable = false;
    }
}
