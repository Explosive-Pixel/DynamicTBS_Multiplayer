using UnityEngine;

public class OnlineLobbyLostConnectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    [SerializeField] private GameObject notConnectedInfo;
    [SerializeField] private GameObject reconnectInfo;
    [SerializeField] private GameObject connectionDeadInfo;

    [SerializeField] private GameObject opponentLostConnectionInfo;

    private void Awake()
    {
        canvas.SetActive(false);
    }

    void Update()
    {
        if (GameManager.GameType != GameType.ONLINE || GameManager.CurrentGamePhase == GamePhase.NONE || Client.Role != ClientType.PLAYER)
            return;

        canvas.SetActive(!Client.IsConnectedToServer || !Client.CurrentLobby.IsFull);
        notConnectedInfo.SetActive(!Client.IsConnectedToServer);
        reconnectInfo.SetActive(Client.ConnectionStatus == ConnectionState.RECONNECTING);
        connectionDeadInfo.SetActive(Client.ConnectionStatus == ConnectionState.DEAD);
        opponentLostConnectionInfo.SetActive(Client.IsConnectedToServer && !Client.CurrentLobby.IsFull);
    }
}
