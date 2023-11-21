using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class StatisticRecorder : MonoBehaviour
{
    private const int maxDraftCount = 14;

    #region SingletonImplementation
    public static StatisticRecorder Instance { set; get; }

    public static bool IsActive { get { return Instance != null && Instance.Stats != null; } }

    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Start()
    {
        StartSendDailyCSVReport();
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
        public string recordedSince = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        public int gamesTotal = 0;
        public List<MapStatistics> mapStatistics = new();
        public List<CharacterStatistics> characterStatistics = new();
        public List<DraftStatistics> draftStatistics = new();
    }

    [Serializable]
    public class MapStatistics
    {
        public MapType map;
        public int gamesTotal = 0;
        public int drawGamesTotal = 0;
        public List<WinnerCount> winsTotalPerPlayer = new();
        public List<TimerCount> timerCounts = new();

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
    public class TimerCount
    {
        public TimerSetupType timerSetup;
        public int count = 0;

        public TimerCount(TimerSetupType timerSetup)
        {
            this.timerSetup = timerSetup;
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
        public List<MapCount> mapCounts = new();

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
        if (mapStatistics == null)
        {
            mapStatistics = new MapStatistics(map);
            stats.mapStatistics.Add(mapStatistics);
        }
        return mapStatistics;
    }

    public WinnerCount GetWinnerCount(MapStatistics mapStatistics, PlayerType winner)
    {
        WinnerCount winnerCount = mapStatistics.winsTotalPerPlayer.Find(wc => wc.winner == winner);
        if (winnerCount == null)
        {
            winnerCount = new WinnerCount(winner);
            mapStatistics.winsTotalPerPlayer.Add(winnerCount);
        }

        return winnerCount;
    }

    public TimerCount GetTimerCount(MapStatistics mapStatistics, TimerSetupType timerSetup)
    {
        TimerCount timerCount = mapStatistics.timerCounts.Find(tc => tc.timerSetup == timerSetup);
        if (timerCount == null)
        {
            timerCount = new TimerCount(timerSetup);
            mapStatistics.timerCounts.Add(timerCount);
        }

        return timerCount;
    }

    public CharacterStatistics GetCharacterStatistics(CharacterType character, int draftTotal)
    {
        CharacterStatistics characterStatistics = stats.characterStatistics.Find(cs => cs.character == character && cs.draftTotal == draftTotal);
        if (characterStatistics == null)
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
        if (draftStatistics == null)
        {
            draftStatistics = new DraftStatistics(uniqueDraft);
            stats.draftStatistics.Add(draftStatistics);
        }
        return draftStatistics;
    }

    public MapCount GetMapCount(DraftStatistics draftStatistics, MapType mapType)
    {
        MapCount mapCount = draftStatistics.mapCounts.Find(mc => mc.mapType == mapType);
        if (mapCount == null)
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

    #region Recording

    private void RecordMessage(OnlineMessage msg)
    {
        if (msg.GetType() == typeof(MsgGameOver))
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
        GetTimerCount(mapStats, lobby.Timer.TimerSetup).count++;
    }

    private void RecordCharacterStatistics(Lobby lobby)
    {
        foreach (KeyValuePair<PlayerType, List<CharacterType>> draftByPlayer in lobby.Draft)
        {
            List<CharacterType> distinctDraftedCharacters = draftByPlayer.Value.Distinct().ToList();
            foreach (CharacterType character in distinctDraftedCharacters)
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

            if (!msg.isDraw && msg.winner == draftByPlayer.Key)
            {
                mapCount.winCount++;
            }
        }
    }

    #endregion

    #region CSV Export

    private void ExportToCSV()
    {
        if (directory == null)
            return;

        string path = directory + "/statistics.csv";

        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.WriteLine("SKYMATES STATISTICS");
            writer.WriteLine(DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));
            writer.WriteLine("Games recorded since: " + stats.recordedSince);
            writer.WriteLine();
            writer.WriteLine();

            ExportMapStatisticsToCSV(writer);
            writer.WriteLine();
            ExportUnitStatisticsToCSV(writer);
            writer.WriteLine();
            ExportDraftStatisticsToCSV(writer);
            writer.WriteLine();
            ExportTimeStatisticsToCSV(writer);
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
            int c_total = GetDraftNumbers().Sum(x => x * GetCharacterStatistics(c, x).count);
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

        foreach (DraftStatistics draftStatistics in stats.draftStatistics)
        {
            string line = draftStatistics.GetName();
            int total_count = draftStatistics.mapCounts.Sum(mc => mc.count);
            int total_win_count = draftStatistics.mapCounts.Sum(mc => mc.winCount);
            foreach (MapType mapType in GetMapTypes())
            {
                MapCount mapCount = GetMapCount(draftStatistics, mapType);
                line += ";" + mapCount.count + ";" + ToPercent(mapCount.count, total_count) + ";" + mapCount.winCount + ";" + ToPercent(mapCount.winCount, total_win_count);
            }
            line += ";" + total_count + ";" + ToPercent(total_count, stats.gamesTotal) + ";" + total_win_count + ";" + ToPercent(total_win_count, stats.gamesTotal);
            writer.WriteLine(line);
        }

        writer.WriteLine();
    }

    private void ExportTimeStatisticsToCSV(StreamWriter writer)
    {
        writer.WriteLine("IV. Time");
        writer.WriteLine();

        string header = "Time / Map";
        GetMapTypes().ForEach(map => header += ";Played on " + map);
        header += ";Total played;Total played (in %)";
        writer.WriteLine(header);

        foreach (TimerSetupType timerSetup in GetTimerSetupTypes())
        {
            string line = timerSetup.ToString();
            GetMapTypes().ForEach(map => line += ";" + GetTimerCount(GetMapStatistics(map), timerSetup).count);
            int total = GetMapTypes().Sum(map => GetTimerCount(GetMapStatistics(map), timerSetup).count);
            line += ";" + total + ";" + ToPercent(total, stats.gamesTotal);
            writer.WriteLine(line);
        }

        writer.WriteLine();
    }

    #endregion

    #region Helper methods

    private List<PlayerType> GetPlayerTypes()
    {
        var playerTypes = new List<PlayerType>();
        foreach (PlayerType playerType in Enum.GetValues(typeof(PlayerType)))
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

    private List<TimerSetupType> GetTimerSetupTypes()
    {
        var timerSetupTypes = new List<TimerSetupType>();
        foreach (TimerSetupType timerSetupType in Enum.GetValues(typeof(TimerSetupType)))
        {
            timerSetupTypes.Add(timerSetupType);
        }
        return timerSetupTypes;
    }

    private List<CharacterType> GetCharacterTypes()
    {
        var characterTypes = new List<CharacterType>();
        foreach (CharacterType characterType in Enum.GetValues(typeof(CharacterType)))
        {
            if (characterType == CharacterType.CaptainChar)
                continue;

            characterTypes.Add(characterType);
        }
        return characterTypes;
    }

    private List<int> GetDraftNumbers()
    {
        var draftNumbers = new List<int>();
        for (int x = 0; x <= maxDraftCount / 2; x++)
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

    #endregion

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

    private void StartSendDailyCSVReport()
    {
        // Bestimme die gew�nschte Uhrzeit f�r die Ausf�hrung (hier um 10:00 Uhr)
        int targetHour = 13;
        int targetMinute = 37;
        int targetSecond = 0;

        // Berechne die Zeit bis zur n�chsten Ausf�hrung
        System.DateTime now = System.DateTime.Now;
        System.DateTime nextRunTime = new System.DateTime(now.Year, now.Month, now.Day, targetHour, targetMinute, targetSecond);
        if (nextRunTime < now)
        {
            nextRunTime = nextRunTime.AddDays(1); // F�hre die Methode am n�chsten Tag aus
        }
        float delayInSeconds = (float)(nextRunTime - now).TotalSeconds;

        // Rufe die Methode jeden Tag um die angegebene Uhrzeit auf
        InvokeRepeating("SendCSVFile", delayInSeconds, 24 * 60 * 60);
    }

    private void SendCSVFile()
    {
        if (directory == null)
            return;

        string path = directory + "/statistics.csv";

        if (File.Exists(path))
        {
            byte[] byteArray = File.ReadAllBytes(path);

            Telegram.SendFile(byteArray, "statistics.csv", "Daily statistics report");
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
