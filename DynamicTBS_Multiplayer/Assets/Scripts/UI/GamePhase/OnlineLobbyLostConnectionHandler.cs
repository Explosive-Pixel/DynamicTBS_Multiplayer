using UnityEngine;

public class OnlineLobbyLostConnectionHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject reconnectInfo;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (GameManager.GameType != GameType.ONLINE)
            return;

        gameObject.SetActive(!Client.IsConnectedToServer);
        reconnectInfo.SetActive(Client.ConnectionStatus == ConnectionStatus.ATTEMPT_CONNECTION);
    }
}
