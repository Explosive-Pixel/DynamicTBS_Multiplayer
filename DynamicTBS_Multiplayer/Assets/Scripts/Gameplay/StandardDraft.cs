using UnityEngine;
using System.Collections.Generic;

public class StandardDraft : MonoBehaviour
{
    public void PerformStandardDraft()
    {
        List<Character> characters = new List<Character>();

        characters.Add(SpawnCharacter(CharacterType.TankChar, PlayerType.blue));
        characters.Add(SpawnCharacter(CharacterType.ShooterChar, PlayerType.blue));
        characters.Add(SpawnCharacter(CharacterType.ShooterChar, PlayerType.blue));
        characters.Add(SpawnCharacter(CharacterType.RunnerChar, PlayerType.blue));
        characters.Add(SpawnCharacter(CharacterType.MechanicChar, PlayerType.blue));
        characters.Add(SpawnCharacter(CharacterType.MechanicChar, PlayerType.blue));
        characters.Add(SpawnCharacter(CharacterType.MedicChar, PlayerType.blue));

        characters.Add(SpawnCharacter(CharacterType.TankChar, PlayerType.pink));
        characters.Add(SpawnCharacter(CharacterType.ShooterChar, PlayerType.pink));
        characters.Add(SpawnCharacter(CharacterType.ShooterChar, PlayerType.pink));
        characters.Add(SpawnCharacter(CharacterType.RunnerChar, PlayerType.pink));
        characters.Add(SpawnCharacter(CharacterType.MechanicChar, PlayerType.pink));
        characters.Add(SpawnCharacter(CharacterType.MechanicChar, PlayerType.pink));
        characters.Add(SpawnCharacter(CharacterType.MedicChar, PlayerType.pink));

        DraftEvents.EndDraft();
        PlacementEvents.StartPlacement();

        foreach (Character character in characters) 
        {
            Tile startTile = Board.FindStartTiles(character)[0];
            Vector3 position = startTile.GetPosition();
            character.GetCharacterGameObject().transform.position = new Vector3(position.x, position.y, 0.997f);
            startTile.SetCurrentInhabitant(character);
        }

        PlacementManager.SpawnMasters();

        GameplayEvents.StartGameplayPhase();
    }

    private Character SpawnCharacter(CharacterType characterType, PlayerType playerType) 
    {
        Character character = CharacterFactory.CreateCharacter(characterType, PlayerManager.GetPlayer(playerType));
     
        DraftEvents.CharacterCreated(character);
        return character;
    }
}
