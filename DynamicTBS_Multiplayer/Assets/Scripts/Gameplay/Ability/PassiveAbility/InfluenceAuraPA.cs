using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InfluenceAuraPA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private int maxInfluence; //= 3;
    [SerializeField] private PatternType influenceAuraPatternType; // = PatternType.Star;

    private CharacterMB owner;

    private readonly Dictionary<CharacterMB, int> influencePoints = new();

    private void Awake()
    {
        owner = gameObject.GetComponent<CharacterMB>();
    }

    public void Apply()
    {
        GameplayEvents.OnPlayerTurnEnded += UpdateInfluences;
    }

    private void UpdateInfluences(PlayerType side)
    {
        if (side != owner.Side)
        {
            List<CharacterMB> characters = CharacterManager.GetAllLivingCharactersOfSide(side)
                .FindAll(character => character.PassiveAbility.GetType() != typeof(InfluenceAuraPA));

            foreach (CharacterMB character in characters)
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

    private void SwapSides(CharacterMB character)
    {
        influencePoints.Remove(character);
        UpdateInfluenceAnimator(character, 0);

        if (character != null)
        {
            character.Side = PlayerManager.GetOtherSide(character.Side);

            // TODO: Figure out how to swap
            //CharacterMB newCharacter = CharacterFactoryMB.CreateCharacter(character.CharacterType, character.Side);
            //newCharacter.gameObject.GetComponents(typeof(Component)).ToList().ForEach(component => Destroy(component));
            //character.gameObject.GetComponents(typeof(Component)).ToList().ForEach(component => newCharacter.gameObject.AddComponent(component));

            // Change all sprites from childs to sprites of childs of prefab of other side
            /*GameObject newPrefab = character.GetCharacterPrefab(character.side);
            for(int i = 0; i < newPrefab.transform.childCount; i++)
            {
                GameObject child = character.GetCharacterGameObject().transform.GetChild(i).gameObject;
                if(child.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                {
                    spriteRenderer.sprite = newPrefab.transform.GetChild(i).gameObject.GetComponent<SpriteRenderer>().sprite;
                }
            }*/
        }
    }

    private void UpdateInfluenceAnimator(CharacterMB character, int influence)
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