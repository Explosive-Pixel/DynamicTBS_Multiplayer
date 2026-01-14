using UnityEngine;

public class OnlineLobbyClosedHandler : MonoBehaviour
{
    [SerializeField] private GameObject closedLobbyInfo_regularly;
    [SerializeField] private GameObject closedLobbyInfo_unexpected;
    [SerializeField] private GameObject closedLobbyInfo_unexpected_spectator;

    private void Awake()
    {
        gameObject.SetActive(GameManager.GameType == GameType.ONLINE && !Client.InLobby);

        if (GameManager.GameType == GameType.ONLINE)
        {
            MessageReceiver.OnWSMessageReceive += ActivateLobbyClosedCanvas;
        }
    }

    private void ActivateLobbyClosedCanvas(WSMessage msg)
    {
        if (msg.code == WSMessageCode.WSMsgCloseLobbyCode)
        {
            bool regularly = ((WSMsgCloseLobby)msg).regularly;
            SetActive(regularly);
        }
        else if (msg.code == WSMessageCode.WSMsgServerNotificationCode)
        {
            SetActive(false);
        }
    }

    private void SetActive(bool regularly)
    {
        gameObject.SetActive(true);
        closedLobbyInfo_regularly.SetActive(regularly);
        closedLobbyInfo_unexpected.SetActive(!regularly && Client.Role != ClientType.SPECTATOR);
        closedLobbyInfo_unexpected_spectator.SetActive(!regularly && Client.Role == ClientType.SPECTATOR);
    }

    private void OnDestroy()
    {
        MessageReceiver.OnWSMessageReceive -= ActivateLobbyClosedCanvas;
    }
}
