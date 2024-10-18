using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAndMapInfoHandler : MonoBehaviour
{
    [SerializeField] private GameObject timeAndMapInfo;
    [SerializeField] private GameObject rematchInfo;

    [SerializeField] private TMPro.TMP_Text draftAndPlacementTimeInfo;
    [SerializeField] private TMPro.TMP_Text gameplayTimeInfo;

    [SerializeField] private TMPro.TMP_Text mapCategoryInfo;
    [SerializeField] private TMPro.TMP_Text mapTypeInfo;

    [SerializeField] private bool initialized = false;

    private void Update()
    {
        timeAndMapInfo.SetActive(GameSetup.SetupCompleted);
        rematchInfo.SetActive(!GameSetup.SetupCompleted);

        if (GameSetup.SetupCompleted && !initialized)
            Init();
    }

    private void Init()
    {
        draftAndPlacementTimeInfo.text = GameSetup.TimerSetup.DraftAndPlacementTimeFormatted;
        gameplayTimeInfo.text = GameSetup.TimerSetup.GameplayTimeFormatted;

        mapCategoryInfo.text = GameSetup.MapSetup.MapCategory;
        mapTypeInfo.text = GameSetup.MapSetup.MapType.Description();

        MapLayout mapLayout = timeAndMapInfo.GetComponentInChildren<MapLayout>();
        mapLayout.Init(GameSetup.MapSetup.MapType);

        initialized = true;
    }
}
