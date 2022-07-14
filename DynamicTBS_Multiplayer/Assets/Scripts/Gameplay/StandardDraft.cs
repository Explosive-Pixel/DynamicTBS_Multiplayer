using UnityEngine;

public class StandardDraft : MonoBehaviour
{
    private PlayerManager playerManager;
    private Board board;

    private void Awake()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        board = GameObject.Find("GameplayCanvas").GetComponent<Board>();
    }

    public void PerformStandardDraft()
    {
        SpawnCharacter(CharacterType.MasterChar, PlayerType.blue);
        SpawnCharacter(CharacterType.TankChar, PlayerType.blue);
        SpawnCharacter(CharacterType.ShooterChar, PlayerType.blue);
        SpawnCharacter(CharacterType.ShooterChar, PlayerType.blue);
        SpawnCharacter(CharacterType.RunnerChar, PlayerType.blue);
        SpawnCharacter(CharacterType.MechanicChar, PlayerType.blue);
        SpawnCharacter(CharacterType.MechanicChar, PlayerType.blue);
        SpawnCharacter(CharacterType.MedicChar, PlayerType.blue);
        
        SpawnCharacter(CharacterType.MasterChar, PlayerType.pink);
        SpawnCharacter(CharacterType.TankChar, PlayerType.pink);
        SpawnCharacter(CharacterType.ShooterChar, PlayerType.pink);
        SpawnCharacter(CharacterType.ShooterChar, PlayerType.pink);
        SpawnCharacter(CharacterType.RunnerChar, PlayerType.pink);
        SpawnCharacter(CharacterType.MechanicChar, PlayerType.pink);
        SpawnCharacter(CharacterType.MechanicChar, PlayerType.pink);
        SpawnCharacter(CharacterType.MedicChar, PlayerType.pink);
    }

    private void SpawnCharacter(CharacterType characterType, PlayerType playerType) 
    {
        Character character = CharacterFactory.CreateCharacter(characterType, playerManager.GetPlayer(playerType));
        
        // TODO: Find character tile position
        // Tile spawnTile = board.FindMasterStartTile(playerType);
        // Vector3 position = spawnTile.GetPosition();
        // character.GetCharacterGameObject().transform.position = new Vector3(position.x, position.y, 0.997f);
        // spawnTile.SetCurrentInhabitant(character);
        
        DraftEvents.CharacterCreated(character);
    }
}
