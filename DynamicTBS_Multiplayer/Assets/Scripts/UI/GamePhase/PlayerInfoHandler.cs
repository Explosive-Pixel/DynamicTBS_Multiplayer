using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro playerNamePink;
    [SerializeField] private TMPro.TextMeshPro playerNameBlue;

    [SerializeField] private Text youArePlayer;

    private void Awake()
    {
        playerNamePink.text = "Pink";
        playerNameBlue.text = "Blue";
    }

    private void Start()
    {
        youArePlayer.text = "";
        if (GameManager.IsPlayer() && GameManager.GameType == GameType.ONLINE)
        {
            youArePlayer.text = "You are player " + Client.Side + ".";
        }
    }

    private void Update()
    {
        if (playerNamePink == null || playerNameBlue == null)
            return;

        if (GameManager.GameType == GameType.ONLINE && Client.InLobby)
        {
            playerNamePink.text = Client.CurrentLobby.GetPlayerName(PlayerType.pink);
            playerNameBlue.text = Client.CurrentLobby.GetPlayerName(PlayerType.blue);
        }
    }
}
