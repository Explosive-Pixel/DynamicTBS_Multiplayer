using UnityEngine;
using UnityEngine.UI;

public class ActiveAbilityHandler : MonoBehaviour
{
    [SerializeField] private Button activeAbilityButton;
    private Character currentCharacter;

    private bool gameHasStarted = false;

    private void Awake()
    {
        SubscribeEvents();
        ChangeButtonVisibility(null);
    }

    public void ExecuteActiveAbility()
    {
        GameplayEvents.ExecuteActiveAbilityStarted();
        currentCharacter.GetActiveAbility().Execute();
        currentCharacter.SetActiveAbilityOnCooldown();
        // Please remember to call this after every execution (in AAHandler classes): GameplayEvents.ActionFinished(UIActionType.ActiveAbility);
    }

    private void ChangeButtonVisibility(Character character)
    {
        currentCharacter = character;

        bool active = gameHasStarted && character != null;
        bool disabled = active && character.IsActiveAbilityOnCooldown();

        activeAbilityButton.gameObject.SetActive(active);
        activeAbilityButton.interactable = !disabled;
    }

    private void SetGameHasStarted()
    {
        gameHasStarted = true;
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += SetGameHasStarted;
        GameplayEvents.OnCharacterSelectionChange += ChangeButtonVisibility;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= SetGameHasStarted;
        GameplayEvents.OnCharacterSelectionChange -= ChangeButtonVisibility;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}