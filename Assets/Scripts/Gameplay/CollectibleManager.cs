using UnityEngine;
using System;

/// <summary>
/// Counts collectibles in the current level so LevelGoal can check whether
/// the player picked up everything before letting them finish.
/// </summary>
public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    public int TotalCollectibles { get; private set; }
    public int CollectedCount { get; private set; }

    public event Action<int, int> OnCollectedChanged; // (collected, total)

    public bool AllCollected => TotalCollectibles == 0 || CollectedCount >= TotalCollectibles;

    private void Awake()
    {
        Instance = this;
        TotalCollectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None).Length;
    }

    public void RegisterCollected()
    {
        CollectedCount++;
        OnCollectedChanged?.Invoke(CollectedCount, TotalCollectibles);
    }
}
