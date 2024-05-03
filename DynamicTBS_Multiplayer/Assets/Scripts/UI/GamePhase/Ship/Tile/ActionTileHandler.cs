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
        if (GameManager.IsSpectator() || GameplayManager.gameIsPaused)
            return;

        ActionUtils.ExecuteAction(action, gameObject);
    }
}
