using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private List<GameObject> characters;

    public void AddCharacter(GameObject character) {
        this.characters.Add(character);
    }

    public bool RemoveCharacter(GameObject character) {
        return this.characters.Remove(character);
    }
}