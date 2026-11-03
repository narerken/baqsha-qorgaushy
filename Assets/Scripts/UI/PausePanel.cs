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
}
