using UnityEngine;

public class OnlineLobbyClosedHandler : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(GameManager.GameType == GameType.ONLINE && !Client.InLobby);

        if (GameManager.GameType == GameType.ONLINE)
            MenuEvents.OnClosedCurrentLobby += ActivateLobbyClosedCanvas;
    }

    private void ActivateLobbyClosedCanvas()
    {
        gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        MenuEvents.OnClosedCurrentLobby += ActivateLobbyClosedCanvas;
    }
}
