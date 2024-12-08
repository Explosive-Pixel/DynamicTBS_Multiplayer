using UnityEngine;
using UnityEngine.UI;

public class LobbyDetailsHandler : MonoBehaviour
{
    [SerializeField] private GameObject lobbyNotAvailableInfo;
    [SerializeField] private GameObject content;

    [SerializeField] private TMPro.TMP_Text lobbyName;
    [SerializeField] private TMPro.TMP_Text lobbyType;

    [SerializeField] private TMPro.TMP_Text bluePlayerName;
    [SerializeField] private TMPro.TMP_Text pinkPlayerName;
    [SerializeField] private GameObject waitingForBluePlayer;
    [SerializeField] private GameObject waitingForPinkPlayer;

    [SerializeField] private TMPro.TMP_Text spectators;
    [SerializeField] private Button joinAsSpectatorButton;

    private Lobby SelectedLobby { get; set; }

    private void Awake()
    {
        joinAsSpectatorButton.onClick.AddListener(() => JoinLobby());
    }

    public void UpdateInfo(Lobby selectedLobby)
    {
        SelectedLobby = selectedLobby;

        if (content == null || lobbyNotAvailableInfo == null)
            return;

        content.SetActive(selectedLobby != null);
        lobbyNotAvailableInfo.SetActive(selectedLobby == null);

        if (selectedLobby == null)
            return;

        lobbyName.text = selectedLobby.LobbyId.FullId;
        lobbyType.text = selectedLobby.IsPrivate ? "PRIVATE" : "PUBLIC";

        HandlePlayerText(selectedLobby, PlayerType.blue);
        HandlePlayerText(selectedLobby, PlayerType.pink);

        spectators.text = selectedLobby.SpectatorCount.ToString();
        joinAsSpectatorButton.gameObject.SetActive(!Client.InLobby);
    }

    private void HandlePlayerText(Lobby selectedLobby, PlayerType side)
    {
        ClientInfo player = selectedLobby.GetPlayer(side);

        if (player == null)
        {
            if (side == PlayerType.blue)
            {
                bluePlayerName.gameObject.SetActive(false);
                waitingForBluePlayer.SetActive(true);
            }
            else
            {
                pinkPlayerName.gameObject.SetActive(false);
                waitingForPinkPlayer.SetActive(true);
            }
        }
        else
        {
            if (side == PlayerType.blue)
            {
                bluePlayerName.gameObject.SetActive(true);
                bluePlayerName.text = player.name;
                waitingForBluePlayer.SetActive(false);
            }
            else
            {
                pinkPlayerName.gameObject.SetActive(true);
                pinkPlayerName.text = player.name;
                waitingForPinkPlayer.SetActive(false);
            }
        }

    }

    private void JoinLobby()
    {
        if (SelectedLobby == null)
            return;

        Client.JoinLobby(SelectedLobby.LobbyId.FullId, ClientType.SPECTATOR);
    }
}
