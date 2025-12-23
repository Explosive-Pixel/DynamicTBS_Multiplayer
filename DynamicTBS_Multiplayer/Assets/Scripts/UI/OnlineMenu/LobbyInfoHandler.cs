using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

public class LobbyInfoHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class LobbyStatusEntry
    {
        public LobbyStatus lobbyStatus;
        public GameObject text;
    }

    [SerializeField] private GameObject highlightBackground;

    [SerializeField] private TMP_Text lobbyName;
    [SerializeField] private TMP_Text playerCount;
    [SerializeField] private TMP_Text spectatorCount;
    [SerializeField] private TMP_Text draftAndPlacementTime;
    [SerializeField] private TMP_Text gameplayTime;
    [SerializeField] private TMP_Text map;

    [SerializeField] private LobbyStatusEntry[] lobbyStatusEntries;

    public Lobby Lobby { get; private set; }

    private bool selected = false;

    private void Awake()
    {
        MenuEvents.OnChangeLobbySelection += Highlight;
    }

    public Lobby Init(LobbyInfo lobbyInfo, Lobby selectedLobby)
    {
        Lobby = new(lobbyInfo);

        lobbyName.text = Lobby.LobbyId.FullId;
        playerCount.text = Lobby.PlayerCount + "/2";
        spectatorCount.text = Lobby.SpectatorCount.ToString();
        draftAndPlacementTime.text = TimerUtils.FormatTime(Lobby.GameConfig.timerConfig.draftAndPlacementTime);
        gameplayTime.text = TimerUtils.FormatTime(Lobby.GameConfig.timerConfig.gameplayTime);
        map.text = Lobby.GameConfig.mapType.Description().ToUpper();

        lobbyStatusEntries.ToList().ForEach(entry => entry.text.SetActive(entry.lobbyStatus == Lobby.Status));

        Highlight(selectedLobby);

        return Lobby;
    }

    private void Highlight(Lobby selectedLobby)
    {
        selected = Lobby != null && selectedLobby != null && selectedLobby.LobbyId.Id == Lobby.LobbyId.Id;
        highlightBackground.SetActive(selected);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightBackground.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightBackground.SetActive(selected);
    }

    private void OnDestroy()
    {
        MenuEvents.OnChangeLobbySelection -= Highlight;
    }
}
