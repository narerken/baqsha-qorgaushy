using UnityEngine;

/// <summary>
/// Put this on the MainMenu's Canvas (or SafeArea) object and wire the Play
/// and Quit buttons to THIS component's methods, not directly to a GameManager
/// object in the scene.
///
/// Why: GameManager is a DontDestroyOnLoad singleton. Every time MainMenu is
/// (re)loaded, a fresh GameManager object is created in it, and its own
/// Awake() immediately destroys itself if a singleton instance already
/// exists (e.g. after returning here from Level01). A button wired straight
/// to that local, now-destroyed object will silently do nothing. Routing the
/// call through this component (which is NOT destroyed) and reading
/// GameManager.Instance at click time avoids that dangling-reference bug.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.PlayGame();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    // Button hookups (Inspector -> OnClick):
    //   "PLAY" button -> MainMenuController.PlayGame
    //   "QUIT" button -> MainMenuController.QuitGame
}
