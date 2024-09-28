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
    
    protected List<Character> AvailableCharacters = new List<Character>();
    protected AIAction ActionToTake;
    protected List<GameObject> ActionDestinations = new List<GameObject>();
    protected Character Captain;



    public abstract AIAction CalculateBestMove();

    protected void SetParams()
    {
        AvailableCharacters = CharacterManager.GetAllLivingCharactersOfSide(PlayerType.blue);
        foreach (Character character in AvailableCharacters)
        {
            if (character.CharacterType == CharacterType.CaptainChar)
            {
                Captain = character;
                break;
            }
        }
    }

    protected void MoveTowards()
    {
        //Find way from captain to flag and move accordingly
    }
}
