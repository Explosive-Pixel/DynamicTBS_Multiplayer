using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineMenuCanvasHandler : MonoBehaviour
{
    [SerializeField] private WSClient wsClient;

    [SerializeField] private InputField userName;
    [SerializeField] private InputField lobbyNameOrId;
    [SerializeField] private Toggle spectatorToggle;

    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;

    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private GameObject onlineMetadataCanvas;

    private void Awake()
    {
        onlineMetadataCanvas.SetActive(false);
    }

    private void Update()
    {
        createLobbyButton.interactable = IsValidName() && IsValidLobbyName();
        joinLobbyButton.interactable = IsValidName() && IsValidLobbyID();
    }

    public void CreateLobby()
    {
        JoinLobby(new LobbyId(lobbyNameOrId.text), true);
    }

    public void JoinLobby()
    {
        JoinLobby(LobbyId.FromFullId(lobbyNameOrId.text), false);
    }

    private void JoinLobby(LobbyId lobbyId, bool newLobby)
    {
        GameManager.GameType = GameType.ONLINE;

        wsClient.Init(ConfigManager.Instance.Hostname, spectatorToggle.isOn ? ClientType.SPECTATOR : ClientType.PLAYER, userName.text.Trim(), lobbyId, newLobby);
        //wsClient.Init("2a00:10:7175:6301:e8dc:f050:567f:99b0", 8007, spectatorToggle.isOn ? ClientType.SPECTATOR : ClientType.PLAYER, userName.text.Trim(), lobbyId, newLobby);


        onlineMetadataCanvas.SetActive(true);
        SceneChangeManager.Instance.LoadScene(Scene.GAME_SETUP);
    }

    private bool IsValidName()
    {
        return userName.text.Trim().Length > 0;
    }

    private bool IsValidLobbyName()
    {
        return lobbyNameOrId.text.Trim().Length > 0 && !lobbyNameOrId.text.Contains("#");
    }

    private bool IsValidLobbyID()
    {
        return lobbyNameOrId.text.Trim().Length > 0 && LobbyId.FromFullId(lobbyNameOrId.text) != null;
    }
}
