using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private PlayerType type;
    private List<Character> characters;

    public Player(PlayerType type) 
    {
        this.type = type;
    }

    public PlayerType GetPlayerType() 
    {
        return this.type;
    }


    public void AddCharacter(Character character) {
        this.characters.Add(character);
    }

    public bool RemoveCharacter(Character character) {
        return this.characters.Remove(character);
    }
}