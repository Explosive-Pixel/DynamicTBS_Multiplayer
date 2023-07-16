using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipAction : MonoBehaviour
{
    public static void Execute(int numberOfActionsToSkip = 1)
    {
        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = PlayerManager.CurrentPlayer,
            ExecutedActionType = ActionType.Skip,
            ActionCount = numberOfActionsToSkip,
            CharacterInAction = null,
            CharacterInitialPosition = null,
            ActionDestinationPosition = null
        });
    }
}
