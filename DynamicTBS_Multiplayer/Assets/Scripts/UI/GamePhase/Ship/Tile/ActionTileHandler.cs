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
        if (!PlayerManager.ClientIsCurrentPlayer() || GameplayManager.gameIsPaused || action.CharacterInAction.Side != PlayerManager.CurrentPlayer)
            return;

        bool actionFinished = ActionUtils.ExecuteAction(action, gameObject);

        if (actionFinished)
            GameplayEvents.ChangeCharacterSelection(null);
    }
}
