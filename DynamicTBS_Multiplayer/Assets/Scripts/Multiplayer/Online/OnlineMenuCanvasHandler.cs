using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineMenuCanvasHandler : MonoBehaviour
{
    [SerializeField] private OnlineClient client;

    [SerializeField] private InputField userName;
    [SerializeField] private InputField lobbyNameOrId;
    [SerializeField] private Toggle spectatorToggle;

    [SerializeField] private GameObject lobbyCanvas;

    public void CreateLobby()
    {
        JoinLobby(new LobbyId(lobbyNameOrId.text));
    }

    public void JoinLobby()
    {
        JoinLobby(LobbyId.FromFullId(lobbyNameOrId.text));
    }

    private void JoinLobby(LobbyId lobbyId)
    {
        UserData userData = new UserData(userName.text, spectatorToggle.isOn ? ClientType.spectator : ClientType.player);
        client.Init("127.0.0.1", 8007, userData, lobbyId);

        lobbyCanvas.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void BackToMainMenu()
    {
        client.Shutdown();
    }
}
