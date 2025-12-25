using UnityEngine;

public class OnlineLobbyLostConnectionHandler : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject reconnectInfo;

    private void Awake()
    {
        canvas.SetActive(false);
    }

    void Update()
    {
        if (GameManager.GameType != GameType.ONLINE)
            return;

        canvas.SetActive(!Client.IsConnectedToServer);
        reconnectInfo.SetActive(Client.ConnectionStatus == ConnectionStatus.ATTEMPT_CONNECTION);
    }
}
