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

    private void Awake()
    {
        timeAndMapInfo.SetActive(GameSetup.SetupCompleted);
        rematchInfo.SetActive(!GameSetup.SetupCompleted);

        if (!GameSetup.SetupCompleted)
            return;

        draftAndPlacementTimeInfo.text = GameSetup.TimerSetup.DraftAndPlacementTimeFormatted;
        gameplayTimeInfo.text = GameSetup.TimerSetup.GameplayTimeFormatted;

        mapCategoryInfo.text = GameSetup.MapSetup.MapCategory;
        mapTypeInfo.text = GameSetup.MapSetup.MapType.ToString();
    }
}
