using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private PlayerType type;
    private int roundCounter;

    public Player(PlayerType type) 
    {
        this.type = type;
        roundCounter = 1;
    }

    public PlayerType GetPlayerType() 
    {
        return this.type;
    }

    public bool HasAvailableAction()
    {
        List<Character> charactersOfOtherPlayer = CharacterHandler.GetAllLivingCharacters()
               .FindAll(character => character.GetSide() == this);

        foreach (Character character in charactersOfOtherPlayer)
        {
            if (character.CanPerformAction())
            {
                return true;
            }
        }

        return false;
    }

    public void IncreaseRoundCounter() 
    {
        this.roundCounter++;
    }

    public void ResetRoundCounter() 
    {
        this.roundCounter = 1;
    }
}