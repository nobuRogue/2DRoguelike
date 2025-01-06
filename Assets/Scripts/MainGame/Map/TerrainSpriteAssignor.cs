/**
 * @file PartMain.cs
 * @brief メインゲームパート
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpriteAssignor {
	private static List<List<Sprite[]>> _terrainSpriteList = null;

	private static readonly string MAP_SPRITE_PATH = "TutorialAssets/Sprites/Map/";

	private static readonly string[][] MAP_SPRITE_NAME_LIST = new string[][]{
		new string[]{"rogue_map_sand_floor","rogue_map_sand_wall","rogue_map_sand_stair"},
		new string[]{"rogue_map_snow_floor","rogue_map_snow_wall","rogue_map_snow_stair"},
		new string[]{"rogue_map_urban_floor","rogue_map_urban_wall","rogue_map_urban_stair"}};

	public static void Initialize() {
		int mapTypeMax = 3;
		int terrainSpriteMax = 3;
		_terrainSpriteList = new List<List<Sprite[]>>( mapTypeMax );
		for (int mapType = 0; mapType < mapTypeMax; mapType++) {
			_terrainSpriteList.Add( new List<Sprite[]>( terrainSpriteMax ) );
			for (int terrainSprite = 0; terrainSprite < terrainSpriteMax; terrainSprite++) {
				var spriteList = Resources.LoadAll<Sprite>( MAP_SPRITE_PATH + MAP_SPRITE_NAME_LIST[mapType][terrainSprite] );
				_terrainSpriteList[mapType].Add( spriteList );
			}
		}
	}

	public static Sprite GetTerrainSprite( eTerrain terrain ) {
		var spriteList = _terrainSpriteList[0][GetSpriteIndex( terrain )];
		return spriteList[Random.Range( 0, spriteList.Length )];
	}

	private static int GetSpriteIndex( eTerrain terrain ) {
		switch (terrain) {
			case eTerrain.Passage:
			case eTerrain.Room:
				return 0;
			case eTerrain.Wall:
				return 1;
			case eTerrain.Stair:
				return 2;
			default:
				return -1;
		}
	}
}
