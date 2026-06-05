using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形スプライト割り当てモジュール
/// </summary>
public class TerrainSpriteAssignor {
	// シングルトンインスタンス
	private static TerrainSpriteAssignor _instance = null;
	// インスタンスへの参照プロパティ
	public static TerrainSpriteAssignor instance {
		get {
			if (_instance == null) _instance = new TerrainSpriteAssignor();

			return _instance;
		}
	}
	// スプライトリソースフォルダへのパス
	private const string _MAP_SPRITE_PATH = "Design/Sprites/Map/";

	private readonly string[] _MAP_SPRITE_NAME_LIST = new string[] {
		"rogue_map_sand_floor","rogue_map_sand_wall","rogue_map_sand_stair"
	};

	private List<Sprite[]> _terrainSpriteList = null;

	private TerrainSpriteAssignor() {

	}

	/// <summary>
	/// 地形に対応したスプライトを取得
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public Sprite GetTerrainSprite(eTerrain terrain) {
		Sprite[] terrainSprite = Resources.LoadAll<Sprite>(_MAP_SPRITE_PATH + GetTerrainSpriteName(terrain));
		return terrainSprite[0];
	}

	/// <summary>
	/// 地形に対応したスプライト名取得
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	private string GetTerrainSpriteName(eTerrain terrain) {
		switch (terrain) {
			case eTerrain.Passage:
			case eTerrain.Room:
				return _MAP_SPRITE_NAME_LIST[0];
			case eTerrain.Wall:
				return _MAP_SPRITE_NAME_LIST[1];
			case eTerrain.Stair:
				return _MAP_SPRITE_NAME_LIST[2];
		}
		return string.Empty;
	}

}
