using UnityEngine;

/// <summary>
/// The finish point of the level. Only completes the level if every
/// Collectible has been picked up first - this is the level's goal
/// ("collect everything + reach the finish"). If the player isn't done
/// yet, shows an optional "collect everything first" hint instead.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelGoal : MonoBehaviour
{
    [SerializeField] private GameObject lockedFeedback;
    [SerializeField] private GameObject winFeedback;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        bool allCollected = CollectibleManager.Instance == null || CollectibleManager.Instance.AllCollected;

        if (!allCollected)
        {
            if (lockedFeedback != null) lockedFeedback.SetActive(true);
            return;
        }

        if (winFeedback != null) winFeedback.SetActive(true);

        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();

        Debug.Log("Level complete! All collectibles gathered and finish reached.");
    }
}
