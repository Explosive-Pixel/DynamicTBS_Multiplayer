using UnityEngine;

public class SkipAction : MonoBehaviour, IPlayerAction
{
    public PlayerActionType PlayerActionType { get { return PlayerActionType.Skip; } }

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public bool IsActionAvailable(PlayerType player)
    {
        return PlayerManager.IsCurrentPlayer(player) && GameplayManager.GetRemainingActions() == 1;
    }

    public void ExecuteAction(PlayerType player)
    {

    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.GAMEPLAY)
        {
            PlayerActionRegistry.Register(this);
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
    }
}
