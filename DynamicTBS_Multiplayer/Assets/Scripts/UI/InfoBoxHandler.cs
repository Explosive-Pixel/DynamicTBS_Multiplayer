using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;

public class InfoBoxHandler : MonoBehaviour
{
    [SerializeField] private GameObject units;
    [SerializeField] private GameObject activeAbilities;
    [SerializeField] private GameObject passiveAbilities;
    [SerializeField] private GameObject buttons;

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

        buttons.SetActive(character == null);

        UnitIcons.ForEach(unitIcon => unitIcon.gameObject.SetActive(character != null && unitIcon.character == character.CharacterType && unitIcon.side == character.Side));
        ActiveAbilityIcons.ForEach(aaIcon => aaIcon.gameObject.SetActive(character != null && aaIcon.activeAbilityType == character.ActiveAbility.AbilityType && aaIcon.disabled == !character.MayPerformActiveAbility() && (aaIcon.side == character.Side || aaIcon.disabled)));
        PassiveAbilityIcons.ForEach(paIcon => paIcon.gameObject.SetActive(character != null && paIcon.passiveAbilityType == character.PassiveAbility.AbilityType && paIcon.disabled == character.PassiveAbility.IsDisabled() && (paIcon.side == character.Side || paIcon.disabled)));
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCharacterSelectionChange -= UpdateInfoBox;
    }
}
