using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject healPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> healTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return healTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<Character> buffedCharacters = new List<Character>();

    private List<GameObject> patternTargets = new List<GameObject>();

    private void Awake()
    {
        GameplayEvents.OnGameplayPhaseStart += Register;
        GameplayEvents.OnFinishAction += DebuffCharacter;
    }

    public void ShowActionPattern(Character character)
    {
        Tile tile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Vector3> patternPositions = Board.GetTilesInAllStarDirections(tile, HealAA.range)
            .ConvertAll(tile => tile.GetPosition());

        patternTargets = ActionUtils.InstantiateActionPositions(patternPositions, healPrefab);
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> healPositions = FindHealPositions(character);

        if (healPositions != null)
        {
            return healPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> healPositions = FindHealPositions(character);

        if(healPositions != null && healPositions.Count > 0)
        {
            healTargets = ActionUtils.InstantiateActionPositions(healPositions, healPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Character characterToHeal = tile.GetCurrentInhabitant();
            int hitPointsBeforeHeal = characterToHeal.HitPoints;
            characterToHeal.Heal(HealAA.healingPoints);

            if (characterToHeal.HitPoints > hitPointsBeforeHeal)
            {
                characterToHeal.moveSpeed += HealAA.moveSpeedBuff;
                buffedCharacters.Add(characterToHeal);
                UpdateBufferGameObject(characterToHeal);
            }
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(healTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<Vector3> FindHealPositions(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> healTiles = Board.GetTilesOfClosestCharactersOfSideInAllStarDirections(characterTile, character.GetSide().GetPlayerType(), HealAA.range)
            .FindAll(tile => tile.IsOccupied() && tile.GetCurrentInhabitant().isHealableBy(character) && !tile.GetCurrentInhabitant().HasFullHP());

        List<Vector3> healPositions = healTiles.ConvertAll(tile => tile.GetPosition());

        return healPositions;
    }

    private void DebuffCharacter(ActionMetadata actionMetadata)
    {
        if(actionMetadata.ExecutedActionType == ActionType.Move)
        {
            int bufferCount = BufferCount(actionMetadata.CharacterInAction);
            if(bufferCount > 0)
            {
                actionMetadata.CharacterInAction.moveSpeed -= (HealAA.moveSpeedBuff * bufferCount);
                buffedCharacters = buffedCharacters.FindAll(c => c != actionMetadata.CharacterInAction);
                UpdateBufferGameObject(actionMetadata.CharacterInAction);
            }
        }
    }

    private int BufferCount(Character character)
    {
        return buffedCharacters.FindAll(c => c == character).Count;
    }

    private void UpdateBufferGameObject(Character character)
    {
        GameObject child = UIUtils.FindChildGameObject(character.GetCharacterGameObject(), "Speedup");
        int bufferCount = BufferCount(character);
        child.GetComponent<TMPro.TextMeshPro>().text = "+" + bufferCount.ToString();
        child.SetActive(bufferCount > 0);
    }

    private void Register()
    {
        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameplayPhaseStart -= Register;
        GameplayEvents.OnFinishAction -= DebuffCharacter;
    }
}
