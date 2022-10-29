using UnityEngine;
using UnityEngine.UI;

public class ActiveAbilityHandler : MonoBehaviour
{
    [SerializeField] private Button activeAbilityButton;
    private Character currentCharacter;

    private void Awake()
    {
        SubscribeEvents();
        ChangeButtonVisibility(null);
    }

    public void ExecuteActiveAbility()
    {
        GameplayEvents.ExecuteActiveAbilityStarted();
        currentCharacter.GetActiveAbility().Execute();
        // Please remember to call this after every execution (in AAHandler classes): GameplayEvents.ActionFinished(UIActionType.ActiveAbility);
    }

    private void ChangeButtonVisibility(Character character)
    {
        currentCharacter = character;
        if (character == null)
            activeAbilityButton.gameObject.SetActive(false);
        else
            activeAbilityButton.gameObject.SetActive(true);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameplayEvents.OnCharacterSelectionChange += ChangeButtonVisibility;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnCharacterSelectionChange -= ChangeButtonVisibility;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}