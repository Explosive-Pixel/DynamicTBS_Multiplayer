using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTileHandler : MonoBehaviour
{
    private IAction action;

    public static ActionTileHandler Create(IAction action, GameObject actionTile)
    {
        ActionTileHandler actionTileHandler = actionTile.AddComponent<ActionTileHandler>();
        actionTileHandler.action = action;
        return actionTileHandler;
    }

    private void OnMouseDown()
    {
        if (GameManager.IsSpectator())
            return;

        Character characterInAction = action.CharacterInAction;
        Vector3 initialPosition = characterInAction.gameObject.transform.position;
        Vector3 actionDestinationPosition = gameObject.transform.position;

        action.ExecuteAction(gameObject);
        ActionUtils.ResetActionDestinations();

        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = characterInAction.Side,
            ExecutedActionType = action.ActionType,
            CharacterInAction = characterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestinationPosition
        });
    }
}
