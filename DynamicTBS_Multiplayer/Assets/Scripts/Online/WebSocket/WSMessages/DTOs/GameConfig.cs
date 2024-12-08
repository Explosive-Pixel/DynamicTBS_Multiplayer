using System;

[Serializable]
public class GameConfig
{
    [Serializable]
    public class TimerConfig
    {
        public float draftAndPlacementTime;
        public float gameplayTime;
        public bool gameplayTimePerTurn;
    }

    public TimerConfig timerConfig;
    public MapType mapType;
}
