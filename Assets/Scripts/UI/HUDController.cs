using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Hearts (assign 3 heart Image objects, left to right)")]
    [SerializeField] private Image[] heartIcons;
    [SerializeField] private Sprite heartFullSprite;
    [SerializeField] private Sprite heartEmptySprite;

    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private PausePanel pausePanel;

    private int _lives = 3;
    private int _score = 0;

    private void Start()
    {
        _lives = heartIcons != null ? heartIcons.Length : 3;
        pauseButton.onClick.AddListener(() => pausePanel.Show());
        RefreshHearts();
        RefreshScore();
    }

    public void AddScore(int amount)
    {
        _score += amount;
        RefreshScore();
    }

    public void LoseLife()
    {
        _lives = Mathf.Max(0, _lives - 1);
        RefreshHearts();
        if (_lives == 0)
        {
            // in the future InshaAllah
        }
    }

    private void RefreshHearts()
    {
        if (heartIcons == null) return;
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] == null) continue;
            heartIcons[i].sprite = i < _lives ? heartFullSprite : heartEmptySprite;
        }
    }

    private void RefreshScore()
    {
        if (scoreLabel != null)
            scoreLabel.text = $"SCORE {_score}";
    }
}