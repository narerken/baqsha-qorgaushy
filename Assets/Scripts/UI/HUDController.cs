using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TMP_Text heartsLabel; 
    [SerializeField] private TMP_Text scoreLabel;  
    [SerializeField] private Button pauseButton;  
    [SerializeField] private PausePanel pausePanel;

    private int _lives = 3;
    private int _score = 0;

    private void Start()
    {
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
        if (heartsLabel != null)
            heartsLabel.text = new string('♥', _lives);
    }

    private void RefreshScore()
    {
        if (scoreLabel != null)
            scoreLabel.text = $"SCORE {_score}";
    }
}