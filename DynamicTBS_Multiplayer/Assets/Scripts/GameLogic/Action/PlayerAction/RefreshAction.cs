using UnityEngine;
using System.Linq;

public class RefreshAction : MonoBehaviour, IPlayerAction
{
    public PlayerActionType PlayerActionType { get { return PlayerActionType.Refresh; } }

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public bool IsActionAvailable(PlayerType player)
    {
        return CharacterManager.GetAllLivingCharactersOfSide(player).Count(p => p.IsActiveAbilityOnCooldown()) > 0;
    }

    public void ExecuteAction(PlayerType player)
    {
        CharacterManager.GetAllLivingCharactersOfSide(player).ForEach(character => character.ResetActiveAbilityCooldown());
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
