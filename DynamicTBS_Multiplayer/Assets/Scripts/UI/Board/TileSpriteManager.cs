using System.Collections.Generic;
using UnityEngine;

public class TileSpriteManager
{   
    public static Sprite EMPTY_TILE_SPRITE;
    public static Sprite EMPTY_TILE_SPRITE_WITH_DEPTH;
    public static Sprite FLOOR_TILE_SPRITE;
    public static Sprite FLOOR_TILE_SPRITE_VAR_1;
    public static Sprite FLOOR_TILE_SPRITE_VAR_2;
    public static Sprite FLOOR_TILE_SPRITE_VAR_3;
    public static Sprite FLOOR_TILE_SPRITE_VAR_4;
    public static Sprite PINK_START_TILE_SPRITE;
    public static Sprite BLUE_START_TILE_SPRITE;
    public static Sprite PINK_MASTER_START_TILE_SPRITE;
    public static Sprite BLUE_MASTER_START_TILE_SPRITE;
    public static Sprite GOAL_TILE_SPRITE;

    private static readonly List<Sprite> floorTileSprites = new List<Sprite>();

    public static void LoadSprites()
    {
        EMPTY_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Hole");
        EMPTY_TILE_SPRITE_WITH_DEPTH = Resources.Load<Sprite>("TileSprites/v100/HoleWithDepthsFixed");
        FLOOR_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/FloorTileBase");
        FLOOR_TILE_SPRITE_VAR_1 = Resources.Load<Sprite>("TileSprites/v100/FloorTileVariation_1");
        FLOOR_TILE_SPRITE_VAR_2 = Resources.Load<Sprite>("TileSprites/v100/FloorTileVariation_2");
        FLOOR_TILE_SPRITE_VAR_3 = Resources.Load<Sprite>("TileSprites/v100/FloorTileVariation_3");
        FLOOR_TILE_SPRITE_VAR_4 = Resources.Load<Sprite>("TileSprites/v100/FloorTileVariation_4");
        PINK_START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Pink_StartTile");
        BLUE_START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Blue_StartTile");
        PINK_MASTER_START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Pink_MasterStartTile");
        BLUE_MASTER_START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Blue_MasterStartTile");
        GOAL_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Goal_Dark");

        AddSpritesToFloorTileList();
    }

    public static Sprite GetTileSprite(TileType tileType, PlayerType side, bool withDepth)
    {
        switch (tileType)
        {
            case TileType.EmptyTile:
                if (withDepth)
                    return EMPTY_TILE_SPRITE_WITH_DEPTH;
                else return EMPTY_TILE_SPRITE;
            case TileType.FloorTile:
                return GetRandomFloorSprite();
            case TileType.GoalTile:
                return GOAL_TILE_SPRITE;
            case TileType.StartTile:
                if (side == PlayerType.blue)
                    return BLUE_START_TILE_SPRITE;
                else
                    return PINK_START_TILE_SPRITE;
            case TileType.MasterStartTile:
                if (side == PlayerType.blue)
                    return BLUE_MASTER_START_TILE_SPRITE;
                else
                    return PINK_MASTER_START_TILE_SPRITE;
            default:
                return null;
        }
    }

    private static void AddSpritesToFloorTileList()
    {
        if (floorTileSprites.Count > 0)
            floorTileSprites.Clear();
        
        // Filling the list with 10 sprites
        // Chance for floor tile should be at 60%
        // Chance for variations should be at 10% each
        for (int i = 0; i <= 5; i++)
        {
            // Add base floor tile
            floorTileSprites.Add(FLOOR_TILE_SPRITE);
        }
        for (int i = 0; i <= 0; i++)
        {
            floorTileSprites.Add(FLOOR_TILE_SPRITE_VAR_1);
            floorTileSprites.Add(FLOOR_TILE_SPRITE_VAR_2);
            floorTileSprites.Add(FLOOR_TILE_SPRITE_VAR_3);
            floorTileSprites.Add(FLOOR_TILE_SPRITE_VAR_4);
        }
    }

    // Put this in GetTileSprite in the FloorTile case
    private static Sprite GetRandomFloorSprite() 
    {
        int i = Random.Range(0, floorTileSprites.Count);
        return floorTileSprites[i];
    }
}