using UnityEngine;

public class TimeAndMapInfoHandler : MonoBehaviour
{
    [SerializeField] private GameObject timeAndMapInfo;
    [SerializeField] private GameObject rematchInfo;

    [SerializeField] private TMPro.TMP_Text draftAndPlacementTimeInfo;
    [SerializeField] private TMPro.TMP_Text gameplayTimeInfo;

    [SerializeField] private TMPro.TMP_Text mapCategoryInfo;
    [SerializeField] private TMPro.TMP_Text mapTypeInfo;

    public void UpdateInfo(GameConfig gameConfig)
    {
        timeAndMapInfo.SetActive(gameConfig != null);
        rematchInfo.SetActive(Client.InLobby && gameConfig == null);

        if (gameConfig == null)
            return;

        TimerSetup timerSetup = new(gameConfig.timerConfig);
        draftAndPlacementTimeInfo.text = timerSetup.DraftAndPlacementTimeFormatted;
        gameplayTimeInfo.text = timerSetup.GameplayTimeFormatted;

        MapSetup mapSetup = new(gameConfig.mapType);
        mapCategoryInfo.text = mapSetup.MapCategory;
        mapTypeInfo.text = mapSetup.MapType.Description();

        MapLayout mapLayout = timeAndMapInfo.GetComponentInChildren<MapLayout>();
        mapLayout.Init(mapSetup.MapType);
    }
}
