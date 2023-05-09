using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

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

    private string directory;
    private string filePath;
    private Statistics stats;
    public Statistics Stats { get { return stats; } }

    #region Helper Classes

    [Serializable]
    public class Statistics
    {
        public int gamesTotal = 0;
        public List<MapStatistics> mapStatistics = new List<MapStatistics>();
        public List<CharacterStatistics> characterStatistics = new List<CharacterStatistics>();
        public List<DraftStatistics> draftStatistics = new List<DraftStatistics>();

        public List<MapStatistics> MapStatistics { get { return mapStatistics.OrderBy(ms => ms.map).ToList(); } }
        public List<CharacterStatistics> CharacterStatistics { get { return characterStatistics.OrderBy(cs => cs.draftTotal).ThenByDescending(cs => cs.character).ToList(); } }
    }

    [Serializable]
    public class MapStatistics
    {
        public MapType map;
        public int gamesTotal = 0;
        public int drawGamesTotal = 0;
        public List<WinnerCount> winsTotalPerPlayer = new List<WinnerCount>();

        public MapStatistics(MapType map)
        {
            this.map = map;
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

    [Serializable]
    public class CharacterStatistics
    {
        public CharacterType character;
        public int draftTotal; // number between 0 and <number of characters in one team>
        public int count = 0;

        public CharacterStatistics(CharacterType character, int draftTotal)
        {
            this.character = character;
            this.draftTotal = draftTotal;
        }
    }

    [Serializable]
    public class DraftStatistics
    {
        public List<CharacterType> uniqueDraft;
        public List<MapCount> mapCounts = new List<MapCount>();

        public DraftStatistics(List<CharacterType> uniqueDraft)
        {
            this.uniqueDraft = uniqueDraft;
            this.uniqueDraft.Sort();
        }

        public string GetName()
        {
            string name = "";
            uniqueDraft.ForEach(c => name += c.Name().Substring(0, 1) + ":");
            return name.Substring(0, name.Length - 1);
        }
    }

    [Serializable]
    public class MapCount
    {
        public MapType mapType;
        public int count = 0;
        public int winCount = 0;

        public MapCount(MapType mapType)
        {
            this.mapType = mapType;
        }
    }

    public MapStatistics GetMapStatistics(MapType map)
    {
        MapStatistics mapStatistics = stats.mapStatistics.Find(ms => ms.map == map);
        if(mapStatistics == null)
        {
            mapStatistics = new MapStatistics(map);
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

    public CharacterStatistics GetCharacterStatistics(CharacterType character, int draftTotal)
    {
        CharacterStatistics characterStatistics = stats.characterStatistics.Find(cs => cs.character == character && cs.draftTotal == draftTotal);
        if(characterStatistics == null)
        {
            characterStatistics = new CharacterStatistics(character, draftTotal);
            stats.characterStatistics.Add(characterStatistics);
        }

        return characterStatistics;
    }

    public DraftStatistics GetDraftStatistics(List<CharacterType> uniqueDraft)
    {
        uniqueDraft.Sort();
        DraftStatistics draftStatistics = stats.draftStatistics.Find(ds => ds.uniqueDraft.SequenceEqual(uniqueDraft));
        if(draftStatistics == null)
        {
            draftStatistics = new DraftStatistics(uniqueDraft);
            stats.draftStatistics.Add(draftStatistics);
        }
        return draftStatistics;
    }

    public MapCount GetMapCount(DraftStatistics draftStatistics, MapType mapType)
    {
        MapCount mapCount = draftStatistics.mapCounts.Find(mc => mc.mapType == mapType);
        if(mapCount == null)
        {
            mapCount = new MapCount(mapType);
            draftStatistics.mapCounts.Add(mapCount);
        }

        return mapCount;
    }

    #endregion

    private void Init()
    {
        SubscribeEvents();

        try
        {
            directory = Application.dataPath + "/Resources/GameStatistics";
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
        if(msg.GetType() == typeof(MsgGameOver))
        {
            RecordGameStats((MsgGameOver)msg);
            ExportToCSV();
        }
    }

    private void RecordGameStats(MsgGameOver msg)
    {
        stats.gamesTotal++;

        Lobby lobby = OnlineServer.Instance.FindLobby(msg.LobbyId);

        RecordMapStatistics(msg, lobby);
        RecordCharacterStatistics(lobby);
        RecordDraftStatistics(msg, lobby);
    }

    private void RecordMapStatistics(MsgGameOver msg, Lobby lobby)
    {
        MapStatistics mapStats = GetMapStatistics(lobby.SelectedMap);
        mapStats.gamesTotal++;

        if (msg.isDraw)
        {
            mapStats.drawGamesTotal++;
            return;
        }

        GetWinnerCount(mapStats, msg.winner).count++;
    }

    private void RecordCharacterStatistics(Lobby lobby)
    {
        foreach(KeyValuePair<PlayerType, List<CharacterType>> draftByPlayer in lobby.Draft)
        {
            List<CharacterType> distinctDraftedCharacters = draftByPlayer.Value.Distinct().ToList();
            foreach(CharacterType character in distinctDraftedCharacters)
            {
                GetCharacterStatistics(character, draftByPlayer.Value.Count(c => c == character)).count++;
            }
        }
    }

    private void RecordDraftStatistics(MsgGameOver msg, Lobby lobby)
    {
        foreach (KeyValuePair<PlayerType, List<CharacterType>> draftByPlayer in lobby.Draft)
        {
            MapCount mapCount = GetMapCount(GetDraftStatistics(draftByPlayer.Value), lobby.SelectedMap);
            mapCount.count++;

            if(!msg.isDraw && msg.winner == draftByPlayer.Key)
            {
                mapCount.winCount++;
            }
        }
    }

    private void ExportToCSV()
    {
        if (directory == null)
            return;

        string path = directory + "/statistics.csv";

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("SKYRATS STATISTICS");
            writer.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            writer.WriteLine();
            writer.WriteLine();

            ExportMapStatisticsToCSV(writer);
            writer.WriteLine();
            ExportUnitStatisticsToCSV(writer);
            writer.WriteLine();
            ExportDraftStatisticsToCSV(writer);
            writer.WriteLine();
        }      
    }

    private void ExportMapStatisticsToCSV(StreamWriter writer)
    {
        writer.WriteLine("I. GAME");
        writer.WriteLine();
        writer.WriteLine("Games total;" + stats.gamesTotal);

        writer.WriteLine();
        string header = ";Map played;Map played (in %)";
        GetPlayerTypes().ForEach(playerType => header += ";" + playerType + " wins on map");
        GetPlayerTypes().ForEach(playerType => header += ";" + playerType + " wins on map (in %)");
        writer.WriteLine(header);

        foreach (MapType map in GetMapTypes())
        {
            var mapStatistics = GetMapStatistics(map);
            string line = "Games on " + map + ";" + mapStatistics.gamesTotal + ";" + ToPercent(mapStatistics.gamesTotal, stats.gamesTotal);
            GetPlayerTypes().ForEach(playerType => line += ";" + GetWinnerCount(mapStatistics, playerType).count);
            GetPlayerTypes().ForEach(playerType => line += ";" + ToPercent(GetWinnerCount(mapStatistics, playerType).count, mapStatistics.gamesTotal));
            writer.WriteLine(line);
        }

        writer.WriteLine();
        writer.WriteLine(";Wins;Wins (in %)");
        foreach (PlayerType playerType in GetPlayerTypes())
        {
            int winsTotal = GetMapTypes().Sum(mt => GetWinnerCount(GetMapStatistics(mt), playerType).count);
            writer.WriteLine(playerType + " wins;" + winsTotal + ";" + ToPercent(winsTotal, stats.gamesTotal));
        }

        writer.WriteLine();
    }

    private void ExportUnitStatisticsToCSV(StreamWriter writer)
    {
        writer.WriteLine("II. UNITS");
        writer.WriteLine();

        string header = "Number x of occurences in a draft / Unit";
        GetCharacterTypes().ForEach(c => header += ";" + c.Name());
        header += ";Total number of units that occured x times in a draft; Total number of units that occured x times in a draft (in %)";
        writer.WriteLine(header);

        int total = GetDraftNumbers().Sum(x => GetCharacterTypes().Sum(c => GetCharacterStatistics(c, x).count));
        foreach (int x in GetDraftNumbers())
        {
            string line = x.ToString();
            GetCharacterTypes().ForEach(c => line += ";" + GetCharacterStatistics(c, x).count);
            int x_total = GetCharacterTypes().Sum(c => GetCharacterStatistics(c, x).count);
            line += ";" + x_total + ";" + ToPercent(x_total, total);
            writer.WriteLine(line);
        }

        string new_header = ";";
        GetCharacterTypes().ForEach(c => new_header += ";");
        writer.WriteLine(new_header + "Total number of drafted characters");

        string total_line = "Total number of occurences in all drafts / Unit";
        string total_line_percent = "Total number of occurences in all drafts / Unit (in %)";
        int total_drafts = 0;
        foreach (var c in GetCharacterTypes())
        {
            int c_total = GetDraftNumbers().Sum(x => x*GetCharacterStatistics(c, x).count);
            total_drafts += c_total;
            total_line += ";" + c_total;
            total_line_percent += ";" + ToPercent(c_total, total);
        }
        total_line += ";" + total_drafts;
        writer.WriteLine(total_line);
        writer.WriteLine(total_line_percent);

        writer.WriteLine();
    }

    private void ExportDraftStatisticsToCSV(StreamWriter writer)
    {
        writer.WriteLine("III. DRAFT");
        writer.WriteLine();

        string header = "Unique Drafts";
        GetMapTypes().ForEach(mapType => header += ";Times played on " + mapType + ";Times played on " + mapType + " (in %);Wins on " + mapType + ";Wins on " + mapType + " (in %)");
        header += ";Total times played;Total times played (in %);Total wins with this draft;Total wins with this draft (in %)";
        writer.WriteLine(header);

        foreach(DraftStatistics draftStatistics in stats.draftStatistics)
        {
            string line = draftStatistics.GetName();
            int total_count = draftStatistics.mapCounts.Sum(mc => mc.count);
            int total_win_count = draftStatistics.mapCounts.Sum(mc => mc.winCount);
            foreach(MapType mapType in GetMapTypes())
            {
                MapCount mapCount = GetMapCount(draftStatistics, mapType);
                line += ";" + mapCount.count + ";" + ToPercent(mapCount.count, total_count) + ";" + mapCount.winCount + ";" + ToPercent(mapCount.winCount, total_win_count);
            }
            line += ";" + total_count + ";" + ToPercent(total_count, stats.gamesTotal) + ";" + total_win_count + ";" + ToPercent(total_win_count, stats.gamesTotal);
            writer.WriteLine(line);
        }

        writer.WriteLine();
    }

    private List<PlayerType> GetPlayerTypes()
    {
        var playerTypes = new List<PlayerType>();
        foreach(PlayerType playerType in Enum.GetValues(typeof(PlayerType)))
        {
            playerTypes.Add(playerType);
        }
        return playerTypes;
    }

    private List<MapType> GetMapTypes()
    {
        var mapTypes = new List<MapType>();
        foreach (MapType mapType in Enum.GetValues(typeof(MapType)))
        {
            mapTypes.Add(mapType);
        }
        return mapTypes;
    }

    private List<CharacterType> GetCharacterTypes()
    {
        var characterTypes = new List<CharacterType>();
        foreach(CharacterType characterType in Enum.GetValues(typeof(CharacterType)))
        {
            if (characterType == CharacterType.MasterChar)
                continue;

            characterTypes.Add(characterType);
        }
        return characterTypes;
    }

    private List<int> GetDraftNumbers()
    {
        var draftNumbers = new List<int>();
        for (int x = 0; x <= DraftManager.MaxDraftCount / 2; x++)
        {
            draftNumbers.Add(x);
        }
        return draftNumbers;
    }

    private string ToPercent(int nominator, int denominator)
    {
        if (nominator == 0 && denominator == 0)
            return "-";

        double value = denominator == 0 ? 0 : ((double)nominator / (double)denominator);
        return String.Format("{0:0.0%}", value);
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

            ExportToCSV();
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
