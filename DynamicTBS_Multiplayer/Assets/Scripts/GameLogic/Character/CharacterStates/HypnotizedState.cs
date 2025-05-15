using UnityEngine;

public class HypnotizedState : MonoBehaviour, IState
{
    private Character character;
    private Character hypnotizedBy;

    public static HypnotizedState Create(GameObject parent, Character hypnotizedBy)
    {
        HypnotizedState hs = parent.AddComponent<HypnotizedState>();
        hs.hypnotizedBy = hypnotizedBy;
        HypnotizeAA.HypnotizeAAExecuted(hypnotizedBy, hypnotizedBy.transform.position);
        hs.Init();
        return hs;
    }

    private void Init()
    {
        character = gameObject.GetComponent<Character>();
        SwapSide();
        GameplayEvents.ChangeCharacterSelection(character);
        CharacterManager.SelectedCharacter.Select();

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

    private void Destroy(ActionMetadata actionMetadata)
    {
        if (actionMetadata.CharacterInAction == character)
        {
            hypnotizedBy.SetActiveAbilityOnCooldown();
        }
        Destroy();
    }

    private void Destroy(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        Destroy();
    }

    private void OnDestroy()
    {
        HypnotizeAA.HypnotizeAAExecuted(null, null);
        GameplayEvents.OnCharacterSelectionChange -= Destroy;
        GameplayEvents.OnFinishAction -= Destroy;
        GameplayEvents.OnPlayerTurnAborted -= Destroy;
        SwapSide();
    }
}
