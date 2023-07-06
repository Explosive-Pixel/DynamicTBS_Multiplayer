using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceAuraPA : MonoBehaviour, IPassiveAbility
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

        if (!character.IsDead())
        {
            character.side = PlayerManager.GetOtherPlayer(character.side);

            // Change all sprites from childs to sprites of childs of prefab of other side
            GameObject newPrefab = character.GetCharacterPrefab(character.side);
            for(int i = 0; i < newPrefab.transform.childCount; i++)
            {
                GameObject child = character.GetCharacterGameObject().transform.GetChild(i).gameObject;
                if(child.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                {
                    spriteRenderer.sprite = newPrefab.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
    }

    private void UpdateInfluenceAnimator(Character character, int influence)
    {
        GameObject child = UIUtils.FindChildGameObject(character.GetCharacterGameObject(), "MasterTakeoverProgression");
        UIUtils.UpdateAnimator(child.GetComponent<Animator>(), influence);
    }

    ~InfluenceAuraPA()
    {
        GameplayEvents.OnPlayerTurnEnded -= UpdateInfluences;
    }
}