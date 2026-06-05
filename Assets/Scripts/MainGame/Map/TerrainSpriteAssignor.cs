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

	/// <summary>
	/// シングルトンなのでコンストラクタを private にしている（クラス外からnew（インスタンス化）できなくする）
	/// </summary>
	private TerrainSpriteAssignor() {
		int spriteFileCount = _MAP_SPRITE_NAME_LIST.Length;
		_terrainSpriteList = new List<Sprite[]>(spriteFileCount);
		// 全ての使用スプライトの読み込み
		for (int i = 0; i < spriteFileCount; i++) {
			Sprite[] loadSpriteList = Resources.LoadAll<Sprite>(_MAP_SPRITE_PATH + _MAP_SPRITE_NAME_LIST[i]);
			_terrainSpriteList.Add(loadSpriteList);
		}
	}

	/// <summary>
	/// 地形に対応したスプライトを取得
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	public Sprite GetTerrainSprite(eTerrain terrain, int index = -1) {
		Sprite[] terrainSprite = _terrainSpriteList[GetTerrainSpriteIndex(terrain)];
		if (CommonModule.IsEmpty(terrainSprite)) return null;
		// 無効なインデクスならランダムなスプライトを返す
		if (!CommonModule.IsEnableIndex(terrainSprite, index)) index = Random.Range(0, terrainSprite.Length);

		return terrainSprite[index];
	}

	/// <summary>
	/// 地形に対応したスプライト名取得
	/// </summary>
	/// <param name="terrain"></param>
	/// <returns></returns>
	private int GetTerrainSpriteIndex(eTerrain terrain) {
		switch (terrain) {
			case eTerrain.Passage:
			case eTerrain.Room:
				return 0;
			case eTerrain.Wall:
				return 1;
			case eTerrain.Stair:
				return 2;
		}
		return 0;
	}

}
