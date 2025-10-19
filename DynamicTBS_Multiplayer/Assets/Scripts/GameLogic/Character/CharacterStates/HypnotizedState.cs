using UnityEngine;

public class HypnotizedState : MonoBehaviour, IState
{
    private Character character;

    public static HypnotizedState Create(GameObject parent)
    {
        HypnotizedState hs = parent.AddComponent<HypnotizedState>();
        hs.Init();
        return hs;
    }

    private void Init()
    {
        character = gameObject.GetComponent<Character>();
        SwapSide();

        ActionHandler.Instance.InstantiateAllActionPositions(character);
        GameplayEvents.ChangeCharacterSelection(character);

        GameplayEvents.OnCharacterSelectionChange += Destroy;
        GameplayEvents.OnFinishAction += Destroy;
        GameplayEvents.OnPlayerTurnAborted += Destroy;
    }

    private void SwapSide()
    {
        character.Side = PlayerManager.GetOtherSide(character.Side);
    }

    public void Destroy()
    {
        Destroy(this);
    }

    private void Destroy(Character selectedCharacter)
    {
        if (selectedCharacter != character)
        {
            Destroy();
        }
    }

    private void Destroy(Action action)
    {
        Destroy();
    }

    private void Destroy(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        Destroy();
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCharacterSelectionChange -= Destroy;
        GameplayEvents.OnFinishAction -= Destroy;
        GameplayEvents.OnPlayerTurnAborted -= Destroy;
        SwapSide();
    }
}
