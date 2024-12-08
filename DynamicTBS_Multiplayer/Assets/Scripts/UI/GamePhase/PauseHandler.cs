using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    private void Awake()
    {
        GameplayEvents.OnGamePause += TogglePauseCanvas;

        TogglePauseCanvas(false);
    }

    public static void HandlePause()
    {
        if (GameManager.GameType == GameType.ONLINE)
            GameplayEvents.UIActionExecuted(PlayerManager.ExecutingPlayer, GameplayManager.gameIsPaused ? UIAction.UNPAUSE_GAME : UIAction.PAUSE_GAME);
        else
            GameplayEvents.PauseGame(!GameplayManager.gameIsPaused);
    }

    private void TogglePauseCanvas(bool paused)
    {
        gameObject.SetActive(paused);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGamePause -= TogglePauseCanvas;
    }
}
