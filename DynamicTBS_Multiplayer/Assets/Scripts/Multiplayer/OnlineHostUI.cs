using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineHostUI : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button selectPinkButton;
    [SerializeField] private Button selectBlueButton;
    [SerializeField] private Text connectedPlayer;

    private bool sideSelected = false;

    private void Awake()
    {
        startGameButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        connectedPlayer.text = "Connected players: " + Server.Instance.PlayerCount;
        if(Server.Instance.PlayerCount == 2 && sideSelected)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }

    public void OnSelectPink()
    {
        OnSelectSide(PlayerType.pink);
        selectPinkButton.interactable = false;
        selectBlueButton.gameObject.SetActive(false);
    }

    public void OnSelectBlue()
    {
        OnSelectSide(PlayerType.blue);
        selectBlueButton.interactable = false;
        selectPinkButton.gameObject.SetActive(false);
    }

    private void OnSelectSide(PlayerType side)
    {
        Client.Instance.SendToServer(new NetWelcome() { AssignedTeam = (int)side, Role = (int)Client.Instance.role});
        sideSelected = true;
    }

    public void OnStartGame()
    {
        Server.Instance.Broadcast(new NetStartGame());
    }
}
