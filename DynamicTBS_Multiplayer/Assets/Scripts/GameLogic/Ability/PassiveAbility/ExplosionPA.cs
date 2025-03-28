using UnityEngine;

public class ExplosionPA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private PatternType explodePatternType; // PatternType.Star
    [SerializeField] private int explodeDamage; // 1

    public PassiveAbilityType AbilityType { get { return PassiveAbilityType.EXPLOSION; } }

    private bool active = false;

    private void Awake()
    {
        active = false;
    }

    public void Apply()
    {
        active = true;
    }

    public bool IsDisabled()
    {
        return false;
    }

    private void Explode(Vector3 lastPosition)
    {
        Tile ownerLastTile = Board.GetTileByPosition(lastPosition);

        if (ownerLastTile == null)
            return;

        foreach (Character character in CharacterManager.GetAllLivingCharacters())
        {
            if (character != null && character.gameObject != null)
            {
                Tile neighborTile = Board.GetTileByCharacter(character);
                if (Board.Neighbors(ownerLastTile, neighborTile, explodePatternType))
                {
                    character.TakeDamage(explodeDamage);
                }
            }
        }

        ownerLastTile.Transform(TileType.EmptyTile);

        AudioEvents.Exploding();
    }

    private void OnDestroy()
    {
        if (active && gameObject != null)
            Explode(gameObject.transform.position);
    }
}