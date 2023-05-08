using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

public class StatisticRecorder : MonoBehaviour
{
    #region SingletonImplementation
    public static StatisticRecorder Instance { set; get; }

    public static bool IsActive { get { return Instance != null && Instance.Stats != null; } }

    private void Awake()
    {
        Instance = this;
        Init();
    }

    #endregion

    [Serializable]
    public class Statistics
    {
        public int gamesTotal = 0;
        public List<MapStatistics> mapStatistics = new List<MapStatistics>();
    }

    [Serializable]
    public class MapStatistics
    {
        public int boardDesignIndex;
        public int gamesTotal = 0;
        public int drawGamesTotal = 0;
        public List<WinnerCount> winsTotalPerPlayer = new List<WinnerCount>();

        public MapStatistics(int boardDesignIndex)
        {
            this.boardDesignIndex = boardDesignIndex;
        }
    }

    [Serializable]
    public class WinnerCount
    {
        public PlayerType winner;
        public int count = 0;

        public WinnerCount(PlayerType winner)
        {
            this.winner = winner;
        }
    }

    private string filePath;
    private Statistics stats;

    public Statistics Stats { get { return stats; } }

    public MapStatistics GetMapStatistics(int boardDesignIndex)
    {
        MapStatistics mapStatistics = stats.mapStatistics.Find(ms => ms.boardDesignIndex == boardDesignIndex);
        if(mapStatistics == null)
        {
            mapStatistics = new MapStatistics(boardDesignIndex);
            stats.mapStatistics.Add(mapStatistics);
        }
        return mapStatistics;
    }

    public WinnerCount GetWinnerCount(MapStatistics mapStatistics, PlayerType winner)
    {
        WinnerCount winnerCount = mapStatistics.winsTotalPerPlayer.Find(wc => wc.winner == winner);
        if(winnerCount == null)
        {
            winnerCount = new WinnerCount(winner);
            mapStatistics.winsTotalPerPlayer.Add(winnerCount);
        } 

        return winnerCount;
    }

    private void Init()
    {
        SubscribeEvents();

        try
        {
            string directory = Application.dataPath + "/Resources/GameStatistics";
            Directory.CreateDirectory(directory);
            filePath = directory + "/Statistics.json";

            LoadStats();
        }
        catch (Exception ex)
        {
            Debug.LogError("Cannot record stats: " + ex.ToString());
            filePath = null;
            UnsubscribeEvents();
        }
    }

    private void RecordMessage(OnlineMessage msg)
    {
        if(msg.GetType() == typeof(MsgStartGame)) 
        {
            RecordStartGame((MsgStartGame)msg);
        } else if(msg.GetType() == typeof(MsgGameOver))
        {
            RecordGameOver((MsgGameOver)msg);
        }
    }

    private void RecordStartGame(MsgStartGame msg)
    {
        
    }

    private void RecordGameOver(MsgGameOver msg)
    {
        stats.gamesTotal++;

        Lobby lobby = OnlineServer.Instance.FindLobby(msg.LobbyId);

        MapStatistics mapStats = GetMapStatistics(lobby.BoardDesignIndex);
        mapStats.gamesTotal++;

        if (msg.isDraw)
        {
            mapStats.drawGamesTotal++;
            return;
        }

        GetWinnerCount(mapStats, msg.winner).count++;
    }

    private void LoadStats()
    {
        if (filePath == null)
            return;

        if (File.Exists(filePath))
        {
            string loadString = File.ReadAllText(filePath);
            stats = JsonUtility.FromJson<Statistics>(loadString);
        }
        else
        {
            stats = new Statistics();
            SaveStats();
        }
    }

    private void SaveStats()
    {
        if (filePath == null)
            return;

        try
        {
            string saveString = JsonUtility.ToJson(stats);
            File.WriteAllText(filePath, saveString);
        }
        catch (Exception ex)
        {
            Debug.LogError("Cannot save stats: " + ex.ToString());
            filePath = null;
            UnsubscribeEvents();
        }
    }

    private void SubscribeEvents()
    {
        ServerEvents.OnMessageReceive += RecordMessage;
    }

    private void UnsubscribeEvents()
    {
        ServerEvents.OnMessageReceive -= RecordMessage;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
        SaveStats();
    }
}
