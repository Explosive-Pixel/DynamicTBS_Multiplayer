using UnityEngine;

public class SkipAction : MonoBehaviour
{
    public static void Execute()
    {
        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = PlayerManager.CurrentPlayer,
            ExecutedActionType = ActionType.Skip,
            CharacterInAction = null,
            CharacterInitialPosition = null,
            ActionDestinationPosition = null
        });
    }
}
