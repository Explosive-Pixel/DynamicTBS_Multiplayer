using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipAction : MonoBehaviour
{
    public static void Execute()
    {
        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = PlayerManager.GetCurrentPlayer(),
            ExecutedActionType = ActionType.Skip
        });
    }
}
