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
        if (!PlayerManager.ClientIsCurrentPlayer() || GameplayManager.gameIsPaused)
            return;

        ActionUtils.ExecuteAction(action, gameObject);
        GameplayEvents.ChangeCharacterSelection(null);
    }
}
