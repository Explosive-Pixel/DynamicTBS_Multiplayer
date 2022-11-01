using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceAuraPA : IPassiveAbility
{
    private static int maxInfluence = 3;
    private static PatternType influenceAuraPatternType = PatternType.Cross;

    private Character owner;
    private CharacterHandler characterHandler;
    private PlayerManager playerManager;

    private Dictionary<Character, int> influencePoints = new Dictionary<Character, int>();

    public InfluenceAuraPA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        characterHandler = GameObject.Find("GameplayCanvas").GetComponent<CharacterHandler>();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

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
            List<Character> characters = characterHandler.GetAllLivingCharacters()
                .FindAll(character => character.GetSide() == player);

            foreach(Character character in characters)
            {
                if (characterHandler.Neighbors(owner, character, influenceAuraPatternType))
                {
                    if (!influencePoints.ContainsKey(character))
                        influencePoints.Add(character, 0);

                    influencePoints[character] += 1;

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
        character.side = playerManager.GetOtherPlayer(character.side);
        character.GetCharacterGameObject().GetComponent<SpriteRenderer>().sprite = character.GetCharacterSprite(character.side);
    }

    ~InfluenceAuraPA()
    {
        GameplayEvents.OnPlayerTurnEnded -= UpdateInfluences;
    }
}