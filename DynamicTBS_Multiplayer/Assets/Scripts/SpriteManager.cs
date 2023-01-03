using System.Collections.Generic;
using UnityEngine;

public class SpriteManager
{
    // Characters
    public static Sprite BLUE_MASTER_SPRITE;
    public static Sprite PINK_MASTER_SPRITE;
    public static Sprite BLUE_TANK_SPRITE;
    public static Sprite PINK_TANK_SPRITE;
    public static Sprite BLUE_SHOOTER_SPRITE;
    public static Sprite PINK_SHOOTER_SPRITE;
    public static Sprite BLUE_RUNNER_SPRITE;
    public static Sprite PINK_RUNNER_SPRITE;
    public static Sprite BLUE_MECHANIC_SPRITE;
    public static Sprite PINK_MECHANIC_SPRITE;
    public static Sprite BLUE_MEDIC_SPRITE;
    public static Sprite PINK_MEDIC_SPRITE;
    
    // Tiles
    public static Sprite EMPTY_TILE_SPRITE;
    public static Sprite FLOOR_TILE_SPRITE;
    public static Sprite FLOOR_TILE_SPRITE_VAR_1;
    public static Sprite FLOOR_TILE_SPRITE_VAR_2;
    public static Sprite START_TILE_SPRITE;
    public static Sprite MASTER_START_TILE_SPRITE;
    public static Sprite GOAL_TILE_SPRITE;
    
    // UI
    public static Sprite ABILITY_CIRCLE_SPRITE;
    public static Sprite ATTACK_CIRCLE_SPRITE;
    public static Sprite MOVE_CIRCLE_SPRITE;
    public static Sprite ATTACK_ROW_DOWN_SPRITE;
    public static Sprite ATTACK_ROW_LEFT_SPRITE;
    public static Sprite ATTACK_ROW_RIGHT_SPRITE;
    public static Sprite ATTACK_ROW_UP_SPRITE;
    public static Sprite COOLDOWN_1_SPRITE;
    public static Sprite COOLDOWN_2_SPRITE;
    public static Sprite COOLDOWN_3_SPRITE;
    public static Sprite HP_1_SPRITE;
    public static Sprite HP_2_SPRITE;
    public static Sprite HP_3_SPRITE;
    public static Sprite HP_4_SPRITE;
    public static Sprite TANK_BLOCK_FRAME_SPRITE;

    private static List<Sprite> floorTileSprites = new List<Sprite>();

    public static void LoadSprites()
    {
        BLUE_MASTER_SPRITE = Resources.Load<Sprite>("CharacterSprites/Blue_Master");
        PINK_MASTER_SPRITE = Resources.Load<Sprite>("CharacterSprites/Pink_Master");
        BLUE_TANK_SPRITE = Resources.Load<Sprite>("CharacterSprites/Blue_Tank");
        PINK_TANK_SPRITE = Resources.Load<Sprite>("CharacterSprites/Pink_Tank");
        BLUE_SHOOTER_SPRITE = Resources.Load<Sprite>("CharacterSprites/Blue_Shooter");
        PINK_SHOOTER_SPRITE = Resources.Load<Sprite>("CharacterSprites/Pink_Shooter");
        BLUE_RUNNER_SPRITE = Resources.Load<Sprite>("CharacterSprites/Blue_Runner");
        PINK_RUNNER_SPRITE = Resources.Load<Sprite>("CharacterSprites/Pink_Runner");
        BLUE_MECHANIC_SPRITE = Resources.Load<Sprite>("CharacterSprites/Blue_Mechanic");
        PINK_MECHANIC_SPRITE = Resources.Load<Sprite>("CharacterSprites/Pink_Mechanic");
        BLUE_MEDIC_SPRITE = Resources.Load<Sprite>("CharacterSprites/Blue_Medic");
        PINK_MEDIC_SPRITE = Resources.Load<Sprite>("CharacterSprites/Pink_Medic");

        //EMPTY_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/EmptyTile");
        //FLOOR_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/FloorTile");
        //START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/StartTile");
        //MASTER_START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/MasterStartTile");
        //GOAL_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/GoalTile");

        EMPTY_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Hole_v3");
        FLOOR_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Floor_v1");
        FLOOR_TILE_SPRITE_VAR_1 = Resources.Load<Sprite>("TileSprites/v100/FloorTileVariation_1");
        FLOOR_TILE_SPRITE_VAR_2 = Resources.Load<Sprite>("TileSprites/v100/FloorTileVariation_2");
        START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/UnitStartBlank_v1");
        MASTER_START_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/MasterStartBlank_v1");
        GOAL_TILE_SPRITE = Resources.Load<Sprite>("TileSprites/v100/Goal_v2");

        ABILITY_CIRCLE_SPRITE = Resources.Load<Sprite>("UI/AbilityCircle");
        ATTACK_CIRCLE_SPRITE = Resources.Load<Sprite>("UI/AttackCircle");
        MOVE_CIRCLE_SPRITE = Resources.Load<Sprite>("UI/MoveCircle");
        ATTACK_ROW_DOWN_SPRITE = Resources.Load<Sprite>("UI/AttackRowDown");
        ATTACK_ROW_LEFT_SPRITE = Resources.Load<Sprite>("UI/AttackRowLeft");
        ATTACK_ROW_RIGHT_SPRITE = Resources.Load<Sprite>("UI/AttackRowRight");
        ATTACK_ROW_UP_SPRITE = Resources.Load<Sprite>("UI/AttackRowUp");
        COOLDOWN_1_SPRITE = Resources.Load<Sprite>("UI/Cooldown_1");
        COOLDOWN_2_SPRITE = Resources.Load<Sprite>("UI/Cooldown_2");
        COOLDOWN_3_SPRITE = Resources.Load<Sprite>("UI/Cooldown_3");
        HP_1_SPRITE = Resources.Load<Sprite>("UI/HP_1");
        HP_2_SPRITE = Resources.Load<Sprite>("UI/HP_2");
        HP_3_SPRITE = Resources.Load<Sprite>("UI/HP_3");
        HP_4_SPRITE = Resources.Load<Sprite>("UI/HP_4");
        TANK_BLOCK_FRAME_SPRITE = Resources.Load<Sprite>("UI/Tank_BlockFrame");

        AddSpritesToFloorTileList();
    }

    public static Sprite GetTileSprite(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.EmptyTile:
                return EMPTY_TILE_SPRITE;
            case TileType.FloorTile:
                return GetRandomFloorSprite();
            case TileType.GoalTile:
                return GOAL_TILE_SPRITE;
            case TileType.StartTile:
                return START_TILE_SPRITE;
            case TileType.MasterStartTile:
                return MASTER_START_TILE_SPRITE;
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
        // Chance for variations should be at 20% each
        for (int i = 0; i <= 5; i++)
        {
            // Add base floor tile
            floorTileSprites.Add(FLOOR_TILE_SPRITE);
        }
        for (int i = 0; i <= 1; i++)
        {
            floorTileSprites.Add(FLOOR_TILE_SPRITE_VAR_1);
            floorTileSprites.Add(FLOOR_TILE_SPRITE_VAR_2);
        }
    }

    // Put this in GetTileSprite in the FloorTile case
    private static Sprite GetRandomFloorSprite() 
    {
        int i = Random.Range(0, floorTileSprites.Count);
        return floorTileSprites[i];
    }
}