using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject healPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> healTargets = new();
    public List<GameObject> ActionDestinations { get { return healTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<Character> buffedCharacters = new();

    private List<GameObject> patternTargets = new();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
        GameplayEvents.OnFinishAction += DebuffCharacter;
    }

    public void ShowActionPattern(Character character)
    {
        Tile tile = Board.GetTileByCharacter(character);

        List<Vector3> patternPositions = Board.GetTilesInAllStarDirections(tile, HealAA.range)
            .ConvertAll(tile => tile.gameObject.transform.position);

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

        if (healPositions != null && healPositions.Count > 0)
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
            Character characterToHeal = tile.CurrentInhabitant;
            int hitPointsBeforeHeal = characterToHeal.HitPoints;
            characterToHeal.Heal(HealAA.healingPoints);

            if (characterToHeal.HitPoints > hitPointsBeforeHeal)
            {
                characterToHeal.MoveSpeed += HealAA.moveSpeedBuff;
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
        Tile characterTile = Board.GetTileByCharacter(character);

        List<Tile> healTiles = Board.GetTilesOfClosestCharactersOfSideInAllStarDirections(characterTile, character.Side, HealAA.range)
            .FindAll(tile => tile.IsOccupied() && tile.CurrentInhabitant.isHealableBy(character) && !tile.CurrentInhabitant.HasFullHP());

        List<Vector3> healPositions = healTiles.ConvertAll(tile => tile.gameObject.transform.position);

        return healPositions;
    }

    private void DebuffCharacter(ActionMetadata actionMetadata)
    {
        if (actionMetadata.ExecutedActionType == ActionType.Move)
        {
            int bufferCount = BufferCount(actionMetadata.CharacterInAction);
            if (bufferCount > 0)
            {
                actionMetadata.CharacterInAction.MoveSpeed -= (HealAA.moveSpeedBuff * bufferCount);
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
        // TODO: Make nicer (not hard coded)
        GameObject child = UIUtils.FindChildGameObject(character.gameObject, "Speedup");
        int bufferCount = BufferCount(character);
        child.GetComponent<TMPro.TextMeshPro>().text = "+" + bufferCount.ToString();
        child.SetActive(bufferCount > 0);
    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
        GameplayEvents.OnFinishAction -= DebuffCharacter;
    }
}
