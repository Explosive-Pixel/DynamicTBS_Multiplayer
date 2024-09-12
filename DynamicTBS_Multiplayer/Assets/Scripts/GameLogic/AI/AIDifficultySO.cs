using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AIAction
{
    public ActionType Type;
    public Character Character;
    public GameObject Target;
}

public abstract class AIDifficultySO : ScriptableObject
{
    
    protected List<Character> AvailableCharacters;
    protected AIAction ActionToTake;
    protected List<GameObject> ActionDestinations;


    public abstract AIAction CalculateBestMove();

    protected void SetParams()
    {
        AvailableCharacters = CharacterManager.GetAllLivingCharactersOfSide(PlayerType.blue);
    }
}
