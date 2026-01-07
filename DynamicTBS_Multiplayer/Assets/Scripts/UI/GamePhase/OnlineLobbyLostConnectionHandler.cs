using UnityEngine;

public class OnlineLobbyLostConnectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    [SerializeField] private GameObject notConnectedInfo;
    [SerializeField] private GameObject connectionInstableInfo;
    [SerializeField] private GameObject reconnectInfo;
    [SerializeField] private GameObject connectionDeadInfo;

    [SerializeField] private GameObject opponentLostConnectionInfo;

    private void Awake()
    {
        canvas.SetActive(false);
    }

    void Update()
    {
        if (GameManager.GameType != GameType.ONLINE || GameManager.CurrentGamePhase == GamePhase.NONE)
            return;

        notConnectedInfo.SetActive(!Client.IsConnectedToServer);
        connectionInstableInfo.SetActive(Client.ConnectionStatus == ConnectionState.INSTABLE);
        reconnectInfo.SetActive(Client.ConnectionStatus == ConnectionState.RECONNECTING);
        connectionDeadInfo.SetActive(Client.ConnectionStatus == ConnectionState.DEAD);
        opponentLostConnectionInfo.SetActive(Client.IsConnectedToServer && !Client.CurrentLobby.IsFull);

        canvas.SetActive(notConnectedInfo.activeSelf || connectionInstableInfo.activeSelf || reconnectInfo.activeSelf || connectionDeadInfo.activeSelf || opponentLostConnectionInfo.activeSelf);
    }
}
