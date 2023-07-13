using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodePA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private PatternType explodePatternType; // PatternType.Star
    [SerializeField] private int explodeDamage; // 1

    private bool active = false;

    private void Awake()
    {
        active = false;
    }

    public void Apply()
    {
        active = true;
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
        if (active)
            Explode(gameObject.transform.position);
    }
}