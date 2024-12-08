using System.Collections.Generic;
using UnityEngine;

public class InfluenceAuraPA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private int maxInfluence; //= 3;
    [SerializeField] private PatternType influenceAuraPatternType; // = PatternType.Star;

    [SerializeField] private GameObject masterTakeoverProgression;
    [SerializeField] private GameObject characterSprite;
    [SerializeField] private Color influenceColorPink;
    [SerializeField] private Color influenceColorBlue;

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

        if (character == null)
            return;

        UpdateInfluenceAnimator(character, 0);
        character.Side = PlayerManager.GetOtherSide(character.Side);
    }

    private void UpdateInfluenceAnimator(Character character, int influence)
    {
        /* GameObject child = UIUtils.FindChildGameObject(character.gameObject, masterTakeoverProgression.name);
         Animator animator = child.GetComponentInChildren<Animator>();

         if (influence == 0)
             animator.Rebind();

         UIUtils.UpdateAnimator(animator, influence);*/

        GameObject sprite = UIUtils.FindChildGameObject(character.gameObject, characterSprite.name);
        var assets = sprite.GetComponentsInChildren<SpriteRenderer>();
        var defaultColor = new Color(0.981f, 0.981f, 0.981f, 1);

        for (int i = 0; i < assets.Length; i++)
        {
            assets[i].color = i < influence ? (character.Side == PlayerType.pink ? influenceColorBlue : influenceColorPink) : defaultColor;
        }
    }

    private void OnDestroy()
    {
        GameplayEvents.OnPlayerTurnEnded -= UpdateInfluences;
    }
}