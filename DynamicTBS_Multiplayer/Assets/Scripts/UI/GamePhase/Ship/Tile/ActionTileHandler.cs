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
        if (SceneChangeManager.Instance.CurrentScene != Scene.TUTORIAL && (!GameplayManager.UIPlayerActionAllowed || !PlayerManager.ClientIsCurrentPlayer() || (action.CharacterInAction != null && action.CharacterInAction.Side != PlayerManager.CurrentPlayer)))
            return;

        bool actionFinished = ActionHandler.Instance.ExecuteAction(action, gameObject);

        if (actionFinished)
            GameplayEvents.ChangeCharacterSelection(null);
    }
}
