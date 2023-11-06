using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro playerNamePink;
    [SerializeField] private TMPro.TextMeshPro playerNameBlue;

    private void Update()
    {
        if (GameManager.gameType == GameType.ONLINE)
        {
            playerNamePink.text = OnlineClient.Instance.GetPlayerName(PlayerType.pink);
            playerNameBlue.text = OnlineClient.Instance.GetPlayerName(PlayerType.blue);
        }
    }
}
