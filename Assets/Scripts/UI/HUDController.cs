using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the HUD: hearts (lives, as a full/empty sprite swap on each
/// heart's Image), score text, and the pause button. Other scripts talk
/// to this through the singleton - PlayerHealth calls LoseLife() on a
/// hit, Collectible calls AddScore() on pickup.
///
/// Field names/types match exactly what was already wired in Level01's
/// Inspector (heartIcons as Image[] with full/empty sprites, scoreLabel
/// as TMP text, pauseButton + pausePanel + gameOverPanel) so the
/// existing scene references reconnect automatically - nothing needs to
/// be re-dragged. This replaces a version of this file that was
/// accidentally overwritten with a duplicate copy of
/// PlayerController2D's code (see DECISIONS.md).
/// </summary>
public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("Lives")]
    [SerializeField] private Image[] heartIcons;
    [SerializeField] private Sprite heartFullSprite;
    [SerializeField] private Sprite heartEmptySprite;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private string scoreFormat = "SCORE {0}";

    [Header("Pause")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private PausePanel pausePanel;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    public int CurrentLives { get; private set; }
    public int CurrentScore { get; private set; }

    private void Awake()
    {
        Instance = this;
        CurrentLives = heartIcons != null && heartIcons.Length > 0 ? heartIcons.Length : 3;
        CurrentScore = 0;
    }

    private void Start()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseButtonPressed);

        RefreshHearts();
        RefreshScore();
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        RefreshScore();
    }

    public void LoseLife()
    {
        if (CurrentLives <= 0) return;

        CurrentLives = Mathf.Max(0, CurrentLives - 1);
        RefreshHearts();

        if (CurrentLives == 0)
        {
            GameOver();
        }
    }

    /// <summary>Wired in code (Start()) to the pause button's onClick - matches the
    /// scene, which has an empty m_OnClick persistent call list.</summary>
    public void OnPauseButtonPressed()
    {
        if (pausePanel != null) pausePanel.Show();
    }

    private void RefreshHearts()
    {
        if (heartIcons == null) return;
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] == null) continue;
            heartIcons[i].sprite = i < CurrentLives ? heartFullSprite : heartEmptySprite;
        }
    }

    private void RefreshScore()
    {
        if (scoreLabel != null)
            scoreLabel.text = string.Format(scoreFormat, CurrentScore);
    }

    private void GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.PauseGame();
    }
}
