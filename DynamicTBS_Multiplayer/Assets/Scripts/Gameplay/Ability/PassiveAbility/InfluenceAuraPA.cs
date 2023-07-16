using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InfluenceAuraPA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private int maxInfluence; //= 3;
    [SerializeField] private PatternType influenceAuraPatternType; // = PatternType.Star;

    public PassiveAbilityType AbilityType { get { return PassiveAbilityType.INFLUENCE_AURA; } }

    private Character owner;

    private readonly Dictionary<Character, int> influencePoints = new();

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
        GameplayEvents.OnPlayerTurnEnded += UpdateInfluences;
    }

    public bool IsDisabled()
    {
        return false;
    }

    private void UpdateInfluences(PlayerType side)
    {
        if (side != owner.Side)
        {
            List<Character> characters = CharacterManager.GetAllLivingCharactersOfSide(side)
                .FindAll(character => character.PassiveAbility.GetType() != typeof(InfluenceAuraPA));

            foreach (Character character in characters)
            {
                if (CharacterManager.Neighbors(owner, character, influenceAuraPatternType))
                {
                    if (!influencePoints.ContainsKey(character))
                        influencePoints.Add(character, 0);

                    influencePoints[character] += 1;
                    UpdateInfluenceAnimator(character, influencePoints[character]);

                    if (influencePoints[character] == maxInfluence)
                    {
                        SwapSides(character);
                    }
                }
            }
        }
    }

    private void SwapSides(Character character)
    {
        influencePoints.Remove(character);
        UpdateInfluenceAnimator(character, 0);

        if (character != null)
        {
            character.Side = PlayerManager.GetOtherSide(character.Side);

            // Change all sprites from childs to sprites of childs of prefab of other side
            GameObject newPrefab = CharacterFactory.GetPrefab(character.CharacterType, character.Side);
            for (int i = 0; i < newPrefab.transform.childCount; i++)
            {
                GameObject child = character.gameObject.transform.GetChild(i).gameObject;
                if (child.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                {
                    spriteRenderer.sprite = newPrefab.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
    }

    private void UpdateInfluenceAnimator(Character character, int influence)
    {
        // TODO
        GameObject child = UIUtils.FindChildGameObject(character.gameObject, "MasterTakeoverProgression");
        UIUtils.UpdateAnimator(child.GetComponent<Animator>(), influence);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnPlayerTurnEnded -= UpdateInfluences;
    }
}