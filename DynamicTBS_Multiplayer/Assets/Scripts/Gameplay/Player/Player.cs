using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private List<Character> characters;

    public void AddCharacter(Character character) {
        this.characters.Add(character);
    }

    public bool RemoveCharacter(Character character) {
        return this.characters.Remove(character);
    }
}