public class CaptainChar : Character
{
    private void Start()
    {
        characterType = CharacterType.CaptainChar;
    }

    private void Awake()
    {
        GameplayEvents.OnFinishAction += CheckIfOwnerMovedOntoGoalSquare;
    }

    private void CheckIfOwnerMovedOntoGoalSquare(ActionMetadata actionMetadata)
    {
        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        if (CurrentTile != null && CurrentTile.TileType.Equals(TileType.GoalTile))
        {
            GameplayEvents.GameIsOver(Side, GameOverCondition.CAPTAIN_TOOK_CONTROL);
        }
    }

    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(Side), GameOverCondition.CAPTAIN_DIED);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnFinishAction -= CheckIfOwnerMovedOntoGoalSquare;
    }
}