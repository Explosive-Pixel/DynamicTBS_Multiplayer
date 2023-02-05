using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceAuraPA : IPassiveAbility
{
    private static int maxInfluence = 3;
    private static PatternType influenceAuraPatternType = PatternType.Star;

    private Character owner;

    private Dictionary<Character, int> influencePoints = new Dictionary<Character, int>();

    public InfluenceAuraPA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        GameplayEvents.OnPlayerTurnEnded += UpdateInfluences;
    }

    private void UpdateInfluences(Player player)
    {
        if(owner.IsDead())
        {
            GameplayEvents.OnPlayerTurnEnded -= UpdateInfluences;
            return;
        }

        if (player != owner.GetSide())
        {
            List<Character> characters = CharacterHandler.GetAllLivingCharacters()
                .FindAll(character => character.GetSide() == player && character.GetPassiveAbility().GetType() != typeof(InfluenceAuraPA));

            foreach(Character character in characters)
            {
                if (CharacterHandler.Neighbors(owner, character, influenceAuraPatternType))
                {
                    if (!influencePoints.ContainsKey(character))
                        influencePoints.Add(character, 0);

                    influencePoints[character] += 1;
                    UpdateInfluenceAnimator(character, influencePoints[character]);

                    if(influencePoints[character] == maxInfluence)
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

        character.side = PlayerManager.GetOtherPlayer(character.side);
        GameObject characterSpriteGameObject = UIUtils.FindChildGameObject(character.GetCharacterGameObject(), "CharacterSprite");
        characterSpriteGameObject.GetComponent<SpriteRenderer>().sprite = character.GetCharacterSprite(character.side);
    }

    private void UpdateInfluenceAnimator(Character character, int influence)
    {
        GameObject child = UIUtils.FindChildGameObject(character.GetCharacterGameObject(), "MasterTakeoverProgression");
        UIUtils.UpdateAnimator(child.GetComponent<Animator>(), influence);
        child.SetActive(influence > 0);
    }

    ~InfluenceAuraPA()
    {
        GameplayEvents.OnPlayerTurnEnded -= UpdateInfluences;
    }
}