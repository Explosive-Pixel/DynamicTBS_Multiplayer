using UnityEngine;
using UnityEngine.UI;

public class ActiveAbilityButtonHandler : MonoBehaviour
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
        currentCharacter.GetActiveAbility().Execute();
        // Please remember to call this after every execution (in AAHandler classes): GameplayEvents.ActionFinished(actionMetadata);

        activeAbilityButton.interactable = false;
    }

    private void ChangeButtonVisibility(Character character)
    {
        currentCharacter = character;

        bool active = GameplayManager.HasGameStarted() && character != null;
        bool disabled = active && (character.IsActiveAbilityOnCooldown() || character.isDisabled());

        activeAbilityButton.gameObject.SetActive(active);
        activeAbilityButton.interactable = !disabled;
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