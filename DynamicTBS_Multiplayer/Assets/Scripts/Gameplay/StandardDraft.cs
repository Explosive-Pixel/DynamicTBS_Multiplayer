using UnityEngine;
using System.Collections.Generic;

public class StandardDraft : MonoBehaviour
{
    private PlayerManager playerManager;
    private Board board;

    private void Awake()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }

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

        board = GameObject.Find("GameplayCanvas").GetComponent<Board>();

        foreach (Character character in characters) 
        {
            Tile startTile = board.FindStartTiles(character)[0];
            Vector3 position = startTile.GetPosition();
            character.GetCharacterGameObject().transform.position = new Vector3(position.x, position.y, 0.997f);
            startTile.SetCurrentInhabitant(character);
        }

        SpawnMasters();

        GameplayEvents.StartGameplayPhase();
    }

    private Character SpawnCharacter(CharacterType characterType, PlayerType playerType) 
    {
        Character character = CharacterFactory.CreateCharacter(characterType, playerManager.GetPlayer(playerType));
     
        DraftEvents.CharacterCreated(character);
        return character;
    }

    private void SpawnMasters()
    {
        SpawnMaster(PlayerType.blue);
        SpawnMaster(PlayerType.pink);
    }

    private void SpawnMaster(PlayerType playerType)
    {
        Character master = CharacterFactory.CreateCharacter(CharacterType.MasterChar, playerManager.GetPlayer(playerType));

        Tile masterSpawnTile = board.FindMasterStartTile(playerType);
        Vector3 position = masterSpawnTile.GetPosition();
        master.GetCharacterGameObject().transform.position = new Vector3(position.x, position.y, 0.998f);
        masterSpawnTile.SetCurrentInhabitant(master);
        DraftEvents.CharacterCreated(master);
    }
}