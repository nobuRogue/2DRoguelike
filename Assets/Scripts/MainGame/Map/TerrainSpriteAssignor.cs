/**
 * @file PartMain.cs
 * @brief メインゲームパート
 * @author yaonobu
 * @date 2025/1/4
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class TerrainSpriteAssignor {
	private static readonly string _MAP_SPRITE_PATH = "Design/Sprites/Map/";
	private static readonly string[][] _MAP_SPRITE_NAME_LIST = new string[][]{
		new string[]{"rogue_map_sand_floor","rogue_map_sand_wall","rogue_map_sand_stair"},
		new string[]{"rogue_map_snow_floor","rogue_map_snow_wall","rogue_map_snow_stair"},
		new string[]{"rogue_map_urban_floor","rogue_map_urban_wall","rogue_map_urban_stair"}};

	private static List<List<Sprite[]>> _terrainSpriteList = null;
	private static int _floorSpriteIndex = -1;

	public static void Initialize() {
		int mapTypeMax = 3;
		int terrainSpriteMax = 3;
		_terrainSpriteList = new List<List<Sprite[]>>( mapTypeMax );
		for (int mapType = 0; mapType < mapTypeMax; mapType++) {
			_terrainSpriteList.Add( new List<Sprite[]>( terrainSpriteMax ) );
			for (int terrainSprite = 0; terrainSprite < terrainSpriteMax; terrainSprite++) {
				var spriteList = Resources.LoadAll<Sprite>( _MAP_SPRITE_PATH + _MAP_SPRITE_NAME_LIST[mapType][terrainSprite] );
				_terrainSpriteList[mapType].Add( spriteList );
			}
		}
	}

	public static void SetFloorSpriteIndex( int setIndex ) {
		_floorSpriteIndex = setIndex;
	}

	public static Sprite GetTerrainSprite( eTerrain terrain ) {
		if (!IsEnableIndex( _terrainSpriteList, _floorSpriteIndex )) return null;

		var spriteList = _terrainSpriteList[_floorSpriteIndex][GetSpriteIndex( terrain )];
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
