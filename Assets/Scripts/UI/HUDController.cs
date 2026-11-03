using UnityEngine;
using UnityEngine.UI;


public class HUDController : MonoBehaviour
{
    [SerializeField] private Text heartsLabel;   // top-left, anchored (0,1)
    [SerializeField] private Text scoreLabel;    // top-center, anchored (0.5,1)
    [SerializeField] private Button pauseButton; // top-right, anchored (1,1)
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
            // next assignments
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
