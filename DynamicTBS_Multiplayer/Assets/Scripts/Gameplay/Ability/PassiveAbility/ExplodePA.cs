using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodePA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private PatternType explodePatternType; // PatternType.Star
    [SerializeField] private int explodeDamage; // 1

    private Character owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<Character>();
    }

    public void Apply()
    {
    }

    private void Explode(Vector3 lastPosition)
    {
        Tile ownerLastTile = Board.GetTileByPosition(lastPosition);
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
        Explode(gameObject.transform.position);
    }
}