using TMPro;
using UnityEngine;

public class LobbyInfoHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text playerCount;
    [SerializeField] private TMP_Text spectatorCount;
    [SerializeField] private TMP_Text lobbyStatus;
    [SerializeField] private TMP_Text draftAndPlacementTime;
    [SerializeField] private TMP_Text gameplayTime;
    [SerializeField] private TMP_Text map;

    public Lobby Lobby { get; private set; }

    public Lobby Init(LobbyInfo lobbyInfo)
    {
        Lobby = new(lobbyInfo);

        lobbyName.text = Lobby.LobbyId.FullId;
        playerCount.text = Lobby.PlayerCount + "/2";
        spectatorCount.text = Lobby.SpectatorCount.ToString();
        lobbyStatus.text = Lobby.Status.Description();
        draftAndPlacementTime.text = TimerUtils.FormatTime(Lobby.GameConfig.timerConfig.draftAndPlacementTime);
        gameplayTime.text = TimerUtils.FormatTime(Lobby.GameConfig.timerConfig.gameplayTime);
        map.text = Lobby.GameConfig.mapType.Description().ToUpper();

        return Lobby;
    }
}
