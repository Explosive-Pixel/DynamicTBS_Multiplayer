using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    public static Sprite START_TILE_SPRITE;
    public static Sprite MASTER_START_TILE_SPRITE;
    public static Sprite GOAL_TILE_SPRITE;

    public static void LoadSprites()
    {
        BLUE_MASTER_SPRITE = Resources.Load<Sprite>("");
    }
}