using UnityEngine;

/// <summary>
/// A vegetable/flower the player picks up by walking into it. Adds score,
/// tells CollectibleManager, then destroys itself. Collider2D must be
/// "Is Trigger".
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private AudioClip pickupSound;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (HUDController.Instance != null)
            HUDController.Instance.AddScore(scoreValue);

        if (CollectibleManager.Instance != null)
            CollectibleManager.Instance.RegisterCollected();

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}
