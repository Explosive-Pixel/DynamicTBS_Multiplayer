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
    
    // Update is called once per frame
    void Update()
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

        foreach(var mapStatistics in StatisticRecorder.Instance.Stats.mapStatistics)
        {
            statistics.text += "\nTotal number of games on map " + mapStatistics.boardDesignIndex + ": " + mapStatistics.gamesTotal;
        }

        statistics.text += "\n\n";

        foreach (var mapStatistics in StatisticRecorder.Instance.Stats.mapStatistics)
        {
            statistics.text += "\nTotal number of draws on map " + mapStatistics.boardDesignIndex + ": " + mapStatistics.drawGamesTotal;
            foreach(var winnerCount in mapStatistics.winsTotalPerPlayer)
            {
                statistics.text += "\nTotal number of wins of player " + winnerCount.winner + " on map " + mapStatistics.boardDesignIndex + ": " + winnerCount.count;
            }
            statistics.text += "\n";
        }
    }
}
