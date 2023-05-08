using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject blockActionPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility;} }

    private GameObject blockTarget = null;
    public List<GameObject> ActionDestinations { get { return blockTarget != null ? new List<GameObject> { blockTarget } : new List<GameObject>(); } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private GameObject patternTarget = null;

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(Character character)
    {
        patternTarget = ActionUtils.InstantiateActionPosition(character.GetCharacterGameObject().transform.position, blockActionPrefab);
    }

    public void HideActionPattern()
    {
        if (patternTarget != null)
            Destroy(patternTarget);

        patternTarget = null;
    }

    public int CountActionDestinations(Character character)
    {
        return 1;
    }

    public void CreateActionDestinations(Character character)
    {
        blockTarget = ActionUtils.InstantiateActionPosition(character.GetCharacterGameObject().transform.position, blockActionPrefab);
        characterInAction = character;
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        ((BlockAA)characterInAction.GetActiveAbility()).ActivateBlock();

        AbortAction();
    }

    public void AbortAction()
    {
        if(blockTarget != null)
            Destroy(blockTarget);

        blockTarget = null;
        ActionRegistry.Remove(this);
        characterInAction = null;
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
    }
}
