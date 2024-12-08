using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfoBoxHandler : MonoBehaviour
{
    [SerializeField] private GameObject units;
    [SerializeField] private GameObject activeAbilities;
    [SerializeField] private GameObject activeAbilityWriting;
    [SerializeField] private GameObject passiveAbilities;
    [SerializeField] private GameObject passiveAbilityWriting;
    [SerializeField] private GameObject buttons;
    [SerializeField] private List<GameObject> confirmationObjects;

    private List<CharacterClass> UnitIcons;
    private List<AbilityClass> ActiveAbilityIcons;
    private List<AbilityClass> PassiveAbilityIcons;

    private void Awake()
    {
        UnitIcons = units.GetComponentsInChildren<CharacterClass>(true).ToList();
        UnitIcons.ForEach(unitIcon => unitIcon.gameObject.SetActive(false));

        ActiveAbilityIcons = activeAbilities.GetComponentsInChildren<AbilityClass>(true).ToList();
        ActiveAbilityIcons.ForEach(aaIcon => aaIcon.gameObject.SetActive(false));

        PassiveAbilityIcons = passiveAbilities.GetComponentsInChildren<AbilityClass>(true).ToList();
        PassiveAbilityIcons.ForEach(paIcon => paIcon.gameObject.SetActive(false));

        GameplayEvents.OnCharacterSelectionChange += UpdateInfoBox;
    }

    private void UpdateInfoBox(Character character)
    {
        if (GameManager.CurrentGamePhase != GamePhase.PLACEMENT && GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        buttons.SetActive(character == null && !UIUtils.ContainsActive(confirmationObjects));

        UnitIcons.ForEach(unitIcon => unitIcon.gameObject.SetActive(character != null && unitIcon.character == character.CharacterType && unitIcon.side == character.Side));
        ActiveAbilityIcons.ForEach(aaIcon => aaIcon.gameObject.SetActive(character != null && aaIcon.activeAbilityType == character.ActiveAbility.AbilityType && aaIcon.disabled == !character.MayPerformActiveAbility() && (aaIcon.side == character.Side || aaIcon.disabled)));
        activeAbilityWriting.SetActive(character != null);
        PassiveAbilityIcons.ForEach(paIcon => paIcon.gameObject.SetActive(character != null && paIcon.passiveAbilityType == character.PassiveAbility.AbilityType && paIcon.disabled == character.PassiveAbility.IsDisabled() && (paIcon.side == character.Side || paIcon.disabled)));
        passiveAbilityWriting.SetActive(character != null);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCharacterSelectionChange -= UpdateInfoBox;
    }
}
