public class CaptainChar : Character
{
    private void Start()
    {
        characterType = CharacterType.CaptainChar;
    }

    public override void Die()
    {
        base.Die();
        GameplayEvents.GameIsOver(PlayerManager.GetOtherSide(Side), GameOverCondition.CAPTAIN_DIED);
    }
}