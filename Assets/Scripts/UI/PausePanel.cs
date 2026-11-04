using UnityEngine;
public class PausePanel : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public void RestartLevel()
    {
        GameManager.Instance.RestartLevel();
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }
}