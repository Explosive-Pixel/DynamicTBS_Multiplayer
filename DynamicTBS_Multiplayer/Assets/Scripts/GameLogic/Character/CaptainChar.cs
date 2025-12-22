using UnityEngine;

public class CaptainChar : Character
{
    private bool isDead = false;

    private void Start()
    {
        characterType = CharacterType.CaptainChar;
    }

    private void Awake()
    {
        GameplayEvents.OnFinishAction += CheckIfOwnerMovedOntoGoalSquare;
        GameplayEvents.OnFinishAction += CheckIfCaptainDied;
    }

    private void CheckIfOwnerMovedOntoGoalSquare(Action action)
    {
        if (GameManager.CurrentGamePhase != GamePhase.GAMEPLAY)
            return;

        if (CurrentTile != null && CurrentTile.TileType.Equals(TileType.GoalTile))
        {
            GameplayEvents.GameIsOver(Side, GameOverCondition.CAPTAIN_TOOK_CONTROL);
        }
    }

    private void CheckIfCaptainDied(Action action)
    {
        if (isDead)
        {
            base.Die();
            Debug.Log("Captain dies.");
            GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(Side), GameOverCondition.CAPTAIN_DIED);
        }
    }

    public override void Die()
    {
        isDead = true;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnFinishAction -= CheckIfOwnerMovedOntoGoalSquare;
        GameplayEvents.OnFinishAction -= CheckIfCaptainDied;
    }
}