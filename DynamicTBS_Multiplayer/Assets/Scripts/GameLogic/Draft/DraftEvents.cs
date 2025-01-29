public static class DraftEvents
{
    public delegate void CharacterCreation(Character character);
    public static event CharacterCreation OnCharacterCreated;

    public delegate void PerformDraftAction();
    public static event PerformDraftAction OnDraftActionFinished;

    public static void CharacterCreated(Character character)
    {
        if (OnCharacterCreated != null)
            OnCharacterCreated(character);
    }

    public static void FinishDraftAction()
    {
        if (OnDraftActionFinished != null)
            OnDraftActionFinished();
    }
}