using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodePA : MonoBehaviour, IPassiveAbility
{
    [SerializeField] private PatternType explodePatternType; // PatternType.Star
    [SerializeField] private int explodeDamage; // 1

    private CharacterMB owner;

    private void Awake()
    {
        owner = gameObject.GetComponent<CharacterMB>();
    }

    public void Apply()
    {
    }

    private void Explode(Vector3 lastPosition)
    {
        TileMB ownerLastTile = BoardNew.GetTileByPosition(lastPosition);
        foreach (CharacterMB character in CharacterManager.GetAllLivingCharacters())
        {
            if (character != null && character.gameObject != null)
            {
                TileMB neighborTile = BoardNew.GetTileByCharacter(character);
                if (BoardNew.Neighbors(ownerLastTile, neighborTile, explodePatternType))
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