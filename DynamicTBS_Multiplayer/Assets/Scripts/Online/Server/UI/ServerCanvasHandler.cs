using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class ServerCanvasHandler : MonoBehaviour
{

    [SerializeField] private Text connectionCount;
    [SerializeField] private Text lobbyCount;
    [SerializeField] private Text status;
    [SerializeField] private Text ip;

    [SerializeField] private Text statistics;
    
    private void Update()
    {
        if (!OnlineServer.Instance)
            return;

        status.text = "Server is active: " + OnlineServer.Instance.IsActive;
        ip.text = "Server IP: " + OnlineServer.Instance.IP;
        connectionCount.text = "Connected Clients: " + OnlineServer.Instance.ConnectionCount;
        lobbyCount.text = "Active Lobbies: " + OnlineServer.Instance.LobbyCount;

        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        if (!StatisticRecorder.IsActive)
            return;

        statistics.text = "Statistics:\nTotal number of games: " + StatisticRecorder.Instance.Stats.gamesTotal;

        UpdateMapStatistics();
        UpdateCharacterStatistics();
    }

    private void UpdateMapStatistics()
    {
        foreach (var mapStatistics in StatisticRecorder.Instance.Stats.MapStatistics)
        {
            statistics.text += "\nTotal number of games on map " + mapStatistics.map + ": " + mapStatistics.gamesTotal;
        }

        statistics.text += "\n";

        foreach (var mapStatistics in StatisticRecorder.Instance.Stats.MapStatistics)
        {
            statistics.text += "\nTotal number of draws on map " + mapStatistics.map + ": " + mapStatistics.drawGamesTotal;
            foreach (var winnerCount in mapStatistics.winsTotalPerPlayer)
            {
                statistics.text += "\nTotal number of wins of player " + winnerCount.winner + " on map " + mapStatistics.map + ": " + winnerCount.count;
            }
            statistics.text += "\n";
        }
    }

    private void UpdateCharacterStatistics()
    {
        foreach(var characterStatistics in StatisticRecorder.Instance.Stats.CharacterStatistics)
        {
            statistics.text += "\nTotal number of drafts where chracter " + characterStatistics.character + " has been drafted " + characterStatistics.draftTotal + " times: " + characterStatistics.count;
        }
    }
}
