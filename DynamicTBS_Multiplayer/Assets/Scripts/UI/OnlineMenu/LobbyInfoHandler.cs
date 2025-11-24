using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using System.Linq;

public class LobbyInfoHandler : MonoBehaviour
{
    [System.Serializable]
    public class LobbyStatusEntry
    {
        public LobbyStatus lobbyStatus;
        public GameObject text;
    }

    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text playerCount;
    [SerializeField] private TMP_Text spectatorCount;
    [SerializeField] private TMP_Text draftAndPlacementTime;
    [SerializeField] private TMP_Text gameplayTime;
    [SerializeField] private TMP_Text map;

    [SerializeField] private LobbyStatusEntry[] lobbyStatusEntries;

    public Lobby Lobby { get; private set; }

    public Lobby Init(LobbyInfo lobbyInfo)
    {
        Lobby = new(lobbyInfo);

        lobbyName.text = Lobby.LobbyId.FullId;
        playerCount.text = Lobby.PlayerCount + "/2";
        spectatorCount.text = Lobby.SpectatorCount.ToString();
        draftAndPlacementTime.text = TimerUtils.FormatTime(Lobby.GameConfig.timerConfig.draftAndPlacementTime);
        gameplayTime.text = TimerUtils.FormatTime(Lobby.GameConfig.timerConfig.gameplayTime);
        map.text = Lobby.GameConfig.mapType.Description().ToUpper();

        lobbyStatusEntries.ToList().ForEach(entry => entry.text.SetActive(entry.lobbyStatus == Lobby.Status));

        return Lobby;
    }
}
