using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActiveAbilityButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject activeAbilityTriggerPink;
    [SerializeField] private GameObject activeAbilityTriggerBlue;

    private Character currentCharacter;

    private void Awake()
    {
        SubscribeEvents();
        UpdateAppearance();
    }

    private void ChangeButtonVisibility(Character character)
    {
        currentCharacter = character;

        UpdateAppearance();
    }

    private void ChangeButtonVisibility(ActionMetadata actionMetadata)
    {
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        SetActive(activeAbilityTriggerPink, PlayerType.pink);
        SetActive(activeAbilityTriggerBlue, PlayerType.blue);
    }

    private void SetActive(GameObject aaTrigger, PlayerType side)
    {
        aaTrigger.SetActive(currentCharacter != null && currentCharacter.Side == side && currentCharacter.MayPerformActiveAbility());
    }

    private void Activate(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.GAMEPLAY)
        {
            GameplayEvents.OnCharacterSelectionChange += ChangeButtonVisibility;
            GameplayEvents.OnFinishAction += ChangeButtonVisibility;
        }
        else
        {
            GameplayEvents.OnCharacterSelectionChange -= ChangeButtonVisibility;
            GameplayEvents.OnFinishAction -= ChangeButtonVisibility;
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += Activate;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnCharacterSelectionChange -= ChangeButtonVisibility;
        GameplayEvents.OnFinishAction -= ChangeButtonVisibility;
        GameEvents.OnGamePhaseStart -= Activate;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}